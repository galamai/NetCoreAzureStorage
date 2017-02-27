using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreAzureStorage
{
    class Program
    {
        static int _sended;
        static int _added;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            // the count parallel sending messages
            // if equally < 8 - work!
            var parallelTasks = 200;

            var tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(AddMessageAsync());

                while (tasks.Count >= parallelTasks)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                }
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\r\nOk");
            Console.ReadLine();
        }

        static async Task AddMessageAsync()
        {
            // new instance
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var client = account.CreateCloudQueueClient();
            var queue = client.GetQueueReference("netcoreazurestorage");

            Interlocked.Increment(ref _sended);
            Console.Write($"\r{_sended} {_added}");

            var message = Encoding.UTF8.GetString(new byte[1024]);
            await queue.AddMessageAsync(new CloudQueueMessage(message));

            Interlocked.Increment(ref _added);
            Console.Write($"\r{_sended} {_added}");
        }
    }
}