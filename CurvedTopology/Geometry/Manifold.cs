namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// The definition of
    /// </summary>
    public struct Manifold : System.IEquatable<Manifold>
    {
        /// <summary>
        /// Represents no manifold
        /// </summary>
        public static readonly Manifold None = new Manifold();

        /// <summary>
        /// Construct a new Ellipsoid.  This is private to ensure the values are
        /// consistent (flattening = 1.0 / inverseFlattening).  Use the methods 
        /// FromAAndInverseF() and FromAAndF() to create new instances.
        /// </summary>
        /// <param name="semiMajorAxisMeters"></param>
        /// <param name="semiMinorAxisMeters"></param>
        /// <param name="flattening"></param>
        /// <param name="inverseFlattening"></param>
        private Manifold(double semiMajorAxisMeters, double semiMinorAxisMeters, double flattening, double inverseFlattening) => Components = new((float)semiMinorAxisMeters, (float)flattening, (float)inverseFlattening, (float)semiMajorAxisMeters);

        #region Reference Ellipsoids

        public struct Ellipsoids
        {
            /// <summary>The WGS84 ellipsoid.</summary>
            public static readonly Manifold WGS84 = FromAxisAndInverseFlattening(6378137.0, 298.257223563);

            /// <summary>The GRS80 ellipsoid.</summary>
            public static readonly Manifold GRS80 = FromAxisAndInverseFlattening(6378137.0, 298.257222101);

            /// <summary>The GRS67 ellipsoid.</summary>
            public static readonly Manifold GRS67 = FromAxisAndInverseFlattening(6378160.0, 298.247167427);

            /// <summary>The ANS ellipsoid.</summary>
            public static readonly Manifold ANS = FromAxisAndInverseFlattening(6378160.0, 298.25);

            /// <summary>The WGS72 ellipsoid.</summary>
            public static readonly Manifold WGS72 = FromAxisAndInverseFlattening(6378135.0, 298.26);

            /// <summary>The Clarke1858 ellipsoid.</summary>
            public static readonly Manifold Clarke1858 = FromAxisAndInverseFlattening(6378293.645, 294.26);

            /// <summary>The Clarke1880 ellipsoid.</summary>
            public static readonly Manifold Clarke1880 = FromAxisAndInverseFlattening(6378249.145, 293.465);

            /// <summary>A spherical "ellipsoid".</summary>
            public static readonly Manifold Sphere = FromAxisAndFlattening(6371000, 0.0);
        }

        #endregion

        /// <summary>
        /// Build an Ellipsoid from the semi major axis measurement and the inverse flattening.
        /// </summary>
        /// <param name="semiMajorAxisMeters">semi major axis (meters)</param>
        /// <param name="inverseFlattening"></param>
        /// <returns></returns>
        public static Manifold FromAxisAndInverseFlattening(double semiMajorAxisMeters, double inverseFlattening)
        {
            double f = 1.0 / inverseFlattening;

            double b = (1.0 - f) * semiMajorAxisMeters;

            return new Manifold(semiMajorAxisMeters, b, f, inverseFlattening);
        }

        /// <summary>
        /// Build an Ellipsoid from the semi major axis measurement and the flattening.
        /// </summary>
        /// <param name="semiMajorAxisMeters">semi major axis (meters)</param>
        /// <param name="flattening"></param>
        /// <returns></returns>
        public static Manifold FromAxisAndFlattening(double semiMajorAxisMeters, double flattening)
        {
            double inverseF = 1.0 / flattening;

            double b = (1.0 - flattening) * semiMajorAxisMeters;

            return new Manifold(semiMajorAxisMeters, b, flattening, inverseF);
        }

        /// <summary>
        /// W,X,Y,Z
        /// </summary>
        public System.Numerics.Vector4 Components { get; }

        /// <summary>Get semi minor axis (meters).</summary>
        public double SemiMinorAxisMeters => Components.X;

        /// <summary>Get flattening.</summary>
        public double Flattening => Components.Y;

        /// <summary>Get inverse flattening.</summary>
        public double InverseFlattening => Components.Z;

        /// <summary>Get semi major axis (meters).</summary>
        public double SemiMajorAxisMeters => Components.W;

        /// <summary>
        /// 
        /// </summary>
        public double EccentricitySquared => 2 * Flattening - Flattening * Flattening;

        /// <summary>
        /// 
        /// </summary>
        public double Eccentricity => System.Math.Sqrt(EccentricitySquared);

        // others are derived from these values.
        public static int GetHashCode(Manifold value) => value.Components.GetHashCode();

        public static bool Equals(Manifold first, Manifold second) => first.Components.Equals(second.Components);

        public static string ToString(Manifold value) => $"Manifold[SemiMajorAxisMeters={value.SemiMajorAxisMeters}, Flattening={value.Flattening}, SemiMinorAxisMeters={value.SemiMinorAxisMeters}, InverseFlattening={value.InverseFlattening}]";

        public override bool Equals(object obj) => obj is Manifold m && Equals(this, m);
        public bool Equals(Manifold other) => Equals(this, other);
        public override int GetHashCode() => GetHashCode(this);
        public override string ToString() => ToString(this);

        #region Operators

        public static bool operator ==(Manifold lhs, Manifold rhs) => Equals(lhs, rhs);
        public static bool operator !=(Manifold lhs, Manifold rhs) => !Equals(lhs, rhs);

        #endregion
    }
}