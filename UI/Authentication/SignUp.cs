using System;
using System.Collections.Generic;
using System.Text;
using BankApplication.Services;

namespace BankApplication.UI.Authentication
{
    class SignUp
    {
        private UserService _userService;
        private string _bankID;
        private Utility.UIStyling _UIStyling;
        private Utility.PasswordHider _passwordHider;
        private Utility.DataValidations _dataValidations;

        public SignUp(string JsonFilePath, Utility.UIStyling UIStyling)
        {
            _userService = new UserService("",JsonFilePath);
            _UIStyling = UIStyling;
            _passwordHider = new Utility.PasswordHider();
            _dataValidations = new Utility.DataValidations();
        }
        
        // Send request to join into the bank.
        public void SignUpRequest(string bankID,string JsonFilePath)
        {
            string name;
            string password;
            string phoneNumber;
            string EMail;
            bool IsEMailAvailable;
            string ID;
           
            
            _bankID = bankID;

            Console.WriteLine();
            Console.Write("  Enter your name: ");
            name = Console.ReadLine();
            Console.WriteLine();
            Console.Write("  Enter Password: ");
            password = _passwordHider.ReadPassword();
            Console.WriteLine();
            Console.Write("  Enter phone number: ");
            phoneNumber = Console.ReadLine();
            Console.WriteLine();
            while (true)
            {
                do
                {
                    Console.Write("  Enter EmailID: ");
                    EMail = Console.ReadLine();
                    if (_dataValidations.IsEMailValid(EMail))
                        break;
                    _UIStyling.ChangeForegroundForErrorMessage();
                    Console.WriteLine("  Enter valid Email ID");
                    _UIStyling.RestoreForegroundColor();
                } while (true);
                IsEMailAvailable = _userService.CheckEMailAvailbility(EMail, _bankID);
                if (IsEMailAvailable)
                {
                    ID = _userService.AddRequestedUser(_bankID, name, password, EMail, phoneNumber);
                    Console.WriteLine(ID + " is your Request-ID");
                    break;
                }
                else
                {
                    _UIStyling.ChangeForegroundForErrorMessage();
                    Console.WriteLine("  Email is not available.Please enter another EMail ID");
                    _UIStyling.RestoreForegroundColor();
                }

            }
        }
    }
}
