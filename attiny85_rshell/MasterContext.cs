using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace attiny85_rshell { 
    internal class MasterContext {
        public string TargetServerIp;
        public MasterContext(string TarServerIp) { 
            this.TargetServerIp = TarServerIp;
        }
    }
}
