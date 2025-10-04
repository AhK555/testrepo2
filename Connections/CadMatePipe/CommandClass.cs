using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Runtime;
using Gssoft.Gscad.EditorInput;
using System.Security.Policy;
using Gssoft.Gscad.Geometry;
using System.IO;
using System.Windows;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using Gssoft.Gscad.DatabaseServices.Filters;
using NSVLibUtils;
using static System.Net.WebRequestMethods;
using System.ComponentModel.Design;
using System.Windows.Forms;
using NSVLIBInitialize.Layer;
using NSVLIBConstants.Enums;
using PipeApp.DrawPipe;
using Converter = Gssoft.Gscad.Runtime.Converter;
using QuikGraph.Algorithms.Exploration;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Linq.Expressions;
using System.Net.NetworkInformation;

using CadMatePipe;
using Application = Gssoft.Gscad.ApplicationServices.Application;
using Gile.AutoCAD.R20.Geometry;

namespace PipeApp
{
    public class CommandClass
    {
        /// <summary>
        /// Draw pipe along its pretty pipe
        /// </summary>
        [CommandMethod("NPI")]
        public void BreakPipeAddPrettyPipe()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            try
            {
                NSVLIBInitialize.Initialize.INITIALIZE();
            }
            catch (System.Exception ex)
            {
#if !DEBUG
                ed.WriteMessage("\nError is: " + ex.Message);
                return;
#endif
            }

            LocalUtils.ResetUCSToWorld();
            var lwDisplay = ACAD.GetSystemVariable("LWDISPLAY");
            var showForm = false;
            var osmode = (Int16)ACAD.GetSystemVariable("OSMODE");
            //1 endpoint
            //32 intersection
            //64 insertion point
            //128 perpendicular
            //2048 apparent intersection
            // 512 nearest
            //var mode = 512 + 64 + 1
            ACAD.SetSystemVariable("OSMODE", 512 + 64 + 1);
            var osoptions = ACAD.GetSystemVariable("OSOPTIONS");
            ACAD.SetSystemVariable("OSOPTIONS", 1);
            if (InputData.SprinklerNames.Count() == 0)
                showForm = true;
            try
            {
                Point3d? startPoint = null;
                do
                {
                    if (showForm)
                    {
                        var form = new DrawPipeForm(doc);
                        form.ShowDialog();
                        //if (!form.OK)
                        //    return;

                        showForm = false;
                    }
                    if (InputData.UseMainPipeLineWeight)
                    {
                        LayerUtils.ChangeLineWeight(NSVLayers.NSVMainPipe.ToString(),
                            InputData.BranchLineWeight);
                    }
                    else
                    {
                        LayerUtils.ChangeLineWeight(NSVLayers.NSVMainPipe.ToString(),
                            LineWeight.LineWeight000);
                    }

                    if (InputData.UseBranchPipeLineWeight)
                    {
                        LayerUtils.ChangeLineWeight(NSVLayers.NSVBranchPipe.ToString(),
                            InputData.BranchLineWeight);
                    }
                    else
                    {
                        LayerUtils.ChangeLineWeight(NSVLayers.NSVBranchPipe.ToString(),
                            LineWeight.LineWeight000);
                    }

                    if (InputData.UseBranchPipeLineWeight || InputData.UseMainPipeLineWeight)
                        ACAD.SetSystemVariable("LWDISPLAY", 1);

                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        var jigPipe = new JigPipe(doc, startPoint);
                        var jigResult = jigPipe.Move();
                        if (jigResult.Status == PromptStatus.OK)
                        {
                            var topBottomStraightCutter = new TopBottomStraightCut(jigPipe.Pipe);
                            IEnumerable<Line> pipeList = topBottomStraightCutter.Init();

                            foreach (var pipeLine in pipeList)
                            {
                                var cutter = new PipeCutter(pipeLine);
                                var crossSymbols = CrossPipeSymbol.GetPairSymbol(pipeLine);
                                cutter.BreakPreviousPipesAtPipeIntersections();
                                cutter.BreakCurrentPipeAtSprinklerIntersections();
                                startPoint = cutter.NextStartPoint;
                                PipeSizer.AssingSizeToNewPipes(cutter.NewPipes.ToArray());
                                var visualizer = new PipeVisualizer(cutter.NewPipes);
                                visualizer.AddGraphicalPipes();
                                var (symbolsOnPipe, symbolsOutOfPipe) 
                                    = SymbolUtils.SeperateSymbols(crossSymbols, pipeLine);
                                crossSymbols = SymbolUtils.GroupSymbols(symbolsOnPipe).ToList();
                                crossSymbols.AddRange(symbolsOutOfPipe);
                                visualizer.EnhanceElevationOnBreakPipe(crossSymbols);
                                visualizer.DrawSymbols(crossSymbols);
                            }
                            tr.Commit();
                        }
                        else if (jigResult.Status == PromptStatus.Cancel || jigResult.Status == PromptStatus.None)
                        {
                            tr.Abort();
                            return;
                        }
                        else if (jigResult.StringResult == "F")
                        {
                            tr.Abort();
                            showForm = true;
                        }
                    }
                } while (true);
            }
            catch (System.Exception ex)
            {
                throw;
                ed.WriteMessage("\n" + ex.Message);
            }
            finally
            {
                ACAD.SetSystemVariable("LWDISPLAY", lwDisplay);
                ACAD.SetSystemVariable("OSMODE",osmode);
                ACAD.SetSystemVariable("OSOPTIONS", osoptions);
                DOT.BringPipesToBottom();
                LocalUtils.ResetUCSToUser();
            }
        }
        [CommandMethod("readPipedata")]
        public void ReadPipeSize()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            try
            {
                NSVLIBInitialize.Initialize.INITIALIZE();
            }
            catch (System.Exception ex)
            {
#if !DEBUG
                ed.WriteMessage("\nError is: " + ex.Message);
                return;
#endif
            }

            var pipe = ed.GetEntity("\nSelect pipe:");
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    var data = NSVLibUtils.XDataUtil.ReadXData(tr, pipe.ObjectId, "NSVPipe", "size");
                    ed.WriteMessage("\nSize is: " + data);
                }
                catch (System.Exception)
                {
                    ed.WriteMessage("\nError reading pipe diameter");
                }
                try
                {
                    var data = NSVLibUtils.XDataUtil.ReadXData(tr, pipe.ObjectId, "NSVPipe", "dia");
                    ed.WriteMessage("\nPipe Schedule is: " + data);
                }
                catch (System.Exception)
                {
                    ed.WriteMessage("\nError reading pipe schedule");
                }
                try
                {
                    var data = XDataUtil.ReadXData(tr, pipe.ObjectId, "NSVPipe", "Cfactor");
                    ed.WriteMessage("\nC Factor is: " + data);
                }
                catch (System.Exception)
                {
                    ed.WriteMessage("\nError reading pipe C factor");
                }
            }
        }

        /// <summary>
        /// PrintOnScreen pipe size to pipes
        /// </summary>
        [CommandMethod("NPS")]
        public void PipeSize()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            LocalUtils.ResetUCSToWorld();
            try
            {
                NSVLIBInitialize.Initialize.INITIALIZE();
            }
            catch (System.Exception ex)
            {
#if !DEBUG
                ed.WriteMessage("\nError is: " + ex.Message);
                return;
#endif
            }

            try
            {
                var pipeSizeForm = new PipeSizeForm();
                pipeSizeForm.ShowDialog();
                if (pipeSizeForm.Cancel)
                    return;
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var pipeIds = new ObjectIdCollection();
                    if (InputData.SizingMethod == SizingMethod.Default)
                    {
                        pipeIds = FilterSelection.GetPipes(ed);
                        if (pipeIds is null)
                            return;
                    }
                    else if (InputData.SizingMethod == SizingMethod.PipeScheduling)
                    {
                        PipeScheduling.InputData.Unit = InputData.Unit;
                        var pipeScheduling = new PipeScheduling.PipeSchedulingAssigner();
                        pipeScheduling.Init();
                        pipeIds = pipeScheduling.PipeIDs;
                        if (pipeIds is null)
                            return;

                    }
                    else
                    {
                        //DOT.BringPipesToTop();
                        db.TransactionManager.QueueForGraphicsFlush();

                        var pso = new PromptSelectionOptions();
                        pso.MessageForAdding = "\nSelect pipes to assign this size [R to remove]: ";
                        pso.MessageForRemoval = "\nPress A to add pipes to selection set:";
                        pso.AllowDuplicates = false;
                        pso.RejectObjectsOnLockedLayers = true;

                        var filter = new SelectionFilter(new TypedValue[]
                        {
                                new TypedValue((int)DxfCode.Start ,"LINE"),
                                new TypedValue((int)DxfCode.LayerName , "NSVBranchPipe,NSVMainPipe")
                        });

                        var selectedPipes = ed.GetSelection(pso, filter);
                        if (selectedPipes.Status != PromptStatus.OK)
                        {
                            //DOT.BringPipesToBottom();
                            tr.Commit();
                            return;
                        }

                        foreach (ObjectId id in selectedPipes.Value.GetObjectIds())
                        {
                            var ent = tr.GetObject(id, OpenMode.ForRead) as Line;
                            pipeIds.Add(id);
                        }
                    }
                    var sizer = new PipeSizer(pipeIds);
                    sizer.ShowPipeSize();
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("\n" + ex.Message);
            }
            finally
            {
                DOT.BringPipesToBottom();
                LocalUtils.ResetUCSToUser();
            }
        }

        //        private double _breakPipeDistance;

        //        /// <summary>
        //        /// Add break pipe symbol
        //        /// </summary>
        //        [CommandMethod("NBP")]
        //        public void BreakPipe()
        //        {
        //            var doc = ACAD.DocumentManager.MdiActiveDocument;
        //            var db = doc.Database;
        //            var ed = doc.Editor;
        //            try
        //            {
        //                NSVLIBInitialize.Initialize.INITIALIZE();
        //            }
        //            catch (System.Exception ex)
        //            {
        //#if !DEBUG
        //                ed.WriteMessage("\nError is: " + ex.Message);
        //                return;
        //#endif
        //            }
        //            try
        //            {
        //                PromptResult jigResult;
        //                do
        //                {
        //                    using (var tr = db.TransactionManager.StartTransaction())
        //                    {
        //                        var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
        //                        var peo = new PromptEntityOptions("\nSelect pretty pipe");
        //                        peo.SetRejectMessage("\nThis opperation is only valid on pretty pipes");
        //                        peo.AddAllowedClass(typeof(Polyline), true);
        //                        var per = ed.GetEntity(peo);

        //                        if (per.Status != PromptStatus.OK)
        //                            return;

        //                        var prettyPipe = tr.GetObject(per.ObjectId, OpenMode.ForRead) as Polyline;
        //                        if (prettyPipe.Layer == "NSVPrettyPipe")
        //                        {
        //                            var jigBreakPipe = new JigBreakPipe(prettyPipe);
        //                            if (_breakPipeDistance == 0)
        //                                jigResult = jigBreakPipe.Move();
        //                            else
        //                                jigResult = jigBreakPipe.Move(_breakPipeDistance);

        //                            if (jigResult.Status == PromptStatus.Cancel)
        //                                return;
        //                            else
        //                            {
        //                                _breakPipeDistance = jigBreakPipe.Distance;
        //                                currentSpace.AppendEntity(jigBreakPipe.BlkRefPair.Item1);
        //                                tr.AddNewlyCreatedDBObject(jigBreakPipe.BlkRefPair.Item1, true);
        //                                currentSpace.AppendEntity(jigBreakPipe.BlkRefPair.Item2);
        //                                tr.AddNewlyCreatedDBObject(jigBreakPipe.BlkRefPair.Item2, true);
        //                                PretyPipe.TrimOnBreakPipes(db, jigBreakPipe.BlkRefPair);
        //                                tr.Commit();
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ed.WriteMessage("\nThis opperation is only valid on pretty pipes");
        //                        }
        //                        tr.Abort();
        //                    }
        //                } while (true);
        //            }
        //            catch (Gssoft.Gscad.Runtime.Exception)
        //            {
        //                ACAD.ShowAlertDialog("Drawing is unsuccesfully");
        //            }

        //        }
        //        /// <summary>
        //        /// Convert Autosprink pipes and connections to line objects
        //        /// </summary>
        //        [CommandMethod("NPLN")]
        //        public void AutoSprinkBlockToLine()
        //        {
        //            var doc = ACAD.DocumentManager.MdiActiveDocument;
        //            try
        //            {
        //                NSVLIBInitialize.Initialize.INITIALIZE();
        //            }
        //            catch (System.Exception ex)
        //            {
        //#if !DEBUG
        //                doc.Editor.WriteMessage("\nError is: " + ex.Message);
        //                return;
        //#endif
        //            }
        //            try
        //            {
        //                var exploder = new Exploder();
        //                exploder.RemovePipes();
        //                exploder.RemoveFittings();
        //                exploder.DrawObjects();
        //            }
        //            catch (System.Exception ex)
        //            {
        //                doc.Editor.WriteMessage("\nError occured: "+ex.Message);
        //            }
        //        }


        //        /// <summary>
        //        /// Get water supply and draw 3dView of pipe
        //        /// </summary>
        //        [CommandMethod("N3DP")]
        //        public void To3d()
        //        {
        //            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
        //            var db = Application.DocumentManager.MdiActiveDocument.Database;
        //            var doc = ACAD.DocumentManager.MdiActiveDocument;
        //            try
        //            {
        //                NSVLIBInitialize.Initialize.INITIALIZE();
        //                var threeDPipeLayer = new Layer(
        //                    NSVLayers.NSV3dPipe.ToString(),
        //                    1,
        //                    LineWeight.LineWeight060);
        //                    CreateLayers.NameThem(db, new List<Layer> { threeDPipeLayer });
        //            }
        //            catch (System.Exception ex)
        //            {
        //#if (!DEBUG)
        //                doc.Editor.WriteMessage("\nError is: " + ex.Message);
        //                return;
        //#endif
        //            }

        //            var ppr = ed.GetPoint("\nSelect water supply");
        //            if (ppr.Status != PromptStatus.OK)
        //                return;

        //            var waterSupplyPoint = ppr.Value;
        //            var resultingGraph = NSVLibUtils.To3dPipe.ConvertTo3dPipes.Get3dPipeGraph(waterSupplyPoint);
        //            foreach (var graph in resultingGraph)
        //            {
        //                var graphDrawer = new PipeGraphDrawer(graph);
        //                graphDrawer.DrawGraph();
        //            }
        //        }

        //        /// <summary>
        //        /// Insert connection at pipes
        //        /// </summary>
        //        [CommandMethod("NIC")]
        //        public void InsertConnection()
        //        {
        //            var doc = ACAD.DocumentManager.MdiActiveDocument;
        //            var db = ACAD.DocumentManager.MdiActiveDocument.Database;
        //            var ed = doc.Editor;
        //            try
        //            {
        //                NSVLIBInitialize.Initialize.INITIALIZE();
        //            }
        //            catch (System.Exception ex)
        //            {
        //#if !DEBUG
        //                doc.Editor.WriteMessage("\nError is: " + ex.Message);
        //                return;
        //#endif
        //            }
        //            var connectionLayer = new Layer(nameof(NSVLayers.NSVConnections));
        //            CreateLayers.NameThem(db, new List<Layer> { connectionLayer });

        //            var frm = new RiserSetupForm();
        //            frm.ShowDialog();

        //            if (frm.OK)
        //            {
        //                Point3d pickedPoint = PickAPoint();
        //                List<IPipe> pipeList = new PipeFinder().FindPipe(pickedPoint);
        //                IConnection resultingConnection;

        //                if (InputData.ConnectionType == ConnectionType.RN)
        //                    resultingConnection = new RiserNipple(pickedPoint, pipeList, isFirstTime: false).GetConnection();
        //                else
        //                    resultingConnection = new SprinklerConnection(pickedPoint, pipeList);


        //                var connection = resultingConnection;

        //                //TODO: fix structure
        //                var blk = connection.GetBlock();
        //                bool firstTime = true;

        //                using (Transaction tr = db.TransactionManager.StartTransaction())
        //                {
        //                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

        //                    while(true)
        //                    {
        //                        try
        //                        {
        //                            //TODO : sorry amir, I'll fix this later (later never comes)
        //                            if (!firstTime)
        //                            {
        //                                pickedPoint = PickAPoint();
        //                            }

        //                            firstTime = false;
        //                            pipeList = new PipeFinder().FindPipe(pickedPoint);
        //                            if (connection is SprinklerConnection)
        //                            {
        //                                connection = new SprinklerConnection(pickedPoint, pipeList);
        //                                blk = connection.GetBlock();
        //                                AttributeAssigner.Assign(blk);
        //                                db.TransactionManager.QueueForGraphicsFlush();
        //                                continue;
        //                            }


        //                            var lastToWayPipe = pipeList[0];
        //                            var connectionAssigner = new ConnectionAssigner(pipeList, pickedPoint, isUp: true);

        //                            if (PipeDirectionAnalyser.IsThreeWay(pipeList))
        //                            {
        //                                var pipes = connectionAssigner.ThreeWay(isFirstTime: false);
        //                                if (connection is LookDown ld)
        //                                {
        //                                    connection = new LookDown(pickedPoint, pipes.Item1);
        //                                    lastToWayPipe = pipes.Item1;

        //                                }
        //                                else if (connection is Nipple nipple)
        //                                    connection = new Nipple(pipes.Item1, pipes.Item2);
        //                            }
        //                            else if (PipeDirectionAnalyser.IsFourWay(pipeList))
        //                            {
        //                                var pipes = connectionAssigner.FourWays(isFirstTime: false);
        //                                connection = new Nipple(pipes.Item1, pipes.Item2);

        //                            }
        //                            else if (pipeList.Count == 2 && (
        //                                PipeDirectionAnalyser.IsCorner(pipeList) || 
        //                                PipeDirectionAnalyser.IsTwoWaysStraight(pipeList)))
        //                            {
        //                                //var pipe = connectionAssigner.TwoWays();
        //                                connection = new LookDown(pickedPoint, lastToWayPipe);

        //                            }
        //                            else if (pipeList.Count == 1)
        //                            {
        //                                connection = new LookDown(pickedPoint, pipeList[0]);
        //                                lastToWayPipe = pipeList[0];

        //                            }

        //                            BlockTableRecord ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        //                            blk = connection.GetBlock();
        //                            //ms.AppendEntity(blk);
        //                            //tr.AddNewlyCreatedDBObject(blk, true);
        //                            AttributeAssigner.Assign(blk);
        //                            db.TransactionManager.QueueForGraphicsFlush();

        //                            while (PipeDirectionAnalyser.IsThreeWay(pipeList) || PipeDirectionAnalyser.IsFourWay(pipeList) || (pipeList.Count == 2 && (PipeDirectionAnalyser.IsCorner(pipeList) || PipeDirectionAnalyser.IsTwoWaysStraight(pipeList))))
        //                            {
        //                                PromptKeywordOptions pko = new PromptKeywordOptions("\nPress R to rotate 90 dergree: ");
        //                                pko.Keywords.Add("Rotate");
        //                                pko.AllowNone = true;

        //                                PromptResult res = ed.GetKeywords(pko);

        //                                if (res.Status == PromptStatus.OK)
        //                                {
        //                                    if (PipeDirectionAnalyser.IsThreeWay(pipeList))
        //                                    {
        //                                        var pipes = connectionAssigner.ThreeWay(isFirstTime: false);
        //                                        blk.Erase();
        //                                        if (connection is Nipple nipple)
        //                                        {
        //                                            connection = new LookDown(pickedPoint, pipes.Item1);
        //                                            lastToWayPipe = pipes.Item1;
        //                                        }
        //                                        else if (connection is LookDown ld)
        //                                            connection = new Nipple(pipes.Item1, pipes.Item2);
        //                                    }
        //                                    else if (PipeDirectionAnalyser.IsFourWay(pipeList))
        //                                    {
        //                                        blk.Erase();
        //                                        if (connection is Nipple nipple)
        //                                            connection = new Nipple(nipple.CrossingPipe, nipple.InsertOnPipe);
        //                                    }
        //                                    else
        //                                    {
        //                                        blk.Erase();
        //                                        if(lastToWayPipe == pipeList[0])
        //                                        {
        //                                            connection = new LookDown(pickedPoint, pipeList[1]);
        //                                            lastToWayPipe = pipeList[1];
        //                                        }
        //                                        else
        //                                        {
        //                                            connection = new LookDown(pickedPoint, pipeList[0]);
        //                                            lastToWayPipe = pipeList[0];
        //                                        }
        //                                    }

        //                                    blk = connection.GetBlock();
        //                                    //ms.AppendEntity(blk);
        //                                    //tr.AddNewlyCreatedDBObject(blk, true);
        //                                    AttributeAssigner.Assign(blk);
        //                                    db.TransactionManager.QueueForGraphicsFlush();

        //                                }
        //                                else
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                            if (PipeDirectionAnalyser.IsThreeWay(pipeList))
        //                            {
        //                                var pipes = connectionAssigner.ThreeWay(false);

        //                                if (pipes.Item2 != null)
        //                                {
        //                                    if (isVertical(pipes.Item1.PipeLine) && blk.Rotation % Math.PI == 0 ||
        //                                        !isVertical(pipes.Item1.PipeLine) && blk.Rotation % Math.PI == Math.PI / 2)
        //                                    {
        //                                        PipeTrimer.TrimInsideRiser(pipes.Item2.PipeLine, blk, (connection is LookDown? lastToWayPipe : null));
        //                                    }
        //                                    else
        //                                    {
        //                                        PipeTrimer.TrimInsideRiser(pipes.Item1.PipeLine, blk, (connection is LookDown ? lastToWayPipe : null));
        //                                    }
        //                                }
        //                            }
        //                            else if (PipeDirectionAnalyser.IsFourWay(pipeList))
        //                            {
        //                                var pipes = connectionAssigner.FourWays(false);

        //                                PipeTrimer.TrimInsideRiser(pipes.Item2.PipeLine, blk);
        //                            }
        //                            else if (pipeList.Count == 2 && (
        //                                PipeDirectionAnalyser.IsCorner(pipeList) ||
        //                                PipeDirectionAnalyser.IsTwoWaysStraight(pipeList)))
        //                            {
        //                                var pipe = PipeJoiner.JoinPipes(pipeList);

        //                                PipeTrimer.TrimInsideRiser(pipe[0].PipeLine, blk, (connection is LookDown ? lastToWayPipe : null));
        //                            }

        //                            db.TransactionManager.QueueForGraphicsFlush();
        //                        }
        //                        catch
        //                        {
        //                            break;
        //                        }

        //                    }

        //                    tr.Commit();
        //                }

        //                if (connection == null)
        //                {
        //                    ed.WriteMessage("selected point was not a correct place to put block on");
        //                    return;
        //                }
        //                //AttributeAssigner.Assign(blk);
        //            }
        //        }
        //        private Point3d PickAPoint()
        //        {
        //            Document _doc = ACAD.DocumentManager.MdiActiveDocument;
        //            Editor _ed = _doc.Editor;
        //            var promptOptions = new PromptPointOptions("Select insertion point");
        //            var promptPoint = _ed.GetPoint(promptOptions);
        //            if (promptPoint.Status != PromptStatus.OK)
        //                throw new System.Exception("Prompt Error when picking a point");

        //            return promptPoint.Value;
        //        }

        //        private bool isVertical(Line line)
        //        {
        //            Vector3d dir = line.EndPoint - line.StartPoint;

        //            if (dir.X == 0 && dir.Y != 0)
        //            {
        //                return true;
        //            }
        //            return false;
        //        }

        //        /// <summary>
        //        /// Insert connection at pipes
        //        /// </summary>
        //        [CommandMethod("NAPM")]
        //        public void AssignPipeStatus()
        //        {
        //            var doc = ACAD.DocumentManager.MdiActiveDocument;
        //            var db = ACAD.DocumentManager.MdiActiveDocument.Database;
        //            var ed = doc.Editor;
        //            try
        //            {
        //                NSVLIBInitialize.Initialize.INITIALIZE();
        //            }
        //            catch (System.Exception ex)
        //            {
        //#if !DEBUG
        //                doc.Editor.WriteMessage("\nError is: " + ex.Message);
        //                return;
        //#endif
        //            }

        //            var peo = new PromptSelectionOptions();
        //            peo.MessageForAdding = "Assign Pipe mater";
        //            peo.AllowDuplicates = false;
        //            peo.RejectObjectsOnLockedLayers = false;

        //            var filter = new SelectionFilter(new TypedValue[]
        //            {
        //                new TypedValue((int)DxfCode.Start,"LINE"),
        //                new TypedValue((int)DxfCode.LayerName,$"{NSVLIBConstants.Enums.NSVLayers.NSVMainPipe},{NSVLIBConstants.Enums.NSVLayers.NSVBranchPipe}")
        //            });

        //            var sset = ed.GetSelection(peo, filter);

        //            if(sset.Status == PromptStatus.Cancel)
        //                return;
        //            else if (sset.Status == PromptStatus.Error)
        //            {
        //                sset = ed.SelectAll(filter);
        //                if(sset.Status != PromptStatus.OK)
        //                {
        //                    ed.WriteMessage("\nNo NSV pipe found");
        //                    return;
        //                }
        //            }
        //            if (sset.Status != PromptStatus.OK)
        //            {
        //                ed.WriteMessage("\nNo NSV pipe found");
        //                return;
        //            }

        //            var pko = new PromptKeywordOptions("\nSchedule:");
        //            pko.Keywords.Add("40");
        //            pko.Keywords.Add("30");
        //            pko.Keywords.Add("20");
        //            pko.Keywords.Add("10");
        //            pko.Keywords.Default = "40";
        //            var pkr = ed.GetKeywords(pko);
        //            if (pkr.Status != PromptStatus.OK)
        //                return;
        //            var pipeClass = pkr.StringResult;

        //            pko.Keywords.Clear();
        //            pko.Keywords.Add("Wet");
        //            pko.Keywords.Add("Dry");
        //            pko.Keywords.Default = "Wet";
        //            pko.Message = "\nSystem type:";
        //            pkr = ed.GetKeywords(pko);
        //            if(pkr.Status != PromptStatus.OK)
        //                return;
        //            var cFactor = pkr.StringResult == "Wet"? "120":"100";

        //            using (var tr = db.TransactionManager.StartTransaction())
        //            {
        //                foreach (ObjectId id in sset.Value.GetObjectIds())
        //                {
        //                    var pipe =tr.GetObject(id,OpenMode.ForWrite) as Line;
        //                    var size = NSVLibUtils.XDataUtil.ReadXData(tr, pipe.ObjectId, "NSVPipe", "size");

        //                    XDataUtil.SetXdata(tr, pipe, "NSVPipe", new Dictionary<string, string>
        //                    {
        //                        {"Cfactor",cFactor},
        //                        {"dia",PipeDiameterTable.Schedule40[size] }
        //                    });
        //                }
        //                tr.Commit();
        //            }

        //        }
    }
}
