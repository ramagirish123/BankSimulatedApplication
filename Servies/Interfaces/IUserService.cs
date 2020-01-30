using BankApplication.Models.User;
using System.Collections.Generic;

namespace BankApplication.Services.Interfaces
{
    public interface IUserService
    {
        string AcceptUserRequestByID(string ID);
        string AddRequestedUser(string bankID, string name, string password, string eMail, string mobileNumber);
        string AddUser(string bankID, string name, string password, string eMail, string mobileNumber);
        string AuthenticateUser(string EmailID, string password, string bankID);
        bool CheckEMailAvailbility(string Email, string bankID);
        string DeclineUserRequestByID(string ID, string declineReason);
        string DeleteUserByID(string ID);
        IList<User> GetAllCustomersOfBank(string userID, string bankID);
        int GetRequestedUsersCount(string bankID);
        IList<User> GetRequestedUsersOfBank(string bankID);
        User GetUserByID(string userID);
        string UpdateCustomerEMailByID(string ID, string modifiedEMail);
        string UpdateCustomerMobileNumberByID(string ID, string modifiedMobileNumber);
        string UpdateCustomerNameByID(string ID, string modifiedname);
    }
}