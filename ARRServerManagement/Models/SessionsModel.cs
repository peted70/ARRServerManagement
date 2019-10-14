using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public class SessionsModel
    {
        public SessionRoot Root { get; set; }
    }

    public class SessionRoot
    {
        public Session[] sessions { get; set; }
    }


    public class Session
    {
        public string message { get; set; }
        public string sessionElapsedTime { get; set; }
        public string sessionHostname { get; set; }
        public string sessionId { get; set; }
        public string sessionIp { get; set; }
        public string sessionMaxLeaseTime { get; set; }
        public string sessionSize { get; set; }
        public string sessionStatus { get; set; }
    }

}
