using Gssoft.Gscad.Geometry;
using System.Collections.Generic;
using System.Windows.Documents;

namespace CadMatePipe.Connections
{
    internal interface IConnection
    {
        Point3d Point { get; }
        List<IPipe> ConnectedPipes { get; }
        void Draw();
        void Update();
        void Subscribe(IPipe pipe);
        void Unsubscribe(IPipe pipe);
        void OnNotify();
    }
}