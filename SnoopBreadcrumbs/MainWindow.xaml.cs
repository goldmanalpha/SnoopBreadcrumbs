using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnoopBreadcrumbs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM _vm;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = (_vm = new MainVM());
        }

        private void Go_Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.TestMethod();
        }


    }
}
