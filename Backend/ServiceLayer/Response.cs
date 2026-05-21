using System.Text.Json;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Represents a response returned from the service layer.
    /// </summary>
    public class Response<T>
    {
        public string ErrorMessage { get; set; }
        public T ReturnValue { get; set; }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="Response{T}"/> class.
        /// </summary>
        public Response() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class with an error message.
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        public Response(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class with a return value.
        /// </summary>
        /// <param name="returnValue">The return value</param>
        public Response(T returnValue)
        {
            this.ReturnValue = returnValue;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
