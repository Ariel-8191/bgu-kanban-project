using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides a factory for creating and resolving service instances.
    /// </summary>
    public class ServiceFactory
    {
        public UserService UserService { get; }
        public BoardService BoardService { get; }
        public TaskService TaskService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        public ServiceFactory()
        {
            AuthenticationFacade authenticationFacade = new AuthenticationFacade();
            UserFacade userFacade = new UserFacade(authenticationFacade);
            BoardFacade boardFacade = new BoardFacade(authenticationFacade);

            this.UserService = new UserService(userFacade);
            this.BoardService = new BoardService(boardFacade);
            this.TaskService = new TaskService(boardFacade);
        }
    }
}
