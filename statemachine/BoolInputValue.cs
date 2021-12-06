using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class BoolInputValue : InputValue
    {
        public bool value = false;

        public BoolInputValue(string CurrentnodeId, string Name, bool Value) :base(CurrentnodeId,Name)
        {     
            value = Value;
        }
    }
}
