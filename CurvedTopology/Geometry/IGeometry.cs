namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Represents <see cref="NetTopologySuite.Geometries.Geometry"/> properties which are usually non virtual
    /// </summary>
    public interface IGeometry //IGeometric
    {
        /// <summary>
        /// See <see cref="NetTopologySuite.Geometries.OgcGeometryType"/>
        /// </summary>
        public abstract NetTopologySuite.Geometries.OgcGeometryType OgcGeometryType { get; }

        /// <summary>
        /// The name of this Geometrys actual class.
        /// </summary>
        public abstract string GeometryType { get; }

        /// <summary>
        /// The number of contained usually 1 unless MULTI
        /// </summary>
        public abstract int NumGeometries { get; }

        /// <summary>
        /// Returns the dimension of the geometry without forcing access to the coordinate sequence
        /// </summary>
        public abstract int CoordinatesDimension { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.SRID"/>
        /// </summary>
        public abstract int SRID { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.Factory"/>
        /// </summary>
        /// <remarks>maybe GeometryFactoryEX</remarks>
        public abstract NetTopologySuite.Geometries.GeometryFactory Factory { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.PrecisionModel"/>
        /// </summary>
        public NetTopologySuite.Geometries.PrecisionModel PrecisionModel { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.PointOnSurface"/>
        /// </summary>
        public NetTopologySuite.Geometries.Point PointOnSurface { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.Envelope"/>
        /// </summary>
        public NetTopologySuite.Geometries.Geometry Envelope { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.EnvelopeInternal"/>
        /// </summary>
        public NetTopologySuite.Geometries.Envelope EnvelopeInternal { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.IsGeometryCollection"/>
        /// </summary>
        bool IsGeometryCollection { get; }

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.AsBinary"/>
        /// </summary>
        /// <returns></returns>
        public byte[] AsBinary();

        /// <summary>
        /// <see cref="NetTopologySuite.Geometries.Geometry.AsText"/>
        /// </summary>
        /// <returns></returns>
        public string AsText();
    }
}
