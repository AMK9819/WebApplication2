using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PracticeWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("testq")] string message, ILogger logger)
        {
            logger.LogInformation($"Processed queue message 5/22/2026 3:41PM: {message}");
        }
    }
}
