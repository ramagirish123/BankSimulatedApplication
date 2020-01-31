using System;
using System.Collections.Generic;
using System.Text;
using BankApplication.Services;
using BankApplication.Models;
using System.IO;
using BankApplication.Data;
using System.Linq;

namespace BankApplication.UI
{
    public class App
    {
        private BankApplication.Models.Bank _currentBank;
        private Authentication.Login _userAuthentication;
        private Authentication.SignUp _userSignUp;
        private Bank _bankUI;
        private string _JsonFilePath;
        private Utility.UIStyling _UIStyling;
        private UIBuffer _UIBuffer;
       
        public App(string JsonFilePath)
        {
            _UIStyling = new Utility.UIStyling(100, 50);
            _UIBuffer = new UIBuffer();
            _userSignUp = new Authentication.SignUp(JsonFilePath, _UIStyling, _UIBuffer);
            _bankUI = new Bank(JsonFilePath, _UIStyling, _UIBuffer);
            _JsonFilePath = JsonFilePath;
            _userAuthentication = new Authentication.Login(JsonFilePath, _UIStyling, _UIBuffer);
            Console.SetWindowSize(100, 50);
            Console.SetBufferSize(100, 50);
        }

         public void Run()
        {
            Models.User.User AuthenticatedUser;
            string PressedKey;
         
            // UI for user logging or exiting from the application.
            while(true)
            {
                try
                {
                    _currentBank = _bankUI.DisplayAndSelectBank();

                }
                catch
                {
                    Console.WriteLine("Failed to Log JSON FILE ERROR MESSAGE: ");
                    return;
                }
                while (true)
                {
                    Console.WriteLine(_UIStyling.PadBoth("Welcome to ", '*'));
                    Console.WriteLine(_UIStyling.PadBoth(_currentBank.Name, '*'));
                    Console.WriteLine("  Press 1 to Login");
                    Console.WriteLine("  Press 2 to Signup");
                    Console.WriteLine("  Press 3 to Goback");
                    Console.WriteLine("  Press 4 to Exit\n");
                    PressedKey = Console.ReadLine();
                    Console.WriteLine("\n");
                    if(PressedKey=="3")
                    {
                        Console.Clear();
                        break;
                    }
                    switch (PressedKey)
                    {
                        case "1":
                            // Iterate the loop till user enter current correct details.
                            while (true)
                            {
                                AuthenticatedUser = _userAuthentication.Authenticate(_currentBank.ID);
                                if (AuthenticatedUser == null)
                                {
                                    Console.WriteLine("Wrong Credentials");
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("WELCOME TO " + AuthenticatedUser.Name);
                                    break;
                                }
                            }
                            // If the user is staff then staff UI will be displayed or else customer UI will be displayed.
                            if (AuthenticatedUser.Type == Models.User.UserType.StaffMember)
                            {
                                Console.Clear();
                                new BankApplication.UI.User.StaffMember.StaffMemberUI(AuthenticatedUser.ID,AuthenticatedUser.Name, _currentBank.ID, _JsonFilePath, _UIStyling, _UIBuffer).run();
                                Console.Clear();
                            }
                            else
                            {
                                Console.Clear();
                                new BankApplication.UI.User.Customer.CustomerUI(AuthenticatedUser.ID,AuthenticatedUser.Name, _currentBank.ID, _JsonFilePath, _UIStyling, _UIBuffer).run();
                                Console.Clear();
                            }
                            break;
                        case "2":
                            _userSignUp.SignUpRequest(_currentBank.ID, _JsonFilePath);
                            break;
                        case "4":
                            return;
                        default:
                            _UIStyling.ChangeForegroundForErrorMessage();
                            Console.WriteLine("Pressed incorrect key\n");
                            _UIStyling.RestoreForegroundColor();
                            break;

                    }
                }
               

            }
          
            
        }
    }
}
