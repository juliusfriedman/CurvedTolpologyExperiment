using NetTopologySuite.Geometries;
using System;
using System.Linq;
using CurvedTopologyExperiment.Geometry.Models;
using NetTopologySuite.Algorithm;
//https://github.com/geotools/geotools/blob/master/modules/library/main/src/main/java/org/geotools/geometry/
namespace CurvedTopologyExperiment.Geometry.CoordinateSequences
{
    /// <summary>
    /// </summary>
    /// In Euclidean geometry, an arc is a connected subset of a differentiable curve. 
    /// Arcs of lines are called segments or rays, depending whether they are bounded or not. 
    /// A common curved example is a <see cref="Shapes.Circle"/>, called a circular arc. 
    /// In a sphere, an arc of a great circle is called a great arc.
    /// <remarks>
    /// inherit from PackedVectorFloatCoordinateSequence can already grow...count on base is fixed :(
    /// Should be in a different / folder namespace
    /// </remarks>
    public partial class Arc : 
        CoordinateSequence, 
        ICoordinateSequence,
        System.Collections.Generic.IEnumerable<Coordinate>
    {
        /// <summary>
        /// An arc which has 3 control points set to 0
        /// </summary>
        /// <remarks>
        /// Should have Coordinate.Zero in GeometryHelper but this needs 3 distinct coords?, it might not matter since it should never be modified but if Zero was used here for all 3 and 0 was changed then all 3 would change at the same time.
        /// </remarks>
        public static readonly Arc None = new Arc(new(), new(), new());        

        /// <summary>
        /// <see cref="Linearize(double, GeometryFactory)"/> to vector ordinates and create from.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static LineString Linearize(Arc a, GeometryFactory factory)
        {
            var linearized = a.Linearize(double.MaxValue, factory);
            var cs = new PackedVectorFloatCoordinateSequence(linearized.ToList());
            return new LineString(cs, factory);
        }

        /// <summary>
        /// Returns the angle of the point from the center and the horizontal line from the center.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="p">a point in space</param>
        /// <returns>The angle of the point from the center of the circle</returns>
        public static double ComputeAngle(Arc arc, Coordinate p)
        {
            double dx = p.X - arc.centerX;
            double dy = p.Y - arc.centerY;
            double angle;

            if (dx == 0.0)
            {
                if (dy == 0.0)
                {
                    angle = 0.0;
                }
                else if (dy > 0.0)
                {
                    angle = Angle.HalfPi;
                }
                else
                {
                    angle = Angle.HalfThreePi;
                }
            }
            else if (dy == 0.0)
            {
                if (dx > 0.0)
                {
                    angle = 0.0;
                }
                else
                {
                    angle = Math.PI;
                }
            }
            else
            {
                if (dx < 0.0)
                {
                    angle = Math.Atan(dy / dx) + Math.PI;
                }
                else if (dy < 0.0)
                {
                    angle = Math.Atan(dy / dx) + Angle.TwoPi;
                }
                else
                {
                    angle = Math.Atan(dy / dx);
                }
            }
            return angle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Angle ComputeAngle(Arc arc, Point p) => new(ComputeAngle(arc, p.Coordinate));

        /// <summary>
        /// Computes the <see cref="Angle"/> from the <paramref name="arc"/>
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="offset">start at this coordinate</param>
        /// <param name="end">end here</param>
        /// <returns></returns>
        public static Angle ComputeAngle(Arc arc, int offset, int end)
        {
            var p1 = arc.GetCoordinate(offset);
            var p2 = arc.GetCoordinate(end);
            var midPt = arc.Center;
            var p1Angle = ComputeAngle(arc, p1);

            // See if this arc covers the whole circle
            if (p1.Equals2D(p2))
            {
                return new(Angle.TwoPi);
            }
            else
            {
                var p2Angle = ComputeAngle(arc, p2);

                double midPtAngle = ComputeAngle(arc, midPt);

                // determine the direction
                var ccAngle = (Angle.Subtract(new(p1Angle), new(midPtAngle))
                                + Angle.Subtract(new(midPtAngle), new(p2Angle)));
                
                if (ccAngle.Radians < Angle.TwoPi)
                {
                    //this.clockwise = false;
                    return ccAngle;
                }
                else
                {
                    //this.clockwise = true;
                    return Angle.FromDegrees(Angle.TwoPi - ccAngle.TotalDegrees);
                }
            }
        }

        public static Angle ComputeAngle(Arc arc) => ComputeAngle(arc, 0, arc.Count - 1);

        /// <summary>
        /// Right?
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool IsClockwise(Arc a) => ComputeAngle(a).Radians < Angle.TwoPi;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double DefaultTolerace = 1.0e-12;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double DefaultCollinears = double.PositiveInfinity;

        /// <summary>
        /// Minimum number of segments per quadrant
        /// </summary>
        public static int DefaultMinSegmentsPerQuadrant { get; set; } = 12;

        /// <summary>
        /// Max number of segments per quadrant the system will use to satisfy the given tolerance
        /// </summary>
        public static int DefaultMaxSegmentsPerQuadrant { get; set; } = 10000;

        /// <summary>
        /// Allows to programmatically set the number of segments per quadrant (default to 12)
        /// </summary>
        /// <param name="baseSegmentsQuadrant"></param>
        public static void SetBaseSegmentsQuadrant(int baseSegmentsQuadrant)
        {
            if (baseSegmentsQuadrant < 0)
            {
                throw new InvalidOperationException("The base segments per quadrant must be at least 1");
            }
            DefaultMinSegmentsPerQuadrant = baseSegmentsQuadrant;
        }

        /// <summary>
        /// Allows to programmatically set the maximum number of segments per quadrant (default to 10000)
        /// </summary>
        /// <param name="baseSegmentsQuadrant"></param>
        public static void SetMaxSegmentsQuadrant(int baseSegmentsQuadrant)
        {
            if (baseSegmentsQuadrant < 0)
            {
                throw new InvalidOperationException("The max segments per quadrant must be at least 1");
            }
            DefaultMaxSegmentsPerQuadrant = baseSegmentsQuadrant;
        }        

        Vector8Float controlPoints;

        //Todo, Vector8Float this also and then use Vector math in the Linearization
        double w;

        double radius = double.NaN;

        double centerX;

        double centerY;

        public Arc(float[] controlPoints) : base(controlPoints.Length, 2, 0)
        {
            if (controlPoints == null || controlPoints.Length != 6)
            {
                throw new InvalidOperationException("Invalid control point array, it must be made of 6 ordinates for a total of 3 control points, start, mid and end");
            }
            this.controlPoints = new(controlPoints);
        }

        /// <summary>
        /// Narrows to <see cref="float"/>
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        /// <param name="ex"></param>
        /// <param name="ey"></param>
        public Arc(double sx, double sy, double mx, double my, double ex, double ey)
            : this(new float[] { (float)sx, (float)sy, (float)mx, (float)my, (float)ex, (float)ey })
        {
        }

        public Arc(Vector8Float controlPoints) :base(6,2,0) => this.controlPoints = controlPoints;

        public Vector8Float ControlPoints => controlPoints;

        public double Radius
        {
            get
            {
                InitializeCenterRadius();
                return radius;
            }
        }

        public Coordinate Center
        {
            get
            {
                InitializeCenterRadius();
                if (radius == DefaultCollinears)
                {
                    return null;
                }
                else
                {
                    return new Coordinate(centerX, centerY);
                }
            }
        }

        /// <summary>
        /// May be equal to <see cref="EndPoint"/>
        /// </summary>
        public Coordinate StartPoint => GetCoordinate(0);

        /// <summary>
        /// A <see cref="Point"/>
        /// </summary>
        public Coordinate EndPoint => GetCoordinate(Count - 1);


        /// <summary>
        /// 
        /// </summary>
        public Coordinate ChordCenter
        {
            get
            {
                var p1 = controlPoints.High;
                var p2 = controlPoints.Low;
                double centerX = p1.X + (p2.X - p1.X) / 2;
                double centerY = p1.Y + (p2.Y - p1.Y) / 2;
                return new Coordinate(centerX, centerY);
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="midPt"></param>
        /// <param name="p2"></param>
        public Arc (Coordinate p1, Coordinate midPt, Coordinate p2)
            : this(p1.X, p1.Y, midPt.X, midPt.Y, p2.X, p2.Y)
        {
            #region When you need the Angle and Orientation right away
            //var p1Angle = ComputeAngle(this, p1);
            //// See if this arc covers the whole circle
            //if (p1.Equals2D(p2))
            //{
            //    var p2Angle = Angle.TwoPi + p1Angle;
            //    //this.arcAngle = Angle.TwoPi;
            //}
            //else
            //{
            //    var p2Angle = ComputeAngle(this, p2);
            //    double midPtAngle = ComputeAngle(this, midPt);

            //    // determine the direction
            //    double ccDegrees =
            //            (Angle.Subtract(new(p1Angle), new(midPtAngle))
            //                    + Angle.Subtract(new(midPtAngle), new(p2Angle))).Radians;

            //    if (ccDegrees < Angle.TwoPi)
            //    {
            //        //this.clockwise = false;
            //        //this.arcAngle = ccDegrees;
            //    }
            //    else
            //    {
            //        //this.clockwise = true;
            //        //this.arcAngle = Angle.TwoPi - ccDegrees;
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="factory">ununsed</param>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<System.Numerics.Vector4> Linearize(double tolerance, GeometryFactory factory)
        {
            InitializeCenterRadius();

            // the collinear case is simple, we just return the control points (and do the same for same points case)
            if (radius == DefaultCollinears || radius == 0)
            {
                yield return controlPoints.High;
                yield return controlPoints.Low;
                yield break;
            }

            foreach(var vector in Linearize(tolerance, new GrowableOrdinateArray()))
            {
                yield return vector;
            }
        }

        GrowableOrdinateArray Linearize(double tolerance, GrowableOrdinateArray array)
        {
            InitializeCenterRadius();
            if (tolerance < 0)
            {
                throw new InvalidOperationException(
                        "The tolerance must be a positive number (or zero, to make the system use the "
                                + "max number of segments per quadrant configured in "
                                + "DefaultMaxSegmentsPerQuadrant, default is 10000)");
            }

            // ok, we need to find out which number of segments per quadrant
            // will get us below the threshold
            int segmentsPerQuadrant;
            if (tolerance == 0)
            {
                segmentsPerQuadrant = DefaultMaxSegmentsPerQuadrant;
            }
            else if (tolerance == double.MaxValue)
            {
                segmentsPerQuadrant = DefaultMinSegmentsPerQuadrant;
            }
            else
            {
                segmentsPerQuadrant = DefaultMinSegmentsPerQuadrant;
                double currentTolerance = ComputeChordCircleDistance(segmentsPerQuadrant);
                if (currentTolerance < tolerance)
                {
                    while (currentTolerance < tolerance && segmentsPerQuadrant > 1)
                    {
                        // going down
                        segmentsPerQuadrant /= 2;
                        currentTolerance = ComputeChordCircleDistance(segmentsPerQuadrant);
                    }
                    if (currentTolerance > tolerance)
                    {
                        segmentsPerQuadrant *= 2;
                    }
                }
                else
                {
                    while (currentTolerance > tolerance
                            && segmentsPerQuadrant < DefaultMaxSegmentsPerQuadrant)
                    {
                        // going up
                        segmentsPerQuadrant *= 2;
                        currentTolerance = ComputeChordCircleDistance(segmentsPerQuadrant);
                    }
                }
            }

            // now we linearize, using the following approach
            // - create regular segments, our base angle is always 0, to make sure concentric
            // arcs won't touch when linearized
            // - make sure the control points are included in the result

            //Vectorizing this is a little difficult there are 6 8 byte elements in use which is 48 bytes
            //Adding 2 unused elements makes this a Vector8Float
            double sx = controlPoints.A;
            double sy = controlPoints.B;
            double mx = controlPoints.C;
            double my = controlPoints.D;
            double ex = controlPoints.X;
            double ey = controlPoints.Y;

            // our reference angles
            double sa = Math.Atan2(sy - centerY, sx - centerX);
            double ma = Math.Atan2(my - centerY, mx - centerX);
            double ea = Math.Atan2(ey - centerY, ex - centerX);
            double step = Angle.HalfPi / segmentsPerQuadrant;

            // check if clockwise

            bool clockwise = (sa > ma && ma > ea) || (sa > ma && sa < ea) || (ma > ea && sa < ea);
            if (clockwise)
            {
                // we need to walk all arcs the same way, or we incur in the risk of having
                // two close but concentric arcs to touch each other
                double tx = sx;
                sx = ex;
                ex = tx;
                double ty = sy;
                sy = ey;
                ey = ty;
                double ta = sa;
                sa = ea;
                ea = ta;
            }

            // normalize angle so that we can treat steps like a linear progression
            if (ma < sa)
            {
                ma += System.Math.Tau;
                ea += System.Math.Tau;
            }
            else if (ea < sa)
            {
                ea += System.Math.Tau;
            }

            // the starting point
            double angle = (Math.Floor(sa / step) + 1) * step;
            // very short arc case, or high tolerance, we only use the control points
            if (angle > ea)
            {
                array.Add(controlPoints);
                return array;
            }

            // guessing number of points
            int points = 2 + (int)Math.Ceiling((ea - angle) / step);
            // this test might fail due to numeric reasons, in that case we might be
            // a couple of indexes short or long (depending on the way it fails)
            if (!IsWhole((ma - angle) / step))
            {
                points++;
            }

            int start = array.Count;
            array.Capacity = start + points * 2;
            // add the start point
            array.Add(new Vector8Float(sx, sy, double.NaN, double.NaN));

            // case where the "mid" point is actually very close to the start point
            if (angle > ma)
            {
                array.Add(new Vector8Float(mx, my, double.NaN, double.NaN));
                if (Equals(angle, ma))
                {
                    angle += step;
                }
            }
            // move on and add the other points
            double end = ea - DefaultTolerace;
            while (angle < end)
            {
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                array.Add(new Vector8Float(x, y, double.NaN, double.NaN));
                double next = angle + step;
                if (angle < ma && next > ma && !Equals(angle, ma) && !Equals(next, ma))
                {
                    array.Add(new Vector8Float(mx, my, double.NaN, double.NaN));
                }
                angle = next;
            }
            array.Add(new Vector8Float(ex, ey, double.NaN, double.NaN));
            if (clockwise)
            {
                array.ReverseOrdinates(start, array.Count - 1);
            }
            return array;
        }

        private bool IsWhole(double d)
        {
            long num = (long)d;
            double fractional = d - num;
            return fractional < DefaultTolerace;
        }

        private double ComputeChordCircleDistance(int segmentsPerQuadrant)
        {
            double halfChordLength = radius * Math.Sin(Angle.HalfPi / segmentsPerQuadrant);
            double apothem = Math.Sqrt(radius * radius - halfChordLength * halfChordLength);
            return radius - apothem;
        }

        private void InitializeCenterRadius()
        {
            if (double.IsNaN(radius))
            {
                double temp, bc, cd, determinate;

                double sx = controlPoints.A;
                double sy = controlPoints.B;
                double mx = controlPoints.C;
                double my = controlPoints.D;
                double ex = controlPoints.X;
                double ey = controlPoints.Y;

                /* Closed circle */
                if (Equals(sx, ex) && Equals(sy, ey))
                {
                    centerX = sx + (mx - sx) / 2.0;
                    centerY = sy + (my - sy) / 2.0;

                    radius = Math.Sqrt((centerX - sx) * (centerX - sx) + (centerY - sy) * (centerY - sy));
                }
                else
                {
                    temp = mx * mx + my * my;
                    bc = (sx * sx + sy * sy - temp) / 2.0;
                    cd = (temp - ex * ex - ey * ey) / 2.0;
                    determinate = (sx - mx) * (my - ey) - (mx - ex) * (sy - my);

                    /* Check collinearity */
                    if (Math.Abs(determinate) < DefaultTolerace)
                    {
                        radius = DefaultCollinears;
                        return;
                    }
                    determinate = 1.0 / determinate;
                    centerX = (bc * (my - ey) - cd * (sy - my)) * determinate;
                    centerY = ((sx - mx) * cd - (mx - ex) * bc) * determinate;

                    radius = Math.Sqrt((centerX - sx) * (centerX - sx) + (centerY - sy) * (centerY - sy));
                }
            }
        }

        public override string ToString() => $"Arc[{controlPoints}]";

        /// <summary>
        ///  Clears the caches (useful if the control points have been changed)
        /// </summary>
        internal void Reset() => radius = double.NaN;

        /// <summary>
        /// Checks if the two doubles provided are at a distance less than <see cref="DefaultTolerace"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static bool Equals(double a, double b) => Math.Abs(a - b) < DefaultTolerace;

        public Envelope Envelope
        {
            get
            {
                Envelope result = new Envelope();
                ExpandEnvelope(result);
                return result;
            }
        }

        /// <summary>
        /// Expands the given envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public override Envelope ExpandEnvelope(Envelope envelope)
        {
            InitializeCenterRadius();

            // get the points
            double sx = controlPoints.A;
            double sy = controlPoints.B;
            double mx = controlPoints.C;
            double my = controlPoints.D;
            double ex = controlPoints.X;
            double ey = controlPoints.Y;

            // add start and end
            envelope.ExpandToInclude(sx, sy);
            envelope.ExpandToInclude(ex, ey);

            // if it's not curved, we can exit now
            if (radius == DefaultCollinears)
            {
                return envelope;
            }

            // compute the reference angles
            double sa = Math.Atan2(sy - centerY, sx - centerX);
            double ma = Math.Atan2(my - centerY, mx - centerX);
            double ea = Math.Atan2(ey - centerY, ex - centerX);

            bool clockwise = (sa > ma && ma > ea) || (sa < ma && sa > ea) || (ma < ea && sa > ea);
            if (clockwise)
            {
                // we want to go counter-clock wise to simplify the rest of the algorithm
                double tx = sx;
                sx = ex;
                ex = tx;
                double ty = sy;
                sy = ey;
                ey = ty;
                double ta = sa;
                sa = ea;
                ea = ta;
            }

            // normalize angle so that we can treat steps like a linear progression
            if (ma <= sa)
            {
                ma += System.Math.Tau;
                ea += System.Math.Tau;
            }
            else if (ea <= sa)
            {
                ea += System.Math.Tau;
            }

            // scan the circle and add the points at 90° corners
            double angle = (Math.Floor(sa / Angle.HalfPi) + 1) * Angle.HalfPi;
            while (angle < ea)
            {
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                envelope.ExpandToInclude(x, y);
                angle += Angle.HalfPi;
            }

            return envelope;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ordinateIndex"></param>
        /// <returns></returns>
        public override double GetOrdinate(int index, int ordinateIndex)
        {
            if (index != 0) return double.NaN;
            switch (ordinateIndex)
            {
                case 0: return controlPoints.A;
                case 1: return controlPoints.B;
                case 2: return controlPoints.C;
                case 3: return controlPoints.D;
                case 4: return controlPoints.X;
                case 5: return controlPoints.Y;
                case 6: return controlPoints.Z;
                case 7: return controlPoints.W;
                default: return double.NaN;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ordinateIndex"></param>
        /// <param name="value"></param>
        public override void SetOrdinate(int index, int ordinateIndex, double value)
        {
            if (index != 0) return;
            switch (ordinateIndex)
            {
                case 0: controlPoints.A = (float)value; break;
                case 1: controlPoints.B = (float)value; break;
                case 2: controlPoints.C = (float)value; break;
                case 3: controlPoints.D = (float)value; break;
                case 4: controlPoints.X = (float)value; break;
                case 5: controlPoints.Y = (float)value; break;
                case 6: controlPoints.Z = (float)value; break;
                case 7: controlPoints.W = (float)value; break;
                default: return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CoordinateSequence Copy()=> new Arc(controlPoints);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="precisionModel"></param>
        /// <returns></returns>
        public Coordinate GetPoint(double angle, PrecisionModel precisionModel)
        {
            var center = Center;

            double x = Math.Cos(angle) * Radius;            
            x = x + center.X;
            x = precisionModel.MakePrecise(x);

            double y = Math.Sin(angle) * this.radius;
            y = y + center.Y;
            y = precisionModel.MakePrecise(y);

            return new Coordinate(x, y);
        }

        /// <summary>
        /// </summary>
        /// <param name="p">A <see cref="Coordinate"/> in space</param>
        /// <returns> The distance the point is from the center of the circle</returns>
        public double DistanceFromCenter(Coordinate p)=> Math.Abs(this.Center.Distance(p));

        /// <summary>
        /// Split into two starting at <paramref name="offset"/> and ending at <paramref name="end"/>
        /// </summary>
        /// <param name="isClockwise"></param>
        /// <param name="precisionModel"></param>
        /// <param name="offset"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Arc[] Split(bool isClockwise, PrecisionModel precisionModel, int offset, int end)
        {
            int directionFactor = isClockwise ? -1 : 1;
            var arcAngle = ComputeAngle(this);
            double angleOffset = directionFactor * (arcAngle.Radians / 2);
            var p1 = GetCoordinate(offset);
            var p1Angle = ComputeAngle(this, p1);
            double midAngle = p1Angle + angleOffset;
            Coordinate newMidPoint = GetPoint(midAngle, precisionModel);

            Arc arc1 = new Arc(p1, ChordCenter, newMidPoint);
            Arc arc2 = new Arc(newMidPoint, ChordCenter, GetCoordinate(end));
            return new Arc[] { arc1, arc2 };
        }

        ICoordinateSequence ICoordinateSequence.Copy() => new Arc(controlPoints);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<Coordinate> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return GetCoordinate(i);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();        
    }
}
