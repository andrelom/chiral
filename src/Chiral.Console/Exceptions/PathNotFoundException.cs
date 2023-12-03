namespace Chiral.Console.Exceptions;

public class PathNotFoundException : Exception
{
    public PathNotFoundException() {}

    public PathNotFoundException(string message) : base(message) { }
}
