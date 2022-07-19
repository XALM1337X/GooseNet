using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//using System.Diagnostics;
//This namespace gives access to Process.Start("<cmd_path>");

namespace attiny85_rshell { 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

        }

        private void MasterClientConfigureView(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(master_client_configure_canvas, 1);
            //MessageBox.Show("Hello");
        }

        private void MasterClientConfigBack(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Hello");
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(master_client_configure_canvas, 0);
        }

        private void MasterClientConfigSubmit(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(master_client_domain_textbox.Text);

            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(master_client_configure_canvas, 0);

        }

        private void ConfigureServerClick(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(server_configuration_canvas, 1);
        }
        private void ServerConfBackClick(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(server_configuration_canvas, 0);
        }

        private void PayloadConfBackButton(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(payload_conf_canvas, 0);
            
        }

        private void PayloadConfigureButtonClick(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(payload_conf_canvas, 1);
        }
    }
}
