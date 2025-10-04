using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeApp
{
    public class CrossSymbol
    {
        public (BlockReference blk1, BlockReference blk2) Pair { get; protected set; }
        public Line CorrespondingPipe { get; protected set; }
        public CrossSymbol((BlockReference blk1, BlockReference blk2) blockPair, Line pipe)
        {
            this.Pair = blockPair;
            CorrespondingPipe = pipe;
        }

        internal CrossSymbol Flip()
        {
            this.Pair = (Pair.blk2, Pair.blk1);
            return this;
        }
    }
}
