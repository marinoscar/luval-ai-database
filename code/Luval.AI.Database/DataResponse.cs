using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public class DataResponse
    {
        public string Response { get; set; }
        public string HtmlChart { get; set; }
        public string SqlQuery { get; set; }
        public Dataset Data { get; set; }
    }
}
