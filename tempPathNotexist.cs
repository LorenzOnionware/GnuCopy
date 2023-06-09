using System;
using System.IO;

namespace Project1;

[Serializable]
public class TempPathNotExistException : Exception
{
    public TempPathNotExistException(string message):base(message)
    {}
}