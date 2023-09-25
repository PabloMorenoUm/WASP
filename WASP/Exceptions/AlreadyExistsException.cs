namespace WASP.Exceptions;

public class AlreadyExistsException : CustomException
{
    public AlreadyExistsException(string message) : base(message)
    {
    }
}