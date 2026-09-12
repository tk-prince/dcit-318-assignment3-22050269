using System;
using System.Collections.Generic;

// Core financial model using a record
public record Transaction(
    int Id,
    DateTime Date,
    decimal Amount,
    string Category
);

// Interface for transaction processing
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c. Mobile Money processor
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Mobile Money: Processing GHs{transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// Bank Transfer processor
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Bank Transfer: Processing GHs{transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// Crypto Wallet processor
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Crypto Wallet: Processing GHs{transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// General Account class
public class Account
{
    public string AccountNumber { get; }

    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty.");

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.");

        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    // Virtual method that can be overridden by derived classes
    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;

        Console.WriteLine(
            $"Transaction applied. Current balance: GH₵{Balance:F2}"
        );
    }
}

// e. Specialized SavingsAccount class
// sealed prevents further inheritance
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;

            Console.WriteLine(
                $"Transaction of GHs{transaction.Amount:F2} applied."
            );

            Console.WriteLine(
                $"Updated balance: GHs{Balance:F2}"
            );
        }
    }
}

// FinanceApp class
public class FinanceApp
{
    private List<Transaction> transactions = new List<Transaction>();

    public void Run()
    {
        // Create SavingsAccount with GH₵1000 initial balance
        SavingsAccount account = new SavingsAccount(
            "ACC-001",
            1000m
        );

        Console.WriteLine("===== FINANCE MANAGEMENT SYSTEM =====");
        Console.WriteLine($"Account Number: {account.AccountNumber}");
        Console.WriteLine($"Initial Balance: GH₵{account.Balance:F2}");
        Console.WriteLine();

        // Create three immutable Transaction records
        Transaction transaction1 = new Transaction(
            1,
            DateTime.Now,
            150m,
            "Groceries"
        );

        Transaction transaction2 = new Transaction(
            2,
            DateTime.Now,
            250m,
            "Utilities"
        );

        Transaction transaction3 = new Transaction(
            3,
            DateTime.Now,
            700m,
            "Entertainment"
        );

        // Create processors
        ITransactionProcessor mobileMoney =
            new MobileMoneyProcessor();

        ITransactionProcessor bankTransfer =
            new BankTransferProcessor();

        ITransactionProcessor cryptoWallet =
            new CryptoWalletProcessor();

        // Process and apply Transaction 1
        Console.WriteLine("--- Transaction 1 ---");
        mobileMoney.Process(transaction1);
        account.ApplyTransaction(transaction1);
        transactions.Add(transaction1);
        Console.WriteLine();

        // Process and apply Transaction 2
        Console.WriteLine("--- Transaction 2 ---");
        bankTransfer.Process(transaction2);
        account.ApplyTransaction(transaction2);
        transactions.Add(transaction2);
        Console.WriteLine();

        // Process and apply Transaction 3
        Console.WriteLine("--- Transaction 3 ---");
        cryptoWallet.Process(transaction3);
        account.ApplyTransaction(transaction3);
        transactions.Add(transaction3);
        Console.WriteLine();

        // Display transaction history
        Console.WriteLine("===== TRANSACTION HISTORY =====");

        foreach (Transaction transaction in transactions)
        {
            Console.WriteLine(
                $"ID: {transaction.Id} | " +
                $"Date: {transaction.Date:g} | " +
                $"Amount: GHs{transaction.Amount:F2} | " +
                $"Category: {transaction.Category}"
            );
        }

        Console.WriteLine();
        Console.WriteLine($"Final Balance: GHs{account.Balance:F2}");
    }
}

// Main application
public class Program
{
    public static void Main()
    {
        FinanceApp app = new FinanceApp();
        app.Run();
    }
}