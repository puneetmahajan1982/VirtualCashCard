using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualCashCard.Logic
{
    public class CashCard : ICashCard
    {
        private readonly ushort _pin;
        private decimal _balance;
        private object syncObject;

        public CashCard(ushort pin)
        {
            _pin = pin;
        }

        public bool IsValidPin(ushort pin)
        {
            return _pin == pin;
        }

        public void Withdraw(ushort pin, decimal amt)
        {
            if (IsValidPin(pin))
            {
                UpdateBalance(-amt, TransactionType.Withdraw);
            }
            else
            {
                throw new InvalidPinException("Invalid Pin, cannot withdraw money");
            }
        }

        public void Topup(decimal amt)
        {
            UpdateBalance(amt, TransactionType.Topup);
        }

        public decimal GetBalance()
        {
            return _balance;
        }

        private void UpdateBalance(decimal amt, TransactionType transactionType)
        {
            // create lock before updating balance
            lock (syncObject ?? new object())
            {
                if (transactionType == TransactionType.Withdraw && -amt > _balance)
                {
                    throw new InsufficientBalanceException("Insufficient Balance");
                }

                // Check to raise arithmetic overflow exception (OverflowException), we do not want to overlook overflows for bank balance
                checked
                {
                    _balance += amt;
                }
            }
        }
    }
}
