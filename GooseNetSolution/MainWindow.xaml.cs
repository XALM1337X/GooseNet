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
using System.IO.Compression;
using System.Windows.Threading;
using Microsoft.VisualBasic.Logging;
using System.Reflection;
using static System.Windows.Forms.LinkLabel;


//TODO_FUNCTIONALITY_LIST:
//Master Client Options
//Start button: (Requires creating GUI master client)

//Target Client Options:
//All buttons

//Server Options:
//Client List Button:


//Notes on how to launch async thread function calls
/*
    DispatcherTimer dispatcherTimer = new DispatcherTimer();
    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
    dispatcherTimer.Start();
*/
/*
    private void dispatcherTimer_Tick(object sender, EventArgs e)  {
    }
*/

namespace attiny85_rshell { 

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

            if (server_port.Text == "") {
                System.Windows.MessageBox.Show("Server port cannot be empty.", "Entry empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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

            if (host_fqdn_wan_textbox.Text == "") {
                System.Windows.MessageBox.Show("Host FQDN field cannot be empty","Empty Entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (slave_server_textbox.Text == "") {
                System.Windows.MessageBox.Show("Slave FQDN field cannot be empty", "Empty Entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (slave_server_port_textbox.Text == "") {
                System.Windows.MessageBox.Show("Slave port field cannot be empty", "Empty Entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
        private void PayloadLocalHostCheckboxClick(object sender, RoutedEventArgs e) {
            if (local_hosting_checkbox.IsChecked ?? false) {
                local_host_select_button.IsEnabled = true;
                local_host_path_test_display.IsEnabled = true;
            } else {
                local_host_select_button.IsEnabled = false;
                local_host_path_test_display.IsEnabled = false;
            }
        }
        private void PayloadLocallyHostedSelectButtonClick(object sender, RoutedEventArgs e) {
            FolderBrowserDialog folderBrowserDialog1;
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description ="Select the hosting path for your scripts.";
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                string folderName = folderBrowserDialog1.SelectedPath;
                local_host_path_test_display.Text = folderName;
            }
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
        private void SlaveClientBroadcastCheckBoxClick(object sender, RoutedEventArgs e) {
            if (broadcast_checbox.IsChecked ?? false) {
                client_id_textbox.IsEnabled = false;
            } else {
                client_id_textbox.IsEnabled = true;
            }
        }
        private void PayloadUploadClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 0);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 1);
        }
        private void PayloadUploadBackClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 0);
        }
        private void ManualUploadButtonClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 0);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_manual_option_canvas, 1);
        }
        private void ManualPayloadBackButtonClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_manual_option_canvas, 0);
        }
        private void HyperlinkRequestIDE(object sender, RoutedEventArgs e) {
            using (Process myProcess = new Process()) {
                myProcess.StartInfo = new ProcessStartInfo("https://www.arduino.cc/en/software");
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.Start();
                e.Handled = true;
            }
        }
        private void HyperlinkRequestDrivers(object sender, RoutedEventArgs e) {
            using (Process myProcess = new Process()) {
                myProcess.StartInfo = new ProcessStartInfo("https://github.com/digistump/DigistumpArduino/releases/download/1.6.7/Digistump.Drivers.zip");
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.Start();
                e.Handled = true;
            }
        }
        private void HyperlinkRequestDigistump(object sender, RoutedEventArgs e)  {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo = new ProcessStartInfo("http://digistump.com/products/1");
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.Start();
                e.Handled = true;
            }
        }
        private void PayloadUploadAutoButtonClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 0);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_auto_option_canvas, 1);
        }
        private void StartGooseNetButtonClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(terms_of_service, 0);
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
        }
        private void TermsOfServiceCheckboxClick(object sender, RoutedEventArgs e) {
            if (service_agreement_auto_install_checkbox.IsChecked ?? true) {
                start_goosenet_button.IsEnabled = true;
            } else {
                start_goosenet_button.IsEnabled = false;
            }
        }
        private void PayloadUploadAutoButtonBackClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(payload_upload_options_canvas, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_upload_auto_option_canvas, 0);
        }
        private void ArduinoDownloadButtonClick(object sender, RoutedEventArgs e) {
            zip_down_button.IsEnabled = false;
            zip_down_button.Content = "Loading";
            zip_down_button.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, InstallStart);
        }
        private void InstallStart() {
            DownloadCLIZipAndUnpack();
            DownloadDriverZipAndUnpack();
            zip_down_button.IsEnabled = true;
            zip_down_button.Content = "Install";
        }
        private async void DownloadCLIZipAndUnpack() {
            if (File.Exists("../../../ThirdParty/arduino-cli/arduino-cli.exe")) {
                if (System.Windows.MessageBox.Show("arduino-cli.exe already exists. Would you like to overwrite it?", "File Exists", MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }           
            using var client = new HttpClient();
            using (var response = await client.GetAsync("https://downloads.arduino.cc/arduino-cli/arduino-cli_latest_Windows_64bit.zip"))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var file = File.OpenWrite("../../../ThirdParty/arduino-cli.zip")) {
                stream.CopyTo(file);
            }    
            
            if (Directory.Exists("../../../ThirdParty/arduino-cli")) {
                Directory.Delete("../../../ThirdParty/arduino-cli",true);
            }

            ZipFile.ExtractToDirectory("../../../ThirdParty/arduino-cli.zip", "../../../ThirdParty/arduino-cli");
            System.Windows.MessageBox.Show("Arduino-cli extracted successfully");
        }
        private async void DownloadDriverZipAndUnpack() {
            if (File.Exists("../../../ThirdParty/digistump-drivers.zip")) {
                if (System.Windows.MessageBox.Show("digistump-drivers.zip already exists. Would you like to overwrite it?", "File Exists", MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                }
            }
            using var client = new HttpClient();
            using (var response = await client.GetAsync("https://github.com/digistump/DigistumpArduino/releases/download/1.6.7/Digistump.Drivers.zip"))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var file = File.OpenWrite("../../../ThirdParty/digistump-drivers.zip")) {
                stream.CopyTo(file);
            }

            if (Directory.Exists("../../../ThirdParty/Digistump Drivers")) {
                Directory.Delete("../../../ThirdParty/Digistump Drivers", true);
            }

            ZipFile.ExtractToDirectory("../../../ThirdParty/digistump-drivers.zip", "../../../ThirdParty/");
            System.Windows.MessageBox.Show("Digistump drivers extracted successfully");
        }
        private void InstallDriverClick(object sender, RoutedEventArgs e) {
            if (File.Exists("../../../ThirdParty/Digistump Drivers/DPinst64.exe")) {
                var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\Digistump Drivers\\DPinst64.exe");
                procStIfo.RedirectStandardOutput = true;
                procStIfo.UseShellExecute = false;
                procStIfo.CreateNoWindow = true;
                var proc = new Process();
                proc.StartInfo = procStIfo;
                proc.Start();
                proc.WaitForExit();
            } else {
                System.Windows.MessageBox.Show("../../../ThirdParty/Digistump Drivers/DPinst64.exe does not exist. Please run step (#1) to download and unpack drivers","Executeable doesn't exist",MessageBoxButton.OK,MessageBoxImage.Error);

            }

        }
        private void ArduinoCLIInitClick(object sender, RoutedEventArgs e) {
            //Commands to run from arduino-cli to make things functional.
            // .\arduino-cli core install
            
            if (!File.Exists("../../../ThirdParty/arduino-cli/arduino-cli.exe")) {
                System.Windows.MessageBox.Show("../../../ThirdParty/arduino-cli/arduino-cli.exe does not exist. Please run step (#1) to download and unpack drivers","Executeable doesn't exist",MessageBoxButton.OK,MessageBoxImage.Error);
            } else {
                ArduinoCLIUpdateIndex();
                ArduinoCLIClientInit();
                ArduinoCLIBoardConfigAdd();
                ArduinoCLIUpdateIndex();
                ArduinoCLICoreInstall();
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                
                if (Directory.Exists(localAppDataPath + "\\Arduino15\\packages\\digistump")) {
                    System.Windows.MessageBox.Show("Digistump AVR boards succesfully installed.");
                }
            }           
        }
        private void ArduinoCLIUpdateIndex() {
            Regex regex = new Regex(@".*(Downloading.*)");
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = true;
            procStIfo.UseShellExecute = false;
            procStIfo.CreateNoWindow = true;
            procStIfo.Arguments = "core update-index";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();

            StreamReader reader = proc.StandardOutput;
            string output = reader.ReadToEnd();
            if (regex.IsMatch(output)) {
                System.Windows.MessageBox.Show(regex.Replace(output, "$1"));
            }

        }
        private void ArduinoCLIClientInit() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Arduino15\\arduino-cli.yaml";
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = true;
            procStIfo.UseShellExecute = false;
            procStIfo.CreateNoWindow = true;
            procStIfo.Arguments = "config init";
            var proc = new Process();
            proc.StartInfo = procStIfo;


            if (File.Exists(path)) {
                if (System.Windows.MessageBox.Show("The arduino cli has already been configured. Would you like to overwrite previous configuration?", "Configuration Detected", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                    File.Delete(path);                    
                    proc.Start();
                    proc.WaitForExit();
                    var reader = proc.StandardOutput;
                    var output = reader.ReadToEnd();
                    System.Windows.MessageBox.Show(output);
                }
            } else {
                proc.Start();
                proc.WaitForExit();
                var reader = proc.StandardOutput;
                var output = reader.ReadToEnd();
                System.Windows.MessageBox.Show(output);
            }
        }
        private void ArduinoCLIBoardConfigAdd() {
            Regex re = new Regex(@"package_digistump_index.json");
            bool skip = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Arduino15\\arduino-cli.yaml";
            if (File.Exists(path)) {
                string[] lines = System.IO.File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++) {
                    if (re.IsMatch(lines[i])) {
                        skip = true;
                        break;
                    }
                }
            }
            if (!skip) {
                var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
                procStIfo.RedirectStandardOutput = true;
                procStIfo.UseShellExecute = false;
                procStIfo.CreateNoWindow = true;
                procStIfo.Arguments = "config add board_manager.additional_urls https://raw.githubusercontent.com/digistump/arduino-boards-index/master/package_digistump_index.json";
                var proc = new Process();
                proc.StartInfo = procStIfo;
                proc.Start();
                proc.WaitForExit();
            } else {
                System.Windows.MessageBox.Show("skipped");
            }
        }
        private void ArduinoCLICoreInstall() {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (Directory.Exists(localAppDataPath +"\\Arduino15\\packages\\digistump")) {
                if (System.Windows.MessageBox.Show("Digistump core boards already installed. Would you like to overwrite this installation?","Directory exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    return;
                } else {
                    var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
                    procStIfo.RedirectStandardOutput = true;
                    procStIfo.UseShellExecute = false;
                    procStIfo.CreateNoWindow = true;
                    procStIfo.Arguments = "core uninstall digistump:avr";
                    var proc = new Process();
                    proc.StartInfo = procStIfo;
                    proc.Start();
                    proc.WaitForExit();
                    Directory.Delete(localAppDataPath + "\\Arduino15\\packages\\digistump", true);


                    var procStIfo2 = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
                    procStIfo2.RedirectStandardOutput = true;
                    procStIfo2.UseShellExecute = false;
                    procStIfo2.CreateNoWindow = true;
                    procStIfo2.Arguments = "core install digistump:avr";
                    var proc2 = new Process();
                    proc2.StartInfo = procStIfo2;
                    proc2.Start();
                    proc2.WaitForExit();

                }
            } else {

                var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
                procStIfo.RedirectStandardOutput = true;
                procStIfo.UseShellExecute = false;
                procStIfo.CreateNoWindow = true;
                procStIfo.Arguments = "core install digistump:avr";
                var proc = new Process();
                proc.StartInfo = procStIfo;
                proc.Start();
                proc.WaitForExit();

            }

        }
        private void CompileAndUpload(object sender, RoutedEventArgs e) {
            if (!File.Exists("..\\..\\..\\payload_out\\payload_out.ino")) {
                System.Windows.MessageBox.Show("payload_out.ino file not found. Run \"Payload Options: Configure\" on the landing screen.", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string port = GetCommPort();
            if (port == "") {
                return;
            }
            if (!CompileInoFile()) {
                System.Windows.MessageBox.Show("Error during ino file compile.");
                return;
            }
            UploadToDevice(port);
        }
        private string GetCommPort() {
            char[] delims = new[] { '\r', '\n' };
            string result = "";
            Regex usb_re = new Regex(@"(.*)\(USB\)(.*)");
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = true;
            procStIfo.UseShellExecute = false;
            procStIfo.CreateNoWindow = true;
            procStIfo.Arguments = "board list";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();
            StreamReader reader = proc.StandardOutput;
            string output = reader.ReadToEnd();

            string[] lines = output.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++) {
                if (usb_re.IsMatch(lines[i])) {
                    string[] space_split = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    result = space_split[0];
                    break;
                }            
            }

            if (result == "" || result == null) {
                System.Windows.MessageBox.Show("Internal error, could not get communications port.");
                return "";
            }
            return result;
        }
        private bool CompileInoFile() {
            char[] delims = new[] { '\r', '\n' };
            Regex re = new Regex("Sketch uses");
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = true;
            procStIfo.UseShellExecute = false;
            procStIfo.CreateNoWindow = true;
            procStIfo.Arguments = "compile -b digistump:avr:digispark-tiny ..\\..\\..\\payload_out\\payload_out.ino";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();
            StreamReader reader = proc.StandardOutput;
            string output = reader.ReadToEnd();
            string[] lines = output.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++) {
                if (re.IsMatch(output)) {
                    return true;
                }
            }
            return false;
        }
        private void UploadToDevice(string port) {
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = false;
            procStIfo.UseShellExecute = true;
            procStIfo.CreateNoWindow = false;
            procStIfo.Arguments = "upload -p "+ port + " -b digistump:avr:digispark-tiny ..\\..\\..\\payload_out\\payload_out.ino";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();

        }

    }
}
