using System;
using Bank_Simulated_Application.models;
using Newtonsoft.Json;

namespace Bank_Simulated_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Bank HyderabadBank = new Bank("HyderabadBank");
            string json = JsonConvert.SerializeObject(HyderabadBank, Formatting.Indented);
            Console.Write(json);
        }
    }
}
