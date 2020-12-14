using NetTopologySuite.Geometries;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Adds <see cref="ControlPoints"/> and <see cref="Arcs"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICurvedLineGeometry<T> : ICurvedGeometry<T> where T : LineString
    {
        /// <summary>
        /// Returns the linearized coordinates at the given tolerance
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public CoordinateSequence GetLinearizedCoordinateSequence(double tolerance);

        /// <summary>
        /// Returns the control points for this geometry.
        /// </summary>
        /// <returns></returns>
        public System.Numerics.Vector4[] ControlPoints { get; }

        /// <summary>
        /// The number of <see cref="Arc"/> contained
        /// </summary>
        public int NumArcs { get; }

        /// <summary>
        /// Returns the n-th circular arc making up the geometry
        /// </summary>
        /// <param name="arcIndex"></param>
        /// <returns></returns>
        public CoordinateSequences.Arc GetArcN(int n);

        /// <summary>
        /// May be equal to <see cref="EndPoint"/>
        /// </summary>
        public Point StartPoint { get; }

        /// <summary>
        /// A <see cref="Point"/>
        /// </summary>
        public Point EndPoint { get; }
    }
}
