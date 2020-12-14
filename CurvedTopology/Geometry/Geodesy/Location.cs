using NetTopologySuite.Geometries;
using NetTopologySuite.GeometriesGraph;
using System;
using System.Globalization;
using System.Text;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>Represents a Latitude/Longitude/Altitude/Radii coordinate.</summary>
    public sealed partial class Location : Position, IEquatable<Location>, IFormattable
    {
        /// <summary>
        /// 0,0
        /// </summary>
        public static readonly Location Equator = new Location(new(Angle.Zero), new(Angle.Zero));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Location FromPoint(Point p) => new Location(new Latitude(new Angle(p.Y)), new Longitude(new Angle(p.X)), p.Z);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Location FromCoordinate(Coordinate c) => new Location(new Latitude(new Angle(c.Y)), new Longitude(new Angle(c.X)), c.Z);

        // Equatorial = 6378137, polar = 6356752.
        internal const int EarthRadius = 6366710; // The mean radius of the Earth in meters.

        private System.Numerics.Vector4 _data;

        /// <summary>
        /// Gets all the components of the location.
        /// <see cref="Altitude"/> is <see cref="System.Numerics.Vector4.Z"/>, <see cref="Radii"/> is <see cref="System.Numerics.Vector4.W"/>.
        /// </summary>
        public System.Numerics.Vector4 Data => Data;

        /// <summary>Initializes a new instance of the Location class.</summary>
        /// <param name="latitude">The latitude of the coordinate.</param>
        /// <param name="longitude">The longitude of the coordinate.</param>
        /// <param name="altitude">
        /// The altitude, specifed in meters, of the coordinate.
        /// </param>
        /// <param name="radii"></param>
        /// <exception cref="ArgumentNullException">
        /// latitude/longitude is null.
        /// </exception>
        public Location(Latitude latitude, Longitude longitude, double altitude = double.NaN, double radii = EarthRadius)
        {
            if (latitude == null)
            {
                throw new ArgumentNullException(nameof(latitude));
            }
            if (longitude == null)
            {
                throw new ArgumentNullException(nameof(longitude));
            }

            _data = new((float)longitude.Radians, (float)latitude.Radians, (float)altitude, (float)radii);
        }

        // XmlSerializer requires a parameterless constructor
        private Location()
        {
        }

        /// <summary>
        /// Gets the altitude of the coordinate, or null if the coordinate
        /// does not contain altitude information.
        /// </summary>
        public double Altitude => _data.Z;

        /// <summary>
        /// The Radii of the manifold in which this instance is plotted
        /// </summary>
        public double Radii => _data.W;

        /// <summary>Gets the latitude of the coordinate.</summary>
        public Latitude Latitude => new(new(_data.Y));

        /// <summary>Gets the longitude of the coordinate.</summary>
        public Longitude Longitude => new(new(_data.X));

        /// <summary>
        /// Would be better as operator?
        /// </summary>
        public Coordinate Coordinate => new Coordinate(Longitude.Degrees, Latitude.Degrees) { Z = Altitude, M = Radii };

        /// <summary>
        /// Would be better as operator?
        /// </summary>
        public Point Point => new Point(Longitude.Degrees, Latitude.Degrees) { Z = Altitude, M = Radii };

        /// <summary>
        /// <see cref="Latitude"/>, <see cref="Longitude"/>, <see cref="Altitude"/>, <see cref="Radii"/>
        /// </summary>
        public double[] Coordinates => new double[] {  Latitude.Degrees, Longitude.Degrees, Altitude, Radii };

        /// <summary>
        /// Determines whether two specified Locations have different values.
        /// </summary>
        /// <param name="locationA">The first Location to compare, or null.</param>
        /// <param name="locationB">The second Location to compare, or null.</param>
        /// <returns>
        /// true if the value of locationA is different from the value of
        /// locationB; otherwise, false.
        /// </returns>
        public static bool operator !=(Location locationA, Location locationB) => false == locationA.Equals(locationB);
        
        /// <summary>
        /// Determines whether two specified Locations have the same value.
        /// </summary>
        /// <param name="locationA">The first Location to compare, or null.</param>
        /// <param name="locationB">The second Location to compare, or null.</param>
        /// <returns>
        /// true if the value of locationA is the same as the value of locationB;
        /// otherwise, false.
        /// </returns>
        public static bool operator ==(Location locationA, Location locationB)
        {
            if (object.ReferenceEquals(locationA, null))
            {
                return object.ReferenceEquals(locationB, null);
            }
            return locationA.Equals(locationB);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must
        /// also be a Location, have the same value.
        /// </summary>
        /// <param name="obj">The Location to compare to this instance.</param>
        /// <returns>
        /// true if obj is a Location and its value is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as Location);

        /// <summary>
        /// Determines whether this instance and another specified Location object
        /// have the same value.
        /// </summary>
        /// <param name="other">The Location to compare to this instance.</param>
        /// <returns>
        /// true if the value of the value parameter is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Location other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return _data.Equals(other._data);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => _data.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current Location in degrees,
        /// minutes and seconds form.
        /// </summary>
        /// <returns>A string that represents the current instance.</returns>
        public override string ToString() => this.ToString(null, null);

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        /// The format to use or null to use the default format (see
        /// <see cref="Angle.ToString(string, IFormatProvider)"/>).
        /// </param>
        /// <param name="formatProvider">
        /// The provider to use to format the value or null to use the format
        /// information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        /// The value of the current instance in the specified format.
        /// </returns>
        /// <exception cref="ArgumentException">format is unknown.</exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "DMS";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            StringBuilder builder = new StringBuilder();
            if (format.Equals("ISO", StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(this.Latitude.ToString("ISO", null));
                builder.Append(this.Longitude.ToString("ISO", null));
                if (false == double.IsNaN(this.Altitude))
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0:+0.###;-0.###}", this.Altitude);
                }
                builder.Append('/');
            }
            else
            {
                var parsed = Angle.ParseFormatString(format);

                builder.Append(this.Latitude.ToString(format, formatProvider));
                builder.Append(' ');
                builder.Append(this.Longitude.ToString(format, formatProvider));
                
                if(false == double.IsNaN(this.Altitude))
                {
                    builder.Append(' ');
                    builder.Append(Angle.GetString(this.Altitude, 1, parsed.Item2, formatProvider));
                    builder.Append('m');
                }
            }
            return builder.ToString();
        }

        // These functions are based on the Aviation Formulary V1.45
        // by Ed Williams (http://williams.best.vwh.net/avform.htm)

        const double TwoPi = 2 * Math.PI;

        /// <summary>
        /// Calculates the initial course (or azimuth; the angle measured
        /// clockwise from true north) from this instance to the specified
        /// value.
        /// </summary>
        /// <param name="point">The location of the other point.</param>
        /// <returns>
        /// The initial course from this instance to the specified point.
        /// </returns>
        /// <example>
        /// The azimuth from 0,0 to 1,0 is 0 degrees. From 0,0 to 0,1 is 90
        /// degrees (due east).
        /// </example>
        public Angle Course(Location point)
        {
            double lat1 = this._data.Y;
            double lon1 = this._data.X;
            double lat2 = point._data.Y;
            double lon2 = point._data.X;

            double x = (Math.Cos(lat1) * Math.Sin(lat2)) -
                       (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1));
            double tan = Math.Atan2(Math.Sin(lon2 - lon1) * Math.Cos(lat2), x);

            return Angle.FromRadians(tan % TwoPi);
        }

        /// <summary>
        /// Calculates the great circle distance, in meters, between this instance
        /// and the specified value.
        /// </summary>
        /// <param name="point">The location of the other point.</param>
        /// <returns>The great circle distance, in meters.</returns>
        /// <remarks>The antimeridian was not considered.</remarks>
        /// <exception cref="ArgumentNullException">point is null.</exception>
        public double Distance(Location point, double radius = EarthRadius)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            //Radians
            double lat1 = this._data.Y;
            double lon1 = this._data.X;
            double lat2 = point._data.Y;
            double lon2 = point._data.X;

            double latitudeSqrd = Math.Pow(Math.Sin((lat1 - lat2) / 2), 2);
            double longitudeSqrd = Math.Pow(Math.Sin((lon1 - lon2) / 2), 2);
            double sqrt = Math.Sqrt(latitudeSqrd + (Math.Cos(lat1) * Math.Cos(lat2) * longitudeSqrd));
            double distance = 2 * Math.Asin(sqrt) / EarthRadius;

            if (double.IsFinite(Altitude))
            {
                double altitudeDelta = point._data.Z - this._data.Z;
                return Math.Sqrt(Math.Pow(distance, 2) + Math.Pow(altitudeDelta, 2));
            }

            return distance;
        }

        /// <summary>
        /// Calculates a point at the specified distance along the specified
        /// radial from this instance.
        /// </summary>
        /// <param name="distance">The distance, in meters.</param>
        /// <param name="radial">
        /// The course radial from this instance, measured clockwise from north.
        /// </param>
        /// <param name="radius">The default is <see cref="EarthRadius"/></param>
        /// <returns>A Location containing the calculated point.</returns>
        /// <exception cref="ArgumentNullException">radial is null.</exception>
        /// <remarks>The antemeridian is not considered.</remarks>
        public Location GetPoint(double distance, Angle radial, double radius = EarthRadius)
        {
            if (radial == null)
            {
                throw new ArgumentNullException(nameof(radial));
            }

            double lat = this._data.Y;
            double lon = this._data.X;
            distance /= radius;

            double latDist = Math.Cos(lat) * Math.Sin(distance);
            double radialLat = Math.Asin((Math.Sin(lat) * Math.Cos(distance)) +
                                         (latDist * Math.Cos(radial.Radians)));

            double y = Math.Sin(radial.Radians) * latDist;
            double x = Math.Cos(distance) - (Math.Sin(lat) * Math.Sin(radialLat));
            double atan = Math.Atan2(y, x);

            double radialLon = ((lon + atan + Math.PI) % TwoPi) - Math.PI;

            return new Location(
                Latitude.FromRadians(radialLat),
                Longitude.FromRadians(radialLon));
        }
    }
}
