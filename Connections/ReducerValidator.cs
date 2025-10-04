using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadMatePipe.Connections
{
    internal class ReducerValidator : IConnValidator
    {
        public IConnection Connection { get; private set; }
        public IInvalidAction InvalidAction { get; private set; }

        public ReducerValidator(IConnection connection, IInvalidAction invalidAction)
        {
            Connection = connection;
            InvalidAction = invalidAction;
        }

        public bool Validate()
        {
            bool isValid = true;

            var reducer = Connection as ThreadedReducer;
            if (reducer == null)
                isValid = false;
            else if (reducer.ConnectedPipes.Count != 2)
                isValid = false;
            else
            {
                var diameters = reducer.ConnectedPipes.Select(p => p.PipeData.Size).ToList();
                if (Math.Abs(diameters[0] - diameters[1]) < 0.0001)
                    isValid = false;
            }

            if (!isValid)
                InvalidAction.Action();

            return isValid;
        }
    }
}

