using CurvedTopologyExperiment.Geometry;

namespace CurvedTopology.Geometry.Shapes
{
    /// <summary>
    /// Todo, 3 points and close.
    /// Each point is an angle from the other
    /// </summary>
    public class Triangle : Shape
    {
        public Triangle() : base(GeometryHelper.GeometryFactory, Manifold.None) { }

        public bool IsEquilateral => throw new System.NotImplementedException();

        public bool IsIsosceles => throw new System.NotImplementedException();

        public bool IsRightAngle => throw new System.NotImplementedException();

        public bool IsScalene => throw new System.NotImplementedException();

        public bool IsObtuse => throw new System.NotImplementedException();
    }
}
