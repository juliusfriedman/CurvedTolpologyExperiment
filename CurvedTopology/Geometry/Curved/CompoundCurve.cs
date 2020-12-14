using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurvedTopologyExperiment.Geometry.Curved
{
    /// <summary>
    /// A CompoundCurve is a collection of zero or more continuous CircularString or LineString instances of either geometry or geography types.
    /// </summary>
    public class CompoundCurve : GeometryCollectionEx, 
        ICompoundCurvedGeometry<LineString>
    {
        double tolerance;

        public double Tolerance => tolerance;

        //NetTopologySuite.Geometries.Geometry
        public LineString linearized;

        public CompoundCurve(IEnumerable<NetTopologySuite.Geometries.Geometry> components, GeometryFactory factory, double tolerance) : base(factory, components)
        {
            this.tolerance = tolerance;

            LineString prev = null;

            int i = 0;

            // sanity check, we don't want compound curves containing other compound curves
            foreach (LineString ls in components)
            {
                //I do not inherit from CircularString
                //if (ls is CompoundCurve cc)
                //{
                //    this.components.addAll(cc.Components);
                //}
                //else
                //{
                //    this.components.add(ls);
                //}

                // check connectedness
                LineString curr = ls;
                if (i > 0 && prev != null)
                {
                    Coordinate endPoint = prev.GetCoordinateN(prev.NumPoints - 1);
                    Coordinate startPoint = curr.GetCoordinateN(0);
                    if (!endPoint.Equals(startPoint))
                    {
                        throw new InvalidOperationException(
                                "Found two elements that are not connected, " + prev + " and " + curr);
                    }
                }

                prev = curr;
                ++i;
            }
        }        

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override int CoordinatesDimension
        {
            get
            {
                if (IsEmpty)
                {
                    return 2;
                }
                int dimension = int.MaxValue;
                int curr = 0;
                foreach (ILineal component in Components)
                {
                    if (component is ICurvedGeometry g)
                    {
                        curr = g.CoordinatesDimension;
                    }
                    else if (component is LineString l)
                    {
                        curr = l.CoordinateSequence.Dimension;
                    }
                    else if (component is IGeometry c)
                    {
                        curr = c.CoordinatesDimension;
                    }
                    else if(component is NetTopologySuite.Geometries.Geometry _)
                    {
                        throw new NotImplementedException("Not yet implemented!");
                    }
                    dimension = Math.Min(curr, dimension);
                }
                return dimension;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEnumerable<LineString> Components
        {
            get
            {
                for(int i = 0; i < NumGeometries; ++i)
                {
                    var g = GetGeometryN(i);

                    if (g is not LineString il) continue;

                    yield return il;
                }
            }
        }

        //NumGeometries > 1 && Components.First().Equals(Components.Last());
        public bool IsClosed => NumGeometries > 1 &&
            GetGeometryN(0).Equals(GetGeometryN(NumGeometries - 1))
            ||
            NumGeometries == 1 && false == GetGeometryN(0).IsValid;

        public override bool IsEmpty => Components.Any(l => l.IsEmpty);

        public LineString Linearize() => Linearize(tolerance);

        public LineString Linearize(double tolerance)
        {
            // use the cached one if we are asked for the default geometry tolerance
            bool isDefaultTolerance = CoordinateSequences.Arc.Equals(tolerance, this.tolerance);
            if (linearized != null && isDefaultTolerance)
            {
                return linearized;
            }

            CoordinateSequence cs = GetLinearizedCoordinateSequence(tolerance);
            LineString result = new LineString(cs, Factory);
            if (isDefaultTolerance)
            {
                linearized = result;
            }

            return result;
        }

        public CoordinateSequence GetLinearizedCoordinateSequence(double tolerance)
        {
            // collect all the points of all components
            GrowableOrdinateArray gar = new GrowableOrdinateArray();
            CoordinateSequence cs;
            foreach (ILineal component in Components)
            {
                // the last point of the previous element is the first point of the next one,
                // remove the duplication
                if (gar.Count > 0)
                {
                    gar.Capacity = gar.Count - 2;
                }
                // linearize with tolerance the circular strings, take the linear ones as is
                if (component is ICurvedLineGeometry<LineString> cg)
                {
                    cs = cg.GetLinearizedCoordinateSequence(tolerance);
                    gar.Add(cs);
                }
                else if (component is LineString ls)
                {
                    cs = ls.CoordinateSequence;
                    for (int i = 0; i < cs.Count; i++)
                    {
                        gar.Add(new Models.Vector8Float(cs.GetX(i), cs.GetY(i), cs.GetZ(i), cs.GetM(i)));
                    }
                }
            }

            cs = gar.ToCoordinateSequence(Factory);
            return cs;
        }

        public override bool IsRectangle => Linearize().IsRectangle;

        public override Coordinate[] Coordinates => Linearize().Coordinates;

        public CoordinateSequence CoordinateSequence
        {
            get 
            {
                Linearize();

                if (linearized is CircularString cs) return cs.CoordinateSequence;
                else if (linearized is LineString ls) return ls.CoordinateSequence;

                return null;
            }
        }

        public Coordinate GetCoordinateN(int n) => Linearize().GetCoordinateN(n);

        public override Coordinate Coordinate => Linearize().Coordinate;

        public override int NumPoints=> Linearize().NumPoints;

        public Point GetPointN(int n) => Linearize().GetPointN(n);

        public Point StartPoint => Linearize().StartPoint;

        public Point EndPoint => Linearize().EndPoint;

        public bool IsRing => Linearize().IsRing;

        // todo: maybe compute the actual circular length?
        public override double Length => Linearize().Length;

        public override NetTopologySuite.Geometries.Geometry Boundary => Linearize().Boundary;

        public bool IsCoordinate(Coordinate pt) => Linearize().IsCoordinate(pt);

        public override void Apply(ICoordinateFilter filter) => Linearize().Apply(filter);

        public override void Apply(ICoordinateSequenceFilter filter) => Linearize().Apply(filter);

        public override void Apply(IGeometryFilter filter) => Linearize().Apply(filter);

        public override void Apply(IGeometryComponentFilter filter) => Linearize().Apply(filter);

        public override string GeometryType => "CompoundCurve";

        public override OgcGeometryType OgcGeometryType => OgcGeometryType.CompoundCurve;

        public override CompoundCurve Reverse() => (CompoundCurve)base.Reverse();

        protected override CompoundCurve ReverseInternal()
        {
            // reverse the component, and reverse each component internal elements
            List<NetTopologySuite.Geometries.Geometry> reversedComponents = new(NumGeometries);
            for(int i = 0; i < NumGeometries; ++i)
            {
                var g = GetGeometryN(i);
                reversedComponents.Insert(0, g.Reverse());
            }
            return new CompoundCurve(reversedComponents, Factory, tolerance);
        }

        public override Point InteriorPoint => GetGeometryN(NumGeometries / 2).InteriorPoint;

        protected override Envelope ComputeEnvelopeInternal()
        {
            Envelope result = new Envelope();
            for (int i = 0; i < NumGeometries; ++i)
            {
                var g = GetGeometryN(i);
                result.ExpandToInclude(g.EnvelopeInternal);
            }
            return result;
        }

        public override bool EqualsExact(NetTopologySuite.Geometries.Geometry other, double tolerance)
        {
            if (other is CompoundCurve cc) {
                if (cc.NumGeometries == NumGeometries)
                {
                    return false;
                }
                for (int i = 0; i < NumGeometries; ++i)
                {
                    var ls1 = GetGeometryN(i);
                    var ls2 = cc.GetGeometryN(i);
                    if (!ls1.EqualsExact(ls2, tolerance))
                    {
                        return false;
                    }
                }

                return true;
            }
            return Linearize(tolerance).EqualsExact(other, tolerance);
        }

        public override bool EqualsTopologically(NetTopologySuite.Geometries.Geometry g) => g is CompoundCurve cc ? cc.Components.SequenceEqual(Components) : Linearize().EqualsTopologically(g);

        protected void Add(LineString lineString) => base.Add(lineString);

        protected bool Remove(LineString lineString, IEqualityComparer<NetTopologySuite.Geometries.Geometry> comparer = null) => base.Remove(lineString, comparer);

        public string ToCurvedText() => ToCurvedText(false, false);

        public string ToCurvedText(bool z = false, bool m = false)
        {
            StringBuilder sb = new StringBuilder("COMPOUNDCURVE ");
            if (IsEmpty)
            {
                sb.Append(" EMTPY");
            }
            else
            {
                sb.Append("(");
                
                for (int i = 0; i < NumGeometries; ++i)
                {
                    var g = GetGeometryN(i);

                    if (g is not Point p) continue;

                    sb.Append(p.X + " " + p.Y);

                    if (z && double.IsFinite(p.Z))
                    {
                        sb.Append(" " + p.Z);
                    }

                    if (m && double.IsFinite(p.M))
                    {
                        sb.Append(" " + p.M);
                    }

                    if (i < NumGeometries - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.Append(")");
            }
            return sb.ToString();
        }

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize() => Linearize();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize(double tolerance) => Linearize(tolerance);
    }
}
