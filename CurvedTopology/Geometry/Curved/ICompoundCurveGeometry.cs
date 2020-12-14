using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CurvedTopologyExperiment.Geometry.Curved
{
    /// <summary>
    /// <see cref="CompoundCurve"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompoundCurvedGeometry<T> : ICurvedGeometry<T> where T : LineString
    {
        /// <summary>
        /// Gets the contains <see cref="LineString"/> or <see cref="CircularString"/>
        /// </summary>
        public IEnumerable<LineString> Components { get; }
    }
}
