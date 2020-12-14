using CurvedTopologyExperiment.Geometry;
using CurvedTopologyExperiment.Geometry.CoordinateSequences;
using CurvedTopologyExperiment.Geometry.Models;
using CurvedTopologyExperiment.Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    public class GeometryTests
    {
        static Coordinate ORIGIN = new Coordinate(0, 0);

        //he distance from 1.0 to the next largest double-precision number
        static double EPS = 1e-6;

        static GeometryFactory GeometryFactory = GeometryHelper.GeometryFactory;

        bool clockwise(Circle circle, System.Numerics.Vector4[] ordinates)
        {
            var temp = ordinates[0];
            double sa = System.Math.Atan2(temp.X - circle.Cx, temp.Y - circle.Cy);
            //double sa = System.Math.Atan2(ordinates[1] - circle.Cx, ordinates[0] - circle.Cy);

            temp = ordinates[1];
            //double ma = System.Math.Atan2(ordinates[3] - circle.Cx, ordinates[2] - circle.Cy);
            double ma = System.Math.Atan2(temp.X - circle.Cx, temp.Y - circle.Cy);


            temp = ordinates[2];
            //double ea = System.Math.Atan2(ordinates[5] - circle.Cx, ordinates[3] - circle.Cy);
            double ea = System.Math.Atan2(temp.X - circle.Cx, temp.Y - circle.Cy);

            return (sa > ma && ma > ea) || (sa < ma && sa > ea) || (ma < ea && sa > ea);
        }

        public void AssertTolerance(Circle circle, System.Numerics.Vector4[] ordinates, double tolerance)
        {
            // force counter-clockwise order to simplify the testing logic
            if (clockwise(circle, ordinates))
            {
                for (int i = 0; i < ordinates.Length / 2; i++)
                {
                    var temp = ordinates[i];
                    ordinates[i] = ordinates[ordinates.Length - i - 1];
                    ordinates[ordinates.Length - i - 1] = temp;
                }
            }

            double prevAngle = 0;
            for (int i = 0; i < ordinates.Length; i += 2)
            {
                // check the point is on the circle
                var ordinate = ordinates[i];
                var x = ordinate.X;
                var y = ordinate.Y;
                double dx = x - circle.Cx;
                double dy = y - circle.Cy;
                double d = System.Math.Sqrt(dx * dx + dy * dy);
                double distanceFromCircle = System.Math.Abs(circle.Radius - d);
                if (distanceFromCircle > tolerance)
                {
                    throw new System.Exception(
                            "Found a point "
                                    + x
                                    + ","
                                    + y
                                    + " that's not on the circle with distance "
                                    + distanceFromCircle
                                    + " from it");
                }
                Assert.AreEqual(circle.Radius, d, tolerance);
                double angle = System.Math.Atan2(dy, dx);

                if (i > 1)
                {
                    double chordAngle = angle - prevAngle;
                    if (chordAngle < 0)
                    {
                        chordAngle += System.Math.PI * 2;
                    }
                    else if (chordAngle > System.Math.PI)
                    {
                        chordAngle = System.Math.PI * 2 - chordAngle;
                    }
                    double halfChordLength = circle.Radius * System.Math.Sin(chordAngle / 2);
                    double apothem = System.Math.Sqrt(circle.Radius * circle.Radius - halfChordLength * halfChordLength);
                    double distance = circle.Radius - apothem;
                    if (distance > tolerance)
                    {
                        throw new System.Exception(
                                "Max tolerance is "
                                        + tolerance
                                        + " but found a chord that is at "
                                        + distance
                                        + " from the circle");
                    }
                }
                prevAngle = angle;
            }
        }

        [TestInitialize]
        public void setupBaseSegmentsQuadrant()
        {
            // we want to run the test at a higher precision
            Arc.DefaultMinSegmentsPerQuadrant = 32;
        }

        [TestCleanup]
        public void resetBaseSegmentsQuadrant()
        {
            // we want to run the test at a higher precision
            Arc.DefaultMinSegmentsPerQuadrant = 12;
        }

        Envelope envelopeFrom(Arc arc, params double[] otherPoints)
        {
            Envelope env = new Envelope();

            //arc.Envelope?

            // add the control points
            env.ExpandToInclude(arc.ControlPoints.A, arc.ControlPoints.B);
            env.ExpandToInclude(arc.ControlPoints.C, arc.ControlPoints.D);
            env.ExpandToInclude(arc.ControlPoints.X, arc.ControlPoints.Y);
            if (otherPoints != null)
            {
                // add the other points
                for (int i = 0; i < otherPoints.Length;)
                {
                    env.ExpandToInclude(otherPoints[i++], otherPoints[i++]);
                }
            }

            return env;
        }

        static void assertCoordinateEquals(Coordinate expected, Coordinate actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.AreEqual(expected.X, actual.X, EPS);
                Assert.AreEqual(expected.Y, actual.Y, EPS);
            }
        }

        [TestMethod]
        public void testCollinear()
        {
            Arc arc = new Arc(0, 0, 0, 10, 0, 20);
            Assert.AreEqual(Arc.DefaultCollinears, arc.Radius, 0d);
            assertCoordinateEquals(null, arc.Center);
            Vector8Float expected = new(0f, 0f, 0f, 10f, 0f, 20f, 0, 0);
            var Linearized = arc.Linearize(0, GeometryFactory).ToList();
            Assert.AreEqual(2, Linearized.Count);
            Assert.AreEqual(expected.High, Linearized[0]);
            Assert.AreEqual(expected.Low, Linearized[1]);
            Assert.AreEqual(envelopeFrom(arc), arc.Envelope);
        }

        [TestMethod]
        public void testSamePoints()
        {
            Arc arc = new Arc(0, 0, 0, 0, 0, 0);
            Assert.AreEqual(0, arc.Radius, 0d);
            assertCoordinateEquals(ORIGIN, arc.Center);
            var Linearized = arc.Linearize(0, GeometryFactory).ToList();
            Vector8Float expected = new();
            Assert.AreEqual(2, Linearized.Count);
            Assert.AreEqual(expected.High, Linearized[0]);
            Assert.AreEqual(expected.Low, Linearized[1]);
            Assert.AreEqual(envelopeFrom(arc), arc.Envelope);
            Assert.AreEqual(0, arc.Envelope.Area, 0d);
        }

        [TestMethod]
        public void testMinuscule()
        {
            Circle circle = new Circle(100);
            Arc arc = circle.GetArc(0, Angle.HalfPi / 128, Angle.HalfPi / 64);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize with a large tolerance, we should get back just the control points
            var Linearized = arc.Linearize(10, GeometryFactory).ToList();
            var expected = arc.ControlPoints;
            Assert.AreEqual(2, Linearized.Count);
            Assert.AreEqual(expected.High, Linearized[0]);
            Assert.AreEqual(expected.Low, Linearized[1]);
            Assert.AreEqual(envelopeFrom(arc), arc.Envelope);
        }

        [TestMethod]
        public void testMatchingSequence()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            Arc arc =
                    circle.GetArc(0, Angle.HalfPi / 32, Angle.HalfPi / 16); ;
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize with a large tolerance, we should get back just the control points
            var Linearized = arc.Linearize(10, GeometryFactory).ToList();
            var expected = new Vector8Float(Linearized[0], Linearized[1]);
            Assert.AreEqual(2, Linearized.Count);
            Assert.AreEqual(expected.High, Linearized[0]);
            Assert.AreEqual(expected.Low, Linearized[1]);
        }

        [TestMethod]
        public void testOutsideSequence()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float halfStep = (float)Angle.HalfPi / 64;
            Arc arc = circle.GetArc(halfStep, halfStep * 3, halfStep * 5);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back the control points, plus the regular points in the middle
            var expected =
                    circle.SamplePoints(
                            halfStep, halfStep * 2f, halfStep * 3f, halfStep * 4f, halfStep * 5f);
            var linearized = arc.Linearize(0.1, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
        }

        [TestMethod]
        public void testOutsideSequenceClockwise()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float halfStep = (float)Angle.HalfPi / 64f;
            Arc arc = circle.GetArc(halfStep * 5, halfStep * 3, halfStep);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back the control points, plus the regular points in the middle
            var expected =
                    circle.SamplePoints(
                            halfStep * 5f, halfStep * 4f, halfStep * 3f, halfStep * 2f, halfStep);
            var linearized = arc.Linearize(0.1, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
            Assert.AreEqual(envelopeFrom(arc), arc.Envelope);
        }

        [TestMethod]
        public void testStartMatchSequence()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float halfStep = (float)Angle.HalfPi / 64;
            Arc arc = circle.GetArc(0, halfStep * 3, halfStep * 5);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back the control points, plus the regular points in the middle
            var expected =
                    circle.SamplePoints(0, halfStep * 2, halfStep * 3, halfStep * 4, halfStep * 5);
            var linearized = arc.Linearize(0.2, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
        }

        [TestMethod]
        public void testMidMatchSequence()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float halfStep = (float)Angle.HalfPi / 64;
            Arc arc = circle.GetArc(halfStep, halfStep * 2, halfStep * 5);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back the control points, plus the regular points in the middle
            var expected = circle.SamplePoints(halfStep, halfStep * 2, halfStep * 4, halfStep * 5);
            var linearized = arc.Linearize(0.2, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
        }

        [TestMethod]
        public void testEndMatchSequence()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float halfStep = (float)Angle.HalfPi / 64;
            Arc arc = circle.GetArc(halfStep, halfStep * 3, halfStep * 4);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back only control points, plus the regular points in the middle
            var expected = circle.SamplePoints(halfStep, halfStep * 3, halfStep * 4);
            var linearized = arc.Linearize(0.2, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
        }

        [TestMethod]
        public void testMatchTolerance()
        {
            Circle circle = new Circle(100);
            Arc arc = circle.GetArc(0, System.Math.PI / 2, System.Math.PI);
            // test with subsequently smaller tolerances, but avoid going too low or
            // numerical issues will byte us, this one stops roughly at 2.4e-7
            double tolerance = 1;
            for (int i = 0; i < 12; i++)
            {
                var Linearized = arc.Linearize(tolerance, GeometryFactory).ToArray();
                // System.out.println(tolerance + " --> " + Linearized.Length);
                //assertTrue(Linearized.Length >= 64);
                Assert.IsTrue(Linearized.Length >= 32);
                AssertTolerance(circle, Linearized, tolerance);
                tolerance /= 4;
            }
        }

        [TestMethod]
        public void testMatchToleranceClockwise()
        {
            Circle circle = new Circle(100);
            Arc arc = circle.GetArc(System.Math.PI, System.Math.PI / 2, 0);
            // test with subsequently smaller tolerances, but avoid going too low or
            // numerical issues will byte us, this one stops roughly at 2.4e-7
            double tolerance = 1;
            for (int i = 0; i < 12; i++)
            {
                var Linearized = arc.Linearize(tolerance, GeometryFactory).ToArray();
                // System.out.println(tolerance + " --> " + Linearized.Length);
                //assertTrue(Linearized.Length >= 64);
                Assert.IsTrue(Linearized.Length >= 32);
                AssertTolerance(circle, Linearized, tolerance);
                tolerance /= 4;
            }
        }

        [TestMethod]
        public void testCrossPIPI()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            float step = (float)Angle.HalfPi / 32;
            Arc arc = circle.GetArc(-step * 2, step, step * 2);
            Assert.AreEqual(100, arc.Radius, 1e-9);
            assertCoordinateEquals(ORIGIN, arc.Center);
            // Linearize, we should get back the control points, plus the regular points in the middle
            var expected = circle.SamplePoints(-step * 2, -step, 0, step, step * 2);
            var linearized = arc.Linearize(0.2, GeometryFactory).ToArray();
            Assert.AreEqual(expected.Length, linearized.Length);
            for (int i = 0, e = expected.Length; i < e; ++i)
            {
                var src = expected[i];
                var dst = linearized[i];
                Assert.AreEqual(src.X, dst.X, EPS);
                Assert.AreEqual(src.Y, dst.Y, EPS);
                Assert.AreEqual(src.Z, dst.Z, EPS);
                Assert.AreEqual(src.W, dst.X, EPS);
            }
            Assert.AreEqual(envelopeFrom(arc, 100, 0), arc.Envelope);
        }

        [TestMethod]
        public void testFullCircle()
        {
            Circle circle = new Circle(100);
            // create control points that will match exactly the points the algo should generate
            Arc arc = circle.GetArc(0, System.Math.PI, 0);
            Assert.AreEqual(envelopeFrom(arc, 100, 0, 0, 100, -100, 0, 0, -100), arc.Envelope);
        }

        [TestMethod]
        public void testOrientations()
        {
            Circle circle = new Circle(100);
            // half circle up
            Assert.AreEqual(
                    OrientationIndex.CounterClockwise,
                    getOrientationIndex(getLinearizedArc(circle, 0, System.Math.PI / 2, System.Math.PI)));
            Assert.AreEqual(
                    OrientationIndex.Clockwise, getOrientationIndex(getLinearizedArc(circle, System.Math.PI, System.Math.PI / 2, 0)));
            Assert.AreEqual(
                    OrientationIndex.Clockwise, getOrientationIndex(getLinearizedArc(circle, -System.Math.PI, System.Math.PI / 2, 0)));
            // half circle down
            Assert.AreEqual(
                    OrientationIndex.CounterClockwise,
                    getOrientationIndex(getLinearizedArc(circle, System.Math.PI, System.Math.PI * 3 / 2, 0)));
            Assert.AreEqual(
                    OrientationIndex.Clockwise,
                    getOrientationIndex(getLinearizedArc(circle, 0, System.Math.PI * 3 / 2, System.Math.PI)));
            Assert.AreEqual(
                    OrientationIndex.Clockwise,
                    getOrientationIndex(getLinearizedArc(circle, 0, -System.Math.PI / 2, -System.Math.PI)));
            // end between start and mid, wrapping
            Assert.AreEqual(
                    OrientationIndex.CounterClockwise,
                    getOrientationIndex(
                            getLinearizedArc(circle, System.Math.PI / 2, System.Math.PI / 4, System.Math.PI * 3 / 8)));
            Assert.AreEqual(
                    OrientationIndex.Clockwise,
                    getOrientationIndex(
                            getLinearizedArc(circle, System.Math.PI * 3 / 8, System.Math.PI / 4, System.Math.PI / 2)));
        }

        private OrientationIndex getOrientationIndex(LineString ls)
        {
            return Orientation.Index(ls.GetCoordinateN(0), ls.GetCoordinateN(1), ls.GetCoordinateN(2));
        }

        private LineString getLinearizedArc(
                Circle c, double startAngle, double midAngle, double endAngle)
        {
            Arc arc = c.GetArc(startAngle, midAngle, endAngle);
            var Linearized = arc.Linearize(double.MaxValue, GeometryFactory).ToArray();
            Coordinate[] coords = new Coordinate[Linearized.Length];
            for (int i = 0; i < coords.Length; i++)
            {
                var vec = Linearized[i];
                coords[i] = new Coordinate(vec.X, vec.Y);
            }
            CoordinateArraySequence cs = new CoordinateArraySequence(coords);
            return new LineString(cs, new GeometryFactory());
        }
    }
}
