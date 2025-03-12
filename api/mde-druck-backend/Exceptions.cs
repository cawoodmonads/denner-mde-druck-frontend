namespace Denner.MDEDruck.Exceptions
{
    public class APIException : Exception
    {
        public APIException() { }
        public APIException(string message) : base(message) { }
        public APIException(string message, Exception inner) : base(message, inner) { }
    }
    public class UnexpectedException : Exception
    {
        public UnexpectedException() { }
        public UnexpectedException(string message) : base(message) { }
        public UnexpectedException(string message, Exception inner) : base(message, inner) { }
    }
}
