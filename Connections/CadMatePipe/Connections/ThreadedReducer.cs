using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Gssoft.Gscad.Geometry;

namespace CadMatePipe.Connections
{
    internal class ThreadedReducer : IConnection
    {
        public Point3d Point { get; private set; }
        public List<IPipe> ConnectedPipes { get; private set; }

        public ThreadedReducer(Point3d point , List<IPipe> pipes)
        {
            Point = point;
            ConnectedPipes = pipes;
        }

        public void Draw()
        {
            // TODO: Custom drawing logic for reducer
        }

        public void Update()
        {
            // 1️⃣ Run the reducer validator
            var invalidAction = new RemoveInvalidReducerAction(this);
            var validator = new ReducerValidator(this, invalidAction);

            if (!validator.Validate())
            {
                return;
            }

            Draw();
        }

        public void Subscribe(IPipe pipe)
        {
            if (!ConnectedPipes.Contains(pipe))
                ConnectedPipes.Add(pipe);
        }

        public void Unsubscribe(IPipe pipe)
        {
            ConnectedPipes.Remove(pipe);
        }

        public void OnNotify()
        {
            // TODO: IDK
        }
    }
}
