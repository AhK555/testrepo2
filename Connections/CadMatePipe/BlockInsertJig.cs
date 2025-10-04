using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.GraphicsInterface;
using Gssoft.Gscad.Runtime;

namespace PipeApp
{
    public class BlockInsertJig : DrawJig
    {
        private BlockReference _blockRef;
        private Point3d _position;

        public BlockInsertJig(BlockReference blockRef)
        {
            _blockRef = blockRef;
            _position = Point3d.Origin;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigOpts = new JigPromptPointOptions("\nSpecify insertion point:");
            jigOpts.UserInputControls =
                UserInputControls.Accept3dCoordinates |
                UserInputControls.NullResponseAccepted;

            PromptPointResult res = prompts.AcquirePoint(jigOpts);

            if (res.Status != PromptStatus.OK)
                return SamplerStatus.Cancel;

            if (res.Value.IsEqualTo(_position))
                return SamplerStatus.NoChange;

            _position = res.Value;
            _blockRef.Position = res.Value;
            return SamplerStatus.OK;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            if (_blockRef != null)
            {
                _blockRef.Position = _position;
                draw.Geometry.Draw(_blockRef);
            }
            return true;
        }

        public Point3d Position => _position;
        public BlockReference BlockRef => _blockRef;
    }
}
