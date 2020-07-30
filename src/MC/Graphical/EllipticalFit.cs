using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using Meta.Numerics;
using Meta.Numerics.Matrices;
using System.Diagnostics;

namespace MC.Graphical
{
    public class EllipticalFit
    {
        public double[] Parameters = new double[6];
        public int NumberOfPoints;
        public double DifferenceFromCircle;

        Rectangle rangeRectangle;

        List<Point> pointList;
        byte[,] dataPoints;

        // Do not crop, but use rectangle
        public EllipticalFit(byte[,] data, byte value, Rectangle rectangle)
        {
            dataPoints = data;
            rangeRectangle = rectangle;
            pointList = new List<Point>();

            bool noPoints = true;

            for (int i = rectangle.Top; i < rectangle.Bottom; i++)
            {
                for (int j = rectangle.Left; j < rectangle.Right; j++)
                {
                    if (dataPoints[j, i] == value)
                    {
                        pointList.Add(new Point(j, i));
                        noPoints = false;
                    }
                }
            }

            if (!noPoints)
            {
                NumberOfPoints = pointList.Count();
                GetParameters();
            }
        }

        private void GetParameters()
        {
            // A x^2 + B x y + C y^2 + D x + E y + F = 0
            double[] par = new double[6];
            int numPoints = pointList.Count;
            if (numPoints > 2)
            {
                Matrix D1 = new Matrix(numPoints, 3);
                Matrix D2 = new Matrix(numPoints, 3);
                SquareMatrix S1 = new SquareMatrix(3);
                SquareMatrix S2 = new SquareMatrix(3);
                SquareMatrix S3 = new SquareMatrix(3);
                SquareMatrix T = new SquareMatrix(3);
                SquareMatrix M = new SquareMatrix(3);
                SquareMatrix C1 = new SquareMatrix(3);
                Matrix a1 = new Matrix(3, 1);
                Matrix a2 = new Matrix(3, 1);                
                Matrix temp;

                C1[0, 0] = 0;
                C1[0, 1] = 0;
                C1[0, 2] = 0.5;
                C1[1, 0] = 0;
                C1[1, 1] = -1;
                C1[1, 2] = 0;
                C1[2, 0] = 0.5;
                C1[2, 1] = 0;
                C1[2, 2] = 0;

                //2 D1 = [x .ˆ 2, x .* y, y .ˆ 2]; % quadratic part of the design matrix
                //3 D2 = [x, y, ones(size(x))]; % linear part of the design matrix
                for (int xx = 0; xx < numPoints; xx++)
                {
                    Point p = pointList[xx];
                    D1[xx, 0] = p.X * p.X;
                    D1[xx, 1] = p.X * p.Y;
                    D1[xx, 2] = p.Y * p.Y;

                    D2[xx, 0] = p.X;
                    D2[xx, 1] = p.Y;
                    D2[xx, 2] = 1;
                }

                //4 S1 = D1’ * D1; % quadratic part of the scatter matrix
                temp = D1.Transpose() * D1;
                for (int xx = 0; xx < 3; xx++)
                    for (int yy = 0; yy < 3; yy++)
                        S1[xx, yy] = temp[xx, yy];

                //5 S2 = D1’ * D2; % combined part of the scatter matrix
                temp = D1.Transpose() * D2;
                for (int xx = 0; xx < 3; xx++)
                    for (int yy = 0; yy < 3; yy++)
                        S2[xx, yy] = temp[xx, yy];

                //6 S3 = D2’ * D2; % linear part of the scatter matrix
                temp = D2.Transpose() * D2;
                for (int xx = 0; xx < 3; xx++)
                    for (int yy = 0; yy < 3; yy++)
                        S3[xx, yy] = temp[xx, yy];

                //7 T = - inv(S3) * S2’; % for getting a2 from a1
                if(Determinant(S3) > 0)
                {
                    T = -1 * S3.Inverse() * S2.Transpose();

                    //8 M = S1 + S2 * T; % reduced scatter matrix
                    M = S1 + S2 * T;

                    //9 M = [M(3, :) ./ 2; - M(2, :); M(1, :) ./ 2]; % premultiply by inv(C1)
                    M = C1 * M;

                    //10 [evec, eval] = eig(M); % solve eigensystem
                    ComplexEigensystem eigenSystem = M.Eigensystem();

                    //11 cond = 4 * evec(1, :) .* evec(3, :) - evec(2, :) .ˆ 2; % evaluate a’Ca
                    //12 a1 = evec(:, find(cond > 0)); % eigenvector for min. pos. eigenvalue
                    for (int xx = 0; xx < eigenSystem.Dimension; xx++)
                    {
                        Vector<Complex> vector = eigenSystem.Eigenvector(xx);
                        Complex condition = 4 * vector[0] * vector[2] - vector[1] * vector[1];
                        if (condition.Im == 0 && condition.Re > 0)
                        {
                            // Solution is found
                            for (int yy = 0; yy < vector.Count(); yy++)
                            {
                                a1[yy, 0] = vector[yy].Re;
                            }
                        }
                    }

                    //13 a2 = T * a1; % ellipse coefficients
                    a2 = T * a1;

                    //14 a = [a1; a2]; % ellipse coefficients
                    par[0] = a1[0, 0];
                    par[1] = a1[1, 0];
                    par[2] = a1[2, 0];
                    par[3] = a2[0, 0];
                    par[4] = a2[1, 0];
                    par[5] = a2[2, 0];
                }
            }
            
            for (int i = 0; i < par.Count(); i++)
            {
                if (!double.IsNaN(par[i]) && !double.IsInfinity(par[i]))
                    Parameters[i] = par[i];
                else
                    Parameters[i] = 0;             
            }
        }

        private void Print(Matrix m)
        {
            for (int i = 0; i < m.RowCount; i++)
            {
                StringBuilder str = new StringBuilder();
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    str.Append(m[i, j] + "\t");
                }
                Debug.WriteLine(str.ToString());
            }
            Debug.WriteLine("===========================================");
        }

        private void Print(SquareMatrix m)
        {
            for (int i = 0; i < m.Dimension; i++)
            {
                StringBuilder str = new StringBuilder();
                for (int j = 0; j < m.Dimension; j++)
                {
                    str.Append(m[i, j] + "\t");
                }
                Debug.WriteLine(str.ToString());
            }
            Debug.WriteLine("===========================================");
        }

        public PointF[] DrawPoints(int numberOfPoints)
        {
            numberOfPoints /= 2;
            var PointsA = new List<PointF>();
            var PointsB = new List<PointF>();
            for (int k = 0; k < numberOfPoints; k++)
            {
                float xLoc = (((float)k / (numberOfPoints - 1)) * rangeRectangle.Width) + rangeRectangle.Left;                              
                PointF predicted = Predict(xLoc);

                PointsA.Add(new PointF(xLoc, predicted.X));
                PointsB.Add(new PointF(xLoc, predicted.Y));
            }
            // Otherwise the order of points is incorrect to draw
            for (int k = PointsB.Count - 1; k >= 0; k--)
            {
                PointsA.Add(PointsB[k]);
            }

            // Add the first point again to make the ellipse complete
            if (PointsA.Count != 0)
            {
                PointsA.Add(PointsA[0]);
            }

            return PointsA.ToArray();
        }

        public PointF Predict(double X)
        {
            PointF result = new PointF();
            double A = Parameters[0];
            double B = Parameters[1];
            double C = Parameters[2];
            double D = Parameters[3];
            double E = Parameters[4];
            double F = Parameters[5];

            if (C == 0 && B * X + E != 0)
            {
                float tmp = (float)((-A * Math.Pow(X, 2) - D * X - F) / (B * X + E));
                result.X = tmp;
                result.Y = tmp;
            }
            else if (C != 0)
            {
                float tmp = (float)Math.Sqrt(Math.Pow(B * X + E, 2) - 4 * C * (A * Math.Pow(X, 2) + D * X + F));
                result.X = (float)((tmp - B * X - E) / (2 * C));
                result.Y = (float)((-tmp - B * X - E) / (2 * C));
            }
            return result;
        }

        public double AverageQuadraticResidual()
        {
            double residu = 0;
            double tmp = 0;
            double counter = 0;

            for (int i = 0; i < pointList.Count; i++)
            {
                // This sounds strange but is correct!
                // Points in PointList are X-Y values
                // But predicted points are not points, but Y1 and Y2 values for a given X
                PointF predicted = Predict(pointList[i].X);
                tmp = Math.Min(
                      Math.Pow(pointList[i].Y - predicted.X, 2),
                      Math.Pow(pointList[i].Y - predicted.Y, 2));

                if (!double.IsNaN(tmp))
                {
                    residu += tmp;
                    counter++;
                }
            }
            if (counter > 0)
            {
                return (double)residu / (double)counter;
            }
            else
            {
                return double.MaxValue;
            }
        }

        // Gives the percentage of points on the circle that lay on a white point
        public double PercentageFit(int numberOfPoints)
        {
            var lst = DrawPoints(numberOfPoints - 1);
            int fit = 0;
            int counter = 0;
            for (int i = 0; i < lst.Count(); i++)
            {
                if (lst[i].X != double.NaN && lst[i].Y != double.NaN && lst[i].X >= 0 && lst[i].Y >= 0 && lst[i].X < dataPoints.GetLength(0) && lst[i].Y < dataPoints.GetLength(1))
                {
                    if (dataPoints[(int)lst[i].X, (int)lst[i].Y] == 1)
                    {
                        fit++;
                    }
                    counter++;
                }
            }
            if (counter > 0)
            {
                return (double)fit / (double)counter;
            }
            else
            {
                return 0;
            }            
        }

        public RectangleF BoundingRectangle()
        {
            double[] p = Parameters;

            double dx = -64 * p[0] * p[2] * p[2] * p[5] + 16 * p[0] * p[2] * p[4] * p[4] + 16 * p[1] * p[1] * p[2] * p[5]
                       - 16 * p[1] * p[2] * p[3] * p[4] + 16 * p[2] * p[2] * p[3] * p[3];
            double x1 = (-2 * (p[1] * p[4] - 2 * p[2] * p[3]) - Math.Sqrt(dx)) / (2 * (p[1] * p[1] - 4 * p[0] * p[2]));
            double x2 = (-2 * (p[1] * p[4] - 2 * p[2] * p[3]) + Math.Sqrt(dx)) / (2 * (p[1] * p[1] - 4 * p[0] * p[2]));

            double dy = -64 * p[2] * p[0] * p[0] * p[5] + 16 * p[0] * p[2] * p[3] * p[3] + 16 * p[1] * p[1] * p[0] * p[5]
                       - 16 * p[1] * p[0] * p[3] * p[4] + 16 * p[0] * p[0] * p[4] * p[4];
            double y1 = (-2 * (p[1] * p[3] - 2 * p[0] * p[4]) - Math.Sqrt(dy)) / (2 * (p[1] * p[1] - 4 * p[0] * p[2]));
            double y2 = (-2 * (p[1] * p[3] - 2 * p[0] * p[4]) + Math.Sqrt(dy)) / (2 * (p[1] * p[1] - 4 * p[0] * p[2]));

            return new RectangleF((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Abs(x1 - x2), (float)Math.Abs(y1 - y2));
        }

        public PointF MidPoint()
        {
            var rect = BoundingRectangle();
            return new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        public override string ToString()
        {
            //A x^2 + B x y + C y^2 + D x + E y + F
            StringBuilder str = new StringBuilder();
            str.Append(Parameters[0] > 0 ? " " + Math.Round(Parameters[0], 4) + "x^2 " : " - " + Math.Round(-Parameters[0], 4) + "x^2 ");
            str.Append(Parameters[1] > 0 ? " " + Math.Round(Parameters[1], 4) + "xy "  : " - " + Math.Round(-Parameters[1], 4) + "xy " );
            str.Append(Parameters[2] > 0 ? " " + Math.Round(Parameters[2], 4) + "y^2 " : " - " + Math.Round(-Parameters[2], 4) + "y^2 ");
            str.Append(Parameters[3] > 0 ? " " + Math.Round(Parameters[3], 4) + "x "   : " - " + Math.Round(-Parameters[3], 4) + "x "  );
            str.Append(Parameters[4] > 0 ? " " + Math.Round(Parameters[4], 4) + "y "   : " - " + Math.Round(-Parameters[4], 4) + "y "  );
            str.Append(Parameters[5] > 0 ? " " + Math.Round(Parameters[5], 4)          : " - " + Math.Round(-Parameters[5], 4)         );
            return str.ToString();
        }

        private double Determinant(SquareMatrix m)
        {
            double det = 0;

            if (m.Dimension == 2)
            {
                det = (m[0,0] * m[1, 1]) - (m[1, 0] * m[0, 1]);
            }
            else if (m.Dimension == 3)
            {
                det = (m[0, 0] * m[1, 1] * m[2, 2]) + (m[0, 1] * m[1, 2] * m[2, 0]) + (m[0, 2] * m[1, 0] * m[2, 1])
                    - (m[0, 2] * m[1, 1] * m[2, 0]) - (m[0, 1] * m[1, 0] * m[2, 2]) - (m[0, 0] * m[1, 2] * m[2, 1]);
            }
            else
            {
                try
                {
                    SquareLUDecomposition lu = m.LUDecomposition();
                    det = lu.Determinant();
                }
                catch (Exception ex)
                {
                    det = 0;
                    MC.Other.Logger.LogLow(ex.Message);
                }
            }
            return det;            
        }

    }

}

