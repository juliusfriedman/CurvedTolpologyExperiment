using NetTopologySuite.Geometries;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// A <see cref="GeometryCollectionEx"/> which has additional components of Width, Height, Radii and Reserved
    /// </summary>
    public abstract class Shape : 
        GeometryCollectionEx, 
        IGeometry
    {
        //public static readonly Shape Empty = new(GeometryHelper.GeometryFactory, Manifold.None);

        public NetTopologySuite.Utilities.GeometricShapeFactory GeometricShapeFactory { get; }

        /// <summary>
        /// </summary>
        protected System.Numerics.Vector4 Components { get; set; }

        /// <summary>
        /// </summary>
        public float Width
        {
            get => Components.W;
            set => Components = new(Components.X, Components.Y, Components.Z, value);
        }

        /// <summary>
        /// </summary>
        public float Height
        {
            get => Components.Z;
            set => Components = new(Components.X, Components.Y, value, Components.W);
        }

        /// <summary>
        /// A straight line from the center to the circumference of a circle or sphere.
        /// </summary>
        public float Radii
        {
            get => Components.X;
            set => Components = new(value, Components.Y, Components.Z, Components.W);
        }

        /// <summary>
        /// </summary>
        public float Reserved
        {
            get => Components.Y;
            set => Components = new(Components.X, value, Components.Z, Components.W);
        }

        /// <summary>
        /// </summary>
        public Manifold Manifold { get; } = Manifold.None;

        /// <summary>
        /// Even if <see cref="NumGeometries"/> is non 0 a <see cref="Shape"/> is abstractly NOT a collection of.
        /// </summary>
        bool IGeometry.IsGeometryCollection => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="factory"></param>
        public Shape(GeometryFactoryEx factory, Manifold manifold)
            : base(factory) => (GeometricShapeFactory, Manifold) = (new(factory), manifold);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="manifold"></param>
        /// <param name="components"></param>
        public Shape(GeometryFactoryEx factory, Manifold manifold, System.Numerics.Vector4 components)
            : base(factory) => (GeometricShapeFactory, Manifold, Components) = (new(factory), manifold, components);

        /// <summary>
        /// From coordinate X,Y <see cref="Width"/>, <see cref="Height"/>
        /// </summary>
        public Envelope BoundingBox
        {
            get
            {
                //Could also use each contained geometry and EnvelopeInternal
                //Envelope is working also probably
                var c = Coordinate;
                return new Envelope(new(c.X, c.Y), new(c.X + Width, c.Y + Height));
            }
        }
    }
}
