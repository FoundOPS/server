using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace FoundOps.Common.Silverlight.Interactivity.Behaviors
{
    public class SetPropertyOnStateBehavior : Behavior<FrameworkElement>
    {
        private bool _loaded;

        protected override void OnAttached()
        {
            base.OnAttached();
            _loaded = true;
            VisualStateGroupNameChanged(VisualStateName);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _loaded = false;
        }

        #region VisualStateName Dependency Property

        /// <summary>
        /// VisualStateName
        /// </summary>
        public string VisualStateName
        {
            get { return (string)GetValue(VisualStateNameProperty); }
            set { SetValue(VisualStateNameProperty, value); }
        }

        /// <summary>
        /// VisualStateName Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VisualStateNameProperty =
            DependencyProperty.Register(
                "VisualStateName",
                typeof(string),
                typeof(SetPropertyOnStateBehavior),
                new PropertyMetadata(null));
        #endregion

        #region VisualStateGroupName Dependency Property

        /// <summary>
        /// VisualStateGroupName
        /// </summary>
        public string VisualStateGroupName
        {
            get { return (string)GetValue(VisualStateGroupNameProperty); }
            set { SetValue(VisualStateGroupNameProperty, value); }
        }

        /// <summary>
        /// VisualStateGroupName Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VisualStateGroupNameProperty =
            DependencyProperty.Register(
                "VisualStateGroupName",
                typeof(string),
                typeof(SetPropertyOnStateBehavior),
                new PropertyMetadata(new PropertyChangedCallback(VisualStateGroupNameChanged)));

        private static void VisualStateGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetPropertyOnStateBehavior c = d as SetPropertyOnStateBehavior;
            if (c == null) return;
            c.VisualStateGroupNameChanged((string)e.NewValue);
        }

        private void VisualStateGroupNameChanged(string stateGroupName)
        {
            if (!_loaded || stateGroupName == null) return;

            var visualStateGroup =
                ((IList<VisualStateGroup>)VisualStateManager.GetVisualStateGroups(AssociatedObject)).
                    FirstOrDefault(vsg => vsg.Name == stateGroupName);

            if (visualStateGroup == null) return;

            visualStateGroup.CurrentStateChanged -= VisualStateGroupCurrentStateChanged;
            visualStateGroup.CurrentStateChanged += VisualStateGroupCurrentStateChanged;
        }

        void VisualStateGroupCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name != VisualStateName) return;
            if (!_loaded) return;

            Type associatedObjectType = AssociatedObject.GetType();
            PropertyInfo associatedObjectProperty = associatedObjectType.GetProperty(PropertyName);

            associatedObjectProperty.SetValue(AssociatedObject, ValueToSet, null);
        }

        #endregion

        #region ValueToSet Dependency Property

        /// <summary>
        /// ValueToSet
        /// </summary>
        public Object ValueToSet
        {
            get { return (Object)GetValue(ValueToSetProperty); }
            set { SetValue(ValueToSetProperty, value); }
        }

        /// <summary>
        /// ValueToSet Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueToSetProperty =
            DependencyProperty.Register(
                "ValueToSet",
                typeof(Object),
                typeof(SetPropertyOnStateBehavior),
                new PropertyMetadata(null));

        #endregion

        #region PropertyName Dependency Property

        /// <summary>
        /// PropertyName
        /// </summary>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// PropertyName Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(
                "PropertyName",
                typeof(string),
                typeof(SetPropertyOnStateBehavior),
                new PropertyMetadata(null));

        #endregion
    }
}
