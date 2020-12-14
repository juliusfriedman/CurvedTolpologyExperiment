namespace CurvedTopologyExperiment.Geometry
{
	/// <summary>
	/// 
	/// </summary>
	public class TopologoicalGeometryComparer : System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(NetTopologySuite.Geometries.Geometry x, NetTopologySuite.Geometries.Geometry y) => x.EqualsTopologically(y);

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(NetTopologySuite.Geometries.Geometry obj) => obj.GetHashCode();
	}

	/// <summary>
	/// 
	/// </summary>
	public class NormalizedGeometryComparer : System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(NetTopologySuite.Geometries.Geometry x, NetTopologySuite.Geometries.Geometry y) => x.EqualsNormalized(y);

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(NetTopologySuite.Geometries.Geometry obj) => obj.GetHashCode();
	}
	/// <summary>
	/// 
	/// </summary>
	public class ExactGeometryComparer : System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(NetTopologySuite.Geometries.Geometry x, NetTopologySuite.Geometries.Geometry y) => x.EqualsExact(y);

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(NetTopologySuite.Geometries.Geometry obj) => obj.GetHashCode();
	}

	/// <summary>
	/// In sequence given, does not check for duplicated steps at this level.
	/// </summary>
	public class ChainedGeometryComparer : System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>
    {
		readonly System.Collections.Generic.IEnumerable<System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>> m_Comparers;

		public virtual System.Collections.Generic.IEnumerable<System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>> Comparers => m_Comparers;

		public ChainedGeometryComparer(System.Collections.Generic.IEnumerable<System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>> comparers) => m_Comparers = comparers ?? new System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>[] { new ExactGeometryComparer(), new TopologoicalGeometryComparer() };

		public ChainedGeometryComparer(params System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>[] comparers) => m_Comparers = comparers ?? new System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry>[] { new ExactGeometryComparer(), new TopologoicalGeometryComparer() };

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(NetTopologySuite.Geometries.Geometry x, NetTopologySuite.Geometries.Geometry y) => System.Linq.Enumerable.Any(m_Comparers, c=> x.EqualsExact(y));

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(NetTopologySuite.Geometries.Geometry obj) => obj.GetHashCode();

		/// <summary>
		/// Combines
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ChainedGeometryComparer operator +(ChainedGeometryComparer a, ChainedGeometryComparer b) => new ChainedGeometryComparer(System.Linq.Enumerable.Concat(a.Comparers, b.Comparers));
	}
}
