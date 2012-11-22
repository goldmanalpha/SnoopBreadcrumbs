using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace SnoopCrumbInclude
{
    public class _SnoopBreadCrumb
    {
        public static readonly DependencyProperty _BreadCrumbProperty = DependencyProperty.RegisterAttached(
              "_BreadCrumb",
              typeof(string),
              typeof(_SnoopBreadCrumb),
              new FrameworkPropertyMetadata(null)
            );

        public static void Set_BreadCrumb(UIElement element, string value)
        {
            element.SetValue(_BreadCrumbProperty, value);
        }
        public static string Get_BreadCrumb(UIElement element)
        {
            return (string)element.GetValue(_BreadCrumbProperty);
        }
    }
}
