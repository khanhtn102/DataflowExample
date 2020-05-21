using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataflowExample.EventHandler
{
	public class SendEmailEventHandler : IEventHandler
	{
		private QueueMessage _queueMessage { get; }

		public SendEmailEventHandler(QueueMessage queueMessage)
		{
			_queueMessage = queueMessage;
		}

		public void Execute()
		{
			Task.Delay(2000);
			Console.WriteLine($"RUN - Job name: SendEmail - Queue name: {_queueMessage.QueueName} - Thread ID: {Thread.CurrentThread.ManagedThreadId}");
		}
	}
}
