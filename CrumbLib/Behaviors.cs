namespace CrumbLib
{
    using System.Windows;

    public static class Behaviors
    {

        public static double GetControlHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ControlHeightProperty);
        }

        public static void SetControlHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ControlHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for ControlHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlHeightProperty =
            DependencyProperty.RegisterAttached("ControlHeight", typeof(double), typeof(Behaviors), new UIPropertyMetadata(0d, ControlHeightPropertyChangedCallback));

        private static void ControlHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var fe = dependencyObject as FrameworkElement;
            fe.MaxHeight = (double) args.NewValue - (double)dependencyObject.GetValue(SubtractHeightProperty);

        }

        public static double GetSubtractHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(SubtractHeightProperty);
        }

        public static void SetSubtractHeight(DependencyObject obj, double value)
        {
            obj.SetValue(SubtractHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for SubtractHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtractHeightProperty =
            DependencyProperty.RegisterAttached("SubtractHeight", typeof(double), typeof(Behaviors), new UIPropertyMetadata(0d, SubtractHeightPropertyChangedCallback));

        private static void SubtractHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var fe = dependencyObject as FrameworkElement;
            var height = (double)dependencyObject.GetValue(ControlHeightProperty) -  (double)args.NewValue;

            if (height > 0)
                fe.MaxHeight = height;
        }
    }
}
