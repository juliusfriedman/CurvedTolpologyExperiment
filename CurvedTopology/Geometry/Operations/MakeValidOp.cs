using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Noding;
using NetTopologySuite.Operation.Linemerge;
using NetTopologySuite.Operation.Polygonize;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Generic;

namespace CurvedTopologyExperiment.Geometry.Operations
{
    /*
 * This program extends Java Topology Suite (JTS) capability and is made
 * available to any Software already using Java Topology Suite.
 *
 * Copyright (C) Michaël Michaud (2017)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the Lesser GNU General Public License as
 * published by the Free Software Foundation, either version 2.1 of the
 * License, or any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * Lesser GNU General Public License for more details.
 *
 * You should have received a copy of the Lesser GNU General Public
 * License along with this program.
 * If not, see <http://www.gnu.org/licenses/>.
 */
    //    package org.h2gis.functions.spatial.clean;

    //    import org.locationtech.jts.algorithm.RayCrossingCounter;
    //    import org.locationtech.jts.algorithm.RobustLineIntersector;
    //    import org.locationtech.jts.geom.*;
    //    import org.locationtech.jts.geom.impl.PackedCoordinateSequenceFactory;
    //    import org.locationtech.jts.geom.util.PolygonExtracter;
    //    import org.locationtech.jts.noding.IntersectionAdder;
    //    import org.locationtech.jts.noding.MCIndexNoder;
    //    import org.locationtech.jts.noding.NodedSegmentString;
    //    import org.locationtech.jts.operation.linemerge.LineMerger;
    //    import org.locationtech.jts.operation.polygonize.Polygonizer;
    //    import org.locationtech.jts.operation.union.UnaryUnionOp;

    //    import java.util.*;

    //    import static org.locationtech.jts.geom.impl.PackedCoordinateSequenceFactory.*;
    //import org.locationtech.jts.geom.util.LineStringExtracter;
    //    import org.locationtech.jts.geom.util.PointExtracter;

    /**
     * Operator to make a geometry valid.
     * <br/>
     * Making a geometry valid will remove duplicate points although duplicate points
     * do not make a geometry invalid.
     *
     * @author Michaël Michaud
     */
    public class MakeValidOp
    {

        private static Coordinate[] EMPTY_COORD_ARRAY = new Coordinate[0];
        private static LinearRing[] EMPTY_RING_ARRAY = new LinearRing[0];

        // If preserveGeomDim is true, the geometry dimension returned by MakeValidOp
        // must be the same as the inputGeometryType (degenerate components of lower
        // dimension are removed).
        // If preserveGeomDim is false MakeValidOp will preserve as much coordinates
        // as possible and may return a geometry of lower dimension or a
        // GeometryCollection if input geometry or geometry components have not the
        // required number of points.
        private bool preserveGeomDim = true;

        // If preserveCoordDim is true, MakeValidOp preserves third and fourth ordinates.
        // If preserveCoordDim is false, third dimension is preserved but not fourth one.
        private bool preserveCoordDim = true;

        // If preserveDuplicateCoord is true, MakeValidOp will preserve duplicate
        // coordinates as much as possible. Generally, duplicate coordinates can be
        // preserved for linear geometries but not for areal geometries (overlay
        // operations used to repair polygons remove duplicate points).
        // If preserveDuplicateCoord is false, all duplicated coordinates are removed.
        private bool preserveDuplicateCoord = true;

        public MakeValidOp()
        {
        }

        public MakeValidOp setPreserveGeomDim(bool preserveGeomDim)
        {
            this.preserveGeomDim = preserveGeomDim;
            return this;
        }

        public MakeValidOp setPreserveCoordDim(bool preserveCoordDim)
        {
            this.preserveCoordDim = preserveCoordDim;
            return this;
        }

        public MakeValidOp setPreserveDuplicateCoord(bool preserveDuplicateCoord)
        {
            this.preserveDuplicateCoord = preserveDuplicateCoord;
            return this;
        }

        /**
         * Decompose a geometry recursively into simple components.
         *
         * @param geometry input geometry
         * @param list a list of simple components (Point, LineString or Polygon)
         */
        private static void decompose(NetTopologySuite.Geometries.Geometry geometry, ICollection<NetTopologySuite.Geometries.Geometry> list)
        {
            for (int i = 0; i < geometry.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry component = geometry.GetGeometryN(i);
                if (component is GeometryCollection) {
                    decompose(component, list);
                } else
                {
                    list.Add(component);
                }
            }
        }

        /**
         * Repair an invalid geometry.
         * <br/>
         * If preserveGeomDim is true, makeValid will remove degenerated geometries
         * from the result, i.e geometries which dimension is lower than the input
         * geometry dimension (except for mixed GeometryCollection).
         * <br/>
         * A multi-geometry will always produce a multi-geometry (eventually empty
         * or made of a single component). A simple geometry may produce a
         * multi-geometry (ex. polygon with self-intersection will generally produce
         * a multi-polygon). In this case, it is up to the client to explode
         * multi-geometries if he needs to.
         * <br/>
         * If preserveGeomDim is off, it is up to the client to filter degenerate
         * geometries.
         * <br/>
         * WARNING : for geometries of dimension 1 (linear), duplicate coordinates
         * are preserved as much as possible. For geometries of dimension 2 (areal),
         * duplicate coordinates are generally removed due to the use of overlay
         * operations.
         *
         * @param geometry input geometry
         * @return a valid Geometry
         */
        public NetTopologySuite.Geometries.Geometry makeValid(NetTopologySuite.Geometries.Geometry geometry)
        {

            // Input geometry is recursively exploded into a list of simple components
            List<NetTopologySuite.Geometries.Geometry> list = new(geometry.NumGeometries);
            decompose(geometry, list);

            // Each single component is made valid
            ICollection<NetTopologySuite.Geometries.Geometry> list2 = new List<NetTopologySuite.Geometries.Geometry>();
            foreach (var component in list)
            {
                if (component is Point) {
                    Point p = makePointValid((Point)component);
                    if (!p.IsEmpty)
                    {
                        list2.Add(p);
                    }
                } else if (component is LineString) {
                    var geom = makeLineStringValid((LineString)component);
                    for (int i = 0; i < geom.NumGeometries; i++)
                    {
                        if (!geom.GetGeometryN(i).IsEmpty)
                        {
                            list2.Add(geom.GetGeometryN(i));
                        }
                    }
                } else if (component is Polygon) {
                    var geom = makePolygonValid((Polygon)component);
                    for (int i = 0; i < geom.NumGeometries; i++)
                    {
                        if (!geom.GetGeometryN(i).IsEmpty)
                        {
                            list2.Add(geom.GetGeometryN(i));
                        }
                    }
                } else
                {
                    //assert false : "Should never reach here";
                }
            }

            list.Clear();
            foreach (NetTopologySuite.Geometries.Geometry g in list2) {
                // If preserveGeomDim is true and original input geometry is not a GeometryCollection
                // components with a lower dimension than input geometry are removed
                if (preserveGeomDim && !geometry.GeometryType.Equals(NetTopologySuite.Geometries.Geometry.TypeNameGeometryCollection)) {
                    removeLowerDimension(g, list, geometry.Dimension);
                } else
                {
                    decompose(g, list);
                }
            }
            list2 = list;

            // In a MultiPolygon, polygons cannot touch or overlap each other
            // (adjacent polygons are not merged in the context of a mixed GeometryCollection)
            if (list2.Count > 1)
            {
                bool multiPolygon = true;
                foreach (var geom in list2)
                {
                    if (geom.Dimension < Dimension.Surface)
                    {
                        multiPolygon = false;
                    }
                }
                if (multiPolygon)
                {
                    list2 = unionAdjacentPolygons(list2);
                }
            }
            if (0 == list2.Count)
            {
                GeometryFactory factory = geometry.Factory;
                if (geometry is Point) {
                    return factory.CreatePoint((Coordinate)null);
                } else if (geometry is LinearRing) {
                    return factory.CreateLinearRing(EMPTY_COORD_ARRAY);
                } else if (geometry is LineString) {
                    return factory.CreateLineString(EMPTY_COORD_ARRAY);
                } else if (geometry is Polygon) {
                    return factory.CreatePolygon(factory.CreateLinearRing(EMPTY_COORD_ARRAY), EMPTY_RING_ARRAY);
                } else if (geometry is MultiPoint) {
                    return factory.CreateMultiPoint(new Point[0]);
                } else if (geometry is MultiLineString) {
                    return factory.CreateMultiLineString(new LineString[0]);
                } else if (geometry is MultiPolygon) {
                    return factory.CreateMultiPolygon(new Polygon[0]);
                } else
                {
                    return factory.CreateGeometryCollection(new NetTopologySuite.Geometries.Geometry[0]);
                }
            }
            else
            {
                CoordinateSequenceFactory csFactory = geometry.Factory.CoordinateSequenceFactory;
                // Preserve 4th coordinate dimension as much as possible if preserveCoordDim is true
                if (preserveCoordDim && csFactory is PackedCoordinateSequenceFactory
                                /*&& ((PackedCoordinateSequenceFactory)csFactory).Dimension == 4*/)
                {
                    System.Collections.Generic.Dictionary<Coordinate, Double> map = new();
                    gatherDim4(geometry, map);
                    list2 = restoreDim4(list2, map);
                }

                var result = geometry.Factory.BuildGeometry(list2);
                // If input geometry was a GeometryCollection and result is a simple geometry
                // Create a multi-geometry made of a single component
                if (geometry is GeometryCollection && !(result is GeometryCollection)) {
                    if (geometry is MultiPoint && result is Point) {
                        result = geometry.Factory.CreateMultiPoint(new Point[] { (Point)result });
                    } else if (geometry is MultiLineString && result is LineString) {
                        result = geometry.Factory.CreateMultiLineString(new LineString[] { (LineString)result });
                    } else if (geometry is MultiPolygon && result is Polygon) {
                        result = geometry.Factory.CreateMultiPolygon(new Polygon[] { (Polygon)result });
                    }
                }
                return result;
            }
        }

        // Reursively remove geometries with a dimension less than dimension parameter
        private void removeLowerDimension(NetTopologySuite.Geometries.Geometry geometry, List<NetTopologySuite.Geometries.Geometry> result, Dimension dimension)
        {
            for (int i = 0; i < geometry.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry g = geometry.GetGeometryN(i);
                if (g is GeometryCollection) {
                    removeLowerDimension(g, result, dimension);
                } else if (g.Dimension >= dimension)
                {
                    result.Add(g);
                }
            }
        }

        // Union adjacent polygons to make an invalid MultiPolygon valid
        private System.Collections.Generic.ICollection<NetTopologySuite.Geometries.Geometry> unionAdjacentPolygons(ICollection<NetTopologySuite.Geometries.Geometry> list)
        {
            UnaryUnionOp op = new UnaryUnionOp(list);
            var result = op.Union();
            if (result.NumGeometries < list.Count)
            {
                list.Clear();
                for (int i = 0; i < result.NumGeometries; i++)
                {
                    list.Add(result.GetGeometryN(i));
                }
            }
            return list;
        }

        // If X or Y is null, return an empty Point
        private Point makePointValid(Point point)
        {
            CoordinateSequence sequence = point.CoordinateSequence;
            GeometryFactory factory = point.Factory;
            CoordinateSequenceFactory csFactory = factory.CoordinateSequenceFactory;
            if (sequence.Count == 0)
            {
                return point;
            }
            else if (Double.IsNaN(sequence.GetOrdinate(0, 0)) || Double.IsNaN(sequence.GetOrdinate(0, 1)))
            {
                return factory.CreatePoint(csFactory.Create(0, sequence.Dimension));
            }
            else if (sequence.Count == 1)
            {
                return point;
            }
            else
            {
                throw new Exception();
                //throw new RuntimeException("JTS cannot Create a point from a CoordinateSequence containing several points");
            }
        }

        /**
         * Returns a coordinateSequence free of Coordinates with X or Y NaN value,
         * and if desired, free of duplicated coordinates. makeSequenceValid keeps
         * the original dimension of input sequence.
         *
         * @param sequence input sequence of coordinates
         * @param preserveDuplicateCoord if duplicate coordinates must be preserved
         * @param close if the sequence must be closed
         * @return a new CoordinateSequence with valid XY values
         */
        private static CoordinateSequence makeSequenceValid(CoordinateSequence sequence,
                bool preserveDuplicateCoord, bool close)
        {
            int dim = sequence.Dimension;
            // we Add 1 to the sequence size for the case where we have to close the linear ring
            double[] array = new double[(sequence.Count + 1) * sequence.Dimension];
            bool modified = false;
            int count = 0;
            // Iterate through coordinates, skip points with x=NaN, y=NaN or duplicate
            for (int i = 0; i < sequence.Count; i++)
            {
                if (Double.IsNaN(sequence.GetOrdinate(i, 0)) || Double.IsNaN(sequence.GetOrdinate(i, 1)))
                {
                    modified = true;
                    continue;
                }
                if (!preserveDuplicateCoord && count > 0 && sequence.GetCoordinate(i).Equals(sequence.GetCoordinate(i - 1)))
                {
                    modified = true;
                    continue;
                }
                for (int j = 0; j < dim; j++)
                {
                    array[count * dim + j] = sequence.GetOrdinate(i, j);
                    if (j == dim - 1)
                    {
                        count++;
                    }
                }
            }
            // Close the sequence if it is not closed and there is already 3 distinct coordinates
            if (close && count > 2 && (array[0] != array[(count - 1) * dim] || array[1] != array[(count - 1) * dim + 1]))
            {
                System.Array.Copy(array, 0, array, count * dim, dim);
                modified = true;
                count++;
            }
            // Close z, m dimension if needed
            if (close && count > 3 && dim > 2)
            {
                for (int d = 2; d < dim; d++)
                {
                    if (array[(count - 1) * dim + d] != array[d])
                    {
                        modified = true;
                    }
                    array[(count - 1) * dim + d] = array[d];
                }
            }
            if (modified)
            {
                double[] shrinkedArray = new double[count * dim];
                System.Array.Copy(array, 0, shrinkedArray, 0, count * dim);
                return PackedCoordinateSequenceFactory.DoubleFactory.Create(shrinkedArray, dim);
            }
            else
            {
                return sequence;
            }
        }

        /**
         * Returns
         * <ul>
         * <li>an empty LineString if input CoordinateSequence has no valid
         * point</li>
         * <li>a Point if input CoordinateSequence has a single valid Point</li>
         * </ul>
         * makeLineStringValid keeps the original dimension of input sequence.
         *
         * @param lineString the LineString to make valid
         * @return a valid LineString or a Point if lineString length equals 0
         */
        private NetTopologySuite.Geometries.Geometry makeLineStringValid(LineString lineString)
        {
            CoordinateSequence sequence = lineString.CoordinateSequence;
            CoordinateSequence sequenceWithoutDuplicates = makeSequenceValid(sequence, false, false);
            GeometryFactory factory = lineString.Factory;
            if (sequenceWithoutDuplicates.Count == 0)
            {
                // no valid point -> empty LineString
                return factory.CreateLineString(factory.CoordinateSequenceFactory.Create(0, sequence.Dimension));
            }
            else if (sequenceWithoutDuplicates.Count == 1)
            {
                // a single valid point -> returns a Point
                if (preserveGeomDim)
                {
                    return factory.CreateLineString(factory.CoordinateSequenceFactory.Create(0, sequence.Dimension));
                }
                else
                {
                    return factory.CreatePoint(sequenceWithoutDuplicates);
                }
            }
            else if (preserveDuplicateCoord)
            {
                return factory.CreateLineString(makeSequenceValid(sequence, true, false));
            }
            else
            {
                return factory.CreateLineString(sequenceWithoutDuplicates);
            }
        }

        /**
         * Making a Polygon valid may Creates
         * <ul>
         * <li>an Empty Polygon if input has no valid coordinate</li>
         * <li>a Point if input has only one valid coordinate</li>
         * <li>a LineString if input has only a valid segment</li>
         * <li>a Polygon in most cases</li>
         * <li>a MultiPolygon if input has a self-intersection</li>
         * <li>a GeometryCollection if input has degenerate parts (ex. degenerate
         * holes)</li>
         * </ul>
         *
         * @param polygon the Polygon to make valid
         * @return a valid Geometry which may be of any type if the source geometry
         * is not valid.
         */
        private NetTopologySuite.Geometries.Geometry makePolygonValid(Polygon polygon)
        {
            //This first step analyze linear components and Create degenerate geometries
            //of dimension 0 or 1 if they do not form valid LinearRings
            //If degenerate geometries are found, it may produce a GeometryCollection with
            //heterogeneous dimension
            NetTopologySuite.Geometries.Geometry geom = makePolygonComponentsValid(polygon);
            List<NetTopologySuite.Geometries.Geometry> list = new();
            for (int i = 0; i < geom.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry component = geom.GetGeometryN(i);
                if (component is Polygon) {
                    NetTopologySuite.Geometries.Geometry nodedPolygon = nodePolygon((Polygon)component);
                    for (int j = 0; j < nodedPolygon.NumGeometries; j++)
                    {
                        list.Add(nodedPolygon.GetGeometryN(j));
                    }
                } else
                {
                    list.Add(component);
                }
            }
            return polygon.Factory.BuildGeometry(list);
        }

        /**
         * The method makes sure that outer and inner rings form valid LinearRings.
         * <p>
         * If outerRing is not a valid LinearRing, every linear component is
         * considered as a degenerated geometry of lower dimension (0 or 1)
         * </p>
         * <p>
         * If outerRing is a valid LinearRing but some innerRings are not, invalid
         * innerRings are transformed into LineString (or Point) and the returned
         * geometry may be a GeometryCollection of heterogeneous dimension.
         * </p>
         *
         * @param polygon simple Polygon to make valid
         * @return a Geometry which may not be a Polygon if the source Polygon is
         * invalid
         */
        private NetTopologySuite.Geometries.Geometry makePolygonComponentsValid(Polygon polygon)
        {
            GeometryFactory factory = polygon.Factory;
            CoordinateSequence outerRingSeq = makeSequenceValid(polygon.ExteriorRing.CoordinateSequence, false, true);
            // The validated sequence of the outerRing does not form a valid LinearRing
            // -> build valid 0-dim or 1-dim geometry from all the rings
            if (outerRingSeq.Count == 0 || outerRingSeq.Count < 4)
            {
                List<NetTopologySuite.Geometries.Geometry> list = new();
                if (outerRingSeq.Count > 0)
                {
                    list.Add(makeLineStringValid(polygon.ExteriorRing));
                }
                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    NetTopologySuite.Geometries.Geometry g = makeLineStringValid(polygon.GetInteriorRingN(i));
                    if (!g.IsEmpty)
                    {
                        list.Add(g);
                    }
                }
                if (0 == list.Count)
                {
                    return factory.CreatePolygon(outerRingSeq);
                }
                else
                {
                    return factory.BuildGeometry(list);
                }
            } // OuterRing forms a valid ring.
              // Inner rings may be degenerated
            else
            {
                List<LinearRing> innerRings = new();
                List<NetTopologySuite.Geometries.Geometry> degeneratedRings = new();
                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    CoordinateSequence seq = makeSequenceValid(polygon.GetInteriorRingN(i).CoordinateSequence, false, true);
                    if (seq.Count > 3)
                    {
                        innerRings.Add(factory.CreateLinearRing(seq));
                    }
                    else if (seq.Count > 1)
                    {
                        degeneratedRings.Add(factory.CreateLineString(seq));
                    }
                    else if (seq.Count == 1)
                    {
                        degeneratedRings.Add(factory.CreatePoint(seq));
                    }
                    // seq.size == 0
                }
                Polygon poly = factory.CreatePolygon(factory.CreateLinearRing(outerRingSeq),
                        innerRings.ToArray());
                if (0 == degeneratedRings.Count)
                {
                    return poly;
                }
                else
                {
                    degeneratedRings.Insert(0, poly);
                    return factory.BuildGeometry(degeneratedRings);
                }
            }
        }

        /**
         * Computes a valid Geometry from a Polygon which may not be valid
         * (auto-intersecting ring or overlapping holes).
         * <ul>
         * <li>Creates a Geometry from the <em>noded</em> exterior boundary</li>
         * <li>remove Geometries computed from noded interior boundaries</li>
         * </ul>
         */
        private NetTopologySuite.Geometries.Geometry nodePolygon(Polygon polygon)
        {
            LinearRing exteriorRing = (LinearRing)polygon.ExteriorRing;
            var geom = getArealGeometryFromLinearRing(exteriorRing);
            // geom can be a GeometryCollection
            // extract polygonal areas because symDifference cannot process GeometryCollections
            List<NetTopologySuite.Geometries.Geometry> polys = new ();
            List<NetTopologySuite.Geometries.Geometry> lines = new ();
            List<NetTopologySuite.Geometries.Geometry> points = new ();
            geom.Apply(new PolygonExtracter(polys));
            geom.Apply(new LineStringExtracter(lines));
            geom.Apply(new PointExtracter(points));
            geom = geom.Factory.BuildGeometry(polys);
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                LinearRing interiorRing = (LinearRing)polygon.GetInteriorRingN(i);
                // extract polygonal areas because symDifference cannot process GeometryCollections
                polys.Clear();
                getArealGeometryFromLinearRing(interiorRing).Apply(new PolygonExtracter(polys));
                // TODO avoid the use of difference operator
                geom = geom.SymmetricDifference(geom.Factory.BuildGeometry(polys));
            }
            List<NetTopologySuite.Geometries.Geometry> result = new ();
            result.Add(geom);
            result.AddRange(lines);
            result.AddRange(points);
            return geom.Factory.BuildGeometry(result);
        }

        /**
         * Node a LinearRing and return a MultiPolygon containing
         * <ul>
         * <li>a single Polygon if the LinearRing is simple</li>
         * <li>several Polygons if the LinearRing auto-intersects</li>
         * </ul>
         * This is used to repair auto-intersecting Polygons
         */
        private NetTopologySuite.Geometries.Geometry getArealGeometryFromLinearRing(LinearRing ring)
        {
            if (ring.IsSimple)
            {
                return ring.Factory.CreateMultiPolygon(new Polygon[]{
                ring.Factory.CreatePolygon(ring, EMPTY_RING_ARRAY)
            });
            }
            else
            {
                // Node input LinearRing and extract unique segments
                ISet<LineString> lines = nodeLineString(ring.Coordinates, ring.Factory);
                lines = getSegments(lines);

                // Polygonize the line network
                Polygonizer polygonizer = new Polygonizer();
                polygonizer.Add((ICollection<NetTopologySuite.Geometries.Geometry>)lines);

                // Computes intersections to determine the status of each polygon
                ICollection<NetTopologySuite.Geometries.Geometry> geoms = new List<NetTopologySuite.Geometries.Geometry>();
                foreach (NetTopologySuite.Geometries.Geometry g in polygonizer.GetPolygons()) {
                    Polygon polygon = (Polygon)g;
                    Coordinate p = polygon.InteriorPoint.Coordinate;
                    var location = RayCrossingCounter.LocatePointInRing(p, ring.CoordinateSequence);
                    if (location == NetTopologySuite.Geometries.Location.Interior)
                    {
                        geoms.Add(polygon);
                    }
                }
                NetTopologySuite.Geometries.Geometry unionPoly = UnaryUnionOp.Union(geoms);
                NetTopologySuite.Geometries.Geometry unionLines = UnaryUnionOp.Union(lines).Difference(unionPoly.Boundary);
                geoms.Clear();
                decompose(unionPoly, geoms);
                decompose(unionLines, geoms);
                return ring.Factory.BuildGeometry(geoms);
            }
        }

        /**
         * Return a set of segments from a linestring
         *
         * @param lines
         * @return
         */
        private ISet<LineString> getSegments(ICollection<LineString> lines)
        {
            ISet<LineString> set = new HashSet<LineString>();
            foreach (LineString line in lines) {
                Coordinate[] cc = line.Coordinates;
                for (int i = 1; i < cc.Length; i++)
                {
                    if (!cc[i - 1].Equals(cc[i]))
                    {
                        LineString segment = line.Factory.CreateLineString(
                                new Coordinate[] { new Coordinate(cc[i - 1]), new Coordinate(cc[i]) });
                        set.Add(segment);
                    }
                }
            }
            return set;
        }

        // Use ring to restore M values on geoms
        private ICollection<NetTopologySuite.Geometries.Geometry> restoreDim4(ICollection<NetTopologySuite.Geometries.Geometry> geoms, Dictionary<Coordinate, Double> map)
        {
            GeometryFactory factory = new GeometryFactory(
                    new PackedCoordinateSequenceFactory(PackedCoordinateSequenceFactory.PackedType.Double));
            List<NetTopologySuite.Geometries.Geometry> result = new List<NetTopologySuite.Geometries.Geometry>();
            foreach (NetTopologySuite.Geometries.Geometry geom in geoms) {
                if (geom is Point) {
                    result.Add(factory.CreatePoint(restoreDim4(
                            ((Point)geom).CoordinateSequence, map)));
                } else if (geom is LineString) {
                    result.Add(factory.CreateLineString(restoreDim4(
                            ((LineString)geom).CoordinateSequence, map)));
                } else if (geom is Polygon) {
                    LinearRing outer = factory.CreateLinearRing(restoreDim4(
                            ((Polygon)geom).ExteriorRing.CoordinateSequence, map));
                    LinearRing[] inner = new LinearRing[((Polygon)geom).NumInteriorRings];
                    for (int i = 0; i < ((Polygon)geom).NumInteriorRings; i++)
                    {
                        inner[i] = factory.CreateLinearRing(restoreDim4(
                                ((Polygon)geom).GetInteriorRingN(i).CoordinateSequence, map));
                    }
                    result.Add(factory.CreatePolygon(outer, inner));
                } else
                {
                    for (int i = 0; i < geom.NumGeometries; i++)
                    {
                        result.AddRange(restoreDim4(new List<NetTopologySuite.Geometries.Geometry>() { geom.GetGeometryN(i) }, map));
                    }
                }
            }
            return result;
        }

        private void gatherDim4(NetTopologySuite.Geometries.Geometry geometry, Dictionary<Coordinate, Double> map)
        {

            if (geometry is Point) {
                gatherDim4(((Point)geometry).CoordinateSequence, map);
            } else if (geometry is LineString) {
                gatherDim4(((LineString)geometry).CoordinateSequence, map);
            } else if (geometry is Polygon) {
                Polygon polygon = (Polygon)geometry;
                gatherDim4(polygon.ExteriorRing.CoordinateSequence, map);
                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    gatherDim4(polygon.GetInteriorRingN(i).CoordinateSequence, map);
                }
            } else
            {
                for (int i = 0; i < geometry.NumGeometries; i++)
                {
                    gatherDim4(geometry.GetGeometryN(i), map);
                }
            }
        }

        private void gatherDim4(CoordinateSequence cs, Dictionary<Coordinate, Double> map)
        {
            if (cs.Dimension == 4)
            {
                for (int i = 0; i < cs.Count; i++)
                {
                    map.TryAdd(cs.GetCoordinate(i), cs.GetOrdinate(i, 3));
                }
            }
        }

        // Use map to restore M values on the coordinate array
        private CoordinateSequence restoreDim4(CoordinateSequence cs, Dictionary<Coordinate, Double> map)
        {
            CoordinateSequence seq = new PackedCoordinateSequenceFactory(PackedCoordinateSequenceFactory.PackedType.Double).Create(cs.Count, 4);
            for (int i = 0; i < cs.Count; i++)
            {
                seq.SetOrdinate(i, 0, cs.GetOrdinate(i, 0));
                seq.SetOrdinate(i, 1, cs.GetOrdinate(i, 1));
                seq.SetOrdinate(i, 2, cs.GetOrdinate(i, 2));
                Double d = map[cs.GetCoordinate(i)];
                seq.SetOrdinate(i, 3, d == null ? Double.NaN : d);
            }
            return seq;
        }

        /**
         * Nodes a LineString and returns a List of Noded LineString's. Used to
         * repare auto-intersecting LineString and Polygons. This method cannot
         * process CoordinateSequence. The noding process is limited to 3d
         * geometries.<br/>
         * Preserves duplicate coordinates.
         *
         * @param coords coordinate array to be noded
         * @param gf geometryFactory to use
         * @return a list of noded LineStrings
         */
        private ISet<LineString> nodeLineString(Coordinate[] coords, GeometryFactory gf)
        {
            MCIndexNoder noder = new MCIndexNoder();
            noder.SegmentIntersector = new IntersectionAdder(new RobustLineIntersector());
            List<ISegmentString> list = new ();
            list.Add(new NodedSegmentString(coords, null));
            noder.ComputeNodes(list);
            List<LineString> lineStringList = new ();
            foreach (NetTopologySuite.Geometries.Geometry segmentString in noder.GetNodedSubstrings()) {
                lineStringList.Add(gf.CreateLineString(
                        segmentString.Coordinates
                ));
            }

            // WARNING : merger loose original linestrings
            // It is useful for LinearRings but should not be used for (Multi)LineStrings
            LineMerger merger = new LineMerger();
            merger.Add(lineStringList);
            lineStringList = (List<LineString>)merger.GetMergedLineStrings();

            // Remove duplicate linestrings preserving main orientation
            ISet<LineString> lineStringSet = new HashSet<LineString>();
            foreach (LineString line in lineStringList)
            {
                // TODO as equals makes a topological comparison, comparison with line.reverse maybe useless
                if (!lineStringSet.Contains(line) && !lineStringSet.Contains((LineString)line.Reverse()))
                {
                    lineStringSet.Add(line);
                }
            }
            return lineStringSet;
        }

    }
}
