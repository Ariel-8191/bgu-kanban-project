namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Class representing a board in the service layer.
    /// </summary>
    public class BoardSL
    {
        public string BoardName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSL"/> class.
        /// </summary>
        /// <param name="boardBL"></param>
        internal BoardSL(BoardBL boardBL)
        {
            this.BoardName = boardBL.BoardBL;
        }
    }
}