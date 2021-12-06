using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class Relation
    {
        public Node next;
        public string source;
        public string name;
        public string id;
        public bool valid = false;
        public List<object> values = new List<object>();

        public Relation(Node Next, string Source, string Name,string Id)
        {
            id = Id;
            source = Source;
            name = Name;
            next = Next;
        }

        public void AddBoolInputValues(string id, string name, bool value)
        {
            BoolInputValue boolinputvalue = new BoolInputValue(id, name, value);
            values.Add(boolinputvalue);
        }

        public void AddIntInputValues(string id, string name, int value)
        {
            IntInputValue intinputvalue = new IntInputValue(id, name, value);
            values.Add(intinputvalue);
        }
    }
}
