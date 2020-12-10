namespace AsyncAwait
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a postcode:");
            string inputPostcode = Console.ReadLine();

            try
            {
                Console.WriteLine("Getting Postcode information...");
                new Postcode(inputPostcode).WriteToConsole();
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
}
    }
}
