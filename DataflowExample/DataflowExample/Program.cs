using DataflowExample.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataflowExample
{
	class Program
	{
		static void Main(string[] args)
		{
			DataflowJobQueue.Instance.RegisterHandler<SendEmailEventHandler>(async job => await job.ExecuteAsync());
			DataflowJobQueue.Instance.RegisterHandler<SendReportEventHandler>(async job => await job.ExecuteAsync());
			DataflowJobQueue.Instance.RegisterHandler<WriteHistoryEventHandler>(async job => await job.ExecuteAsync());

			Run().ConfigureAwait(false);

			Console.ReadKey();
		}

		static async Task Run()
		{
			for (var i = 0; i < 1000; i++)
			{
				var obj = QueueMessage.CreateQueueMessage(QueueNameCollection.SendEmail, new object());
				await DataflowJobQueue.Instance.Enqueue(EventHandlerFactory.GetEventHandlerInstance(obj));

				obj = QueueMessage.CreateQueueMessage(QueueNameCollection.SendReport, new object());
				await DataflowJobQueue.Instance.Enqueue(EventHandlerFactory.GetEventHandlerInstance(obj));

				obj = QueueMessage.CreateQueueMessage(QueueNameCollection.WriteHistory, new object());
				await DataflowJobQueue.Instance.Enqueue(EventHandlerFactory.GetEventHandlerInstance(obj));
			}
		}
	}
}
