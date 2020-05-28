using DataflowExample.EventHandler;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataflowExample
{
	public class DataflowJobQueueErrorHandling
	{
		private BufferBlock<QueueMessage> _jobs;
		private static DataflowJobQueueErrorHandling _instance = null;
		private static readonly object lockObject = new object();

		public static DataflowJobQueueErrorHandling Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (lockObject)
					{
						if (_instance == null)
							_instance = new DataflowJobQueueErrorHandling();
					}
				}

				return _instance;
			}
		}

		public DataflowJobQueueErrorHandling()
		{
			_jobs = new BufferBlock<QueueMessage>();
		}

		public void RegisterHandler(Action<QueueMessage> handleAction)
		{
			// Delay and retry when exception occurred
			var policy = Policy.Handle<Exception>().RetryAsync(3, async (ex, retryCount) =>
			{
				// Write log
				Console.WriteLine($"Error: {ex.Message} - Retry times: {retryCount}");
				// Wait 5s and run again
				await Task.Delay(5000);
			});

			// Handle after retry had still error
			var fallBackpolicy = Policy.Handle<Exception>()
				.FallbackAsync
				(
					fallbackAction: async cancellationtoken =>
					{
						// Move to bad queue
						//var obj = new QueueMessage();
						//await Enqueue(EventHandlerFactory.GetEventHandlerInstance(QueueNameCollection.BadQueue, obj));
						await Task.CompletedTask.ConfigureAwait(false);
					}, onFallbackAsync: async ex =>
					{
						// Write log exception
						Console.WriteLine($"Error: {ex.Message}");
						await Task.CompletedTask.ConfigureAwait(false);
					}
				);

			var retryWithFallback = fallBackpolicy.WrapAsync(policy);

			// create the action block
			var actionBlock = new ActionBlock<QueueMessage>(async (job) =>
			{
				await retryWithFallback.ExecuteAsync(async () =>
				{
					handleAction(job);
					await Task.CompletedTask.ConfigureAwait(false);
				});
			});

			// Link with Predicate - only if a job is of type T
			_jobs.LinkTo(actionBlock, predicate: (job) => job.QueueName == "");
		}

		public async Task EnqueueAsync<T>(string queueName, T message)
		{
			var queueMessage = QueueMessage.CreateQueueMessage(queueName, message);
			var job = EventHandlerFactory.GetEventHandlerInstance(queueName, queueMessage);
			//await _jobs.SendAsync(job);
		}
	}
}
