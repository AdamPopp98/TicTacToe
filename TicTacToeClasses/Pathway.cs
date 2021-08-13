namespace TicTacToeClasses
{
    /// <summary>
    /// Class that contains a possible winning condition.
    /// </summary>
    public class Pathway
    {
        private int[] m_NodePositions;
        public int[] GetNodePositions()
        {
            return m_NodePositions;
        }
        public Pathway(int first, int second, int third)
        {
            SetNodePositions(first, second, third);
        }
        private void SetNodePositions(int first, int second, int third)
        {
            m_NodePositions = new int[] { first, second, third };
        }
    }
}
