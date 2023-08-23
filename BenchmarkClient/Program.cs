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
        private static readonly int totalRequests = 200000; // Number of total requests
        
        static async Task Main(string[] args)
        {
            // Initialize the JsonHttpClient
            var url = args.Length > 0 ? args[0] : baseUrl;
            var totalReqs = args.Length > 1 ? int.Parse(args[1]) : totalRequests;
            var client = new JsonHttpClient(url);
            await client.PostAsync(new Authenticate("credentials")
            {
                UserName = "admin@email.com",
                Password = "p@55wOrd"
            });

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            // Seed 1000 bookings 
            for (var i = 0; i < 1000; i++)
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
            
            // Use multiple threads to make requests
            long iterations = totalReqs;
            
            async void MakeRequest(int i)
            {
                while (Interlocked.Read(ref iterations) > 0)
                {
                    var queryAll = new QueryBookings();
                    await client.GetAsync(queryAll);
                    
                    var queryCombined = new QueryBookings { RoomType = RoomType.Double, BookingStartDateGreaterThan = new DateTime(2023, 6, 1) };
                    await client.GetAsync(queryCombined);
                    
                    var queryByStartDate = new QueryBookings { BookingStartDateGreaterThan = new DateTime(2023, 1, 1) };
                    await client.GetAsync(queryByStartDate);
                    
                    var queryByRoomType = new QueryBookings { RoomType = RoomType.Suite };
                    await client.GetAsync(queryByRoomType);
                    
                    var queryById = new QueryBookings { Id = 123 };
                    await client.GetAsync(queryById);
                    
                    Interlocked.Decrement(ref iterations);
                }
            }
            
            Parallel.For(0, totalRequests, MakeRequest);

            stopwatch.Stop();
            cancellationTokenSource.Cancel();
            Console.WriteLine($"Total time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            
        }
    }
}
