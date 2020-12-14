using System;
using System.Globalization;
using System.Runtime.Serialization;
//https://raw.githubusercontent.com/rbdavison/YachtSim/
namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Stores an angle and allows conversion to different formats.
    /// </summary>
    public class Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable, ISerializable, IComparable
    {
        public const double HalfPi = Math.PI / 2.0;

        internal const double TwoPi = Math.PI + Math.PI;

        internal const double ThreePi = TwoPi + Math.PI;

        internal const double HalfThreePi = ThreePi / 2.0;

        /// <summary>
        /// Returns the <see cref="Angle"/> between the angles a1 and a2 in radians. Angle is calculated in the counterclockwise direction.
        /// </summary>
        /// <param name="a1">first angle</param>
        /// <param name="a2">second angle</param>
        /// <returns><see cref="Angle"/> between a1 and a2 in the clockwise direction</returns>
        public static Angle Subtract(Angle a1, Angle a2)
        {
            if (a1 < a2)
            {
                return a2 - a1;
            }
            else
            {
                return new(TwoPi - Math.Abs((a2 - a1).Radians));
            }
        }

        /// <summary>Zero Angle</summary>
        public static readonly Angle Zero = new Angle(0);

        /// <summary>180 degree Angle</summary>
        public static readonly Angle Angle180 = Angle.FromRadians(Math.PI);

        /// <summary>NaN-valued Angle</summary>
        public static readonly Angle NaN = Angle.FromRadians(Double.NaN);

        private readonly double radians;

        /// <summary>
        /// Initializes a new instance of the Angle class.
        /// </summary>
        /// <param name="radians">The value for the angle in radians.</param>
        public Angle(double radians) => this.radians = radians;

        /// <summary>Gets the whole number of degrees from the angle.</summary>
        public int Degrees => (int) this.TotalDegrees; // Truncate anything after the decimal place

        /// <summary>Gets the whole number of minutes from the angle.</summary>
        public int Minutes=> (int)(this.TotalMinutes % 60.0);

        /// <summary>Gets the number of seconds from the angle.</summary>
        public double Seconds=> this.TotalSeconds % 60.0;

        /// <summary>Gets the value of the angle in radians.</summary>
        public double Radians => radians;

        /// <summary>Gets the value of the angle in degrees.</summary>
        public double TotalDegrees => radians * GeometryHelper.RadiansPerDegree;

        /// <summary>Gets the value of the angle in minutes.</summary>
        public double TotalMinutes => TotalDegrees * 60.0;

        /// <summary>Gets the value of the angle in seconds.</summary>
        public double TotalSeconds => TotalMinutes * 60.0;
    
        /// <summary>Creates a new angle from an amount in degrees.</summary>
        /// <param name="degrees">The value for the angle in degrees.</param>
        /// <returns>A new Angle representing the specified value.</returns>
        public static Angle FromDegrees(double degrees) => new Angle(degrees * GeometryHelper.RadiansPerDegree);
        /// <summary>
        /// Creates a new angle from an amount in degrees and minutes.
        /// </summary>
        /// <param name="degrees">The amount of degrees.</param>
        /// <param name="minutes">The amount of minutes.</param>
        /// <returns>A new Angle representing the specified value.</returns>
        public static Angle FromDegrees(double degrees, double minutes) => FromDegrees(degrees + (minutes / 60.0));

        /// <summary>
        /// Creates a new angle from an amount in degrees, minutes and seconds.
        /// </summary>
        /// <param name="degrees">The amount of degrees.</param>
        /// <param name="minutes">The amount of minutes.</param>
        /// <param name="seconds">The amount of seconds.</param>
        /// <returns>A new Angle representing the specified value.</returns>
        public static Angle FromDegrees(double degrees, double minutes, double seconds) => FromDegrees(degrees + (minutes / 60.0) + (seconds / 3600.0));

        /// <summary>Creates a new angle from an amount in radians.</summary>
        /// <param name="radians">The value for the angle in radians.</param>
        /// <returns>A new Angle representing the specified value.</returns>
        public static Angle FromRadians(double radians)=> new Angle(radians);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static Angle FromDegreesAndMinutes(int degrees, double minutes)
        {
            double d = minutes / 60;

            d = degrees < 0 ? degrees - d : degrees + d;

            return new Angle(d * GeometryHelper.RadiansPerDegree);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Angle FromDegreesMinutesAndSeconds(int degrees, int minutes, double seconds)
        {
            double d = (seconds / 3600) + (minutes / 60.0);

            d = degrees < 0 ? degrees - d : degrees + d;

            return new Angle(d * GeometryHelper.RadiansPerDegree);
        }

        /// <summary>
        /// Get the absolute value of the angle.
        /// </summary>
        public static Angle Abs(Angle angle) => new Angle(Math.Abs(angle.Radians));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool IsNaN(Angle angle) => double.IsNaN(angle.Radians);

        /// <summary>
        /// Returns the result of multiplying the specified value by negative one.
        /// </summary>
        /// <param name="angle">The angle to negate.</param>
        /// <returns>
        /// An angle that is the value as the specified value parameter multiplied
        /// by negative one.
        /// </returns>
        /// <exception cref="ArgumentNullException">angle is null.</exception>
        public static Angle Negate(Angle angle)
        {
            if (angle == null)
            {
                throw new ArgumentNullException(nameof(angle));
            }
            return new Angle(angle.radians * -1);
        }

        /// <summary>
        /// Determines whether two specified Angles have different values.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is different from the value of angleB;
        /// otherwise, false.
        /// </returns>
        public static bool operator !=(Angle angleA, Angle angleB)=> !(angleA == angleB);

        /// <summary>
        /// Determines whether a specified Angle value is less than another
        /// specified Angle value.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is less than the value of angleB;
        /// otherwise, false. If either of the specified value parameters are
        /// null then this method will return false.
        /// </returns>
        public static bool operator <(Angle angleA, Angle angleB)
        {
            if ((angleA == null) || angleA.IsDifferentDerivedClass(angleB))
            {
                return false;
            }
            return angleA.CompareTo(angleB) == -1;
        }

        /// <summary>
        /// Determines whether a specified Angle value is less than or equal
        /// to another specified Angle value.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is less than or equal to the value of
        /// angleB; otherwise, false. If either of the specified value parameters
        /// are null then this method will return false.
        /// </returns>
        public static bool operator <=(Angle angleA, Angle angleB)
        {
            if ((angleA == null) || angleA.IsDifferentDerivedClass(angleB))
            {
                return false;
            }
            return angleA.CompareTo(angleB) != 1;
        }

        /// <summary>
        /// Determines whether two specified Angles have the same value.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is the same as the value of angleB;
        /// otherwise, false.
        /// </returns>
        public static bool operator ==(Angle angleA, Angle angleB)
        {
            if (object.ReferenceEquals(angleA, null))
            {
                return object.ReferenceEquals(angleB, null);
            }
            return angleA.Equals(angleB);
        }

        /// <summary>
        /// Determines whether a specified Angle value is greater than another
        /// specified Angle value.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is greater than the value of angleB;
        /// otherwise, false. If either of the specified value parameters are
        /// null then this method will return false.
        /// </returns>
        public static bool operator >(Angle angleA, Angle angleB)
        {
            if ((angleA == null) || angleA.IsDifferentDerivedClass(angleB))
            {
                return false;
            }
            return angleA.CompareTo(angleB) == 1;
        }

        /// <summary>
        /// Determines whether a specified Angle value is greater than or equal
        /// to another specified Angle value.
        /// </summary>
        /// <param name="angleA">The first Angle to compare, or null.</param>
        /// <param name="angleB">The second Angle to compare, or null.</param>
        /// <returns>
        /// true if the value of angleA is greater than or equal to the value of
        /// angleB; otherwise, false. If either of the specified value parameters
        /// are null then this method will return false.
        /// </returns>
        public static bool operator >=(Angle angleA, Angle angleB)
        {
            if ((angleA == null) || angleA.IsDifferentDerivedClass(angleB))
            {
                return false;
            }
            return angleA.CompareTo(angleB) != -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Angle operator +(Angle lhs, Angle rhs) => new Angle(lhs.radians + rhs.radians);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Angle operator -(Angle lhs, Angle rhs) => new Angle(lhs.radians - rhs.radians);

        /// <summary>
        /// Compares this instance with a specified Angle object and indicates
        /// whether the value of this instance is less than, equal to, or greater
        /// than the value of the specified Angle object.
        /// </summary>
        /// <param name="other">An Angle to compare with this instance.</param>
        /// <returns>
        /// <para>A signed number indicating the relative values of this instance
        /// and value parameter.</para>
        /// <para>A return value less than zero indicates this instance is less
        /// than other or this instance represents an angle that is not a number
        /// (double.NaN) and the angle of other is a number.</para>
        /// <para>A return value of zero indicates this instance represents the
        /// same angle as other.</para>
        /// <para>A return value greater than zero indicates this instance is
        /// greater than other or this instance represents an angle that is a
        /// number and the angle of other is not a number (double.NaN) or other
        /// is null.</para>
        /// </returns>
        public int CompareTo(Angle other)
        {
            if (this.IsDifferentDerivedClass(other))
            {
                return 1;
            }
            return this.radians.CompareTo(other.radians);
        }

        /// <summary>
        /// Compare this angle to another angle.
        /// </summary>
        /// <param name="obj">other angle to compare to.</param>
        /// <returns>result according to IComparable contract/></returns>
        public int CompareTo(object obj)
        {
            if (!(obj is Angle))
            {
                throw new ArgumentException("Can only compare Angles with other Angles.", nameof(obj));
            }

            return this.CompareTo((Angle)obj);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must
        /// also be an Angle, have the same value.
        /// </summary>
        /// <param name="obj">The Angle to compare to this instance.</param>
        /// <returns>
        /// true if obj is an Angle and its value is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => this.Equals(obj as Angle);

        /// <summary>
        /// Determines whether this instance and another specified Angle object
        /// have the same value.
        /// </summary>
        /// <param name="other">The Angle to compare to this instance.</param>
        /// <returns>
        /// true if the value of the value parameter is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Angle other)
        {
            if (this.IsDifferentDerivedClass(other))
            {
                return false;
            }
            return this.radians == other.radians;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()=> this.radians.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current Angle in degrees,
        /// minutes and seconds form.
        /// </summary>
        /// <returns>A string that represents the current instance.</returns>
        public override string ToString()=> this.ToString(null, null);

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        /// The format to use (see remarks) or null to use the default format.
        /// </param>
        /// <param name="formatProvider">
        /// The provider to use to format the value or null to use the format
        /// information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        /// The value of the current instance in the specified format.
        /// </returns>
        /// <exception cref="ArgumentException">format is unknown.</exception>
        /// <remarks>
        /// Valid format strings are one of the following. Specifying a number
        /// after the format string indicates the desired number of decimal
        /// places to show. If the precision specifier is omitted, the current
        /// NumberFormatInfo.NumberDecimalDigits will be used.
        /// <list type="bullet">
        /// <item><term>D</term><description>
        /// Returns the angle in decimal degrees.
        /// </description></item>
        /// <item><term>DM</term><description>
        /// Returns the angle in degrees and decimal minutes.
        /// </description></item>
        /// <item><term>DMS</term><description>
        /// Returns the angle in degrees, minutes and decimal seconds. This
        /// is the default format if no format is specified.
        /// </description></item>
        /// </list>
        /// </remarks>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "DMS";
            }
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            var parsed = ParseFormatString(format);
            switch (parsed.Item1)
            {
                case "D":
                    return string.Format(
                        formatProvider,
                        "{0}\u00B0",
                        GetString(this.TotalDegrees, 0, parsed.Item2, formatProvider));

                case "DM":
                    int degrees = this.Degrees;
                    double minutes = Math.Abs(this.TotalMinutes - (degrees * 60));
                    return string.Format(
                        formatProvider,
                        "{0}\u00B0 {1:00}\u2032",
                        degrees,
                        GetString(minutes, 2, parsed.Item2, formatProvider));

                case "DMS":
                    return string.Format(
                        formatProvider,
                        "{0}\u00B0 {1:00}\u2032 {2}\u2033",
                        this.Degrees,
                        Math.Abs(this.Minutes),
                        GetString(Math.Abs(this.Seconds), 2, parsed.Item2, formatProvider));
            }
            throw new ArgumentException("Invalid string format.", "format");
        }

        /// <summary>
        /// Turns a double value into a string using the specified number of
        /// decimal digits.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="digits">
        /// The number of digits preceeding the decimal place.
        /// </param>
        /// <param name="decimals">The number of decimal digits to display.</param>
        /// <param name="provider">
        /// The provider to use to format the value or null to use the format
        /// information from the current locale setting of the operating system.
        /// </param>
        /// <returns>A string representation of the value.</returns>
        internal static string GetString(double value, int digits, int decimals, IFormatProvider provider)
        {
            if (digits < 1)
            {
                digits = 1;
            }
            string format = new string('0', digits) + '.';

            if (decimals == -1)
            {
                // Need to get the maximum number of digits to display from
                // the current NumberFormatInfo class.
                NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

                // This is the maximum to display, hence '#'
                format += new string('#', numberFormat.NumberDecimalDigits);
            }
            else
            {
                format += new string('0', decimals);
            }
            return value.ToString(format, provider);
        }

        /// <summary>
        /// Splits the format string into the format type and the precision to
        /// format decimal numbers from the format string.
        /// </summary>
        /// <param name="format">The format string to parse.</param>
        /// <returns>
        /// A Tuple containing the format part of the input and the number of
        /// decimal digits to display (-1 indicated the system default).
        /// </returns>
        internal static Tuple<string, int> ParseFormatString(string format)
        {
            int index = 0;
            while (index < format.Length)
            {
                if (char.IsDigit(format[index]))
                {
                    break;
                }
                ++index;
            }

            int precision;
            if (int.TryParse(format.Substring(index), NumberStyles.None, CultureInfo.InvariantCulture, out precision))
            {
                return Tuple.Create(format.Substring(0, index), precision);
            }
            return Tuple.Create(format, -1);
        }

        /// <summary>
        /// Validates a parameter and throws an ArgumentOutOfRangeException if
        /// the value is outside the specified values.
        /// </summary>
        /// <param name="parameter">The name of the parameter to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="min">
        /// The minimum value the value must be greater than or equal to.
        /// </param>
        /// <param name="max">
        /// The maximum value the value must be less than or equal to.
        /// </param>
        protected static void ValidateRange(string parameter, double value, double min, double max)
        {
            // Need to check for double.NaN, double.PositiveInfinity and
            // double.NegativeInfinity, which all have strange behavoir with
            // the normal comparison operators.
            if (!double.IsNaN(value) && !double.IsInfinity(value))
            {
                if ((min <= value) && (value <= max))
                {
                    return; // Don't throw
                }
            }

            string message = string.Format(
                CultureInfo.CurrentUICulture,
                "{0} Value must be between {1:0.##} and {2:0.##} inclusive.",
                parameter,
                min,
                max);
            throw new ArgumentOutOfRangeException(parameter, message);
        }

        // This prevents a Longitude being compared to a Latitude but allows
        // a Longitude/Latitude to be compared to an angle
        private bool IsDifferentDerivedClass(Angle angle)
        {
            if (object.ReferenceEquals(angle, null))
            {
                return true;
            }

            Type angleType = angle.GetType();
            Type type = this.GetType();

            if ((type == typeof(Angle)) || (angleType == typeof(Angle)))
            {
                return false;
            }

            // Both the types are derived, return true if they are different
            return angleType != type;
        }

        #region Serialization / Deserialization

        private Angle(SerializationInfo info, StreamingContext context)
        {
            radians = info.GetDouble("radians");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("radians", this.Radians);
        }

        #endregion
    }
}
