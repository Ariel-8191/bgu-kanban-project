using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Collections.Generic;
using System.Text.Json;

namespace Frontend.Controllers
{
    /// <summary>
    /// Frontend controller that wraps the backend <see cref="BoardService"/>.
    /// It translates the JSON responses of the service layer into frontend models
    /// and throws an exception whenever the service layer reports an error.
    /// </summary>
    public class BoardController
    {
        private readonly BoardService boardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardController"/> class.
        /// </summary>
        /// <param name="boardService">The backend board service to delegate to.</param>
        public BoardController(BoardService boardService)
        {
            this.boardService = boardService;
        }

        /// <summary>
        /// Retrieves all the boards a user is a member of, including each board's name and owner.
        /// </summary>
        /// <param name="email">The email of the user whose boards should be retrieved.</param>
        /// <returns>A list of <see cref="BoardModel"/> objects.</returns>
        /// <exception cref="Exception">Thrown if the boards cannot be retrieved.</exception>
        public List<BoardModel> GetUserBoards(string email)
        {
            string idsJson = boardService.GetUserBoards(email);
            Response<List<long>> idsResponse = JsonSerializer.Deserialize<Response<List<long>>>(idsJson)!;
            if (!string.IsNullOrEmpty(idsResponse.ErrorMessage))
            {
                throw new Exception(idsResponse.ErrorMessage);
            }

            List<BoardModel> boards = new List<BoardModel>();
            foreach (long boardID in idsResponse.ReturnValue!)
            {
                BoardSL board = GetBoard(boardID);
                boards.Add(new BoardModel(board.BoardID, board.BoardName, board.Owner));
            }
            return boards;
        }

        /// <summary>
        /// Creates a new board owned by the given user.
        /// </summary>
        /// <param name="email">The email of the user creating the board.</param>
        /// <param name="boardName">The name of the new board.</param>
        /// <returns>A <see cref="BoardModel"/> representing the created board.</returns>
        /// <exception cref="Exception">Thrown if the board cannot be created.</exception>
        public BoardModel CreateBoard(string email, string boardName)
        {
            string json = boardService.CreateBoard(email, boardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(json)!;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
            BoardSL board = response.ReturnValue!;
            return new BoardModel(board.BoardID, board.BoardName, board.Owner);
        }

        /// <summary>
        /// Deletes a board owned by the given user.
        /// </summary>
        /// <param name="email">The email of the board owner.</param>
        /// <param name="boardName">The name of the board to delete.</param>
        /// <exception cref="Exception">Thrown if the board cannot be deleted.</exception>
        public void DeleteBoard(string email, string boardName)
        {
            string json = boardService.DeleteBoard(email, boardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(json)!;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Retrieves a single board (name and owner) by its ID, throwing on error.
        /// </summary>
        /// <param name="boardID">The ID of the board to retrieve.</param>
        /// <returns>The board contained in the response.</returns>
        /// <exception cref="Exception">Thrown if the board cannot be retrieved.</exception>
        private BoardSL GetBoard(long boardID)
        {
            string json = boardService.GetBoard(boardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(json)!;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.ReturnValue!;
        }
    }
}
