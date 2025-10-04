using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Runtime;
using PipeApp;
using System;
using System.Collections.Generic;
using System.Linq;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace Pipe
{
    public abstract class SuppressOsnapOverrule : OsnapOverrule
    {
        public SuppressOsnapOverrule()
        {
            AddOverrule(RXClass.GetClass(typeof(Entity)), this, true);
        }

        /// <summary>
        /// Override in a derived type and return a value
        /// indicating if the given entity can be osnapped to.
        /// </summary>

        public abstract bool CanOsnap(Entity entity, ObjectSnapModes osnapMode);

        public override void GetObjectSnapPoints(Entity entity, ObjectSnapModes snapMode, IntPtr gsSelectionMark, Point3d pickPoint, Point3d lastPoint, Matrix3d viewTransform, Point3dCollection snapPoints, IntegerCollection geometryIds)
        {
            if (CanOsnap(entity, snapMode))
                base.GetObjectSnapPoints(entity, snapMode, gsSelectionMark, pickPoint, lastPoint, viewTransform, snapPoints, geometryIds);
        }

        public override void GetObjectSnapPoints(Entity entity, ObjectSnapModes snapMode, IntPtr gsSelectionMark, Point3d pickPoint, Point3d lastPoint, Matrix3d viewTransform, Point3dCollection snapPoints, IntegerCollection geometryIds, Matrix3d insertionMat)
        {
            if (CanOsnap(entity, snapMode))
                base.GetObjectSnapPoints(entity, snapMode, gsSelectionMark, pickPoint, lastPoint, viewTransform, snapPoints, geometryIds, insertionMat);
        }

        protected override void Dispose(bool A_0)
        {
            RemoveOverrule(RXClass.GetClass(typeof(Entity)), this);
            base.Dispose(A_0);
        }
    }

    public class SprinklerSnap : SuppressOsnapOverrule
    {
        private List<string> snappingBlocks = new List<string>();
        private List<string> layers = new List<string>()
        {
            NSVLIBConstants.Enums.NSVLayers.NSVMainPipe.ToString(),
            NSVLIBConstants.Enums.NSVLayers.NSVBranchPipe.ToString()
        };
        public SprinklerSnap() 
        {
            snappingBlocks.AddRange(NSVLibUtils.BlockDefs.GetNSVValveBlocks());
            snappingBlocks.AddRange(NSVLibUtils.BlockDefs.GetUserValveBlocks());
            snappingBlocks.AddRange(InputData.SprinklerNames);
        }
        public override bool CanOsnap(Entity entity, ObjectSnapModes osnapMode)
        {
            if (entity is BlockReference br)
            {
                return snappingBlocks.Contains(br.Name);
            }
            else if(entity is Line l)
            {
                return layers.Contains(l.Layer);
            }
            return false;
        }
    }
}