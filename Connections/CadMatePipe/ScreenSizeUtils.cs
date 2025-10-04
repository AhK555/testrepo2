using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSVLibUtils
{
    public static class ScreenSizeUtils
    {
        public static Point2d GetCurrentViewSize()
        {
            
            //Get current view height
            double h = (double)Application.GetSystemVariable("VIEWSIZE");

            //Get current view width,
            //by calculate current view's width-height ratio
            Point2d screen = (Point2d)Application.GetSystemVariable("SCREENSIZE");
            double w = h * (screen.X / screen.Y);

            return new Point2d(w, h);
        }

        public static Point3dCollection GetScreenCoordinate(Document dwg)
        {

            Point2d vSize = GetCurrentViewSize();
            double w = vSize.X ;
            double h = vSize.Y;
            
            //Get current view's centre.
            //Note, the centre point from VIEWCTR is in UCS and
            //need to be transformed back to World CS
            var cent = ((Point3d)Application.GetSystemVariable("VIEWCTR")).
                TransformBy(dwg.Editor.CurrentUserCoordinateSystem);

            var minPoint = new Point3d(cent.X - w / 2.0, cent.Y - h / 2.0,0);
            var maxPoint = new Point3d(cent.X + w / 2.0, cent.Y + h / 2.0,0);

            var screenCoordinates = new Point3dCollection
            {
                (minPoint),
                (maxPoint),
            };

            return screenCoordinates;
        }

        public static double DynamicTextHeight(double dimLineSize)
        {
            var currentViewSize = ScreenSizeUtils.GetCurrentViewSize();
            var screenWidth = currentViewSize.X;
            var screenHeight = currentViewSize.Y;
            //left bottom
            //var minScreenPoint = _screenSize.GetScreenCoordinate()[0];
            //right top
            //var maxScreenPoint = _screenSize.GetScreenCoordinate()[1];

            return screenWidth / 300 * dimLineSize;
        }

    }
    
}
