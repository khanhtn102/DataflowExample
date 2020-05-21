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
			var policy = Policy.Handle<Exception>().Retry(3);

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
