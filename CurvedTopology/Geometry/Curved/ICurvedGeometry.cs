namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Represents geometry with curves
    /// </summary>
    public interface ICurvedGeometry : IGeometry
    {
        /// <summary>
        /// Parallel method to <see cref="NetTopologySuite.Geometries.Geometry.ToText"/> that will output the geometry as curved instead of as linear
        /// </summary>
        /// <returns></returns>
        public string ToCurvedText();

        /// <summary>
        /// The default linearization tolerance
        /// </summary>
        public double Tolerance { get; }        

        /// <summary>
        /// Linearizes the geometry using the built-in linearization tolerance
        /// </summary>
        /// <returns></returns>
        public NetTopologySuite.Geometries.Geometry Linearize();

        /// <summary>
        /// Linearizes the geometry using the specified tolerance
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public NetTopologySuite.Geometries.Geometry Linearize(double tolerance);
    }

    /// <summary>
    /// <see cref="ICurvedGeometry"/>
    /// </summary>
    public interface ICurvedGeometry<T> : ICurvedGeometry where T : NetTopologySuite.Geometries.Geometry
    {
        /// <summary>
        /// Linearizes the geometry using the built-in linearization tolerance
        /// </summary>
        /// <returns></returns>
        public new T Linearize();

        /// <summary>
        /// Linearizes the geometry using the specified tolerance
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public new T Linearize(double tolerance);
    }
}
