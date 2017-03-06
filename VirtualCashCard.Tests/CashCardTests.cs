using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualCashCard.Logic;

namespace VirtualCashCard.Tests
{
    [TestClass]
    public class CashCardTests
    {
        [TestMethod]
        public void CashCard_IsCreated_With_ZeroBalance()
        {
            //arrange //act
            ICashCard card = new CashCard(1234);

            //assert
            Assert.AreEqual(0, card.GetBalance());
        }

        [TestMethod]
        public void CashCard_IsValidPin_Returns_True_For_Valid_Pin()
        {
            //arrange 
            ushort pin = 1234;
            ICashCard card = new CashCard(pin);

            //act
            bool isValidPin = card.IsValidPin(pin);

            //assert
            Assert.IsTrue(isValidPin);
        }

        [TestMethod]
        public void CashCard_IsValidPin_Returns_False_For_InValid_Pin()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            ushort invalidpin = 0000;
            bool isValidPin = card.IsValidPin(invalidpin);

            //assert
            Assert.IsFalse(isValidPin);
        }

        [TestMethod]
        public void CashCard_Topup_Updates_Card_Balance()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(100);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(100, balance);
        }

        [TestMethod]
        public void CashCard_Topup_Updates_Card_Balance_Multipletimes()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(100);
            card.Topup(100);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(200, balance);
        }

        [TestMethod]
        public void CashCard_Withdraws_Reduces_Balance()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(200);
            card.Withdraw(validpin, 100);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(100, balance);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPinException))]
        public void CashCard_Withdraws_Validates_Pin_Before_Withdrawl()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            ushort invalidPin = 1111;
            card.Topup(200);
            card.Withdraw(invalidPin, 100);
            decimal balance = card.GetBalance();

            //assert
            //expected exception = InvalidPinException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPinException))]
        public void CashCard_Withdraws_Throws_InvalidPinException_For_Invalid_Pin()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            ushort invalidPin = 1111;
            card.Topup(200);
            card.Withdraw(invalidPin, 100);
            decimal balance = card.GetBalance();

            //assert
            //expected exception = InvalidPinException
        }

        [TestMethod]
        public void CashCard_Withdraws_DoesNot_Throw_InvalidPinException_For_Valid_Pin()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(200);
            card.Withdraw(validpin, 100);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(100, balance);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientBalanceException))]
        public void CashCard_Withdraws_Throws_InsufficientBalanceException_For_Over_Withdrawls()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Withdraw(validpin, 10);

            //assert
            //expected exception = InsufficientBalanceException
        }

        [TestMethod]
        public void CashCard_Withdraws_DoesNot_Throw_InsufficientBalanceException_When_Enough_Balance()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(100);
            card.Withdraw(validpin, 10);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(90, balance);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void CashCard_Topup_Does_Not_Ignore_ArithmeticOverflow_And_Throws_OverflowException()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(decimal.MaxValue);
            card.Topup(1);
            decimal balance = card.GetBalance();

            //assert
            //OverflowException exception
        }

        [TestMethod]
        public void CashCard_Topup_Does_Not_Throw_OverflowException_For_Valid_Topup()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            //act
            card.Topup(1000);
            card.Topup(1);
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(1001, balance);
        }

        [TestMethod]
        public void CashCard_Topup_Can_Handle_CONCURRENT_Topups()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);

            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
            tasks.Add(Task.Factory.StartNew(() => { card.Topup(100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Topup(100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Topup(100); }));

            //act
            Task.WaitAll(tasks.ToArray());

            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(300, balance);
        }


        [TestMethod]
        public void CashCard_Topup_Can_Handle_CONCURRENT_Withdrawls()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);
            card.Topup(1000);

            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
            tasks.Add(Task.Factory.StartNew(() => { card.Withdraw(validpin, 100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Withdraw(validpin, 100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Withdraw(validpin, 100); }));

            //act
            Task.WaitAll(tasks.ToArray());

            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(700, balance);
        }

        [TestMethod]
        public void CashCard_Topup_Can_Handle_CONCURRENT_Topup__And_Withdrawl()
        {
            //arrange 
            ushort validpin = 1234;
            ICashCard card = new CashCard(validpin);
            card.Topup(1000);

            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
            tasks.Add(Task.Factory.StartNew(() => { card.Withdraw(validpin, 100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Topup(100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Withdraw(validpin, 100); }));
            tasks.Add(Task.Factory.StartNew(() => { card.Topup(100); }));
            
            //act
            Task.WaitAll(tasks.ToArray());
            
            decimal balance = card.GetBalance();

            //assert
            Assert.AreEqual(1000, balance);
        }
    }
}
