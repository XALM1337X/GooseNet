/*
 * Goose Net
 * Goose net is an open source hardware attack botnet written in c#/processing
 * and is used in conjunction with a bad usb at_tiny85 arduino usb keyboard emulator for use in a penetration
 * testing environment.
 * 
 * GNU General Public License (GPL) version 3
 * 
 * Copyright (c) 2023 Anthony Logan Mitchell
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */

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
