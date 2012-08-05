using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrumbLib
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    public interface IHaveSelectedItems
    {
        IList SelectedItems { get; set; }
    }

    public static class ListBoxBehavior
    {

        public static bool GetBindSelectedItems(DependencyObject obj)
        {
            return (bool)obj.GetValue(BindSelectedItemsProperty);
        }

        public static void SetBindSelectedItems(DependencyObject obj, bool value)
        {
            obj.SetValue(BindSelectedItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for BindSelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindSelectedItemsProperty =
            DependencyProperty.RegisterAttached("BindSelectedItems", typeof(bool), typeof(ListBoxBehavior), new UIPropertyMetadata(false, BindSelectedItemsPropertyChangedCallback));

        private static void BindSelectedItemsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var vm = (dependencyObject as FrameworkElement).DataContext as IHaveSelectedItems;
            var listBox = dependencyObject as ListBox;

            listBox.DataContextChanged += (sender, eventArgs) =>
                {
                    vm = (dependencyObject as FrameworkElement).DataContext as IHaveSelectedItems;
                    SetSelected(listBox, vm);
                };


            SetSelected(listBox, vm);
        }

        private static void SetSelected(ListBox listBox, IHaveSelectedItems vm)
        {
            if (vm != null)
                vm.SelectedItems = listBox.SelectedItems;
        }
    }
}
