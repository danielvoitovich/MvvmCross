﻿// MvxPhoneBindingBuilder.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Reflection;
using System.Windows;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Converters;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Bindings.Target;
using Cirrious.MvvmCross.Binding.Bindings.Target.Construction;
using Cirrious.MvvmCross.BindingEx.WindowsPhone.MvxBinding;
using Cirrious.MvvmCross.BindingEx.WindowsPhone.MvxBinding.Target;
using Cirrious.MvvmCross.BindingEx.WindowsPhone.WindowsBinding;

namespace Cirrious.MvvmCross.BindingEx.WindowsPhone
{
    public class MvxPhoneBindingBuilder : MvxBindingBuilder
    {
        public enum BindingType
        {
            Windows,
            MvvmCross
        }

        private readonly BindingType _bindingType;
        private readonly Action<IMvxValueConverterRegistry> _fillValueConvertersAction;

        public MvxPhoneBindingBuilder(
            BindingType bindingType = BindingType.MvvmCross,
            params Assembly[] valueConverterAssemblies)
        {
            _bindingType = bindingType;
            if (valueConverterAssemblies.Length > 0)
                _fillValueConvertersAction = (registry) => registry.Fill(valueConverterAssemblies);
        }

        public override void DoRegistration()
        {
            base.DoRegistration();
            InitializeBindingCreator();
        }

        protected override void RegisterBindingFactories()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    // no need for MvvmCross binding factories - so don't create them
                    break;
                case BindingType.MvvmCross:
                    base.RegisterBindingFactories();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override Binding.Bindings.Target.Construction.IMvxTargetBindingFactoryRegistry
            CreateTargetBindingRegistry()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return base.CreateTargetBindingRegistry();
                    break;
                case BindingType.MvvmCross:
                    return new MvxPhoneTargetBindingFactoryRegistry();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitializeBindingCreator()
        {
            var creator = CreateBindingCreator();
            Mvx.RegisterSingleton(creator);
        }

        protected virtual IMvxBindingCreator CreateBindingCreator()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return new MvxWindowsBindingCreator();
                    break;
                case BindingType.MvvmCross:
                    return new MvxMvvmCrossBindingCreator();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);

            registry.Fill(this.GetType().Assembly);
            registry.Fill(typeof (Localization.MvxLanguageConverter).Assembly);

            if (_fillValueConvertersAction != null)
                _fillValueConvertersAction(registry);
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            registry.RegisterCustomBindingFactory<FrameworkElement>("Visible",
                                                                    view => new MvxVisibleTargetBinding(view));
            registry.RegisterCustomBindingFactory<FrameworkElement>("Collapsed",
                                                                    view => new MvxCollapsedTargetBinding(view));

            base.FillTargetFactories(registry);
        }
    }
}