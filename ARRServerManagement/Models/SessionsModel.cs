using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
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
        public string message { get; set; } = "created by ARR Server Management Web App";
        public TimeSpan sessionElapsedTime { get; set; }
        public string sessionHostname { get; set; }
        public string sessionId { get; set; }
        public string sessionIp { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan sessionMaxLeaseTime { get; set; } = new TimeSpan(2, 0, 0);

        public SessionSize sessionSize { get; set; } = SessionSize.small;
        public string sessionStatus { get; set; }
    }

    public class CreateModel
    {
        public List<string> Models { get; set; } = new List<string>() { "builtin://UnitySampleModel", "blah" };

        public SessionDescriptor Session { get; set; } = new SessionDescriptor();
    }

    public class SessionDescriptor
    {
        [Display(Name ="Max. Lease Time")]
        [DataType(DataType.Time)]
        public TimeSpan maxLeaseTime { get; set; } = new TimeSpan(2, 0, 0);

        public List<string> models { get; set; } = new List<string>();

        public SessionSize size { get; set; } = SessionSize.small;
    }
}

namespace ARRServerManagement
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SessionSize
    {
        small,
        big,
    }
}