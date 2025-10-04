using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadMatePipe.Connections
{
    internal interface IConnectionList
    {
        void Init();
        void Add();
        bool Remove();
        IConnection Search(Point3d pt);
        IConnection Search(IConnection connection);
        List<IConnection> ExposeAsList();
    }
}
