using System;

namespace VirtualCashCard.Logic
{
    public class InvalidPinException : InvalidOperationException
    {
        public InvalidPinException(string message)
            : base(message)
        {
            ;
        }
    }
}