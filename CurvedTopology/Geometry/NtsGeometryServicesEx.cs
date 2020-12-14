using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;

namespace CurvedTopologyExperiment.Geometry
{
	/// <summary>
	/// Useful to override NtsGeometryServices.Instance
	/// </summary>
	public class NtsGeometryServicesEx : NtsGeometryServices
	{
		public NtsGeometryServicesEx(PrecisionModel pm = null)
			: base(new PackedCoordinateSequenceFactory(PackedCoordinateSequenceFactory.PackedType.Float), pm ?? new PrecisionModel(PrecisionModels.FloatingSingle), GeometryHelper.SpatialReferenceId)
		{ }

		protected override GeometryFactory CreateGeometryFactoryCore(PrecisionModel precisionModel, int srid,
			CoordinateSequenceFactory coordinateSequenceFactory)
		{
			return new GeometryFactoryEx(precisionModel, srid, coordinateSequenceFactory)
			{ OrientationOfExteriorRing = LinearRingOrientation.CCW };
		}
	}
}
