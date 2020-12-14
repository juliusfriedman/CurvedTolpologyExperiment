using NetTopologySuite.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// Simple support class that allows accumulating in a <see cref="List{System.Numerics.Vector4}"/>, transparently growing it as the data gets added.
    /// </summary>
    public class GrowableOrdinateArray : IList<System.Numerics.Vector4>
    {
        /// <summary>
        /// The list of <see cref="System.Numerics.Vector4"/> which represent
        /// </summary>
        private List<System.Numerics.Vector4> ordinates;

        /// <summary>
        /// Used for sparseness if <see cref="ordinates"/> is larger than what is actually used.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Used to determine when to increment <see cref="currentIndex"/>
        /// </summary>
        private int currentComponent;

        /// <summary>
        ///  Creates an array of the given initial size
        /// </summary>
        /// <param name="size"></param>
        public GrowableOrdinateArray(int size = 0) => ordinates = new (size);

        /// <summary>
        /// Builds an initialized array, which will be primed when <see cref="EnsureLength(int)"/> is called
        /// </summary>
        GrowableOrdinateArray() { }

        /// <summary>
        /// Appends a single number to the array
        /// This is not efficient
        /// </summary>
        /// <param name="d"></param>
        public void Add(double d)
        {
            EnsureLength(currentIndex + 1);

            //there are 4 components per index
            if (currentComponent++ >= 3)
            {
                currentComponent = 0;
                ++currentIndex;
            }

            var source = ordinates[currentIndex];

            switch (currentComponent++)
            {
                case 0:
                    {
                        ordinates[currentIndex] = new((float)d, source.Y, source.Z, source.W);
                        break;
                    }
                case 1:
                    {
                        ordinates[currentIndex] = new(source.X, (float)d, source.Z, source.W);
                        break;
                    }
                case 2:
                    {
                        ordinates[currentIndex] = new(source.X, source.Y, (float)d, source.W);
                        break;
                    }
                case 3:
                    {
                        ordinates[currentIndex] = new(source.X, source.Y, source.Z, (float)d);
                        break;
                    }
            }
        }

        /// <summary>
        /// Appends a two numbers to the array
        /// This is not efficient
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        public void Add(double d1, double d2)
        {
            Add(d1);
            Add(d2);
        }

        /// <summary>
        /// Adds <paramref name="vec"/>
        /// </summary>
        /// <param name="vec"></param>
        public void Add(System.Numerics.Vector4 vec)
        {
            EnsureLength(currentIndex + 1);
            ordinates.Add(vec);
            currentComponent = 0;
        }

        /// <summary>
        /// Adds <paramref name="vec"/> by storing <see cref="Models.Vector4Double.High"/> and <see cref="Models.Vector4Double.Low"/>
        /// </summary>
        /// <param name="vec"></param>
        public void Add(Models.Vector8Float vec)
        {
            EnsureLength(currentIndex += 2);
            ordinates.Add(vec.High);
            ordinates.Add(vec.Low);
            currentComponent = 0;
        }

        /// <summary>
        /// Appends the given <paramref name="d"/>
        /// </summary>
        /// <param name="d"><see cref="System.Numerics.Vector4"/></param>
        public void AddRange(params System.Numerics.Vector4[] d) => ordinates.AddRange(d);

        /// <summary>
        /// Appends a list of <see cref="System.Numerics.Vector4"/> to the array
        /// </summary>
        /// <param name="d"></param>
        public void AddRange(IEnumerable<System.Numerics.Vector4> d) => ordinates.AddRange(d);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        public void AddRange(IEnumerable<double> d)
        {
            foreach (var a in d) Add(a);
        }

        public void AddRange(IEnumerable<float> d)
        {
            foreach (var a in d) Add(a);
        }

        /// <summary>
        /// Appends a whole coordinate sequence to the array.
        /// </summary>
        /// <param name="cs"></param>
        public void Add(CoordinateSequence cs)
        {
            int coordinatesCount = cs.Count;
            EnsureLength(coordinatesCount);
            for (int i = 0; i < coordinatesCount; ++i)
            {
                switch (cs.Dimension)
                {
                    case 1:
                        {
                            ordinates[currentIndex] = new((float)cs.GetOrdinate(i, 0));

                            continue;
                        }
                    case 2:
                        {
                            ordinates[currentIndex] = new((float)cs.GetOrdinate(i, 0), (float)cs.GetOrdinate(i, 1), float.NaN, float.NaN);

                            continue;
                        }
                    case 3:
                        {
                            ordinates[currentIndex] = new((float)cs.GetOrdinate(i, 0), (float)cs.GetOrdinate(i, 1), (float)cs.GetOrdinate(i, 3), float.NaN);

                            continue;
                        }
                    case 4:
                        {
                            ordinates[currentIndex] = new((float)cs.GetOrdinate(i, 0), (float)cs.GetOrdinate(i, 1), (float)cs.GetOrdinate(i, 3), (float)cs.GetOrdinate(i, 4));

                            continue;
                        }
                    default:
                        {
                            throw new InvalidOperationException("Only 4 Dimensions are supported for now");

                            //this would be a block copy instead but the coordaintes need to be packed for efficiency... ugh
                            //could inherit from PackedVectorFloat but would need a way to easily change the Count.

                            //for (int j = 0; i < cs.Dimension; ++j)
                            //{ for(int i = 0; i < 4; ++i) 
                            //    ordinates[currentIndex + j + i] = cs.GetOrdinate(i, j);
                            //}

                            //continue;
                        }
                }
            }

            currentIndex += coordinatesCount;
        }

        /// <summary>
        /// Returns the accumulated numbers, in an array cut to the current size
        /// </summary>
        /// <returns></returns>
        public System.Numerics.Vector4[] ToArray()
        {
            if (ordinates == null)
            {
                return Array.Empty<System.Numerics.Vector4>();
            }
            if (ordinates.Count == currentIndex)
            {
                return ordinates.ToArray();
            }
            else
            {
                System.Numerics.Vector4[] result = new System.Numerics.Vector4[currentIndex];
                ordinates.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// Returns the current ordiantes array, raw, uncut
        /// </summary>
        public IEnumerable<System.Numerics.Vector4> Ordinates => ordinates;

        /// <summary>
        /// Turns the array of ordinates into a coordinate sequence
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        public CoordinateSequence ToCoordinateSequence(GeometryFactory gf)
        {
            CoordinateSequence cs = gf.CoordinateSequenceFactory.Create(ordinates.Count / 2, 2, 0);
            for (int i = 0; i < cs.Count; ++i)
            {
                var vec = ordinates[i];
                cs.SetOrdinate(i, 0, vec.X);
                cs.SetOrdinate(i, 1, vec.Y);
                cs.SetOrdinate(i, 2, vec.Z);
                cs.SetOrdinate(i, 3, vec.W);
            }
            return cs;
        }

        /// <summary>
        /// Get the amount of ordinates contained
        /// </summary>
        public int Count => currentIndex;

        /// <summary>
        /// Gets or sets the capacity
        /// </summary>
        public int Capacity
        {
            get => currentIndex;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"The size must zero or positive, it was {value} instead");
                }

                EnsureLength(value);
                currentIndex = value;
            }
        }


        /// <summary>
        /// Ensures the data array has the specified lenght, or grows it otherwise
        /// </summary>
        /// <param name="length"></param>
        void EnsureLength(int length)
        {
            if (ordinates == null)
            {
                ordinates = new List<System.Numerics.Vector4>(length);
            }
            else
            {
                ordinates.Capacity = length;
            }
        }

        /// <summary>
        /// Reverses the values between start and end assuming it's a packed array of x/y ordinates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ReverseOrdinates(int start, int end)
        {
            if (end == start) return;
            start = Math.Max(start, 0);
            end = Math.Min(end, Count - 1);
            //Inplace swap rather than throw
            if (start < end)
            {
                start ^= end;
                start ^= end;
                start ^= end;
            }
            //Reverse the array 2 elements at a time.
            for (int near = start, far = end; near < far; ++near, --far)
            {
                //Swap(InArray) using temp
                var temp = ordinates[near];
                ordinates[near] = ordinates[far];
                ordinates[far] = temp;
            }
        }

        public void ReverseOrdinates() => ordinates.Reverse();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"GrowableDataArray({currentIndex})[");
            for (int i = 0; i < currentIndex; ++i)
            {
                sb.Append(ordinates[i]);
                if (i < currentIndex - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Indicates if the first and last ordinate
        /// </summary>
        public bool IsClosed => Count > 0 && ordinates[0] == ordinates[ordinates.Count - 1];

        public bool IsReadOnly => ((ICollection<System.Numerics.Vector4>)ordinates).IsReadOnly;

        public System.Numerics.Vector4 this[int index] { get => ((IList<System.Numerics.Vector4>)ordinates)[index]; set => ((IList<System.Numerics.Vector4>)ordinates)[index] = value; }

        /// <summary>
        /// Closes the sequence by adding the last point as first
        /// </summary>
        public void Close()
        {
            if (Count <= 0) return;
            if (IsClosed) return;
            Add(ordinates[0]);
        }

        /// <summary>
        /// Copies a sub-array from another growable array
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="length"></param>
        public void CopyTo(GrowableOrdinateArray other, int sourceIndex, int destinationIndex, int length)
        {
            EnsureLength(length + 1);
            AddRange(other.ordinates);
            this.currentIndex = length + 1;
        }
        
        public IEnumerator<System.Numerics.Vector4> GetEnumerator()=> ((IEnumerable<System.Numerics.Vector4>)ordinates).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()=> ordinates.GetEnumerator();

        public int IndexOf(System.Numerics.Vector4 item)=> ((IList<System.Numerics.Vector4>)ordinates).IndexOf(item);

        public void Insert(int index, System.Numerics.Vector4 item)=> ((IList<System.Numerics.Vector4>)ordinates).Insert(index, item);

        public void RemoveAt(int index)=> ((IList<System.Numerics.Vector4>)ordinates).RemoveAt(index);

        public void Clear()=> ((ICollection<System.Numerics.Vector4>)ordinates).Clear();

        public bool Contains(System.Numerics.Vector4 item)=> ((ICollection<System.Numerics.Vector4>)ordinates).Contains(item);

        public void CopyTo(System.Numerics.Vector4[] array, int arrayIndex)=> ((ICollection<System.Numerics.Vector4>)ordinates).CopyTo(array, arrayIndex);

        public bool Remove(System.Numerics.Vector4 item) => ((ICollection<System.Numerics.Vector4>)ordinates).Remove(item);
    }
}
