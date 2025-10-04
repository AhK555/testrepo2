using NSVLIBConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadMatePipe.Connections
{
    internal interface IPipe
    {
        void AddAsSubscriber();
        void RemoveAsSubscriber();
        ConnectionPipeData PipeData { get; } // TODO: fix this after renaming the PipeData in NSVLIBConstants
        List<IConnection> Subscribers { get; }
        bool HasBeenUpdated { get; }
        void Notify();
    }
}
