using IntroSE.Kanban.Backend.BusinessLayer.Board;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Represents a board in the service layer.
    /// </summary>
    public class BoardSL
    {
        public string BoardName { get; set; }
        public long BoardID { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSL"/> class.
        /// </summary>
        /// <param name="boardBL">The board instance from the businnes layer that the new instance represents</param>
        internal BoardSL(BoardBL boardBL)
        {
            this.BoardName = boardBL.BoardName;
            this.BoardID = boardBL.BoardID;
        }

        // Required by System.Text.Json for deserialization
        public BoardSL() { }
    }
}