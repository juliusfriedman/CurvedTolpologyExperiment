using NetTopologySuite.Operation.Distance;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
//https://github.com/LiteCore/ConcaveHull/blob/master/ConcaveHull.cs
namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// A simple implementation
    /// </summary>
    public class ConcaveHull
    {
        public static Polygon GetConcaveHull(List<Coordinate> points, double concavity, double maxLength)
        {
            var hull = new ConvexHull(points.ToArray(), GeometryHelper.GeometryFactory).GetConvexHull();
            List<Coordinate> coords = hull.Coordinates.ToList();
            coords.RemoveAt(0);
            foreach (var c in coords)
            {
                points.RemoveAll(x => x.X == c.X && x.Y == c.Y);
            }
            var factory = GeometryHelper.GeometryFactory;
            for (int i = 0; i < coords.Count; ++i)
            {
                ClearLines(ref coords);
                var line = factory.CreateLineString(new Coordinate[] { GetElement(coords, i), GetElement(coords, i + 1) });
                if (line.Length <= maxLength)
                {
                    continue;
                }
                var leftLine = factory.CreateLineString(new Coordinate[] { GetElement(coords, i - 1), GetElement(coords, i) });
                var rightLine = factory.CreateLineString(new Coordinate[] { GetElement(coords, i + 1), GetElement(coords, i + 2) });
                var nearestPoint = GetNearestPoint(points, line, leftLine, rightLine);
                if (nearestPoint == null)
                {
                    continue;
                }
                var eh = line.Length;
                var dd = nearestPoint.M;
                if ((eh / dd) > concavity && !IsIntersectsLines(line, nearestPoint, coords))
                {
                    coords.Insert((i + 1) % points.Count, nearestPoint);
                    points.RemoveAll(x => x.X == nearestPoint.X && x.Y == nearestPoint.Y);
                    i = -1;
                }
            }
            if (coords.Count <= 0) return GeometryHelper.EmptyPolygon;
            coords.Add(coords.First());
            if (coords.Count <= 2)
            {
                return coords.Count <= 1 ? (Polygon)factory.CreatePoint(coords[0]).Buffer(concavity) : GetConcaveHull(new List<Coordinate>(factory.CreateLineString(coords.ToArray()).ConvexHull().Coordinates), concavity, maxLength);
            }
            return factory.CreatePolygon(coords.ToArray());
        }

        private static double GetMaxLength(List<Coordinate> coords)
        {
            var factory = GeometryHelper.GeometryFactory;
            var maxDist = double.NegativeInfinity;
            for (int i = 0; i < coords.Count; ++i)
            {
                var line = factory.CreateLineString(new Coordinate[] { GetElement(coords, i), GetElement(coords, i + 1) });
                if (line.Length > maxDist)
                {
                    maxDist = line.Length;
                }
            }
            return maxDist / 8;
        }

        private static bool IsIntersectsLines(LineString segment, Coordinate point, List<Coordinate> coords)
        {
            var factory = GeometryHelper.GeometryFactory;
            var firstLine = factory.CreateLineString(new Coordinate[] { segment.StartPoint.Coordinate, point });
            var secondLine = factory.CreateLineString(new Coordinate[] { segment.EndPoint.Coordinate, point });
            for (int i = 0; i < coords.Count; ++i)
            {
                var line = factory.CreateLineString(new Coordinate[] { GetElement(coords, i), GetElement(coords, i + 1) });
                if (firstLine.Crosses(line) || secondLine.Crosses(line))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ClearLines(ref List<Coordinate> coordinates)
        {
            coordinates = coordinates.Distinct(new CoordinateComparer()).ToList();
        }

        private static Coordinate GetNearestPoint(List<Coordinate> coordinates, LineString line, LineString left, LineString right)
        {
            var factory = GeometryHelper.GeometryFactory;
            var dist = double.PositiveInfinity;
            Coordinate res = null;
            var middlePoint = GeometryHelper.GetMiddlePoint(line);
            foreach (var point in coordinates)
            {
                var p = factory.CreatePoint(point);
                var d = GetDistance(middlePoint, p);
                var l = GetDistance(left, p);
                var r = GetDistance(right, p);
                if (d < dist && d != 0d && l > d && r > d)
                {
                    dist = d;
                    res = point;
                    res.M = d;
                }
            }
            return res;
        }

        private static Coordinate GetElement(List<Coordinate> points, int i)
        {
            if (i >= points.Count)
            {
                return points[i % points.Count];
            }
            else if (i < 0)
            {
                return points[(i % points.Count) + points.Count];
            }
            else
            {
                return points[i];
            }
        }
        private static double GetDistance(LineString line, Point point)
        {
            //var x0 = point.X;
            //var y0 = point.Y;
            //var x1 = line.StartPoint.X;
            //var x2 = line.EndPoint.X;
            //var y1 = line.StartPoint.Y;
            //var y2 = line.EndPoint.Y;
            //return Math.Abs((((y2 - y1) * point.X) - ((x2 - x1) * point.Y) + (x2 * y1) - (y2 * x1))) / (Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2)));
            return DistanceComputer.PointToSegment(point.Coordinate, line.StartPoint.Coordinate, line.EndPoint.Coordinate);
        }

        private static double GetDistance(Point point1, Point point2) => DistanceOp.Distance(point1, point2);
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class CoordinateComparer : IEqualityComparer<Coordinate>
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        bool IEqualityComparer<Coordinate>.Equals(Coordinate x, Coordinate y)=> x.X == y.X && x.Y == y.Y;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        int IEqualityComparer<Coordinate>.GetHashCode(Coordinate obj)=> obj.GetHashCode();
    }
}