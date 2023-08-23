using ServiceStack;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ArmBenchmarks.ServiceModel;

namespace BenchmarkApp
{
    class Program
    {
        private static readonly string baseUrl = "https://localhost:5001"; // Replace with your ServiceStack API URL
        private static readonly int totalRequests = 20000; // Number of total requests
        private static SemaphoreSlim semaphore = new(Environment.ProcessorCount / 2);
        
        static async Task Main(string[] args)
        {
            // Initialize the JsonHttpClient
            var url = args.Length > 0 ? args[0] : baseUrl;
            var totalReqs = args.Length > 1 ? int.Parse(args[1]) : totalRequests;
            var client = new JsonHttpClient(url);
            Console.WriteLine($"Making {totalReqs} requests to {url}");
            await client.PostAsync(new Authenticate("credentials")
            {
                UserName = "admin@email.com",
                Password = "p@55wOrd"
            });

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("Seeding database...");
            // Seed 25 bookings 
            for (var i = 0; i < 25; i++)
            {
                // Example of CreateBooking request
                await client.PostAsync(new CreateBooking
                {
                    Name = "John Doe",
                    RoomType = RoomType.Single,
                    RoomNumber = 101,
                    Cost = 100.00M + i,
                    BookingStartDate = DateTime.Now
                });
            }

            Console.WriteLine("Seeding complete");
            Console.WriteLine($"Making {totalReqs*5} requests...");
            
            var tasks = new List<Task>();
            for (int i = 0; i < totalReqs; i++)
            {
                tasks.Add(MakeRequest(client, i));
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();
            cancellationTokenSource.Cancel();
            Console.WriteLine($"Total time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            
        }
        
        static async Task MakeRequest(JsonHttpClient client, long i)
        {
            await semaphore.WaitAsync(); // Wait for an available slot

            try
            {
                var queryAll = new QueryBookings { Take = 25};
                await client.GetAsync(queryAll);
                    
                var queryCombined = new QueryBookings { RoomType = RoomType.Double, BookingStartDateGreaterThan = new DateTime(2023, 6, 1), Take = 25 };
                await client.GetAsync(queryCombined);
                    
                var queryByStartDate = new QueryBookings { BookingStartDateGreaterThan = new DateTime(2023, 1, 1), Take = 25 };
                await client.GetAsync(queryByStartDate);
                    
                var queryByRoomType = new QueryBookings { RoomType = RoomType.Suite, Take = 25};
                await client.GetAsync(queryByRoomType);
                    
                var queryById = new QueryBookings { Id = 15 };
                await client.GetAsync(queryById);
            }
            finally
            {
                semaphore.Release(); // Release the slot when done
            }
            
            if((i * 5) % 1000 == 0 && i != 0)
                Console.WriteLine($"Completed {i*5} requests");
        }
    }
}
