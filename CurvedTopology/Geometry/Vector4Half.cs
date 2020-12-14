using System;
using System.Numerics;

namespace CurvedTopologyExperiment.Geometry.Models
{
    //Competes with half and supports vectorization
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 8)]
    public /*ref*/ struct Vector4Half
    {
        /// <summary>
        /// 8 bytes
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double Value;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public Vector<ushort> Vector16;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public Vector2 Vector32;

        //Makes the Vector Mutable
        [System.Runtime.InteropServices.FieldOffset(0)]
        public Half A;
        [System.Runtime.InteropServices.FieldOffset(2)]
        public Half B;
        //Y
        [System.Runtime.InteropServices.FieldOffset(4)]
        public Half C;
        [System.Runtime.InteropServices.FieldOffset(6)]
        public Half D;

        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        //private Vector4Half()
        //{
        //    Vector = default;
        //    Vector16 = default;
        //    Vector32 = default;
        //    A = B = C = D = Half.NaN._value;
        //}
    }
}
