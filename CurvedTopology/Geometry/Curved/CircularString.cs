using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using CurvedTopologyExperiment.Geometry.CoordinateSequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurvedTopologyExperiment.Geometry.Curved
{    
    /// <summary>
    /// A circular <see cref="LineString"/> which can be linearized to such.
    /// </summary>
    public class CircularString : LineString, ICurvedGeometry<LineString>
    {
        /// <summary>
        /// This sequence is used as a fake to trick the constructor
        /// </summary>
        protected static CoordinateSequence FakeString2d =
                new CoordinateArraySequence(
                        new Coordinate[] { new Coordinate(0, 0), new Coordinate(1, 1) });

        /// <summary>
        ///  Helper class that automates the scan of the list of Arc in the control point sequence
        ///  not really used yet as the Factory paramter of Arc is not used, this would support transforms I believe
        /// </summary>
        internal class ArcVisitor
        {
            readonly GeometryFactory Factory;

            readonly System.Numerics.Vector4[] controlPoints;

            readonly double tolerance;

            readonly GrowableOrdinateArray ordinates;

            readonly Action<Arc> onVisitArc;

            /// <summary>
            /// Calls <see cref="Visit"/>
            /// </summary>
            /// <param name="circularString"></param>
            /// <param name="ordinates"></param>
            internal ArcVisitor(CircularString circularString, Action<Arc> onVisitArc, GrowableOrdinateArray ordinates = null)
                : this(onVisitArc, ordinates ?? new GrowableOrdinateArray())
            {
                tolerance = circularString.tolerance;
                Factory = circularString.Factory;
                controlPoints = circularString.controlPoints;                
                Visit();
            }

            internal ArcVisitor(Action<Arc> onVisitArc, GrowableOrdinateArray array) => (this.onVisitArc,ordinates) = (onVisitArc, array);

            void Visit()
            {
                //if (controlPoints.Length == 3)
                //{
                //    // single arc case
                //    //TODO finish Arc constructor Vectorization
                //    Arc arc = new Arc(controlPoints);
                //    OnVisit(arc, tolerance);
                //}
                //else
                //{
                // go over each 3-points set and Linearize it
                
                float[] arcControlPoints = new float[6];

                //Pass the array which will me modified OnVisit
                Arc arc = new Arc(arcControlPoints);

                for (int i = 0; i < controlPoints.Length; ++i)
                {
                    var src = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector4, byte>(ref controlPoints[i]), 16);

                    // have the arc work off the new control points
                    var dst = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<float, byte>(ref arcControlPoints[0]), 16);

                    dst.CopyTo(src);

                    arc.Reset();

                    OnVisit(arc, tolerance);
                }
                //}
            }

            //Todo, this should forward to the Tolerance method and the delete should accept tolerance
            internal void OnVisit(Arc arc) => onVisitArc(arc);

            internal virtual void OnVisit(Arc arc, double tolerance)
            {
                // if it's not the first arc, we need to eliminate the last point,
                // as the end point of the last arc is the start point of the new one
                if (ordinates.Count > 0)
                {
                    ordinates.Capacity = ordinates.Count - 2;
                }
                arc.Linearize(tolerance, Factory);
            }
        }

        //If this inherited from MultiPoint then it would only have points and then also support the dimensions of such
        //Since there are 6 components in each Arc Vector4<double> is required with XYZW support, store 2 vectors as 1 type with 2 vectors
        protected System.Numerics.Vector4[] controlPoints;

        /// <summary>
        /// 
        /// </summary>
        protected double tolerance;

        /// <summary>
        /// 
        /// </summary>
        protected LineString Linearized;

        public CircularString(CoordinateSequence points, GeometryFactory factory, double tolerance) : base(FakeString2d, factory)
        {
            this.tolerance = tolerance;

            int pointCount = points.Count;
            controlPoints = new System.Numerics.Vector4[pointCount];
            for (int i = 0; i < pointCount; ++i)
            {
                controlPoints[i] = new((float)points.GetX(i), (float)points.GetY(i), (float)points.GetZ(i), (float)points.GetM(i));
            }
            Initialize(controlPoints, tolerance);
        }

        public CircularString(System.Numerics.Vector4[] controlPoints, GeometryFactory factory, double tolerance) 
            : base(FakeString2d, factory)=> Initialize(controlPoints, tolerance);

        private void Initialize(System.Numerics.Vector4[] controlPoints, double tolerance)
        {
            int length = controlPoints.Length;
            if ((length & 1) == 1)
            {
                throw new InvalidOperationException(
                        "Invalid number of ordinates, must be even, but it is " + length + " instead");
            }
            int pointCount = length / 2;
            if ((pointCount != 0 && pointCount < 3) || (pointCount > 3 && (pointCount % 2) == 0))
            {
                throw new InvalidOperationException(
                        "Invalid number of points, a circular string "
                                + "is always made of an odd number of points, with a mininum of 3, "
                                + "and adding 2 for each extra circular arc in the sequence. Found: "
                                + pointCount);
            }
            this.controlPoints = controlPoints;
            this.tolerance = tolerance;
        }

        public System.Numerics.Vector4[] ControlPoints => controlPoints;

        public double Tolerance => tolerance;

        public int NumArcs=> (controlPoints.Length - 6) / 4 + 1;

        public Arc GetArcN(int arcIndex)
        {
            int baseIdx = arcIndex * 4;

            // Each Arc is 3 points and their control points for a total of 6 ordinates
            // have the arc work off the new control points

            //float[] arcControlPoints = new float[6];
            //var src = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector4, byte>(ref controlPoints[baseIdx]), 32);
            //var dst = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<float, byte>(ref arcControlPoints[0]), 32);
            //dst.CopyTo(src);

            Models.Vector8Float arcControlPoints = new Models.Vector8Float();
            arcControlPoints.High = controlPoints[baseIdx];
            arcControlPoints.Low = controlPoints[baseIdx + 1];

            Arc arc = new Arc(arcControlPoints);

            return arc;
        }

        public LineString Linearize()=> Linearize(this.tolerance);

        public LineString Linearize(double tolerance)
        {
            // use the cached one if we are asked for the default geometry tolerance
            bool isDefaultTolerance = Arc.Equals(tolerance, this.tolerance);

            if (Linearized != null && isDefaultTolerance)
            {
                return Linearized;
            }

            CoordinateSequence cs = GetLinearizedCoordinateSequence(tolerance);
            
            LineString result = new LineString(cs, Factory);

            if (isDefaultTolerance)
            {
                Linearized = result;
            }

            return result;
        }

        public CoordinateSequence GetLinearizedCoordinateSequence(double tolerance)
        {
            bool isDefaultTolerance = Arc.Equals(tolerance, this.tolerance);

            if (Linearized != null && isDefaultTolerance)
            {
                return Linearized.CoordinateSequence;
            }

            GrowableOrdinateArray gar = new GrowableOrdinateArray();

            CoordinateSequence cs = gar.ToCoordinateSequence(Factory);

            return cs;
        }

        /* Optimized overridden methods */

        public override bool IsClosed => controlPoints[0] == controlPoints[controlPoints.Length - 2]
                    && controlPoints[1] == controlPoints[controlPoints.Length - 1];

        public override bool IsEmpty => controlPoints.Length == 0;

        public override string GeometryType => "CircularString";

        public override OgcGeometryType OgcGeometryType => OgcGeometryType.CircularString;

        public override CircularString Reverse() => (CircularString)base.Reverse();

        protected override CircularString ReverseInternal()
        {
            // reverse the control points
            System.Numerics.Vector4[] reversed = new System.Numerics.Vector4[controlPoints.Length];
            System.Array.Copy(controlPoints, 0, reversed, 0, controlPoints.Length);
            GrowableOrdinateArray array = new GrowableOrdinateArray();
            array.AddRange(controlPoints);
            array.ReverseOrdinates(0, array.Count - 1);
            return new CircularString(reversed, Factory, tolerance);
        }

        public override Point InteriorPoint
        {
            get
            {
                int idx = controlPoints.Length / 2;
                return new Point(
                        new CoordinateArraySequence(
                                new[] {
                            new Coordinate(controlPoints[idx].X, controlPoints[idx].Y)
                                }),
                        Factory);
            }
        }

        protected override Envelope ComputeEnvelopeInternal()
        {
            Envelope result = new Envelope();

            new ArcVisitor(this, (a) => { a.ExpandEnvelope(result); });

            return result;
        }

        public override NetTopologySuite.Geometries.Geometry GetGeometryN(int n) => this;

        public override int NumGeometries => 1;

        public override bool IsRectangle => false;

        public new bool EqualsExact(NetTopologySuite.Geometries.Geometry other)
        {
            return EqualsExact(other, 0);
        }

        public override bool EqualsExact(NetTopologySuite.Geometries.Geometry other, double tolerance)
        {
            if (other is CircularString cs)
            {
                if (controlPoints.SequenceEqual(cs.controlPoints))
                {
                    return true;
                }
            }
            return Linearize(tolerance).EqualsExact(other, tolerance);
        }

        public new bool Equals(NetTopologySuite.Geometries.Geometry other)
        {
            if (other is CircularString cs)
            {
                if (controlPoints.SequenceEqual(cs.controlPoints))
                {
                    return true;
                }
            }
            return Linearize(tolerance).Equals(other);
        }

        public override bool EqualsTopologically(NetTopologySuite.Geometries.Geometry g)
        {
            if (g is CircularString cs)
            {
                if (controlPoints.SequenceEqual(cs.controlPoints))
                {
                    return true;
                }
            }
            return Linearize().EqualsTopologically(g);
        }

        public override int GetHashCode()=> base.GetHashCode();

        public override bool Equals(object o)
        {
            if (o is NetTopologySuite.Geometries.Geometry g) return Equals(g);
            return false;
        }

        public override string ToString() => ToCurvedText();

        public string ToCurvedText()
        {
            StringBuilder sb = new StringBuilder("CIRCULARSTRING ");
            if (IsEmpty)
            {
                sb.Append("EMPTY");
            }
            else
            {
                sb.Append("(");
                for (int i = 0; i < controlPoints.Length;)
                {
                    sb.Append(controlPoints[i++] + " " + controlPoints[i++]);
                    if (i < controlPoints.Length)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        public new Point GetPointN(int n) => n == 0 ? StartPoint : Linearize().GetPointN(n);

        public new Point StartPoint
        {
            get
            {
                var control = controlPoints[0];
                return new (
                   new CoordinateArraySequence(
                           new [] { new Coordinate(control.X, control.Y) }),
                   Factory);
            }
        }

        public new Point EndPoint
        {
            get
            {
                var control = controlPoints[controlPoints.Length - 1];
                return new (
                    new CoordinateArraySequence(
                            new [] {
                                    new Coordinate(
                                            control.X,
                                            control.Y)
                            }),
                    Factory);
            }
        }

        public override Coordinate Coordinate => Linearize().Coordinate;

        public override Coordinate[] Coordinates => Linearize().Coordinates;

        public new CoordinateSequence CoordinateSequence => Linearize().CoordinateSequence;

        public override int NumPoints => Linearize().NumPoints;

        public new Coordinate GetCoordinateN(int n) => Linearize().GetCoordinateN(n);

        public new bool IsRing => Linearize().IsRing;

        // todo: maybe compute the actual circular length?
        public override double Length => Linearize().Length;

        public override NetTopologySuite.Geometries.Geometry Boundary => Linearize().Boundary;

        public override bool IsCoordinate(Coordinate pt) => Linearize().IsCoordinate(pt);

        public override void Apply(ICoordinateFilter filter) => Linearize().Apply(filter);

        public override void Apply(ICoordinateSequenceFilter filter) => Linearize().Apply(filter);

        public override void Apply(IGeometryFilter filter) => Linearize().Apply(filter);

        public override void Apply(IGeometryComponentFilter filter) => Linearize().Apply(filter);

        public override void Normalize() => Linearize().Normalize();

        public override bool IsSimple=> Linearize().IsSimple;

        public override bool IsValid=> Linearize().IsValid;

        public override double Distance(NetTopologySuite.Geometries.Geometry g) => Linearize().Distance(g);

        public override bool IsWithinDistance(NetTopologySuite.Geometries.Geometry geom, double distance) => Linearize().IsWithinDistance(geom, distance);

        public override double Area => Linearize().Area;

        public override Point Centroid => Linearize().Centroid;

        public override bool Touches(NetTopologySuite.Geometries.Geometry g) => Linearize().Touches(g);

        public override bool Intersects(NetTopologySuite.Geometries.Geometry g) => Linearize().Intersects(g);

        public override bool Crosses(NetTopologySuite.Geometries.Geometry g) => Linearize().Crosses(g);

        public override bool Covers(NetTopologySuite.Geometries.Geometry g) => Linearize().Covers(g);

        public override bool Relate(NetTopologySuite.Geometries.Geometry g, string intersectionPattern)=> Linearize().Relate(g, intersectionPattern);

        public override bool Contains(NetTopologySuite.Geometries.Geometry g) => Linearize().Contains(g);

        public override bool Overlaps(NetTopologySuite.Geometries.Geometry g) => Linearize().Overlaps(g);

        public override IntersectionMatrix Relate(NetTopologySuite.Geometries.Geometry g) => Linearize().Relate(g);

        public override NetTopologySuite.Geometries.Geometry ConvexHull() => Linearize().ConvexHull();

        protected override NetTopologySuite.Geometries.Geometry CopyInternal()=> new CircularString(controlPoints, Factory, tolerance);

        public new void GeometryChanged()=> Linearize().GeometryChanged();

        public new bool Disjoint(NetTopologySuite.Geometries.Geometry g)=> Linearize().Disjoint(g);

        public new bool Within(NetTopologySuite.Geometries.Geometry g) => Linearize().Within(g);

        public new bool CoveredBy(NetTopologySuite.Geometries.Geometry other) => Linearize().CoveredBy(other);

        public new NetTopologySuite.Geometries.Geometry Buffer(double distance) => Linearize().Buffer(distance);

        public new NetTopologySuite.Geometries.Geometry Buffer(double distance, int quadrantSegments) => Linearize().Buffer(distance, quadrantSegments);

        public new NetTopologySuite.Geometries.Geometry Buffer(double distance, int quadrantSegments, NetTopologySuite.Operation.Buffer.EndCapStyle endCapStyle) => Linearize().Buffer(distance, quadrantSegments, endCapStyle);

        public new NetTopologySuite.Geometries.Geometry Intersection(NetTopologySuite.Geometries.Geometry other) => Linearize().Intersection(other);

        public new NetTopologySuite.Geometries.Geometry Union(NetTopologySuite.Geometries.Geometry other) => Linearize().Union(other);

        public new NetTopologySuite.Geometries.Geometry Difference(NetTopologySuite.Geometries.Geometry other) => Linearize().Difference(other);

        public new NetTopologySuite.Geometries.Geometry SymmetricDifference(NetTopologySuite.Geometries.Geometry other) => Linearize().SymmetricDifference(other);

        public new NetTopologySuite.Geometries.Geometry Union() => Linearize().Union();

        public new NetTopologySuite.Geometries.Geometry Normalized() => Linearize().Normalized();

        protected override bool IsEquivalentClass(NetTopologySuite.Geometries.Geometry other) => other is ICurvedGeometry<LineString>;

        protected override int CompareToSameClass(object o)
        {
            Linearize();
            return base.CompareToSameClass(o);
        }

        protected override int CompareToSameClass(object o, IComparer<CoordinateSequence> comp)
        {
            Linearize();
            return base.CompareToSameClass(o, comp);
        }

        public new string ToText() => Linearize().ToText();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize() => Linearize();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize(double tolerance) => Linearize(tolerance);

        public int CoordinatesDimension => (int)Dimension.Surface;

        bool IGeometry.IsGeometryCollection => false;
    }
}
