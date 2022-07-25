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
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;



//using System.Diagnostics;
//This namespace gives access to Process.Start("<cmd_path>");



//NOTE: How to create flow document and add to RichTextBox.
//FlowDocument myFlowDoc = new FlowDocument();
//myFlowDoc.Blocks.Add(new Paragraph(new Run("Paragraph 1")));
//TextBox.Document = myFlowDoc;

namespace attiny85_rshell { 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            

        }

        private void MasterClientConfigureView(object sender, RoutedEventArgs e) {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(master_client_configure_canvas, 1);
            //MessageBox.Show("Hello");
        }

        private void MasterClientConfigBack(object sender, RoutedEventArgs e) {
            //MessageBox.Show("Hello");
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(master_client_configure_canvas, 0);
        }

        private void MasterClientConfigSubmit(object sender, RoutedEventArgs e) {
            if (File.Exists("../../../data/master_client.json")) {
                string question = "master_client.json already exists. Would you like to overwrite?";
                if (MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }

            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(master_client_configure_canvas, 0);
            MasterClientConf master_client = new MasterClientConf(master_client_domain_textbox.Text, master_client_server_port.Text);

            string jsonString = JsonSerializer.Serialize(master_client);
            using (StreamWriter writetext = new StreamWriter("../../../data/master_client.json")) {                
                writetext.WriteLine(jsonString);
            }
            FlowDocument myFlowDoc = new FlowDocument();
            myFlowDoc.Blocks.Add(new Paragraph(new Run("Succesfully wrote: ../../../data/master_client.json")));
            landing_page_log.Document = myFlowDoc;
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
        private void ServerConfSubmit(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("THING");
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
