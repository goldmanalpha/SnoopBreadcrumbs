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

            this.Loaded += (sender, args) => _vm.Loaded();
        }

        private void GetFolderButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _vm.RootFolder = dialog.SelectedPath;
            }

            
        }

        private void ProcessXamlsClick(object sender, RoutedEventArgs e)
        {
            _vm.ProcessXamls();
        }


    }
}
