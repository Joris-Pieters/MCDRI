using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MC.Testing;
using MC.PaperTools;
using MC.Forms;
using MC.Other;

namespace MC.Graphical
{
    public static class Calibration
    {
        /// <summary>
        /// Returns the LOCATION of each ItemAlt (midpoint, relative: 0 to 1)
        /// </summary>
        public static PointF[,] RotateAndCrop(Test test, ref byte[,] image, ref byte[] barCodeBytes, float resize, int searchSize)
        {
            var ansList = new PointF[test.Paper.Blocks.X, test.Paper.Blocks.Y];

            image = Processing.Resize(image, resize);
            List<PointF> calibrationPointsRotate = Rotate(test, ref image, searchSize);

            if (calibrationPointsRotate.Count == test.Paper.CalibrationCircles.X + test.Paper.CalibrationCircles.Y - 1)
            {
                byte[,] imgBarCode = new byte[0, 0];
                List<PointF> calibrationPointsCrop = Crop(test, ref image, ref imgBarCode, searchSize);

                if (calibrationPointsCrop.Count == test.Paper.CalibrationCircles.X + test.Paper.CalibrationCircles.Y - 1)
                {
                    barCodeBytes = BarCode.Read(imgBarCode);

                    // Calculate the answerpoints
                    var xList = new List<float>();
                    for (int i = 0; i < test.Paper.CalibrationCircles.X - 1; i++)
                    {
                        float obsW = calibrationPointsCrop[i + 1].X - calibrationPointsCrop[i].X;
                        int blocksLeft = 7;
                        float pixelsPerBlockW = obsW / blocksLeft;
                        if (i == test.Paper.CalibrationCircles.X - 2)
                        {
                            blocksLeft = test.Paper.Blocks.X - ((test.Paper.CalibrationCircles.X - 2) * 7);
                            pixelsPerBlockW = obsW / (blocksLeft + .5f);
                        }
                        for (int j = 0; j < blocksLeft; j++)
                        {
                            xList.Add(calibrationPointsCrop[i].X + (j * pixelsPerBlockW));
                        }
                    }
                    for (int i = calibrationPointsCrop.Count - 1; i >= test.Paper.CalibrationCircles.X; i--)
                    {
                        float obsH = calibrationPointsCrop[i - 1].Y - calibrationPointsCrop[i].Y;
                        int blocksLeft = 7;
                        float pixelsPerBlockH = obsH / blocksLeft;
                        if (i <= test.Paper.CalibrationCircles.X)
                        {
                            blocksLeft = test.Paper.Blocks.Y - ((test.Paper.CalibrationCircles.Y - 2) * 7);
                            pixelsPerBlockH = obsH / (blocksLeft + 1);
                        }
                        int i2 = (calibrationPointsCrop.Count - i - 1) * 7;
                        for (int ii = 0; ii < blocksLeft; ii++)
                        {
                            float yVal = calibrationPointsCrop[i].Y + (ii * pixelsPerBlockH);
                            for (int j = 0; j < xList.Count; j++)
                            {
                                ansList[j, i2 + ii] = new PointF(xList[j] / image.GetLength(0), yVal / image.GetLength(1));
                            }
                        }
                    }
                }
            }
            return ansList;
        }

        private static List<PointF> Rotate(Test test, ref byte[,] image, int searchSize)
        {
            List<PointF> calPoints = CalibrationPoints(test, image, searchSize);

            if (calPoints.Count == test.Paper.CalibrationCircles.X + test.Paper.CalibrationCircles.Y - 1)
            {
                var corner = CornerPoints(test, calPoints);
                var leftDown = corner[0];
                var rightUp = corner[1];
                var rightDown = corner[2];

                double rotLong = Math.Atan((rightDown.X - rightUp.X) / (rightUp.Y - rightDown.Y));
                if (rightUp.Y > rightDown.Y)
                {
                    rotLong += Math.PI;
                }
                if (rotLong < 0)
                {
                    rotLong += 2 * Math.PI;
                }
                else if (rotLong > 2 * Math.PI)
                {
                    rotLong -= 2 * Math.PI;
                }

                double rotShort = Math.Atan((leftDown.Y - rightDown.Y) / (leftDown.X - rightDown.X));
                if (leftDown.X > rightDown.X)
                {
                    rotShort += Math.PI;
                }
                if (rotShort < 0)
                {
                    rotShort += 2 * Math.PI;
                }
                else if (rotShort > 2 * Math.PI)
                {
                    rotShort -= 2 * Math.PI;
                }
                double rotation = AverageAngle(rotLong, rotShort);
                
                Size oldSize = new Size(image.GetLength(0), image.GetLength(1));                
                image = Processing.Rotate(image, -rotation);

                // Crop after rotation is not necessary but makes following fase of exact cropping slightly faster (easier to find calPoints). However, might be wrong when rotation is too high.
                if (Math.Abs(rotation) < 1)
                {
                    image = Processing.Crop(image, new Rectangle((image.GetLength(0) - oldSize.Width) / 2, (image.GetLength(1) - oldSize.Height) / 2, oldSize.Width, oldSize.Height));
                }
              
            }     
            return calPoints;
        }

        private static List<PointF> Crop(Test test, ref byte[,] image, ref byte[,] imgBarCode, int searchSize)
        {
            List<PointF> calPoints = CalibrationPoints(test, image, searchSize);

            if (calPoints.Count > 0)
            {
                PointF leftDown = new PointF(calPoints[0].X, calPoints[0].Y);
                PointF rightUp = new PointF(calPoints[calPoints.Count - 1].X, calPoints[calPoints.Count - 1].Y);                

                RectangleF cropRectangle = new RectangleF(leftDown.X, rightUp.Y, rightUp.X - leftDown.X, leftDown.Y - rightUp.Y);



                float pixelsPerBlockWidth = cropRectangle.Width / (test.Paper.Blocks.X + .5f);
                float pixelsPerBlockHeight = cropRectangle.Height / (test.Paper.Blocks.Y + 1);

                cropRectangle.X -= pixelsPerBlockWidth / 2;
                cropRectangle.Y -= pixelsPerBlockHeight / 2;
                cropRectangle.Width -= pixelsPerBlockWidth / 2;
                cropRectangle.Height -= pixelsPerBlockHeight;

                imgBarCode = Processing.Crop(image, new RectangleF(
                    calPoints[0].X + (float)(.6 * pixelsPerBlockWidth),
                    cropRectangle.Bottom + (float)(.15 * pixelsPerBlockWidth), 
                    calPoints[1].X - calPoints[0].X - (float)(1.2 * pixelsPerBlockWidth),
                    pixelsPerBlockHeight + (float)(.6 * pixelsPerBlockWidth)
                    ));
                image = Processing.Crop(image, cropRectangle);
                for (int i = 0; i < calPoints.Count; i++)
                {
                    calPoints[i] = new PointF(calPoints[i].X - leftDown.X + (pixelsPerBlockWidth / 2), calPoints[i].Y - rightUp.Y + (pixelsPerBlockHeight / 2));
                }
            }
            return calPoints;
        }

        private static List<PointF> CalibrationPoints(Test test, byte[,] image, int searchSize)
        {
            List<PointF> calPoints = new List<PointF>();
            bool solutionFound = false;

            int max = 0;
            int indexOfMax = 0;
            for (int i = 0; i < 100; i++)
            {
                if (Settings.SuccessFullCanny[i] > max)
                {
                    max = Settings.SuccessFullCanny[i];
                    indexOfMax = i;
                }
            }

            int currentCross = indexOfMax;
            if (currentCross == 0)
            {
                currentCross = Program.UserSettings.calibrationCannyStart;
            }

            int attempt = 1; // Canny -> SearchSize -> SearchShift
            int cannyAttempt = 1; // Don't start at zero, otherwise adds zero to currentCross first time

            // Try different levels of canny edge detection until the number of points matches
            int[] best = {-1 , -1};
            int[] secondBest = {-1, -1};
            bool goingDown = false;
            bool goingUp = false;
            while (!solutionFound && currentCross >= 0 && currentCross < 100 && cannyAttempt <= Program.UserSettings.maximumCannyAttempts)
            {
                calPoints.Clear();
                var cannyData = Processing.EdgeDetection(image, currentCross);              

                // Due to edgedetection, cannyData is a bit smaller than original!
                float correctionX = (float)(image.GetLength(0) - cannyData.GetLength(0)) / 2;
                float correctionY = (float)(image.GetLength(1) - cannyData.GetLength(1)) / 2;

                int searchSizeDiffCount = 0;
                int[] searchSizeDiff = { 0, 2, -4, 6, -8, 10, -12, 14, -16, 18, -20, 22 };

                List<EllipticalFit> ellipsesFound = new List<EllipticalFit>();
                bool ellipsesValid = false;
                while (!ellipsesValid && searchSizeDiffCount < searchSizeDiff.GetLength(0))
                {
                    int adjustedSearchSize = searchSize + searchSizeDiff[searchSizeDiffCount];
                    int searchShift = 0;
                    ellipsesFound = FindPointsByCanny(test.Paper.CalibrationCircles.X + test.Paper.CalibrationCircles.Y - 1,
                        currentCross, cannyData, adjustedSearchSize, ref searchShift, ref attempt, ref ellipsesValid);
                    searchSizeDiffCount++;
                    attempt++;
                }

                if (ellipsesValid)
                {
                    foreach (var fit in ellipsesFound)
                    {
                        var mid = fit.MidPoint();
                        calPoints.Add(new PointF(mid.X + correctionX, mid.Y + correctionY));
                    }
                    calPoints.Sort(delegate(PointF pc1, PointF pc2) { return Distance(pc1, new PointF(0, image.GetLength(1))).CompareTo(Distance(pc2, new PointF(0, image.GetLength(1)))); });
                    /* Results in this order:
                            G
                            |
                            F
                            |
                            E
                            |
                        A-B-C-D
                    */
                    Settings.SuccessFullCanny[currentCross]++;
                    solutionFound = true;
                }
                else
                {
                    if (best[0] == -1)
                    {
                        best[0] = currentCross;
                        best[1] = ellipsesFound.Count;
                    }
                    else
                    {
                        if (ellipsesFound.Count > best[1])
                        {
                            secondBest[0] = best[0];
                            secondBest[1] = best[1];
                            best[0] = currentCross;
                            best[1] = ellipsesFound.Count;
                        }
                        else if (ellipsesFound.Count > secondBest[1])
                        {
                            secondBest[0] = currentCross;
                            secondBest[1] = ellipsesFound.Count;
                        }
                    }

                    if (secondBest[1] < 0 || best[1] == secondBest[1]) // No specific direction prefered yet -> try both ways
                    {
                        currentCross += (int)Math.Pow(-1, cannyAttempt) * cannyAttempt * 3;
                    }
                    else // One direction favored
                    {
                        if (best[0] < secondBest[0] || goingDown)
                        {
                            currentCross -= 3;
                            goingDown = true;
                        }
                        else if (best[0] > secondBest[0] || goingUp)
                        {
                            currentCross += 3;
                            goingUp = true;
                        }
                    }
                    cannyAttempt++;
                }
            }
                                    
            return calPoints;
        }

        private static List<EllipticalFit> FindPointsByCanny(int numberToFind,
            int currentCross, byte[,] cannyData, int searchSize, ref int searchShift, ref int attempt, ref bool ellipsesValid)
        {
            searchSize = (int)(Math.Round((double)searchSize / 2) * 2); // has to be even
            int shiftX = searchShift == 1 || searchShift == 3 ? (int)(searchSize / 4) : 0;
            int shiftY = searchShift == 2 || searchShift == 3 ? (int)(searchSize / 4) : 0;
            int searchJump = searchSize / 2;
            int w = cannyData.GetLength(0);
            int h = cannyData.GetLength(1);

            // Create a list with areas to look => cannot be reused due to (even very small) differences in size of scans
            List<Point> searchPoints = new List<Point>();

            for (int x = shiftX; x < w; x += searchJump)
            {
                int searchX = x;
                if (x + searchSize >= w)
                {
                    searchX = w - searchSize;
                    x = w;
                }
                for (int y = shiftY; y < h; y += searchJump)
                {
                    int searchY = y;
                    if (y + searchSize >= h)
                    {
                        searchY = h - searchSize;
                        y = h;
                    }
                    searchPoints.Add(new Point(searchX, searchY));
                }
            }

            // Assumed that the paper is oriented right => faster            
            searchPoints.Sort(delegate(Point p1, Point p2) { return Math.Min(w - p1.X, h - p1.Y).CompareTo(Math.Min(w - p2.X, h - p2.Y)); });
            /*
                    * 
                    *
                    *  
                    * 
             ********                           
             */

#if DEBUG
            var img = Processing.ByteToImage(cannyData);
#endif

            // Crop small rectangles from canny
            List<EllipticalFit> ellipseList = new List<EllipticalFit>();
            for (int i = 0; i < searchPoints.Count; i++)
            {
                // Fit an ellipse
                EllipticalFit fit = null;
                RectangleF boundingRectangle = new RectangleF();
                try
                {
                    var searchRectangle = new Rectangle(searchPoints[i].X, searchPoints[i].Y, searchSize, searchSize);
                    fit = new EllipticalFit(cannyData, 255, searchRectangle);

#if DEBUG
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawRectangle(new Pen(Color.Yellow), searchRectangle);
                    }
#endif

                    // Empty search areas are already ignored in fit
                    boundingRectangle = fit.BoundingRectangle();

                    // If a valid ellipse is found
                    if (!double.IsNaN(boundingRectangle.X) && boundingRectangle.X > 0
                        && !double.IsNaN(boundingRectangle.Y) && boundingRectangle.Y > 0
                        && boundingRectangle.Width < searchSize && boundingRectangle.Height < searchSize
                        && boundingRectangle.Width > .25 * searchSize && boundingRectangle.Height > .25 * searchSize
                        && boundingRectangle.Width > 15 && boundingRectangle.Height > 15)
                    {
                        var observed = Processing.Crop(cannyData, searchRectangle);
                        var moments = Processing.Moments(observed);
                        double squareness = Math.Min((double)boundingRectangle.Width, (double)boundingRectangle.Height) / Math.Max((double)boundingRectangle.Width, (double)boundingRectangle.Height);

                        // Ellipse has to be more or less round, based on enough points and have some variance in each direction
                        if (squareness > .9 && moments[0] >= 8 && moments[3] > 0 && moments[4] > 0)
                        {                                                        
                            // Fill the ellipse with floodfill
                            var observedFilled = Processing.FloodFill(observed, 1, new PointF(
                                boundingRectangle.X - searchRectangle.X + (boundingRectangle.Width / 2),
                                boundingRectangle.Y - searchRectangle.Y + (boundingRectangle.Height / 2)));

                            var momentsFilled = Processing.Moments(observedFilled);

                            if (momentsFilled[1] > (boundingRectangle.Width / 2) && momentsFilled[1] < searchRectangle.Width - (boundingRectangle.Width / 2)
                                && momentsFilled[2] > (boundingRectangle.Height / 2) && momentsFilled[2] < searchRectangle.Height - (boundingRectangle.Height / 2))
                            {
                                // Create a white circle of similar size
                                var expectedFilled = Processing.WhiteCircle(observedFilled.GetLength(0), observedFilled.GetLength(1), new RectangleF(
                                    boundingRectangle.X - searchRectangle.X, boundingRectangle.Y - searchRectangle.Y, boundingRectangle.Width, boundingRectangle.Height));

                                // Compare the result of the floodfill with the white circle
                                fit.DifferenceFromCircle = Processing.AverageDifference(observedFilled, expectedFilled);                                

                                // Only if ellipse was closed they will be similar
                                if (fit.DifferenceFromCircle < .1)
                                {
                                    ellipseList = AddWithoutOverlap(ellipseList, fit);
                                }                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogLow(ex.Message);
                }
                if (ellipseList.Count == numberToFind)
                {
                    break;
                }
            }

            ellipsesValid = EllipsePointsAreValid(Program.Test, ellipseList); // Count and colinearity have to be correct

            /*
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < ellipseList.Count; i++)
                {
                    g.DrawLine(new Pen(Color.Blue, 2), ellipseList[i].MidPoint().X - 15, ellipseList[i].MidPoint().Y, ellipseList[i].MidPoint().X + 15, ellipseList[i].MidPoint().Y);
                    g.DrawLine(new Pen(Color.Blue, 2), ellipseList[i].MidPoint().X, ellipseList[i].MidPoint().Y - 15, ellipseList[i].MidPoint().X, ellipseList[i].MidPoint().Y + 15);
                }
            }
            img.Save(Program.UserSettings.defaultDirectory + "\\img\\"
                        + Convert.ToString(DateTime.Now.Minute)
                + " - " + Convert.ToString(DateTime.Now.Second)
                + " - " + Convert.ToString(DateTime.Now.Millisecond)
                + " - " + currentCross
                + " - " + searchSize
                + " - " + searchShift
                + " - " + attempt
                + " - " + ellipseList.Count
                + " - " + ellipsesValid
                + ".bmp");
            */

            Logger.LogHigh(
                       "Attempt:\t" + attempt +
                       "\tCanny:\t" + currentCross +
                       "\tSearch size:\t" + searchSize +
                       "\tSearch shift:\t" + searchShift +
                       "\tEllipses found:\t" + ellipseList.Count +
                       "\tEllipses valid:\t" + ellipsesValid);

            // If not all ellipses are found, retry with shifted searcharea
            if (ellipseList.Count != numberToFind && searchShift < 3)
            {
                searchShift++;
                attempt++;
                ellipseList = FindPointsByCanny(numberToFind, currentCross, cannyData, searchSize, ref searchShift, ref attempt, ref ellipsesValid);
            }

            return ellipseList;
        }

        private static PointF[] CornerPoints(Test test, List<PointF> calPoints)
        {
            float largestDistance = 0;
            for (int i = 0; i < calPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < calPoints.Count; j++)
                {
                    largestDistance = Math.Max(largestDistance, Distance(calPoints[i], calPoints[j]));
                }
            }

            var p1 = new PointF();
            var p2 = new PointF();
            var leftDown = new PointF();
            var rightUp = new PointF();
            var rightDown = new PointF();
            for (int i = 0; i < calPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < calPoints.Count; j++)
                {
                    if (Distance(calPoints[i], calPoints[j]) == largestDistance)
                    {
                        p1 = calPoints[i];
                        p2 = calPoints[j];
                    }
                }
            }
            float dif = 0;
            for (int i = 0; i < calPoints.Count; i++)
            {
                var dist = Distance(calPoints[i], p1) * Distance(calPoints[i], p2);
                if (dist > dif)
                {
                    dif = dist;
                    rightDown = calPoints[i];
                }
            }

            if ((test.Paper.Orientation == PaperOrientation.Portrait && Distance(rightDown, p1) < Distance(rightDown, p2) ||
                (test.Paper.Orientation == PaperOrientation.Landscape && Distance(rightDown, p1) > Distance(rightDown, p2))))
            {
                leftDown = p1;
                rightUp = p2;
            }
            else
            {
                leftDown = p2;
                rightUp = p1;
            }

            return new PointF[] { leftDown, rightUp, rightDown };
        }

        private static List<EllipticalFit> AddWithoutOverlap(List<EllipticalFit> ellipseList, EllipticalFit fit)
        {
            List<EllipticalFit> outList = new List<EllipticalFit>();
            if (ellipseList.Count == 0)
            {
                outList.Add(fit);
                return outList;
            }
            else
            {
                bool addFit = true;
                for (int i = 0; i < ellipseList.Count; i++)
                {                  
                    if (ellipseList[i].BoundingRectangle().IntersectsWith(fit.BoundingRectangle()))
                    {               
                        // In case over overlap: only add the best one
                        if (ellipseList[i].DifferenceFromCircle < fit.DifferenceFromCircle)
                        {
                            addFit = false;
                            outList.Add(ellipseList[i]);
                        }
                    }
                    else // Add both
                    {
                        outList.Add(ellipseList[i]);
                    }
                }
                if (addFit)
                {
                    outList.Add(fit);
                }
                return outList;
            }
        }

        private static bool EllipsePointsAreValid(Test test, List<EllipticalFit> ellipsePoints)
        {
            if (ellipsePoints.Count != test.Paper.CalibrationCircles.X + test.Paper.CalibrationCircles.Y - 1)
            {
                return false;
            }
            var calPoints = EllipsesToPoints(ellipsePoints);
            var corner = CornerPoints(test, calPoints);
            var leftDown = corner[0];
            var rightUp = corner[1];
            var rightDown = corner[2];

            // Each point has to be on either the horizontal or vertical axis, otherwise one or more points are wrong       
            bool badAlignedPoint = true;
            for (int i = 0; i < calPoints.Count; i++)
            {
                if (!IsOnLine(calPoints[i], leftDown, rightDown, 1f) && !IsOnLine(calPoints[i], rightUp, rightDown, .1f))
                {
                    badAlignedPoint = false;
                    break;
                }
            }
            return badAlignedPoint;
        }

        private static List<PointF> EllipsesToPoints(List<EllipticalFit> ellipsePoints)
        {
            var calPoints = new List<PointF>();
            for (int i = 0; i < ellipsePoints.Count; i++)
            {
                calPoints.Add(ellipsePoints[i].MidPoint());
            }
            return calPoints;
        }

        private static bool IsOnLine(PointF point, PointF linePoint1, PointF linePoint2, float percentTolerance)
        {
            var len = Math.Sqrt(Math.Pow(linePoint1.X - linePoint2.X, 2) +
                                Math.Pow(linePoint1.Y - linePoint2.Y, 2));
            var lambda = ((linePoint2.X - linePoint1.X)*(point.X - linePoint1.X)+(linePoint2.Y - linePoint1.Y)*(point.Y - linePoint1.Y)) 
                             / (Math.Pow(linePoint2.X - linePoint1.X, 2)+ Math.Pow(linePoint2.Y - linePoint1.Y, 2));
            var dist = Math.Sqrt(
                Math.Pow(point.X - linePoint1.X - lambda * (linePoint2.X - linePoint1.X), 2) +
                Math.Pow(point.Y - linePoint1.Y - lambda * (linePoint2.Y - linePoint1.Y), 2)
                );            
            return dist < len * percentTolerance / 100;
        }
        
        private static float Distance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static float RadToDegrees(double rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float DegreesToRad(double degree)
        {
            return (float)(degree * Math.PI / 180);
        }

        private static double AverageAngle(double f1, double f2)
        {
            double f = Math.Atan2(Math.Sin(f1) + Math.Sin(f2), Math.Cos(f1) + Math.Cos(f2));            
            while(f > 2 * Math.PI)
            {
                f += 2 * Math.PI;
            }
            return f;
        }
    }
}

