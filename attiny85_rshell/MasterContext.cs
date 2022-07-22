using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace attiny85_rshell { 
    internal class MasterContext {
        public string TargetServerIp { get; set; }
        public string TargetServerPort { get; set; }
        public MasterContext(string tarServerIp, string targetServerPort)
        {
            this.TargetServerIp = tarServerIp;
            TargetServerPort = targetServerPort;
        }
    }
}
