using System;
using System.Threading.Tasks;

namespace ApigeeSDK.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new ApigeeClientOptions(
                Environment.GetEnvironmentVariable("APIGEE_EMAIL"),
                Environment.GetEnvironmentVariable("APIGEE_PASSWORD"),
                Environment.GetEnvironmentVariable("APIGEE_ORGNAME"),
                "test");

            var client = new ApigeeClient(options);

            var developers = await client.GetDevelopers();

            Console.WriteLine("Developers list:");
            foreach (var developer in developers)
            {
                Console.WriteLine($"Developer name: {developer.FirstName} {developer.LastName}");
                Console.WriteLine($"Developer status: {developer.Status}");
            }
        }
    }
}
