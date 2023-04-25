using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public class Dataset
    {
        public string Csv { get; set; }
        public string Array { get; set; }
        public IEnumerable<IDictionary<string, object>> Data { get; set; }
    }
}
