using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace attiny85_rshell { 
    internal class MasterClientConf {
        [JsonProperty("targetserverIp")]
        public string TargetServerIp { get; set; }
        [JsonProperty("targetserverport")]
        public string TargetServerPort { get; set; }

        [JsonConstructor]
        public MasterClientConf(string targetserverIp, string targetserverport)
        {
            this.TargetServerIp = targetserverIp;
            this.TargetServerPort = targetserverport;
        }
    }
}
