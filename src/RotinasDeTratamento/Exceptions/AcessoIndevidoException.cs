using System;

public class AcessoIndevidoException : Exception
{

    public AcessoIndevidoException(string message)
    {
        Console.WriteLine($"[{message}]");
    }
}