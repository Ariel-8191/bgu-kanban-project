namespace Frontend.Model
{
    /// <summary>
    /// A frontend model representing a board in a user's board list.
    /// Exposes only the information shown to the user (name and owner); the internal
    /// board ID is kept for operations but is never displayed.
    /// </summary>
    public class BoardModel
    {
        /// <summary>
        /// The internal ID of the board. Used for backend operations only and never shown in the UI.
        /// </summary>
        public long BoardID { get; }

        /// <summary>
        /// The name of the board.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The email of the board's owner.
        /// </summary>
        public string Owner { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardModel"/> class.
        /// </summary>
        /// <param name="boardID">The internal ID of the board.</param>
        /// <param name="name">The name of the board.</param>
        /// <param name="owner">The email of the board's owner.</param>
        public BoardModel(long boardID, string name, string owner)
        {
            BoardID = boardID;
            Name = name;
            Owner = owner;
        }
    }
}
