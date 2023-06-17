using System;

namespace Project1;

[Serializable]
public class TempPathNotExistException : Exception
{
    public TempPathNotExistException(string message):base(message)
    {}
}