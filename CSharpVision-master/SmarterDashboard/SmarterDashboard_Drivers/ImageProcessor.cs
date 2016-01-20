using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SmarterDashboard_Drivers
{
    //Abstract Class For Stream Type Classes
    class ImageProcessor
    {
        #region field const values from PDF
        public const double visionTargetEdge = 7.0; //inch
        public const double visionTargetInnerWidth = 2.0; //inch
        public const double spaceBetweenVisionTarget = 2.75; //inch
        public const double boundingRectArea = visionTargetEdge * visionTargetEdge;
        public const double innerRectArea = (visionTargetEdge - visionTargetInnerWidth) * (visionTargetEdge - visionTargetInnerWidth);
        public const double L_Area = boundingRectArea - innerRectArea;
        public const double boundingRectPerimeter = 4 * visionTargetEdge;
        public const double L_Perimeter = boundingRectPerimeter; //because 7+7+5+5+2+2 = 4*7 

        public const double expectedAreaRatio = L_Area / boundingRectArea;
        public const double expectedPerimeterRation = L_Perimeter / boundingRectPerimeter;

        public const double expectedSpaceByEdgeRatio = spaceBetweenVisionTarget / visionTargetEdge;

        public const double error = 0.1;

        public const double BinDiameter = 0.56515;// m
        public const double BinDiameterWithHandles = 0.63773;// m
        public const double BinHight = 0.68895;// m
        #endregion


        #region Angle Consts

        const double FULL_CAMERA_ANGLE = 68.5f;
        static readonly double TAN_HALF_CAMERA_ANGLE = Math.Tan((FULL_CAMERA_ANGLE / 2) * Math.PI / 180.0);
        const double HALF_BIN_DIAMETER = BinDiameter / 2;

        #endregion

        public static readonly CircleF NotFoundCircle = new CircleF(new PointF(-1, -1), -9999);
        public static readonly Rectangle NotFoundRectangle = new Rectangle(-9999, -9999, -9999, -9999);
        public static readonly PointF NotFoundPoint = new PointF(-9999, -9999);
        public static readonly float NotFoundFloat = -9999.0f;

        public double angleToBin { get; private set; }

        protected ThreshData threshData;
        public enum ToteDetectionType { Math, Edges, None };

        class L_Shape
        {
            #region L_Shape
            public bool isRight;
            public Contour<Point> shape;
            public bool isPaired;

            public L_Shape(Contour<Point> shape, bool isRight)
            {
                this.shape = shape;
                this.isRight = isRight;
                this.isPaired = false;
            }
            #endregion
        }

        public ImageProcessor()
        {
            threshData = new ThreshData();
        }



        public Image ProcessImage(Bitmap frame, ToteDetectionType detectionType, bool detectBins)
        {
            Image<Hsv, Byte> img = new Image<Hsv, Byte>(frame);

            // Get The Thresh Image With Given Values
            Image<Gray, byte> thresh = threshData.InRange(img);

            // Pixelate Image
            threshData.Blur(ref thresh);


            Image ret = AnalyzeImage(img.Convert<Bgr, byte>(), thresh, detectionType, detectBins);


            thresh.Dispose();
            img.Dispose();
            return ret;
        }


        //Do Vision
        protected Image AnalyzeImage(Image<Bgr, byte> original, Image<Gray, byte> thresh, ToteDetectionType detectionType, bool detectBins)
        {
            angleToBin = NotFoundFloat;

            float resolutionOffset = ((float)thresh.Width * thresh.Height) / (640.0f * 480.0f);


            //List<L_Shape> targets = FindToteVisionTargets(detectionType, thresh, resolutionOffset, thresh.Width);
            //foreach (L_Shape l in targets)
            //{
            //    original.Draw(l.shape, new Bgr(255, 0, 0), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));
            //}

            //L_Shape[] best_pair = FindBestPair(targets);
            //if (best_pair[0] != null && best_pair[1] != null)
            //{
            //    original.Draw(best_pair[0].shape, new Bgr(0, 0, 255), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));
            //    original.Draw(best_pair[1].shape, new Bgr(0, 0, 255), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));
            //}


            if (detectBins)
            {
                List<Contour<Point>> bins = FindBins(thresh, resolutionOffset, thresh.Width);
                double maxArea = 0.0f;
                foreach (Contour<Point> bin in bins)
                {
                    // Oz changes start
                    MCvMoments moment = bin.GetMoments();
                    PointF centerOfMass = new PointF((float)moment.GravityCenter.x, (float)moment.GravityCenter.y);
                    CircleF circleOfMass = new CircleF(centerOfMass, 4.590f);
                    original.Draw(circleOfMass, new Bgr(0, 0, 255), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));

                    // Check if it's the biggest bin yet, if it is - calculate the angle to it
                    if (maxArea < bin.Area)
                    {
                        maxArea = bin.Area;
                        angleToBin = GetAngleToBin(bin, thresh.Size.Width / 2.0);
                    }

                    Seq<Point> convexHull = bin.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    original.Draw(convexHull, new Bgr(0, 0, 255), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));
                    // Oz changes end
                    original.Draw(bin.BoundingRectangle, new Bgr(0, 255, 0), (int)Math.Ceiling(3.0f * Math.Sqrt(resolutionOffset)));
                }
            }

            // Draw A Red Cross All Over The Threshold Picture So We'D Know It'S Center
            original.Draw(new Cross2DF(new PointF(thresh.Width / 2, thresh.Height / 2), thresh.Width, thresh.Height), new Bgr(0, 0, 255), 1);


            Image ret = original.ToBitmap();
            original.Dispose();

            return ret;
        }


        List<Contour<Point>> FindBins(Image<Gray, byte> thresh, float resolutionOffset, float imageWidth)
        {
            List<Contour<Point>> targets = new List<Contour<Point>>();
            Contour<Point> blob = thresh.FindContours();

            while (blob != null)
            {
                if (blob.Area > 2600 * resolutionOffset)
                    if (Is_Bin(blob))
                    {
                        targets.Add(blob);
                    }
                blob = blob.HNext;
            }
            return targets;
        }

        bool Is_Bin(Contour<Point> blob)
        {
            Rectangle rec = blob.BoundingRectangle;
            double ratio = rec.Width / (double)rec.Height;
            //return (ratio + 0.3  >= ((BinDiameter / BinHight)) && ratio - error <= (BinDiameterWithHandles / BinHight));
            Seq<Point> convexHull = blob.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
            Double area = convexHull.Area / (blob.BoundingRectangle.Height * blob.BoundingRectangle.Width);

            return ((area > 0.7) && (area < 0.90)) && (ratio + 0.3 >= ((BinDiameter / BinHight)) && ratio - error <= (BinDiameterWithHandles / BinHight)); ;
        }


        List<L_Shape> FindToteVisionTargets(ToteDetectionType tdt, Image<Gray, byte> thresh, float resolutionOffset, float imageWidth)
        {
            List<L_Shape> targets = new List<L_Shape>();
            Contour<Point> blob = thresh.FindContours();

            while (blob != null)
            {
                bool isL = false;

                if (blob.Area > 200 * resolutionOffset)
                    switch (tdt)
                    {
                        case ToteDetectionType.Math:
                            isL = IsL_ByMath(blob);
                            break;
                        case ToteDetectionType.Edges:
                            isL = IsL_ByEdges(blob);
                            break;
                        case ToteDetectionType.None:
                            isL = true;
                            break;
                    }

                if (isL)
                {
                    L_Shape s = new L_Shape(blob, IsRightL(blob));
                    targets.Add(s);
                }
                blob = blob.HNext;
            }
            return targets;
        }

        bool IsRightL(Contour<Point> blob)
        {
            int blob_center = blob.BoundingRectangle.X + blob.BoundingRectangle.Width / 2;

            int left_count = 0, right_count = 0;
            Point[] points = blob.ToArray();
            foreach (Point p in points)
            {
                if (p.X < blob_center)
                    left_count++;
                else
                    right_count++;
            }

            return left_count > right_count;
        }

        bool IsL_ByMath(Contour<Point> blob)
        {
            Rectangle boundingRect = blob.BoundingRectangle;
            int boundingRectP = (boundingRect.Width + boundingRect.Height) * 2;
            //if(!((double)blob.Perimeter / boundingRectP < 1.1 && (double)blob.Perimeter / boundingRectP > 0.9)) return false;

            int boundingRectS = boundingRect.Width * boundingRect.Height;
            //if ((double)blob.Area / boundingRectS < 0.5 && (double)blob.Area / boundingRectS > 0.47) return true;
            double ratioSum = (double)blob.Area / boundingRectS + (double)blob.Perimeter / boundingRectP;

            return (ratioSum < expectedAreaRatio + expectedPerimeterRation + error && ratioSum > expectedAreaRatio + expectedPerimeterRation - error);
        }

        bool IsL_ByEdges(Contour<Point> blob)
        {
            Contour<Point> approx = blob.ApproxPoly(blob.Perimeter * 0.05);
            Point[] pts = approx.ToArray();
            LineSegment2D[] edges = PointCollection.PolyLine(pts, true);
            if (approx.Total == 6)
            {
                for (int i = 0; i < edges.Length; i++)
                {
                    double angle = Math.Abs(edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                    if (angle < 70 || angle > 110) return false;
                }
            }
            else
            {
                return false;
            }
            return true;

        }

        L_Shape[] FindBestPair(List<L_Shape> targets)
        {
            L_Shape[] best_pair = new L_Shape[2];

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].isPaired)
                    continue;

                for (int j = i + 1; j < targets.Count; j++)
                {
                    if (targets[j].isPaired || targets[j].isRight == targets[i].isRight)
                        continue;

                    L_Shape right;
                    L_Shape left;

                    if (targets[j].isRight)
                    {
                        right = targets[j];
                        left = targets[i];
                    }
                    else
                    {
                        right = targets[i];
                        left = targets[j];
                    }
                    Rectangle leftBRect = left.shape.BoundingRectangle;
                    Rectangle rightBRect = right.shape.BoundingRectangle;
                    if (rightBRect.Left > leftBRect.Right)
                        if (Math.Abs(rightBRect.Y - leftBRect.Y) < rightBRect.Height / 2)
                        {
                            double ratio = (rightBRect.Left - leftBRect.Right) / (double)rightBRect.Width;
                            //ratio can't be greater than expected, but when viewing from an angle can be lesser.
                            if (ratio < expectedSpaceByEdgeRatio + (error * 4) && ratio > expectedSpaceByEdgeRatio - error)
                            {
                                right.isPaired = true;
                                left.isPaired = true;

                                //Got A Good Pair. Check If Best Pair.
                                if (best_pair[0] != null) //else it's the first pair, thus the best pair
                                {
                                    double yRatio = (double)(rightBRect.Y + leftBRect.Y) / (best_pair[0].shape.BoundingRectangle.Y + best_pair[1].shape.BoundingRectangle.Y);
                                    double areaRatio = (double)(right.shape.Area + left.shape.Area) / (best_pair[0].shape.Area + best_pair[1].shape.Area);

                                    if (0.3 * yRatio + 0.7 * areaRatio < 1)
                                        break;
                                }


                                best_pair[0] = left;
                                best_pair[1] = right;
                                break;
                            }
                        }
                }
            }
            return best_pair;
        }

        public double GetAngleToBin(Contour<Point> bin, double halfResolutionWidth)
        {
            double centerOfMass_x = bin.GetMoments().GravityCenter.x;
            return Math.Atan(((centerOfMass_x / halfResolutionWidth) - 1) * TAN_HALF_CAMERA_ANGLE) * 180 / Math.PI;
        }

        public void LoadLastThreshData(string path)
        {
            threshData.LoadFromFile(path);
        }
    }
}