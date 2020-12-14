namespace OperationalTwins.DataLayer.Models
{
    /// <summary>
    /// A single <see cref="System.Numerics.Vector{ulong}"/> or dual <see cref="System.Numerics.Vector4"/>
    /// Allow contravariant access to binary elements.
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 32)]
    public ref struct LongVector
    {
        // <summary>
        /// 32 bytes
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<byte> Units;

        // <summary>
        /// 16 Halfs/(u)shorts
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<ushort> Smalls;

        #region 8 Single

        /// <summary>
        /// 4 Floats
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<float> High;

        /// <summary>
        /// 4 Floats
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(16)]
        public System.Numerics.Vector<float> Low;

        #endregion

        /// <summary>
        /// 4 Doubles
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<double> Vector;

        /// <summary>
        /// 4 longs
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<long> Signed;

        /// <summary>
        /// 4 ulongs
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public System.Numerics.Vector<ulong> Unsigned;
    }
}
