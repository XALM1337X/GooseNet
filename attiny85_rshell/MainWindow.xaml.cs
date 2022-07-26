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
using System.Windows.Forms;

namespace attiny85_rshell { 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public int GlobalServerID { get; set; }


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

        private void ConfigureServerClick(object sender, RoutedEventArgs e) {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(server_configuration_canvas, 1);
        }

        private void ServerConfBackClick(object sender, RoutedEventArgs e) {
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(server_configuration_canvas, 0);
        }

        private void ServerConfSubmit(object sender, RoutedEventArgs e) {
            if (File.Exists("../../../scripts/rs_server.ps1")) {
                string question = "../../../scripts/rs_server.ps1 already exists. Would you like to overwrite?";
                if (MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }

            Regex regex = new Regex(@"(.*)<OS_PORT>(.*)");

            string[] lines;


            if (File.Exists("../../../Templates/rs_server.ps1.template")) {
                lines = System.IO.File.ReadAllLines(@"../../../Templates/rs_server.ps1.template");
            } else {
                MessageBox.Show("rs_server.ps1.template not found. Reinstall application to fix.", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

                
            int port;
            bool changes_made = false;
            bool success = int.TryParse(server_port.Text, out port);
            if (!success) {
                //err
            } else {
                if (port > 0 && port < 65537) {
                    for (int i = 0; i < lines.Length; i++) {
                        if (regex.IsMatch(lines[i])) {
                            lines[i] = (regex.Replace(lines[i], "$1") + port.ToString() + regex.Replace(lines[i], "$2"));
                            changes_made = true;
                        }
                    }
                } else {
                    MessageBox.Show("Valid port range is [1 - 65536]");
                    return;
                } 
                if (changes_made) {
                    using (StreamWriter writetext = new StreamWriter("../../../scripts/rs_server.ps1")) {
                        foreach (string line in lines) {
                            writetext.WriteLine(line);
                        }
                        writetext.Close();
                    }
                    FlowDocument myFlowDoc = new FlowDocument();
                    myFlowDoc.Blocks.Add(new Paragraph(new Run("Succesfully wrote: ../../../scripts/rs_server.ps1")));
                    landing_page_log.Document.Blocks.Clear();
                    landing_page_log.Document = myFlowDoc;
                }            
            }

            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(server_configuration_canvas, 0);
        }

        private void PayloadConfBackButton(object sender, RoutedEventArgs e) {
            Panel.SetZIndex(landing_page, 1);
            Panel.SetZIndex(payload_conf_canvas, 0);
            
        }

        private void PayloadConfigureButtonClick(object sender, RoutedEventArgs e) {
            Panel.SetZIndex(landing_page, 0);
            Panel.SetZIndex(payload_conf_canvas, 1);
        }

        private void PayloadConfigSubmit(object sender, RoutedEventArgs e) {
            //FolderBrowserDialog folderBrowserDialog1;
            //folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            //folderBrowserDialog1.Description ="Select the directory that you want to use as the default.";
            //folderBrowserDialog1.ShowNewFolderButton = false;
            //folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;

            //Set Hosting domains for task and slave
            //Set Slave Target server and port
        }

        private void RunServerClick(object sender, RoutedEventArgs e) {
            if (File.Exists("..\\..\\..\\scripts\\rs_server.ps1")) {
                using (Process myProcess = new Process()) {

                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    myProcess.StartInfo.UseShellExecute = true;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.Arguments = "..\\..\\..\\scripts\\rs_server.ps1";
                    myProcess.Start();
                    GlobalServerID = myProcess.Id;

                    FlowDocument myFlowDoc = new FlowDocument();
                    myFlowDoc.Blocks.Add(new Paragraph(new Run("Server Started Succesfully.")));
                    myFlowDoc.Blocks.Add(new Paragraph(new Run("Server ID: "+ GlobalServerID.ToString())));
                    landing_page_log.Document.Blocks.Clear();
                    landing_page_log.Document = myFlowDoc;


                }
            } else {
                MessageBox.Show("..\\..\\..\\scripts\\rs_server.ps1 does not exist. Use server configuration button.","Configured server not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void KillServerClick(object sender, RoutedEventArgs e) {

            Process myProcess = Process.GetProcessById(GlobalServerID);
            if (myProcess != null) {
                myProcess.CloseMainWindow();
                myProcess.Close();
                FlowDocument myFlowDoc = new FlowDocument();
                myFlowDoc.Blocks.Add(new Paragraph(new Run("Server shutdown succesfully.")));
                landing_page_log.Document.Blocks.Clear();
                landing_page_log.Document = myFlowDoc;
            } else {
                MessageBox.Show("Failed to shutdown server process.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                return ;
            }
        }


    }
}
