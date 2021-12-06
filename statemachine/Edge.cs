using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class Edge
    {
        public string id;
        public string name;
        public string type;
        public string source;
        public string target;

        public Edge(string Id, string Name, string Type, string Source, string Target)
        {
            id = Id;
            name = Name;
            type = Type;
            source = Source;
            target = Target;
        }
    }
}
