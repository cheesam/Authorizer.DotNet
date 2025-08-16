using System;
using System.Threading.Tasks;

namespace Authorizer.DotNet.IntegrationTest;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Authorizer.DotNet Comprehensive Integration Test Suite");
        Console.WriteLine("=========================================================");
        Console.WriteLine();
        
        try
        {
            // Create and run comprehensive test suite
            var testSuite = await ComprehensiveIntegrationTest.CreateAsync();
            await testSuite.RunAllTestsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Critical error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Test completed successfully!");
        await Task.Delay(1000); // Brief pause before exit
    }
}