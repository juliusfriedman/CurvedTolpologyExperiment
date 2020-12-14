using NetTopologySuite.Geometries;
namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Contains methods which are commonly performed on spatial types
    /// </summary>
    public static class GeometryHelper
	{
		/// <summary>
		/// Refers to WGS 84, a standard used in GPS and other geographic systems.
		/// </summary>
		public const int SpatialReferenceId = 4326;

		#region Nested Types (CompassDirection)

		/// <summary>
		/// 
		/// </summary>
		public enum CompassDirection
		{
			//MinValue = -1,
			//Unknown = -1,
			N = 0,
			//NNE = 22,
			NE = 45,
			//ENE = 67
			E = 90,
			//ESE = 112
			SE = 135,
			//SSE = 157
			S = 180,
			//SSW = 202
			SW = 225,
			//WSW = 247
			W = 270,
			//WNW = 290
			NW = 315,
			//NNW = 337
			//MaxValue = NNW
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bearing">Degrees or Radians?</param>
		/// <returns></returns>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static CompassDirection GetCompassDirection(double bearing)
		{
			//if (bearing < 0 && bearing > -180)
			//{
			//	// Normalize to [0,360]
			//	bearing = 360.0 + bearing;
			//}
			//if (bearing > 360 || bearing < -180)
			//{
			//	return CompassDirection.Unknown;
			//}

			//return (CompassDirection)Math.Floor((bearing + 11.25) % 360 / 22.5);

			if (bearing >= 0 && bearing <= 22.5)
			{
				return CompassDirection.N;
			}
			else if (bearing >= 22.5 && bearing <= 67.5)
			{
				return CompassDirection.NE;
			}
			else if (bearing >= 67.5 && bearing <= 112.5)
			{
				return CompassDirection.E;
			}
			else if (bearing >= 112.5 && bearing <= 157.5)
			{
				return CompassDirection.SE;
			}
			else if (bearing >= 157.5 && bearing <= 205.5)
			{
				return CompassDirection.S;
			}
			else if (bearing >= 202.5 && bearing <= 247.5)
			{
				return CompassDirection.SW;
			}
			else if (bearing >= 247.5 && bearing <= 292.5)
			{
				return CompassDirection.W;
			}
			else if (bearing >= 292.5 && bearing <= 337.5)
			{
				return CompassDirection.NW;
			}
			else
			{
				return CompassDirection.N;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="manifold"></param>
		/// <returns></returns>
		public static CompassDirection GetCompassDirection(Point p1, Point p2, Manifold? manifold = null)
		{
			//Radius and flattening could be given here or GeodeticCalculator is static?
			var geodeticCalulator = new GeodeticCalculator();
			var geodeticCurve = geodeticCalulator.CalculateGeodeticCurve(manifold ?? Manifold.Ellipsoids.WGS84, Location.FromPoint(p1), Location.FromPoint(p2));
			Angle heading = geodeticCurve.Azimuth + Angle.Angle180;
			return GetCompassDirection(heading.TotalDegrees);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direcion"></param>
		/// <returns></returns>
		public static CompassDirection RotateRight(CompassDirection direction) => Rotate(direction, (float)CompassDirection.E);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direcion"></param>
		/// <returns></returns>
		public static CompassDirection RotateLeft(CompassDirection direction) => Rotate(direction, (float)CompassDirection.W);

		/// <summary>
		/// Use negative <paramref name="degrees"/> to go left
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static CompassDirection Rotate(CompassDirection direction, float degrees) => (CompassDirection)(((int)direction + degrees) % 360);

		#endregion

		/// <summary>
		/// Used to easily create points or polygons with the correct <see cref="SpatialReferenceId"/>
		/// </summary>
		public static readonly GeometryFactoryEx GeometryFactory = new GeometryFactoryEx(new PrecisionModel(PrecisionModels.FloatingSingle), SpatialReferenceId)
		{
			OrientationOfExteriorRing = LinearRingOrientation.CCW
		};

		/// <summary>
		/// 
		/// </summary>
		public static readonly LineString EmptyLineString = GeometryFactory.CreateLineString();

		/// <summary>
		/// 
		/// </summary>
		public static readonly Polygon EmptyPolygon = GeometryFactory.CreatePolygon();

		/// <summary>
		/// 
		/// </summary>
		public static readonly GeometryCollection EmptyGeometryCollection = GeometryFactory.CreateGeometryCollection();

		/// <summary>
		/// 
		/// </summary>
		public static readonly Point EmptyPoint = GeometryFactory.CreatePoint();

		/// <summary>
		/// </summary>
		public static readonly GeometryCollectionEx EmptyGeographyCollectionEx = GeometryCollectionEx.Empty;

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static Point GetMiddlePoint(LineString line) => GeometryFactory.CreatePoint(new Coordinate((line.StartPoint.X + line.EndPoint.X) / 2, (line.StartPoint.Y + line.EndPoint.Y) / 2));

		public const double RadiansPerDegree = System.Math.PI / 180;
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static double ToRadians(double degrees) => degrees * RadiansPerDegree;

		public const double DegreesPerRadian = 180 / System.Math.PI;
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static double ToDegrees(double radians) => radians * DegreesPerRadian;

		// convert radians to degrees (as bearing: 0...360)
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static double ToBearing(double radians) => (ToDegrees(radians) + 360) % 360;
	}
}

