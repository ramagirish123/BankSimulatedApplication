using System;
using System.Collections.Generic;
using System.Text;
using BankApplication.Services;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections;
using BankApplication.Data;

namespace BankApplication.UI
{
    // Customer UI.
    public class Customer
    {

        private Services.Interfaces.IUserService _userService;
        private Services.Interfaces.IAccountService _accountService;
        private Services.Interfaces.ITransactionService _transactionService;
        private Services.Interfaces.IExchangeRateService _exchangeRateServices;
        private Utility.UIStyling _UIStyling;
        private string _userID;
        private string _bankID;
        int _selectedOperation;
        string _selectedAccountID;
        string _userName;

        public Customer(string userID,string userName,string bankID,string JsonFilePath, Utility.UIStyling UIStyling)
        {
            _userID = userID;
            _bankID = bankID;
            _userService = new UserService(userID,JsonFilePath);
            _accountService = new AccountService(userID,JsonFilePath);
            _transactionService = new TransactionService(userID,JsonFilePath);
            _exchangeRateServices = new ExchangeRateService(JsonFilePath);
            _UIStyling = UIStyling;
            _userName = userName;

            while (true)
            {
                Console.WriteLine("  Welcome " + _userName+"\n");
                DisplayOperations();
                if(int.TryParse(Console.ReadLine(),out _selectedOperation))
                {
                    if(_selectedOperation==5)
                    {
                        break;
                    }
                    switch(_selectedOperation)
                    {
                        case 1:
                            {
                                WithDraw();
                                break;
                            }
                        case 2:
                            {
                                Deposit();
                                break;
                            }
                        case 3:
                            {
                                Transfer();
                                break;
                            }
                        case 4:
                            {
                                ViewHistoryofAccount();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("press only numbers from 1-4");
                                break;
                            }

                    }
                }
                else 
                {
                    Console.WriteLine("Press only numbers from 1-4");
                }
            }
        }
       private void DisplayOperations()
        {
            Console.WriteLine("  Press a number corresponding to that operation");
            Console.WriteLine("  1.Withdraw");
            Console.WriteLine("  2.Deposit");
            Console.WriteLine("  3.Transfer");
            Console.WriteLine("  4.View History");
            Console.WriteLine("  5.Logout\n");
            
        }

        // WithDraw amount from an account.
        private void WithDraw()
        {
            string message;

            _selectedAccountID = DisplayAndSelectAccountOfUser();
            if(_selectedAccountID==null)
            {
                return;
            }
            float amount;
            while (true)
            {
                Console.Write("  Enter the amount in INR:");
                if (float.TryParse(Console.ReadLine(), out amount))
                {
                    if (amount <= 0)
                    {
                        _UIStyling.ChangeForegroundForErrorMessage();
                        Console.WriteLine("  Enter only possitive values");
                        _UIStyling.RestoreForegroundColor();
                        continue;
                    }
                    else
                    {
                        message=_accountService.WithDraw(_selectedAccountID, amount);
                        Console.WriteLine(message);
                        break;
                    }
                }
                else
                {
                    _UIStyling.ChangeForegroundForErrorMessage();
                    Console.WriteLine("  Enter only numeric values");
                    _UIStyling.RestoreForegroundColor();
                }
            }
        }


        // Deposit amount into an account.
        private void Deposit()
        {
            string message;

            float amount;
            float exchangeRate;

            _selectedAccountID = DisplayAndSelectAccountOfUser();
            if (_selectedAccountID == null)
            {
                return;
            }
            while (true)
            {
                Console.WriteLine("  Enter the amount");
                if (float.TryParse(Console.ReadLine(), out amount))
                {
                    if (amount <= 0)
                    {
                        _UIStyling.ChangeForegroundForErrorMessage();
                        Console.WriteLine("  Enter only possitive values");
                        _UIStyling.RestoreForegroundColor();
                        continue;
                    }
                    else
                    {
                        exchangeRate = SelectExchangeRate();
                        message=_accountService.Deposit(_selectedAccountID, amount * exchangeRate);
                        Console.WriteLine(message);
                        break;
                    }
                }
                else
                {
                    _UIStyling.ChangeForegroundForErrorMessage();
                    Console.WriteLine("  Enter only numeric values");
                    _UIStyling.RestoreForegroundColor();
                }
            }
           
        }


        // Transfer amount from one account to other account.
        private void Transfer()
        {
            string message;

            float amount;
            string creditingAccount;
            bool IsValidAccount;

            Console.WriteLine("  select the account from which you want to transfer\n");
            _selectedAccountID = DisplayAndSelectAccountOfUser();
            if (_selectedAccountID == null)
            {
                return;
            }
            Console.Write("  Enter the account number to which you want to transfer: ");
            creditingAccount = Console.ReadLine();
            IsValidAccount = _accountService.IsAccountExists(creditingAccount);
            if(IsValidAccount)
            {
                Console.Write("  Enter the amount: ");
                while (true)
                {
                    if (float.TryParse(Console.ReadLine(), out amount))
                    {
                        if (amount <= 0)
                        {
                            _UIStyling.ChangeForegroundForErrorMessage();
                            Console.WriteLine("  Enter only possitive values");
                            _UIStyling.RestoreForegroundColor();
                            continue;
                        }
                        else
                        {
                            message=_accountService.Transfer(_selectedAccountID, creditingAccount,amount);
                            Console.WriteLine(message);
                            break;
                        }
                    }
                    else
                    {
                        _UIStyling.ChangeForegroundForErrorMessage();
                        Console.WriteLine("  Enter only numeric values");
                        _UIStyling.RestoreForegroundColor();
                    }
                }
            }
            else
            {
                _UIStyling.ChangeForegroundForErrorMessage();
                Console.WriteLine("Invalid AccountID");
                _UIStyling.RestoreForegroundColor();

            }
           

        }

        // Display history of an account.
        private void ViewHistoryofAccount()
        {
            IList<Models.Transaction.Transaction> transactionsList;
            int index = 1;
            bool IsCreditingTransaction;

            Console.WriteLine("  select the accountr\n");
            _selectedAccountID = DisplayAndSelectAccountOfUser();
            if (_selectedAccountID == null)
            {
                return;
            }

            transactionsList =_transactionService.GetTransactionsByAccountID(_selectedAccountID);
            Console.WriteLine("--------------------------------------------------------------------------------\n");
            Console.WriteLine(string.Format("  {0,-5}  {1,-8}  {2,-40}  {3,-8}  {4,-8}", "S.NO", "Date", "Transaction ID", "Amount", "Type"));
            foreach (Models.Transaction.Transaction transaction in transactionsList )
            {
                
                // Determining wheather transaction is Credit or Debit
                if (transaction.Type==Models.Transaction.Type.Transfer)
                {
                    if(transaction.PayeeID== _selectedAccountID)
                    {
                        IsCreditingTransaction = true;
                    }
                    else
                    {
                        IsCreditingTransaction = false;
                    }
                }
                else
                {
                    if (transaction.Type == Models.Transaction.Type.Deposit)
                    {
                        IsCreditingTransaction = true;
                    }
                    else
                    {
                        IsCreditingTransaction = false;
                    }
                }
                if (IsCreditingTransaction)
                {
                    Console.Write(String.Format("  {0,-5}  {1,-8}  {2,-40}  {3,-8}", index, transaction.CreatedOn.ToString("dd/MM/yyyy"), transaction.ID, transaction.Amount));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(String.Format("{0,-8}", "Credit\n"));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(String.Format("  {0,-5}  {1,-8}  {2,-40}  {3,-8}", index, transaction.CreatedOn.ToString("dd/MM/yyyy"), transaction.ID, transaction.Amount));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(String.Format("{0,-8}", "debit\n"));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                index++;
            }
            Console.WriteLine("--------------------------------------------------------------------------------\n");
        }


        // Prompt User to pick one of his accounts
       private string DisplayAndSelectAccountOfUser()
        {
            int accountsCount;
            int index;
            int selectedAccountIndex;
            IList<String> accountsList = _accountService.GetAccountsbyUserID(_userID);

            accountsCount = accountsList.Count();
            if(accountsCount==0)
            {
                _UIStyling.ChangeForegroundForErrorMessage();
                Console.WriteLine("  You don't have any accounts\n");
                _UIStyling.RestoreForegroundColor();
                return null;
            }
            else
            {
                while(true)
                {
                    index = 0;
                    Console.WriteLine("  Press Number corresponding to the account\n");
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(String.Format("  {0,-5}  {1,-36}\n", "S.No", "AccountID"));
                    while(index<accountsCount)
                    {
                        Console.WriteLine(String.Format("  {0,-5}  {1,-36}", (index + 1),accountsList[index]));
                        index++;
                    }
                    Console.WriteLine("-----------------------------------------------\n");
                    Console.WriteLine("  Enter " +(accountsCount+1) +" to go back" );
                    if (int.TryParse(Console.ReadLine(), out selectedAccountIndex))
                    {
                        // User selecting go back option.
                        if (selectedAccountIndex == accountsCount + 1)
                        {
                            return default;
                        }
                        if (selectedAccountIndex >= 1 && selectedAccountIndex <= (accountsCount))
                        {
                            return accountsList[selectedAccountIndex - 1];
                        }
                        _UIStyling.ChangeForegroundForErrorMessage();
                        Console.WriteLine("Enter between 1 and " + accountsCount);
                        _UIStyling.RestoreForegroundColor();
                    }
                    else
                    {
                        _UIStyling.ChangeForegroundForErrorMessage();
                        Console.WriteLine("Enter Only numeric values");
                        _UIStyling.RestoreForegroundColor();
                    }
                }
            }
        }

        // Displaying exchange rates of bank and selecting one among them.
        private float SelectExchangeRate()
        {
            int exchangeRatesLength;
            int index=0;
            int selectedCurrency;
            IList<BankApplication.Models.ExchangeRate.ExchangeRate> rates = _exchangeRateServices.GetExchangeRatesByBankID(_bankID);
            exchangeRatesLength = rates.Count();
            Console.WriteLine("  Select Currency you want to deposit\n");
            while (true)
            {
                index = 0;
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine(String.Format("  {0,-5}  {1,-12}  {2,-5}\n", "S.No", "Currency Name", "Rate"));
                while (index < exchangeRatesLength)
                {
                    Console.WriteLine(String.Format("  {0,-5}  {1,-12}  {2,-5}\n",(index + 1),rates[index].CurrencyName, rates[index].Rate));
                    index++;
                }
                Console.WriteLine("-----------------------------------------------");
                if (int.TryParse(Console.ReadLine(),out selectedCurrency))
                {
                    if(selectedCurrency>=1&& selectedCurrency<=(exchangeRatesLength))
                    {
                        return rates[selectedCurrency-1].Rate;
                    }
                    _UIStyling.ChangeForegroundForErrorMessage();
                    Console.WriteLine("  Enter between 1 and " + exchangeRatesLength);
                }
                Console.WriteLine("  Enter Only numeric values");
            }
            

        }
    }

}
