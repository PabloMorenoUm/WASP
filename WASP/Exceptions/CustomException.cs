﻿namespace WASP.Exceptions;

[Serializable]
public class CustomException : Exception
{
    protected CustomException(string message) : base(message)
    {
    }
}