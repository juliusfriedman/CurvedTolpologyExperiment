using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

namespace CurvedTopologyExperiment.Geometry.CoordinateSequences
{
    /// <summary>
    /// Packed coordinate sequence implementation based on <see cref="Vector4"/>.
    /// </summary>
    [Serializable]
    public class PackedVectorFloatCoordinateSequence : PackedCoordinateSequence, ICoordinateSequence, ISerializable
    {
        const int DefaultMeasures = 0;
        const int DefaultDimension = 2;
        const int DefaultPackedDimension = 3;

        /// <summary>
        /// The packed coordinate array
        /// </summary>
        [NonSerialized]
        protected readonly System.Collections.Generic.List<Vector4> m_coords;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedFloatCoordinateSequence"/> class.
        /// </summary>
        /// <param name="coords">An array of <c>float</c> values that contains the ordinate values of the sequence.</param>
        /// <param name="dimension">The total number of ordinates that make up a <see cref="Coordinate"/> in this sequence.</param>
        /// <param name="measures">The number of measure-ordinates each <see cref="Coordinate"/> in this sequence has.</param>
        public PackedVectorFloatCoordinateSequence(float[] coords, int dimension, int measures)
            : base(coords?.Length / dimension ?? 0, dimension, measures)
        {
            if (coords == null)
                coords = Array.Empty<float>();

            if (coords.Length % dimension != 0)
                throw new ArgumentException("Packed array does not contain " +
                    "an integral number of coordinates");

            m_coords = new System.Collections.Generic.List<Vector4>(coords.Length / dimension);
            Initialize(this, coords);
        }

        static void Initialize(PackedVectorFloatCoordinateSequence seq, float[] coords)
        {
            for (int i = 0; i < coords.Length;)
            {
                switch (seq.Dimension)
                {
                    case 1:
                        {
                            seq.m_coords.Add(new Vector4(coords[i++], float.NaN, float.NaN, float.NaN));
                            break;
                        }
                    case 2:
                        {
                            seq.m_coords.Add(new Vector4(coords[i++], coords[i++], float.NaN, float.NaN));
                            break;
                        }
                    case 3:
                        {
                            seq.m_coords.Add(new Vector4(coords[i++], coords[i++], coords[i++], float.NaN));
                            break;
                        }
                    case 4:
                        {
                            seq.m_coords.Add(new Vector4(coords[i++], coords[i++], coords[i++], coords[i++]));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedFloatCoordinateSequence"/> class.
        /// </summary>
        /// <param name="coords">An array of <c>double</c> values that contains the ordinate values of the sequence.</param>
        /// <param name="dimension">The total number of ordinates that make up a <see cref="Coordinate"/> in this sequence.</param>
        /// <param name="measures">The number of measure-ordinates each <see cref="Coordinate"/> in this sequence has.</param>
        public PackedVectorFloatCoordinateSequence(double[] coords, int dimension, int measures)
            : base(coords?.Length / dimension ?? 0, dimension, measures)
        {
            if (coords == null)
                coords = Array.Empty<double>();

            if (coords.Length % dimension != 0)
                throw new ArgumentException("Packed array does not contain " +
                    "an integral number of coordinates");

            m_coords = new System.Collections.Generic.List<Vector4>(coords.Length / dimension);

            Initialize(this, coords);
        }

        static void Initialize(PackedVectorFloatCoordinateSequence seq, double[] coords)
        {
            for (int i = 0; i < coords.Length;)
            {
                switch (seq.Dimension)
                {
                    case 1:
                        {
                            seq.m_coords.Add(new Vector4((float)coords[i++], float.NaN, float.NaN, float.NaN));
                            break;
                        }
                    case 2:
                        {
                            seq.m_coords.Add(new Vector4((float)coords[i++], (float)coords[i++], float.NaN, float.NaN));
                            break;
                        }
                    case 3:
                        {
                            seq.m_coords.Add(new Vector4((float)coords[i++], (float)coords[i++], (float)coords[i++], float.NaN));
                            break;
                        }
                    case 4:
                        {
                            seq.m_coords.Add(new Vector4((float)coords[i++], (float)coords[i++], (float)coords[i++], (float)coords[i++]));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedDoubleCoordinateSequence"/> class.
        /// </summary>
        /// <param name="coords">An array of <see cref="Coordinate"/>s.</param>
        public PackedVectorFloatCoordinateSequence(Coordinate[] coords)
            : this(coords, DefaultDimension)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="dim"></param>
        /// <param name="measures"></param>
        public PackedVectorFloatCoordinateSequence(System.Collections.Generic.IList<Vector4> coords, int dim, int measures)
            : base(coords.Count, dim, measures)
        {
            m_coords = new(coords.Count);
            m_coords.AddRange(coords);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        public PackedVectorFloatCoordinateSequence(System.Collections.Generic.IList<Vector4> coords) : this(coords, DefaultDimension, DefaultMeasures)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedFloatCoordinateSequence"/> class.
        /// </summary>
        /// <param name="coords">An array of <see cref="Coordinate"/>s.</param>
        /// <param name="dimension">The total number of ordinates that make up a <see cref="Coordinate"/> in this sequence.</param>
        public PackedVectorFloatCoordinateSequence(Coordinate[] coords, int dimension)
            : this(coords, dimension, DefaultMeasures)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedFloatCoordinateSequence"/> class.
        /// </summary>
        /// <param name="coords">An array of <see cref="Coordinate"/>s.</param>
        /// <param name="dimension">The total number of ordinates that make up a <see cref="Coordinate"/> in this sequence.</param>
        /// <param name="measures">The number of measure-ordinates each <see cref="Coordinate"/> in this sequence has.</param>
        public PackedVectorFloatCoordinateSequence(Coordinate[] coords, int dimension, int measures)
            : base(coords?.Length ?? 0, dimension, measures)
        {
            if (coords == null)
                coords = Array.Empty<Coordinate>();

            m_coords = new System.Collections.Generic.List<Vector4>(coords.Length);
            for (int i = 0; i < coords.Length; ++i)
            {
                var coord = coords[i];
                //int offset = i * dimension;
                switch (Dimension)
                {
                    //case 0:
                    //    {
                    //        _coords.Add(new Vector4((float)coord.X, float.NaN, float.NaN, float.NaN));
                    //        break;
                    //    }
                    case 0:
                    case 1:
                    case 2:
                        {
                            m_coords.Add(new Vector4((float)coord.X, (float)coord.Y, float.NaN, float.NaN));
                            break;
                        }
                    case 3:
                        {
                            m_coords.Add(new Vector4((float)coord.X, (float)coord.Y, (float)coord.Z, float.NaN));
                            break;
                        }
                    case 4:
                        {
                            m_coords.Add(new Vector4((float)coord.X, (float)coord.Y, (float)coord.Z, (float)coord.M));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedFloatCoordinateSequence"/> class.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="dimension"></param>
        /// <param name="measures"></param>
        public PackedVectorFloatCoordinateSequence(int size, int dimension, int measures)
            : base(size, dimension, measures)
        {
            int sz = size * Dimension;
            m_coords = new System.Collections.Generic.List<Vector4>(sz);
            for (int i = 0; i < sz; ++i)
            {
                m_coords.Add(default);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="dimension"></param>
        /// <param name="measures"></param>
        protected PackedVectorFloatCoordinateSequence(Array coords, int dimension, int measures)
                   : base(coords?.Length / dimension ?? 0, dimension, measures)
        {
            m_coords = new System.Collections.Generic.List<Vector4>(coords.Length);
            if (coords is float[] floats)
            {
                Initialize(this, floats);
            }
            else if (coords is double[] doubles)
            {
                Initialize(this, doubles);
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PackedVectorFloatCoordinateSequence(SerializationInfo info, StreamingContext context) : this((Array)info.GetValue("coords", typeof(Array)), DefaultPackedDimension, DefaultMeasures)
        {
        }

        /// <summary>
        /// Returns a Coordinate representation of the specified coordinate, by always
        /// building a new Coordinate object.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Coordinate GetCoordinateInternal(int index)
        {
            var vec = m_coords[index];
            if (Dimension == 2 && Measures == 0)
            {
                return new Coordinate(vec.X, vec.Y);
            }
            else if (Dimension == 3 && Measures == 0)
            {
                return new CoordinateZ(vec.X, vec.Y, vec.Z); //W?
            }
            else if (Dimension == 3 && Measures == 1)
            {
                return new CoordinateM(vec.X, vec.Y, vec.Z);
            }
            else if (Dimension == 4 && Measures == 1)
            {
                return new CoordinateZM(vec.X, vec.Y, vec.Z, vec.W);
            }

            // note: JTS's "Coordinate" is our "CoordinateZ".
            return new CoordinateZ(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Gets the underlying array containing the coordinate values.
        /// </summary>
        /// <returns>The array of coordinate values</returns>
        public float[] GetRawCoordinates()
        {
            return m_coords.SelectMany(c =>
            {
                switch (Dimension)
                {
                    default:
                    case 0:
                        return Array.Empty<float>();
                    case 1:
                        return new float[] { c.X };
                    case 2:
                        return new float[] { c.X, c.Y };
                    case 3:
                        return new float[] { c.X, c.Y, c.Z };
                    case 4:
                        return new float[] { c.X, c.Y, c.Z, c.W };
                }
            }).ToArray();
        }

        /// <inheritdoc cref="CoordinateSequence.Copy"/>
        public override CoordinateSequence Copy()=> new PackedVectorFloatCoordinateSequence(m_coords, Dimension, Measures);

        /// <summary>
        /// Returns the ordinate of a coordinate in this sequence.
        /// Ordinate indices 0 and 1 are assumed to be X and Y.
        /// Ordinate indices greater than 1 have user-defined semantics
        /// (for instance, they may contain other dimensions or measure values).
        /// </summary>
        /// <remarks>
        /// Beware, for performance reasons the ordinate index is not checked, if
        /// it's over dimensions you may not get an exception but a meaningless
        /// value.
        /// </remarks>
        /// <param name="index">The coordinate index in the sequence.</param>
        /// <param name="ordinateIndex">The ordinate index in the coordinate (in range [0, dimension-1]).</param>
        /// <returns></returns>
        public override double GetOrdinate(int index, int ordinateIndex)
        {
            var vec = m_coords[index];

            switch (ordinateIndex)
            {
                case 0: return vec.X;
                case 1: return vec.Y;
                case 2: return vec.Z;
                case 3: return vec.W;
            }

            return double.NaN;
        }

        /// <summary>
        /// Sets the ordinate of a coordinate in this sequence.
        /// </summary>
        /// <param name="index">The coordinate index.</param>
        /// <param name="ordinateIndex">The ordinate index in the coordinate, 0 based,
        /// smaller than the number of dimensions.</param>
        /// <param name="value">The new ordinate value.</param>
        /// <remarks>
        /// Warning: for performance reasons the ordinate index is not checked:
        /// if it is over dimensions you may not get an exception but a meaningless value.
        /// </remarks>
        public override void SetOrdinate(int index, int ordinateIndex, double value)
        {
            CoordRef = null;
            //int idx = index / Dimension;
            var vec = m_coords[index];
            switch (ordinateIndex)
            {
                case 0: m_coords[index] = new Vector4((float)value, vec.Y, vec.Z, vec.W); break;
                case 1: m_coords[index] = new Vector4(vec.X, (float)value, vec.Z, vec.W); break;
                case 2: m_coords[index] = new Vector4(vec.X, vec.Y, (float)value, vec.W); break;
                case 3: m_coords[index] = new Vector4(vec.X, vec.Y, vec.Z, (float)value); break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetOridinate(int index, Vector4 value) => m_coords[index] = value;

        /// <summary>
        /// Expands the given Envelope to include the coordinates in the sequence.
        /// Allows implementing classes to optimize access to coordinate values.
        /// </summary>
        /// <param name="env">The envelope to expand.</param>
        /// <returns>A reference to the expanded envelope.</returns>
        public override Envelope ExpandEnvelope(Envelope env)
        {
            for (int i = 0; i < m_coords.Count; i += Dimension)
            {
                var vec = m_coords[i];
                env.ExpandToInclude(vec.X, vec.Y);
            }
            return env;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CoordinateSequence Reversed()
        {
            var reversed = new System.Collections.Generic.List<Vector4>(m_coords);
            reversed.Reverse();
            return new PackedVectorFloatCoordinateSequence(reversed, Dimension, Measures);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new System.ArgumentNullException("info");
            info.AddValue("coords", GetRawCoordinates());
        }

        ICoordinateSequence ICoordinateSequence.Copy() => new PackedVectorFloatCoordinateSequence(m_coords, Dimension, Measures);
    }
}
