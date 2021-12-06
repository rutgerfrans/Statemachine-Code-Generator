using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class InputValue
    {
        public string currentnodeid;
        public string name;
        public List<string> currentnodeids = new List<string>();

        public InputValue(string CurrentnodeId, string Name)
        {
            currentnodeid = CurrentnodeId;
            name = Name;
        }
    }
}
