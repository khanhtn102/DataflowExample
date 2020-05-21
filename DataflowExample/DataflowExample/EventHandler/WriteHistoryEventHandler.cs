using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataflowExample.EventHandler
{
	public class WriteHistoryEventHandler : IEventHandler
	{
		private QueueMessage _queueMessage { get; }

		public WriteHistoryEventHandler(QueueMessage queueMessage)
		{
			_queueMessage = queueMessage;
		}

		public void Execute()
		{
			Task.Delay(500);
			Console.WriteLine($"RUN - Job name: WriteHistory - Queue name: {_queueMessage.QueueName} - Thread ID: {Thread.CurrentThread.ManagedThreadId}");
		}
	}
}
