using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.CrossCutting
{
    /// <summary>
    /// The base exception for all domain-specific logic errors in the Kanban system.
    /// </summary>
    public class KanbanException : Exception
    {
        public KanbanException() { }
        public KanbanException(string message) : base(message) { }
        public KanbanException(string message, Exception innerException) : base(message, innerException) { }
    }


    /// <summary>
    /// Thrown when an input fails domain validation rules.
    /// </summary>
    public class KanbanValidationException : KanbanException
    {
        public KanbanValidationException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when an action fails due to user identity or access issues.
    /// </summary>
    public class KanbanAuthenticationException : KanbanException
    {
        public KanbanAuthenticationException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when an entity requested by the user does not exist in the system.
    /// </summary>
    public class KanbanNotFoundException : KanbanException
    {
        public KanbanNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when attempting to create something that violates a uniqueness constraint.
    /// </summary>
    public class KanbanConflictException : KanbanException
    {
        public KanbanConflictException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when an action violates the lifecycle or state rules of the domain.
    /// </summary>
    public class KanbanInvalidStateException : KanbanException
    {
        public KanbanInvalidStateException(string message) : base(message) { }
    }
}
