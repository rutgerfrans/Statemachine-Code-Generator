using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class Error
    {
        public int id;
        public string type;
        public string name;
        public string disc;
        public string elementid;
        public string elementname;

        public Error(int Id, string Type, string Name, string Disc, string Elementid, string Elementname)
        {
            id = Id;
            type = Type;
            name = Name;
            disc = Disc;
            elementid = Elementid;
            elementname = Elementname;
        }
    }
}
