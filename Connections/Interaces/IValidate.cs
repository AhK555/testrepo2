using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadMatePipe.Connections
{
    internal interface IValidate
    {
        List<IConnValidator> Validators { get; }
        void Validate();
    }
}
