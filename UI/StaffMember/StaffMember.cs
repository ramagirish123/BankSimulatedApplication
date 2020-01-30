using System;
using System.Collections.Generic;
using System.Text;
using BankApplication.Services;

namespace BankApplication.UI
{
    public class StaffMember
    {
        private Services.Interfaces.IBankService _bankService;

        private Services.Interfaces.IUserService _userService;

        private Services.Interfaces.IAccountService _accountService;

        private Services.Interfaces.ITransactionService _transactionService;

        private Services.Interfaces.IExchangeRateService _exchangeRateService;

        private Utility.UIStyling _UIStyling;

        private string _userID;
        private string _bankID;
        private int _selectedOperation;
        private string _userName;
        public StaffMember(string userID,string userName ,string bankID, string JsonFilepath,Utility.UIStyling UIStyling )
        {
            this._userID = userID;
            this._bankID = bankID;
            _userName = userName;
            _bankService = new BankService(userID,JsonFilepath);
            _userService = new UserService(userID,JsonFilepath);
            _accountService = new AccountService(userID,JsonFilepath);
            _transactionService = new TransactionService(userID,JsonFilepath);
            _exchangeRateService = new ExchangeRateService(JsonFilepath);
            _UIStyling = UIStyling;

            Console.WriteLine("  Welcome " + _userName);
            DisplayNotifications();
            while (true)
            {
                DisplayOperations();
                if (int.TryParse(Console.ReadLine(), out _selectedOperation))
                {
                    if (_selectedOperation == 12)
                    {
                        break;
                    }
                    switch (_selectedOperation)
                    {
                        case 1:
                            {
                                CreateUser();
                                break;
                            }
                        case 2:
                            {
                                UpdateUser();
                                break;
                            }
                        case 3:
                            {
                                DeleteUser();
                                break;
                            }
                        case 4:
                            {
                                ChangeRtgsOfSameBank();
                                break;
                            }
                        case 5:
                            {
                                ChangeImpsOfSameBank();
                                break;
                            }
                        case 6:
                            {
                                ChangeRtgsOfotherBank();
                                break;
                            }
                        case 7:
                            {
                                ChangeImpsOfOtherBank();
                                break;
                            }
                        case 8:
                            {
                                AddExchangeRateToBank();
                                break;
                            }
                        case 9:
                            {
                                ViewTransactionsofCustomer();
                                break;
                            }
                        case 10:
                            {
                                RevertTransferTranaction();
                                break;
                            }
                        case 11:
                            {
                                ChangePendingLoginRequestStatus();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("press only numbers from 1-11");
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
        // Display Notification of staff
        private void DisplayNotifications()
        {
            int numberOfRequests;

            Console.WriteLine("**Notifications**");
            Console.WriteLine("------------------------------------------" );
            numberOfRequests = _userService.GetRequestedUsersCount(_bankID);
            Console.WriteLine(String.Format("There are {0} pending requests", numberOfRequests));
        }

        // Accepting or Decline pending user request
        private void ChangePendingLoginRequestStatus()
        {
            string selectedRequestID;
            int selectedOperation;
            string reason;

            selectedRequestID = DisplayAndSelectPendingLoginRequests();
            // If there are no pending requests the exit this function.
            if (selectedRequestID == null)
                return;
            while(true)
            {
                Console.WriteLine("Press number corresponding to the operation");
                Console.WriteLine("1. Accept");
                Console.WriteLine("2. Decline");
                if (int.TryParse(Console.ReadLine(), out selectedOperation))
                {
                    if(selectedOperation>0 && selectedOperation<=2)
                        switch(selectedOperation)
                        {
                            case 1:
                                _userService.AcceptUserRequestByID(selectedRequestID);
                                _accountService.AddAccount(selectedRequestID);
                                break;
                            case 2:
                                Console.WriteLine("Enter the reason for declining.");
                                reason = Console.ReadLine();
                                _userService.DeclineUserRequestByID(selectedRequestID,reason);
                                break;
                        }
                    break;
                }
                else
                {
                    Console.WriteLine("Enter only numeric");
                }
            }
        }

        // Display Pending user requests
        private string DisplayAndSelectPendingLoginRequests()
        {
            int RequestedUsersCount;
            int index = 0;
            int selectedRequestIndex;
            IList<Models.User.User> requestedUsers = _userService.GetRequestedUsersOfBank(_bankID);

            RequestedUsersCount = requestedUsers.Count;
            if (RequestedUsersCount == 0)
            {
                return null;
            }
            else
            {
                while (true)
                {
                    index = 0;
                    Console.WriteLine("Press Number corresponding to the user");
                    Console.WriteLine("------------------------------------------------------------------------");
                    Console.WriteLine(string.Format("{0,-4}{1,-50}{2,-10}", "S.No","ID","Name"));
                    while (index < RequestedUsersCount)
                    {
                        Console.WriteLine(string.Format("{0,-4}{1,-50}{2,-10}", index + 1, requestedUsers[index].ID, requestedUsers[index].Name));
                        index++;
                    }
                    Console.WriteLine("-------------------------------------------------------------------------");

                    if (int.TryParse(Console.ReadLine(), out selectedRequestIndex))
                    {
                        if (selectedRequestIndex >= 1 && selectedRequestIndex <= (RequestedUsersCount))
                        {
                            return requestedUsers[selectedRequestIndex - 1].ID;
                        }
                        Console.WriteLine("Enter between 1 and " + RequestedUsersCount);
                    }
                }
            }
        }


        // Display operations that a staff can perform.
        private void DisplayOperations()
        {
            Console.WriteLine("Press a number corresponding to that operation");
            Console.WriteLine("1. Create User");
            Console.WriteLine("2. Update User");
            Console.WriteLine("3. Delete User");
            Console.WriteLine("4. Change RTGS charges of Same Bank Transactions");
            Console.WriteLine("5. Change IMPS charges of same Bank Transactions");
            Console.WriteLine("6. Change RTGS charges of other Bank Transactions");
            Console.WriteLine("7. Change IMPS charges of Other Bank Transactions");
            Console.WriteLine("8. Add Exchange Rate");
            Console.WriteLine("9. View Transactions");
            Console.WriteLine("10. Revert Transactions");
            Console.WriteLine("11. Accept/Decline users login request");
            Console.WriteLine("12. Logout");

        }

        // Creating new account.
        private void CreateUser()
        {
            string name;
            string password;
            string phoneNumber;
            string email;
            int type;
            string ID;
            string accountID;
            bool isEMailAvailable;

            Console.WriteLine("Enter Name of the user");
            name = Console.ReadLine();
            Console.WriteLine("Enter Password of the user");
            password = Console.ReadLine();
            Console.WriteLine("Enter phone number of the user");
            phoneNumber = Console.ReadLine();
            while (true)
            {
                Console.WriteLine("Enter email of the user");
                email = Console.ReadLine();
                isEMailAvailable = _userService.CheckEMailAvailbility(email, _bankID);
                if(isEMailAvailable)
                {
                    ID = _userService.AddUser(_bankID, name, password, email, phoneNumber);
                    Console.WriteLine(ID + " is new userID");
                    accountID=_accountService.AddAccount(ID);
                    Console.WriteLine(accountID + " is the default account opened for the customer");
                    break;
                }
                else
                {
                    Console.WriteLine("Email is not available.Please enter another EMail ID");
                }
                
            }
            
        }

        // Updating account.
        private void UpdateUser()
        {
            string selectedCustomerID;
            int selectedOperation;
            string name;
            string EMail;
            string mobileNumber;
            string selectedAccountID;
            string createdAccountID;

            selectedCustomerID = DisplayAndSelectCustomerID();
            if (selectedCustomerID == null)
            {
                Console.WriteLine("There are no users");
                return;
            }
            selectedOperation = DisplayCustomerUpdateOperations();
            switch (selectedOperation)
            {
                case 1:
                    Console.WriteLine("Enter new name of the customer");
                    name = Console.ReadLine();
                    _userService.UpdateCustomerNameByID(selectedCustomerID, name);
                    break;
                
                case 2:
                    Console.WriteLine("Enter new email of the customer");
                    EMail = Console.ReadLine();
                    _userService.UpdateCustomerEMailByID(selectedCustomerID, EMail);
                    break;
                case 3:
                    Console.WriteLine("Enter new mobileNumber of the customer");
                    mobileNumber = Console.ReadLine();
                    _userService.UpdateCustomerMobileNumberByID(selectedCustomerID, mobileNumber);
                    break;
                case 4:
                    createdAccountID=_accountService.AddAccount(selectedCustomerID);
                    Console.WriteLine("New Account Succeddfully");
                    Console.WriteLine("AccountID" + createdAccountID);
                    break;
                case 5:
                    selectedAccountID = DisplayAndSelectAccountOfCustomer(selectedCustomerID);
                    _accountService.DeleteAccount(selectedAccountID);
                    break;
            }

        }

        // Deleting Customer.
        private void DeleteUser()
        {
            string selectedUserID;

            selectedUserID = DisplayAndSelectCustomerID();
            _userService.DeleteUserByID(selectedUserID);

        }

        // Change RTGS charges of Same Bank Transactions.
        private void ChangeRtgsOfSameBank()
        {
            float newRtgs;

            Console.WriteLine("Enter RTGS charge");
            while (true)
            {
                if (float.TryParse(Console.ReadLine(), out newRtgs))
                {
                    _bankService.UpdateRtgsOfSameBank(_bankID,newRtgs);
                    break;
                }
                Console.WriteLine("Enter only numerics");
            }

        }

        // Change IMPS charges of Same Bank Transactions.
        private void  ChangeImpsOfSameBank()
        {
            float newImps;

            Console.WriteLine("Enter IMPS charge");
            while (true)
            {
                if (float.TryParse(Console.ReadLine(), out newImps))
                {
                    _bankService.UpdateImpsOfSameBank(_bankID, newImps);
                    break;
                }
                Console.WriteLine("Enter only numerics");
            }
        }

        // Change RTGS charges of Other Bank Transactions.
        private void ChangeRtgsOfotherBank()
        {
            float newRtgs;

            Console.WriteLine("Enter RTGS charge");
            while (true)
            {
                if (float.TryParse(Console.ReadLine(), out newRtgs))
                {
                    _bankService.UpdateRtgsOfOtherBank(_bankID,newRtgs);
                    break;
                }
                Console.WriteLine("Enter only numerics");
            }
        }

        // Change IMPS charges of Other Bank Transactions.
        private void ChangeImpsOfOtherBank()
        {
            float newImps;

            Console.WriteLine("Enter IMPS charge");
            while (true)
            {
                if (float.TryParse(Console.ReadLine(), out newImps))
                {
                    _bankService.UpdateImpsOfOtherBank(_bankID,newImps);
                    break;
                }
                Console.WriteLine("Enter only numerics");
            }
        }

        // Reverting transaction.
        private void RevertTransferTranaction()
        {
            IList<Models.Transaction.Transaction> transactions;
            string customerID;
            string accountID;
            int index = 0;
            int transactionsCount=0;
            int selectedTransactionIndex;

            customerID = DisplayAndSelectCustomerID();
            accountID = DisplayAndSelectAccountOfCustomer(customerID);
            transactions = _transactionService.GetTransferTransactionsByAccountID(accountID);
            transactionsCount = transactions.Count;
            if (transactionsCount == 0)
            {
                Console.WriteLine("There are no transactions");
            }
            else
            {
                while (true)
                {
                    index = 0;
                    Console.WriteLine("Press Number corresponding to the transactions");
                    while (index < transactionsCount)
                    {
                        Console.WriteLine((index + 1) + " " + transactions[index].ID + " ");
                        index++;
                    }

                    if (int.TryParse(Console.ReadLine(), out selectedTransactionIndex))
                    {
                        if (selectedTransactionIndex >= 1 && selectedTransactionIndex <= (transactionsCount))
                        {
                            //_transactionService.RevertTransacation(transactions[selectedTransactionIndex - 1].ID, _userID);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter between 1 and " + transactionsCount);
                        }
                    }
                }
            }
        }

        // Display all transactions of Account
        public void ViewTransactionsofCustomer()
        {
            IList<Models.Transaction.Transaction> transactions;
            string customerID;
            string accountID;
            int index=0;

            customerID = DisplayAndSelectCustomerID();
            accountID = DisplayAndSelectAccountOfCustomer(customerID);
            transactions = _transactionService.GetTransactionsByAccountID(accountID);
            foreach(Models.Transaction.Transaction transaction in transactions)
            {
                Console.WriteLine(index + " " + transaction.ID + " " + (transaction.PayeeID ?? "------------------------------------") + " " + (transaction.PayorID ?? "-------------------------------------" + " ") + transaction.Amount + " " + transaction.Type);
                index++;
            }

        }


        // Displays all UserID's and Prompt user to pick one userID
        private string DisplayAndSelectCustomerID()
        {
            int usersCount;
            int index = 0;
            int selectedUserIndex;
            IList<Models.User.User> customers = _userService.GetAllCustomersOfBank(_userID, _bankID);

            usersCount = customers.Count;
            if (usersCount == 0)
            {
                return null;
            }
            else
            {
                while (true)
                {
                    index = 0;
                    Console.WriteLine("Press Number corresponding to the user");
                    while (index < usersCount)
                    {
                        Console.WriteLine((index + 1) + " " + customers[index].ID + " " + customers[index].Name);
                        index++;
                    }

                        if (int.TryParse(Console.ReadLine(), out selectedUserIndex))
                        {
                            if (selectedUserIndex >= 1 && selectedUserIndex <= (usersCount))
                            {
                                return customers[selectedUserIndex - 1].ID;
                            }
                            Console.WriteLine("Enter between 1 and " + usersCount);
                        }
                }
            }
        }

        // Prompt staff to pick one of selected customer account.
        private string DisplayAndSelectAccountOfCustomer(string ID)
        {
            int accountsCount;
            int index = 0;
            int selectedAccountIndex;
            IList<String> accountsList = _accountService.GetAccountsbyUserID(ID);

            accountsCount = accountsList.Count;
            if (accountsCount == 0)
            {
                Console.WriteLine("Selected Customer don't  have any accounts");
                return null;
            }
            else
            {
                while (true)
                {
                    index = 0;
                    Console.WriteLine("Press Number corresponding to the account");
                    while (index < accountsCount)
                    {
                        Console.WriteLine((index + 1) + " " + accountsList[index]);
                        index++;
                    }
                    if (int.TryParse(Console.ReadLine(), out selectedAccountIndex))
                    {
                        if (selectedAccountIndex >= 1 && selectedAccountIndex <= (accountsCount))
                        {
                            return accountsList[selectedAccountIndex - 1];
                        }
                        Console.WriteLine("Enter between 1 and " + accountsCount);
                    }
                    Console.WriteLine("Enter Only numeric values");
                }
            }
        }

        // Display update operations of a customer.
        private int DisplayCustomerUpdateOperations()
        {
            int selectedOperation;

            while(true)
            {
                Console.WriteLine("Enter number corresponding to operation");
                Console.WriteLine("1. Update Customer Name");
                Console.WriteLine("2. Update Customer EmailID");
                Console.WriteLine("3. Update Customer MobileNumber");
                Console.WriteLine("4. Add a new account for the customer");
                Console.WriteLine("5. Delete the account for the customer");
                if (int.TryParse(Console.ReadLine(), out selectedOperation))
                {
                    if(selectedOperation>=1 && selectedOperation<=5)
                    {
                        return selectedOperation;
                    }
                    Console.WriteLine("Enter value between 1 to 5");
                }
                Console.WriteLine("Enter only numeric values");
            }
        }

        // Add a Exchange Rate to a bank
        public void AddExchangeRateToBank()
        {
            string name;
            float rate;
            bool IsExist;
            
            while(true)
            {
                Console.WriteLine("Enter name of the exchange");
                name = Console.ReadLine();
                // Reading the exchage rate and if non-numeric values was entered then system prompts user to enter the value again. 
                while (true)
                {
                    Console.WriteLine("Enter rate of the exchange in terms of INDIAN RUPEE");
                    if (float.TryParse(Console.ReadLine(), out rate))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Enter only numeric values");
                        continue;
                    }

                }
                IsExist = _exchangeRateService.CheckExchangeRateExists(_bankID, name);
                if(!IsExist)
                {
                    _exchangeRateService.AddExchangeRate(_bankID, name, rate);
                    break;
                }
                else
                {
                    Console.WriteLine("Exchange Rate already exists");
                    break;
                }

            }
        }


    }
}
