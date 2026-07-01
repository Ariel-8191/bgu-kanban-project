using IntroSE.Kanban.Backend.ServiceLayer;

namespace Frontend.Controllers
{
    /// <summary>
    /// Creates and holds the frontend controllers, all sharing a single backend
    /// <see cref="ServiceFactory"/> instance so that the in-memory state is consistent
    /// across the whole application.
    /// </summary>
    public class ControllerFactory
    {
        private readonly ServiceFactory serviceFactory;

        public UserController UserController { get; }
        public BoardController BoardController { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerFactory"/> class,
        /// creating the shared backend service factory and the controllers that use it.
        /// </summary>
        public ControllerFactory()
        {
            this.serviceFactory = new ServiceFactory();
            this.UserController = new UserController(serviceFactory.UserService);
            this.BoardController = new BoardController(serviceFactory.BoardService);
        }

        /// <summary>
        /// Loads all persisted data from the database into memory.
        /// Should be called once when the application starts.
        /// </summary>
        public void LoadData()
        {
            this.serviceFactory.LoadData();
        }
    }
}
