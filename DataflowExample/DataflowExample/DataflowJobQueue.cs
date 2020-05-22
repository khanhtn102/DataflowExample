using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataflowExample
{
	public class DataflowJobQueue
	{
		private BufferBlock<IEventHandler> _jobs;
		private static DataflowJobQueue _instance = null;
		private static readonly object lockObject = new object();

		public static DataflowJobQueue Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (lockObject)
					{
						if (_instance == null)
							_instance = new DataflowJobQueue();
					}
				}

				return _instance;
			}
		}

		public DataflowJobQueue()
		{
			_jobs = new BufferBlock<IEventHandler>();
		}

		public void RegisterHandler<T>(Action<T> handleAction) where T : IEventHandler
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

						await Task.CompletedTask.ConfigureAwait(false);
					}, onFallbackAsync: async ex =>
					{
						// Write log exception
						Console.WriteLine($"Error: {ex.Message}");
						await Task.CompletedTask.ConfigureAwait(false);
					}
				);

			// We have to have a wrapper to work with IJob instead of T
			Action<IEventHandler> actionWrapper = (job) => handleAction((T)job);

			// create the action block that executes the handler wrapper
			var actionBlock = new ActionBlock<IEventHandler>((job) => actionWrapper(job));

			// Link with Predicate - only if a job is of type T
			_jobs.LinkTo(actionBlock, predicate: (job) => job is T);
		}

		public async Task Enqueue(IEventHandler job)
		{
			await _jobs.SendAsync(job);
		}
	}
}
