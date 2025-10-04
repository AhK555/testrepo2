using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using NSVLIBConstants;
using NSVLibUtils;
using System;
using System.Windows.Forms;
using Application = Gssoft.Gscad.ApplicationServices.Application;
using Line = Gssoft.Gscad.DatabaseServices.Line;

namespace CadMatePipe.Connections
{
    public class ConnectionPipeData
    {
        public Line PipeLine { get; }
        public double Size { get; }

        public ConnectionPipeData(Line ln) 
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            this.PipeLine = ln.Clone() as Line;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                this.Size = ReadPipeSize(tr, ln);
                tr.Commit();
            }
        }

        private double ReadPipeSize(Transaction tr, Line ln)
        {
            if (ln.ObjectId == null)
                throw new Exception("Line dosent have object id");
            var sizeData = XDataUtil.ReadXData(tr, ln.ObjectId, "NSVPipe", "size");
            sizeData = sizeData.Replace("\"", "");
            sizeData = sizeData.Replace("\'", "");
            double size =
                FractionParser.ParseMixedNumber(sizeData);

            return size;
        }

        public ConnectionPipeData(Transaction tr, Line ln) 
        {
            this.PipeLine = ln.Clone() as Line;
            this.Size = ReadPipeSize(tr, ln);
        }

        public ConnectionPipeData(double size, Line pipeLine) 
        {
            this.PipeLine = pipeLine.Clone() as Line;
            this.Size = size;
        }
    }
}