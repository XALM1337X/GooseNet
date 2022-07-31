using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace attiny85_rshell { 
    internal class MasterClientConf {
        public string TargetServerIp { get; set; }
        public string TargetServerPort { get; set; }
        public MasterClientConf(string tarServerIp, string targetServerPort)
        {
            this.TargetServerIp = tarServerIp;
            TargetServerPort = targetServerPort;
        }
    }
}
