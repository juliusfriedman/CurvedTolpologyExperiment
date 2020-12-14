namespace CurvedTopologyExperiment.Geometry.CoordinateSequences
{
    /// <summary>
    /// <see cref="NetTopologySuite.Geometries.CoordinateSequence"/>
    /// </summary>
    public interface ICoordinateSequence
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ordinateIndex"></param>
        /// <returns></returns>
        public double GetOrdinate(int index, int ordinateIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ordinateIndex"></param>
        /// <param name="value"></param>
        public void SetOrdinate(int index, int ordinateIndex, double value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICoordinateSequence Copy();

        //Count?

        //Create
    }
}
