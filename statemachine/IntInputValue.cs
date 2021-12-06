using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class IntInputValue : InputValue
    {
        public int value;

        public IntInputValue(string CurrentnodeId, string Name, int Value) :base(CurrentnodeId,Name)
        {
            value = Value;
        }
    }
}
