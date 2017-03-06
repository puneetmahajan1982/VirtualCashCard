using System;

namespace VirtualCashCard.Logic
{
    public class InsufficientBalanceException : InvalidOperationException
    {
        public InsufficientBalanceException(string message):base(message)
        {
            ;
        }
    }
}