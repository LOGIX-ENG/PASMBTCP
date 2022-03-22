using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASMBTCP.Device
{
    public class Client
    {
        // Properties
        public string? Name { get; set; }
        public string? IPAddress { get; set; }
        public int Port { get; set; }
        public int ConnectTimeout { get; set; }
        public int ReadWriteTimeout { get; set; }
    }
}
