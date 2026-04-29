namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Class representing a response from the service layer.
    /// </summary>
    public class Response<T>
    {
        public string ErrorMessage { get; }
        public T ReturnValue { get; }

        /// <summary>
        /// Create an empty response.
        /// </summary>
        public Response()
        {

        }

        /// <summary>
        /// Create an error message response.
        /// </summary>
        public Response(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Create a response with a return value.
        /// </summary>
        public Response(T returnValue)
        {
            this.ReturnValue = returnValue;
        }

    }
}
