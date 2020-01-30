using System;
using System.IO;
using Bank_Simulated_Application.Models;
using Newtonsoft.Json;

namespace Bank_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Bank HyderabadBank = new Bank("HyderabadBank");
            string json= JsonConvert.SerializeObject(HyderabadBank, Formatting.Indented);
            Console.WriteLine(json);
            File.WriteAllText(@"C:\Users\satyasai.k\source\repos\Bank Simulated Application\Bank Application\JSONFile\bank.json", json);
        }
    }
}
