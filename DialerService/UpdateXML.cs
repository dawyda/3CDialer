using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialerService
{
    public class UpdateXML
    {
        public session Session { get; set; }
        public string CallId { get; set; }
        public bool Status { get; set; }
        public string Notes { get; set; }
    }
    public class UpdateResponse
    {
        public string response { get; set; }
    }
}
