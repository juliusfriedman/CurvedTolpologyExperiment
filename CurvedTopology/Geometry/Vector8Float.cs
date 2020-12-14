namespace CurvedTopologyExperiment.Geometry.Models
{
    /// <summary>
    /// A single <see cref="System.Numerics.Vector{double}"/> or dual <see cref="System.Numerics.Vector4"/>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 32)]
    public struct Vector8Float
    {
        /// <summary>
        /// 4 Doubles
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<double> Vector;

        /// <summary>
        /// 4 Floats
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector4 High;

        /// <summary>
        /// 4 Floats
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(16)]
        public System.Numerics.Vector4 Low;

        public float A
        {
            get => High.X;
            set => High = new System.Numerics.Vector4(value, High.Y, High.Z, High.W);
        }                                                            
                                                                     
        public float B                                               
        {                                                            
            get => High.Y;                                           
            set => High = new System.Numerics.Vector4(High.X, value, High.Z, High.W);
        }

        public float C
        {
            get => High.Z;
            set => High = new System.Numerics.Vector4(High.X, High.Y, value, High.W);
        }

        public float D                                               
        {                                                            
            get => High.W;                                           
            set => High = new System.Numerics.Vector4(High.X, High.Y,High.Z, value);
        }

        public float X
        {
            get => Low.X;
            set => Low = new System.Numerics.Vector4(value, Low.Y, Low.W, Low.Z);
        }

        public float Y
        {
            get => Low.Y;
            set => Low = new System.Numerics.Vector4(Low.X, value, Low.W, Low.Z);
        }

        public float Z
        {
            get => Low.Z;
            set => Low = new System.Numerics.Vector4(Low.X, Low.Y, value, Low.W);
        }

        public float W
        {
            get => Low.W;
            set => Low = new System.Numerics.Vector4(Low.X, Low.Y, Low.Z, value);
        }

        /// <summary>
        /// Gets or Sets the Raw Binary data assoicated with the instance (32 bytes)
        /// </summary>
        public System.Span<byte> RawBytes
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector<double>, byte>(ref Vector), 32); } //Must be updated from Size in declaration
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            set { value.CopyTo(RawBytes); }
        }

        /// <summary>
        /// Access elements by offset
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return A;
                    case 1: return B;
                    case 2: return C;
                    case 3: return D;
                    case 4: return X;
                    case 5: return Y;
                    case 6: return Z;
                    case 7: return W;
                    default: throw new System.ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public Vector8Float(float[] eight) : this()
        {
            //Todo profile if the runtime optomizes load asm for this or if RawBytes copy is better
            //High = new System.Numerics.Vector4(eight[0], eight[1], eight[2], eight[3]);
            //Low = new System.Numerics.Vector4(eight[4], eight[5], eight[6], eight[7]);
            var c = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<float, byte>(ref eight[0]), eight.Length * sizeof(float));
            c.CopyTo(RawBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="four"></param>
        public Vector8Float(double[] four)
        {
            High = Low = default;
            Vector = new System.Numerics.Vector<double>(four);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vector8Float(double x, double y, double z, double w) 
            : this(new double[] { x, y, z, w }) { }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Vector8Float(float a, float b, float c, float d) 
            : this(new float[] { a, b, c, d }) { }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vector8Float(float a, float b, float c, float d, float x, float y, float z, float w) : 
            this(new float[] { a, b, c, d, x, y, z, w }) { }

        public Vector8Float(System.Numerics.Vector4 high, System.Numerics.Vector4 low)
        {
            Vector = default;
            High = high;
            Low = low;
        }

        public override string ToString()=> string.Join(" ", A, B, C, D, X, Y, Z, W);
    }
}
