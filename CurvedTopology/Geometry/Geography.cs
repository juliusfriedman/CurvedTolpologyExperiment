using NetTopologySuite.Geometries;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// v2
    /// </summary>
    //[Column(TypeName= Geography.ColumnTypeName)]
    public abstract class Geography : GeometryCollectionEx
    {
        public const string ColumnTypeName = "geography";

        public Geography(GeometryFactoryEx factoryEx) : base(factoryEx) { }

        //public override string GeometryType => throw new System.NotImplementedException();

        //public override OgcGeometryType OgcGeometryType => throw new System.NotImplementedException();

        //public override Coordinate Coordinate => throw new System.NotImplementedException();

        //public override Coordinate[] Coordinates => throw new System.NotImplementedException();

        //public override int NumPoints => throw new System.NotImplementedException();

        //public override bool IsEmpty => throw new System.NotImplementedException();

        //public override Dimension Dimension => throw new System.NotImplementedException();

        //public override Geometry Boundary => throw new System.NotImplementedException();

        //public override Dimension BoundaryDimension => throw new System.NotImplementedException();

        //protected override SortIndexValue SortIndex => throw new System.NotImplementedException();

        //public override void Apply(ICoordinateFilter filter)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void Apply(ICoordinateSequenceFilter filter)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void Apply(IGeometryFilter filter)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void Apply(IGeometryComponentFilter filter)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override bool EqualsExact(Geometry other, double tolerance)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override double[] GetOrdinates(Ordinate ordinate)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void Normalize()
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override int CompareToSameClass(object o)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override int CompareToSameClass(object o, IComparer<CoordinateSequence> comp)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override Envelope ComputeEnvelopeInternal()
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override Geometry CopyInternal()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
