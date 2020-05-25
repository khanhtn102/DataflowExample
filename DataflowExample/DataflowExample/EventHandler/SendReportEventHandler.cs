using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataflowExample.EventHandler
{
	public class SendReportEventHandler : IEventHandler
	{
		private QueueMessage _queueMessage { get; }

		public SendReportEventHandler(QueueMessage queueMessage)
		{
			_queueMessage = queueMessage;
		}

		public void Execute()
		{
			Task.Delay(1000);
			Console.WriteLine($"RUN - Job name: SendReport - Queue name: {_queueMessage.QueueName} - Thread ID: {Thread.CurrentThread.ManagedThreadId}");

			// random for error
			Random random = new Random();
			var rdNum = random.Next(1, 6);
			if (rdNum % 2 == 0)
				throw new Exception("Send report error");
		}
	}
}
