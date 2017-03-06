namespace VirtualCashCard.Logic
{
    public interface ICashCard
    {
        bool IsValidPin(ushort pin);
        void Withdraw(ushort pin, decimal amt);
        void Topup(decimal amt);
        decimal GetBalance();
    }
}