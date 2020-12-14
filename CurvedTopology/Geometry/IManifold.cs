namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Provides access to a <see cref="Geometry.Manifold"/>
    /// </summary>
    public interface IManifold
    {
        /// <summary>
        /// The <see cref="Geometry.Manifold"/>
        /// </summary>
        public Manifold Manifold { get; }
    }
}
