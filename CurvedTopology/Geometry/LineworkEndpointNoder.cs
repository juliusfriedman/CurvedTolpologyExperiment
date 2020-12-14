using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.Index.Quadtree;
using NetTopologySuite.LinearReferencing;
using System;
using System.Collections.Generic;
namespace CurvedTopologyExperiment.Geometry
{
	/// <summary>
	/// Useful for noding <see cref="LineString"/>'s so they can be routed on the <see cref="PathFinder"/>
	/// </summary>
	public class LineworkEndpointNoder
	{
		private int _processedIndex;

		private readonly IList<LineString> _inputLines;
		private readonly Quadtree<LineString> _tree;

		public LineworkEndpointNoder()
		{
			_inputLines = new List<LineString>();
			_tree = new Quadtree<LineString>();
		}

		public double EnvelopeBuffer { get; set; } = 0.000001;

		public void Add(LineString lineString)
		{
			lineString.UserData = _inputLines.Count;
			_tree.Insert(lineString.EnvelopeInternal, lineString);
			_inputLines.Add(lineString);
		}

		public void AddRange(IEnumerable<LineString> lineStrings)
		{
			foreach (var lineString in lineStrings)
				Add(lineString);
		}

		public bool RemoveAt(int index)
		{
			var lineString = _inputLines[index];
			if (lineString == null)
				return false;

			_tree.Remove(lineString.EnvelopeInternal, lineString);

			_inputLines[index] = null;
			lineString.UserData = null;

			return true;
		}

		public IEnumerable<LineString> GetResult()
		{
			while (_processedIndex < _inputLines.Count)
			{
				var line = _inputLines[_processedIndex];
				if (line != null)
				{
					Envelope e;
					Coordinate pt;

					// StartPoint
					pt = line.StartPoint.Coordinate;
					e = new Envelope(pt);
					e.ExpandBy(EnvelopeBuffer);
					var candidates = _tree.Query(e);
					if (candidates.Count > 0)
					{
						var p = PreparedGeometryFactory.Prepare(line.Factory.ToGeometry(e));
						foreach (var candidate in candidates)
						{
							if (candidate == line)
								continue;
							if (candidate.UserData == null)
								continue;
							if (!p.Intersects(candidate))
								continue;

							NodeAt(candidate, pt);
						}
					}

					// EndPoint
					pt = line.EndPoint.Coordinate;
					e = new Envelope(pt);
					e.ExpandBy(EnvelopeBuffer);
					candidates = _tree.Query(e);
					if (candidates.Count > 0)
					{
						var p = PreparedGeometryFactory.Prepare(line.Factory.ToGeometry(e));
						foreach (var candidate in candidates)
						{
							if (candidate == line)
								continue;
							if (candidate.UserData == null)
								continue;
							if (!p.Intersects(candidate))
								continue;

							NodeAt(candidate, pt);
						}
					}
				}

				// Increment
				_processedIndex++;
			}

			foreach (var lineString in _inputLines)
			{
				if (lineString != null)
					yield return lineString;
			}
		}

		private bool NodeAt(LineString candidate, Coordinate coord)
		{
			var lip = LocationIndexOfPoint.IndexOf(candidate, coord);
			if (IsStartOrEnd(lip, candidate))
				return false;

			if (!lip.IsVertex)
				return false;

			int candidateIndex = (int)candidate.UserData;
			RemoveAt(candidateIndex);

			var csFactory = candidate.Factory.CoordinateSequenceFactory;
			CoordinateSequence cs;

			cs = csFactory.Create(lip.SegmentIndex + 1, candidate.CoordinateSequence.Dimension);
			NetTopologySuite.Geometries.CoordinateSequences.Copy(candidate.CoordinateSequence, 0, cs, 0, cs.Count);
#if DEBUG
			System.Diagnostics.Debug.Assert(cs.GetCoordinate(cs.Count - 1).Equals(coord));
#endif
			var l1 = candidate.Factory.CreateLineString(cs);
			Add(l1);

			cs = csFactory.Create(candidate.CoordinateSequence.Count - lip.SegmentIndex, candidate.CoordinateSequence.Dimension);
			NetTopologySuite.Geometries.CoordinateSequences.Copy(candidate.CoordinateSequence, lip.SegmentIndex, cs, 0, cs.Count);
#if DEBUG
			System.Diagnostics.Debug.Assert(cs.GetCoordinate(0).Equals(coord));
#endif

			var l2 = candidate.Factory.CreateLineString(cs);
			Add(l2);

#if DEBUG
			System.Diagnostics.Debug.Assert(l1.CoordinateSequence.Count + l2.CoordinateSequence.Count == candidate.CoordinateSequence.Count + 1);
			System.Diagnostics.Debug.Assert(l1.EndPoint.EqualsExact(l2.StartPoint));
			System.Diagnostics.Debug.Assert(l1.StartPoint.EqualsExact(candidate.StartPoint));
			System.Diagnostics.Debug.Assert(l2.EndPoint.EqualsExact(candidate.EndPoint));
			System.Diagnostics.Debug.Assert(Math.Abs(l1.Length + l2.Length - candidate.Length) < 1e-9);
#endif

			return true;
		}

		private bool IsStartOrEnd(LinearLocation lip, LineString candidate)
		{
			if (!lip.IsVertex)
				return false;
			if (lip.SegmentIndex == 0)
				return true;
			return lip.IsEndpoint(candidate);
		}
	}
}
