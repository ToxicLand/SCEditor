using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using IntPoint = Accord.IntPoint;
using BlobCounter = Accord.Imaging.BlobCounter;
using Image = Accord.Imaging.Image;
using UnmanagedImage = Accord.Imaging.UnmanagedImage;
using Blob = Accord.Imaging.Blob;
using GrahamConvexHull = Accord.Math.Geometry.GrahamConvexHull;
using PointsCloud = Accord.Math.Geometry.PointsCloud;
using System.Drawing.Drawing2D;
using Point = System.Drawing.Point;

namespace SCEditor.Features
{
    public class fixChunksPoints
    {
        private PointF[] _oldUVArray;
        private List<PointF> _newUVArray;

        private float _offsetXMin;
        private float _offsetYMin;
        private float _offsetXMax;
        private float _offsetYMax;

        private bool _goRight;
        private bool _swapYAxis;

        private List<PointF> _potentialPoints;
        private List<double> _potentialPointsDistance;

        public PointF[] initiate(ScObject data)
        {
            _oldUVArray = ((PointF[])((ShapeChunk)data).UV.Clone());

            List<PointF> newUVArray = performGarham(_oldUVArray);

            return newUVArray.ToArray();

#pragma warning disable CS0162 // Unreachable code detected
            _newUVArray = new List<PointF>();
#pragma warning restore CS0162 // Unreachable code detected
            _oldUVArray = newUVArray.ToArray();

            _offsetXMin = _oldUVArray.Min(f => f.X);
            _offsetYMin = _oldUVArray.Min(f => f.Y);
            _offsetXMax = _oldUVArray.Max(f => f.X);
            _offsetYMax = _oldUVArray.Max(f => f.Y);

            PointF mostRightPoint = _oldUVArray[0];
            /**
            for (int i = 0; i < _oldUVArray.Length; i++)
            {
                PointF p = _oldUVArray[i];

                if (p.X > mostRightPoint.X)
                {
                    mostRightPoint = p;
                }
            }

            List<PointF> mostRightPoints = new List<PointF>();
            for (int i = 0; i < _oldUVArray.Length; i++)
            {
                PointF p = _oldUVArray[i];

                if (p.X == mostRightPoint.X)
                {
                    mostRightPoints.Add(p);
                }
            }

            if (mostRightPoints.Count != 0)
            {
                for (int i = 0; i < mostRightPoints.Count; i++)
                {
                    PointF p = mostRightPoints[i];

                    if (mostRightPoint.Y == _offsetYMax)
                    {
                        if (mostRightPoint.Y > p.Y && p.Y != _offsetYMax)
                        {
                            mostRightPoint = p;
                        }
                    }
                    else
                    {
                        if (mostRightPoint.Y > p.Y && p.Y != _offsetYMax)
                        {
                            mostRightPoint = p;
                        }
                    }
                }
            }
            **/

            _newUVArray.Add(mostRightPoint);

            for (int times = 0; times < _oldUVArray.Length; times++)
            {
                if (times + 1 == _oldUVArray.Length)
                    break;

                PointF currentPoint = _newUVArray[times];
                _swapYAxis = false;
                _goRight = false;

                List<PointF> initialPoints = new List<PointF>();

                for (int index = 0; index < _oldUVArray.Length;)
                {
                    if (_oldUVArray[index] != currentPoint && !_newUVArray.Contains(_oldUVArray[index]))
                    {
                        if (currentPoint.X >= _oldUVArray[index].X & !_goRight)
                            initialPoints.Add(_oldUVArray[index]);

                        if (currentPoint.X <= _oldUVArray[index].X & _goRight)
                            initialPoints.Add(_oldUVArray[index]);
                    }

                    index++;
                    if (index == _oldUVArray.Length)
                    {
                        if (initialPoints.Count == 0)
                        {
                            if (_goRight)
                            {
                                foreach (PointF item in _oldUVArray)
                                {
                                    bool exists = false;
                                    for (int i = 0; i < _oldUVArray.Length - 1; i++)
                                    {
                                        if (item == _newUVArray[i])
                                            exists = true;
                                    }

                                    if (exists == false)
                                    {
                                        _newUVArray[_oldUVArray.Length - 1] = item;
                                    }
                                }
                            }

                            index = 0;
                            _goRight = true;
                        }
                    }
                }

                List<PointF> PointsOnXAxis = new List<PointF>();
                List<PointF> PointsOnYAxis = new List<PointF>();

                List<PointF> PointsEqualX = new List<PointF>();
                List<PointF> PointsEqualY = new List<PointF>();

                List<PointF> PointsToLeft = new List<PointF>();
                List<PointF> PointsToRight = new List<PointF>();

                List<PointF> PointsToBottom = new List<PointF>();
                List<PointF> PointsToTop = new List<PointF>();

                for (int index = 0; index < _oldUVArray.Length; index++)
                {
                    if (currentPoint == _oldUVArray[index])
                        continue;

                    // if point is on outline - y axis
                    if (currentPoint.Y == _offsetYMax)
                    {
                        if (_oldUVArray[index].Y == _offsetYMax)
                        {
                            PointsOnYAxis.Add(_oldUVArray[index]);
                        }
                    }
                    else if (currentPoint.Y == _offsetYMin)
                    {
                        if (_oldUVArray[index].Y == _offsetYMin)
                        {
                            PointsOnYAxis.Add(_oldUVArray[index]);
                        }
                    }

                    // if point is on outline - y axis
                    if (currentPoint.X == _offsetXMax)
                    {
                        if (_oldUVArray[index].X == _offsetXMax)
                        {
                            PointsOnXAxis.Add(_oldUVArray[index]);
                        }
                    }
                    else if (currentPoint.X == _offsetXMin)
                    {
                        if (_oldUVArray[index].X == _offsetXMin)
                        {
                            PointsOnXAxis.Add(_oldUVArray[index]);
                        }
                    }

                    if (currentPoint.Y == _oldUVArray[index].Y)
                    {
                        PointsEqualY.Add(_oldUVArray[index]);
                    }

                    if (currentPoint.X == _oldUVArray[index].X)
                    {
                        PointsEqualX.Add(_oldUVArray[index]);
                    }

                    if (currentPoint.Y < _oldUVArray[index].Y)
                    {
                        PointsToBottom.Add(_oldUVArray[index]);
                    }

                    if (currentPoint.Y > _oldUVArray[index].Y)
                    {
                        PointsToTop.Add(_oldUVArray[index]);
                    }

                    if (currentPoint.X > _oldUVArray[index].X)
                    {
                        PointsToLeft.Add(_oldUVArray[index]);
                    }

                    if (currentPoint.X < _oldUVArray[index].X)
                    {
                        PointsToRight.Add(_oldUVArray[index]);
                    }
                }

                PointsOnXAxis = PointsOnXAxis.Distinct().ToList();
                PointsOnYAxis = PointsOnYAxis.Distinct().ToList();
                PointsToLeft = PointsToLeft.Distinct().ToList();
                PointsToRight = PointsToRight.Distinct().ToList();
                PointsToBottom = PointsToBottom.Distinct().ToList();
                PointsToTop = PointsToTop.Distinct().ToList();
                PointsEqualX = PointsEqualX.Distinct().ToList();
                PointsEqualY = PointsEqualY.Distinct().ToList();

                if (initialPoints.Count == 0)
                {
                    MessageBox.Show("initialPoints empty.");
                    return null;
                }

                _potentialPoints = getPotentialPoints(currentPoint, initialPoints, _goRight, ref _swapYAxis).Distinct().ToList();

                if (_potentialPoints.Count == 0)
                {
                    MessageBox.Show("_potentialPoints empty.");
                    return null;
                }

                _potentialPointsDistance = getPointsDistances(currentPoint, _potentialPoints, _goRight, _swapYAxis);

                List<(PointF, double)> pointWD = new List<(PointF, double)>();
                List<(PointF, int)> PointDistanceInt = new List<(PointF, int)>();
                for (int i = 0; i < _potentialPoints.Count; i++)
                {
                    pointWD.Add((_potentialPoints[i], _potentialPointsDistance[i]));
                    PointDistanceInt.Add((_potentialPoints[i], Convert.ToInt32(_potentialPointsDistance[i])));
                }

                List<(PointF, int)> resultInt = PointDistanceInt.Where((x) => x.Item2 == PointDistanceInt.Min(y => y.Item2)).ToList();

                if (resultInt.Count == 0)
                    MessageBox.Show("uh oh");

                PointF finalPoint = resultInt[0].Item1;

                /**
                if (resultInt.Count > 1)
                {
                    if (!_goRight) // GO LEFT
                    {
                        if (!_swapYAxis) // GO LEFT DOWN
                        {
                            List<int> pointSystem = new List<int>();

                            PointF xPoint = finalPoint;
                            for (int i = 0; i < resultInt.Count; i++)
                            {
                                pointSystem.Add(0);

                                if (resultInt[i].Item1.Y == xPoint.Y)
                                {
                                    pointSystem[i] += 1;
                                }
                                else
                                {
                                    pointSystem[i] += 2;
                                }
                            }

                            int indexMax = !pointSystem.Any() ? -1 : pointSystem.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                            finalPoint = resultInt[indexMax].Item1;
                        }
                        else
                        {

                        }
                    }
                    else // GO RIGHT
                    {
                        if (!_swapYAxis) // GO RIGHT UP
                        {

                        }
                        else
                        {

                        }
                    }
                }

                if (!_goRight) // GO LEFT
                {
                    if (!_swapYAxis) // GO LEFT DOWN
                    {
                        List<PointF> temp = joinList(PointsToBottom, PointsToRight);
                        if (temp.Count == 1)
                        {
                            finalPoint = temp[0];
                        }
                    }
                    else
                    {

                    }
                }
                else // GO RIGHT
                {
                    if (!_swapYAxis) // GO RIGHT UP
                    {

                    }
                    else
                    {

                    }
                }
                **/

                _newUVArray.Add(finalPoint);
                continue;


            }

            List<PointF> finalUVArray = performGarham(_newUVArray.ToArray());
            return finalUVArray.ToArray();
        }

        private List<PointF> performGarham(PointF[] inputList)
        {
            Point[] intUVArray = new Point[inputList.Length];
            for (int i = 0; i < inputList.Length; i++)
            {
                intUVArray[i] = new Point(Convert.ToInt32(inputList[i].X), Convert.ToInt32(inputList[i].Y));
            }

            List<IntPoint> intPointUVArray = new List<IntPoint>();
            for (int i = 0; i < inputList.Length; i++)
            {
                intPointUVArray.Add(new IntPoint(intUVArray[i].X, intUVArray[i].Y));
            }

            GrahamConvexHull grahamScan = new GrahamConvexHull();
            List<IntPoint> opUVPoint = grahamScan.FindHull(intPointUVArray);

            List<PointF> newUVArray = new List<PointF>();
            for (int i = 0; i < intPointUVArray.Count; i++)
            {
                for (int j = 0; j < opUVPoint.Count; j++)
                {
                    if (intPointUVArray[i] == opUVPoint[j])
                    {
                        newUVArray.Add(inputList[i]);
                        break;
                    }
                }
            }

            return newUVArray;
        }

        private List<PointF> joinList(List<PointF> ToKeepList, List<PointF> fullList)
        {
            List<PointF> tempPoints = new List<PointF>();

            for (int i = 0; i < ToKeepList.Count; i++)
            {
                for (int x = 0; x < fullList.Count; x++)
                {
                    if (ToKeepList[i] == fullList[x])
                        tempPoints.Add(fullList[x]);
                }
            }

            return tempPoints;
        }

        private List<PointF> getInitialPoints(int pointIndex)
        {

            return null;
        }

        private List<PointF> getPotentialPoints(PointF currentPoint, List<PointF> initialPoints, bool goRight, ref bool swapYAxis)
        {
            List<PointF> pointListOutput = new List<PointF>();

            for (int index = 0; index < initialPoints.Count;)
            {
                // if point is on outline - y axis
                if (currentPoint.Y == _offsetYMax)
                {
                    if (initialPoints[index].Y == _offsetYMax)
                    {
                        pointListOutput.Add(initialPoints[index]);
                    }
                }
                else if (currentPoint.Y == _offsetYMin)
                {
                    if (initialPoints[index].Y == _offsetYMin)
                    {
                        pointListOutput.Add(initialPoints[index]);
                    }
                }

                // if point is on outline - y axis
                if (currentPoint.X == _offsetXMax)
                {
                    if (initialPoints[index].X == _offsetXMax)
                    {
                        pointListOutput.Add(initialPoints[index]);
                    }
                }
                else if (currentPoint.X == _offsetXMin)
                {
                    if (initialPoints[index].X == _offsetXMin)
                    {
                        pointListOutput.Add(initialPoints[index]);
                    }
                }

                if (!goRight) // GO LEFT
                {
                    if (!swapYAxis) // GO LEFT DOWN
                    {
                        if (currentPoint.Y <= initialPoints[index].Y)
                        {
                            if (currentPoint.X >= initialPoints[index].X)
                            {
                                pointListOutput.Add(initialPoints[index]);
                            }
                        }
                    }
                    else // GO LEFT UP
                    {
                        if (currentPoint.Y >= initialPoints[index].Y)
                        {
                            if (currentPoint.X >= initialPoints[index].X)
                            {
                                pointListOutput.Add(initialPoints[index]);
                            }
                        }
                    }
                }
                else // GO RIGHT
                {
                    if (!swapYAxis) // GO RIGHT UP
                    {
                        if (currentPoint.Y >= initialPoints[index].Y)
                        {
                            if (currentPoint.X <= initialPoints[index].X)
                            {
                                pointListOutput.Add(initialPoints[index]);
                            }
                        }
                    }
                    else // GO RIGHT DOWN
                    {
                        if (currentPoint.Y <= initialPoints[index].Y)
                        {
                            if (currentPoint.X <= initialPoints[index].X)
                            {
                                pointListOutput.Add(initialPoints[index]);
                            }
                        }
                    }
                }

                index++;
                if (index == initialPoints.Count)
                {
                    if (pointListOutput.Count == 0)
                    {
                        if (swapYAxis)
                            MessageBox.Show("UH OH");

                        index = 0;
                        swapYAxis = true;
                    }
                }
            }

            return pointListOutput;
        }

        private List<double> getPointsDistances(PointF currentPoint, List<PointF> pointsList, bool goRight, bool swapYAxis)
        {
            List<PointF> pointsWithoutOffset = getPointsWithoutOffset(pointsList);

            float x1 = currentPoint.X - _offsetXMin;
            float y1 = currentPoint.Y - _offsetYMin;

            List<double> pointsDistanceList = new List<double>();

            for (int index = 0; index < pointsWithoutOffset.Count; index++)
            {
                float x2 = pointsWithoutOffset[index].X;
                float y2 = pointsWithoutOffset[index].Y;

                float a = 0;
                float b = 0;

                if (!goRight) // go left
                {
                    a = x1 - x2;
                }
                else // go right
                {
                    a = x2 - x1;
                }

                if ((!swapYAxis && !goRight) || (goRight && swapYAxis))
                {
                    b = y2 - y1;
                }
                else
                {
                    b = y1 - y2;
                }

                float sqh = a * a + b * b;
                double distance = Math.Sqrt(sqh);

                pointsDistanceList.Add(distance);
            }

            return pointsDistanceList;
        }

        private List<PointF> getPointsWithoutOffset(List<PointF> pointsList)
        {
            List<PointF> pointsListOutput = new List<PointF>();
            for (int index = 0; index < pointsList.Count; index++)
            {
                PointF point = pointsList[index];
                float x = point.X;
                float y = point.Y;

                x = x - _offsetXMin;
                y = y - _offsetYMin;

                pointsListOutput.Add(new PointF(x, y));
            }

            return pointsListOutput;
        }
    }
}
