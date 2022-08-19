using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace attiny85_rshell {
    public class MasterState {
        public int GlobalServerID { get; set; }
        public MasterClient? MasterClientObj;
        public bool IsFirewallInit { get; set; }

        public MasterState() {
            this.IsFirewallInit = false;
            this.MasterClientObj = null;
            this.GlobalServerID = 0;
        }
    }
}
