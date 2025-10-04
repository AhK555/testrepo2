using Gssoft.Gscad.DatabaseServices;

namespace PipeApp
{
    internal class Connection
    {
        public double DeltaZ { get; }
        public BlockReference BlockRef { get; }
        public ConnectionType Type { get; }

        public Connection(BlockReference blockRef, double deltaZ, ConnectionType connectionType)
        {
            this.BlockRef = blockRef;
            this.DeltaZ = deltaZ;
            this.Type = connectionType;
        }
    }
}