using NetTopologySuite.Geometries;
using System.Linq;

namespace CurvedTopologyExperiment.Geometry
{
	/// <summary>
	/// A derived implementation of <see cref="GeometryCollection"/> probably better name GeometryCollectionEx
	/// <see href="https://github.com/NetTopologySuite/NetTopologySuite/issues/451">Here</see>
	/// </summary>
	public class GeometryCollectionEx : GeometryCollection, 
		System.Collections.Generic.ICollection<NetTopologySuite.Geometries.Geometry>, 
		IPolygonal, 
		ILineal, 
		IPuntal, 
		IGeometry
	{
		/// <summary>
		/// An <see cref="GeometryCollectionEx"/> which has 0 <see cref="NetTopologySuite.Geometries.Geometry.NumGeometries"/>.
		/// </summary>
		public new static GeometryCollectionEx Empty = new GeometryCollectionEx(System.Array.Empty<NetTopologySuite.Geometries.Geometry>());

		int pointCount = 0;

		int lineStringCount = 0;

		int polygonCount = 0;

		int curveCount = 0;

		/// <summary>
		/// Indicates if <see cref="Add(NetTopologySuite.Geometries.Geometry)"/> or <see cref="Remove(NetTopologySuite.Geometries.Geometry, System.Collections.Generic.IEqualityComparer{NetTopologySuite.Geometries.Geometry})"/>
		/// </summary>
		public virtual bool IsReadOnly => Geometries.IsReadOnly;

		/// <summary>
		/// Indicates if at least 1 <see cref="LineString"/> is contained
		/// </summary>
		public bool IsLineal => lineStringCount > 0;

		/// <summary>
		/// The amount of contained <see cref="LineString"/>
		/// </summary>
		public int Lineality => lineStringCount;

		/// <summary>
		/// Indicates if at least 1 <see cref="Point"/> is contained
		/// </summary>
		public bool IsPuntal => pointCount > 0;

		/// <summary>
		/// The amount of contained <see cref="Point"/>
		/// </summary>
		public int Puntality => pointCount;

		/// <summary>
		/// Indicates if at least 1 <see cref="Polygon"/> is contained
		/// </summary>
		public bool IsPolygonal => polygonCount > 0;

		/// <summary>
		/// The amount of contained <see cref="Polygon"/>
		/// </summary>
		public int Polygonality => polygonCount;

		/// <summary>
		/// The amount of contained <see cref="ICurvedGeometry"/>
		/// </summary>
		public int Curvature => curveCount;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="geometries"></param>
		public GeometryCollectionEx(params NetTopologySuite.Geometries.Geometry[] geometries) : base(geometries) { }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="geometries"></param>
		public GeometryCollectionEx(GeometryFactory factory, params NetTopologySuite.Geometries.Geometry[] geometries) : base(geometries, factory) { }

		/// <summary>
		/// <see cref="Add(NetTopologySuite.Geometries.Geometry)"/> each in <paramref name="geometries"/>
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="geometries"></param>
		public GeometryCollectionEx(GeometryFactory factory, System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Geometry> geometries) : base(System.Array.Empty< NetTopologySuite.Geometries.Geometry>(), factory)
		{
			foreach (var g in geometries) Add(g);
		}

		/// <summary>
		/// <inheritdoc/>
		/// Indicates if <see cref="NumGeometries"/> is non zero.
		/// </summary>
		public override bool IsEmpty => NumGeometries > 0;

        /// <summary>
        /// <see cref="GeometryCollection.IsEmpty"/>
        /// </summary>
        public bool ContainsEmptyGeometry => base.IsEmpty;

		/// <summary>
		/// Most certainly
		/// </summary>
		bool IGeometry.IsGeometryCollection => true;// IsGeometryCollection;

		/// <summary>
		/// Sum of underlying <see cref="IGeometry.NumGeometries"/>
		/// </summary>
		public virtual int CoordinatesDimension => (int)Dimension.False;

		/// <summary>
		/// Adds to <see cref="Geometries"/>
		/// </summary>
		/// <param name="geometry"></param>
		protected void Add(NetTopologySuite.Geometries.Geometry geometry)
		{
			if (IsReadOnly) return;

			if (geometry is IPolygonal) polygonCount += geometry.NumGeometries;
			else if (geometry is ILineal) lineStringCount+= geometry.NumGeometries;
			else if (geometry is IPuntal) pointCount += geometry.NumGeometries;
			if (geometry is ICurvedGeometry) curveCount += geometry.NumGeometries;
			if (IsEmpty)
			{
				Geometries = new[] { geometry };
			}
			else
			{
				Geometries = Geometries.Append(geometry).ToArray();
				GeometryChanged();
			}
		}
		
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="geometry"></param>
		void System.Collections.Generic.ICollection<NetTopologySuite.Geometries.Geometry>.Add(NetTopologySuite.Geometries.Geometry geometry) => Add(geometry);

		/// <summary>
		/// Removes if contained
		/// </summary>
		/// <param name="geometry"></param>
		protected virtual bool Remove(NetTopologySuite.Geometries.Geometry geometry, System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry> comparer = null)
		{
			if (IsReadOnly || IsEmpty) return false;
			comparer = comparer ?? new Geometry.ExactGeometryComparer();
			for (int i = 0; i < Geometries.Length; ++i)
			{
				if (comparer.Equals(geometry, Geometries[i]))
				{
					if (geometry is IPolygonal) polygonCount -= geometry.NumGeometries;
					else if (geometry is ILineal) lineStringCount -= geometry.NumGeometries;
					else if (geometry is IPuntal) pointCount -= geometry.NumGeometries;
					if (geometry is ICurvedGeometry) curveCount -= geometry.NumGeometries;
					Geometries = Geometries.Take(i).Skip(1).Take(int.MaxValue).ToArray();
					GeometryChanged();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// <inheritdoc/>
		/// <see cref="Geometry.ExactGeometryComparer"/>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool System.Collections.Generic.ICollection<NetTopologySuite.Geometries.Geometry>.Remove(NetTopologySuite.Geometries.Geometry item) => Remove(item);

		/// <summary>
		/// Clears <see cref="Geometries"/>
		/// </summary>
		public virtual void Clear()
		{
			Geometries = System.Array.Empty<NetTopologySuite.Geometries.Geometry>();
			GeometryChanged();
		}

		bool System.Collections.Generic.ICollection<NetTopologySuite.Geometries.Geometry>.Contains(NetTopologySuite.Geometries.Geometry item) => IndexOf(item) >= 0;

		public virtual void CopyTo(NetTopologySuite.Geometries.Geometry[] array, int arrayIndex) => Geometries.CopyTo(array, arrayIndex);

		System.Collections.Generic.IEnumerator<NetTopologySuite.Geometries.Geometry> System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Geometry>.GetEnumerator()
		{
			foreach (var geometry in Geometries)
			{
				yield return geometry;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public virtual NetTopologySuite.Geometries.Geometry Flatten(double buffer = 0)
        {
            switch (NumGeometries)
            {
				case 1: return GetGeometryN(0);
				default:
					{
						return Buffer(buffer);
					}
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public virtual int IndexOf(NetTopologySuite.Geometries.Geometry geometry, System.Collections.Generic.IEqualityComparer<NetTopologySuite.Geometries.Geometry> comparer = null)
		{
			if (IsEmpty) return -1;
			comparer = comparer ?? new ExactGeometryComparer();
			for (int i = 0; i < Geometries.Length; ++i)
			{
				if (comparer.Equals(geometry, Geometries[i]))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator Point(GeometryCollectionEx v) => v.OfType<Point>().FirstOrDefault();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator Polygon(GeometryCollectionEx v) => v.OfType<Polygon>().FirstOrDefault();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator LineString(GeometryCollectionEx v) => v.OfType<LineString>().FirstOrDefault();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator MultiPoint(GeometryCollectionEx v) => v.OfType<MultiPoint>().FirstOrDefault();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator MultiPolygon(GeometryCollectionEx v) => v.OfType<MultiPolygon>().FirstOrDefault();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static explicit operator MultiLineString(GeometryCollectionEx v) => v.OfType<MultiLineString>().FirstOrDefault();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator GeometryCollectionEx(Point v) => new GeometryCollectionEx(v?.Factory ?? GeometryHelper.GeometryFactory, v ?? GeometryHelper.EmptyPoint);
	}
}
	
