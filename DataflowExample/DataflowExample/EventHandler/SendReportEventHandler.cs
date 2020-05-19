using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public async Task<bool> ExecuteAsync()
		{
			await Task.Delay(1000);
			Console.WriteLine($"RUN - Job name: SendReport - Queue name: {_queueMessage.QueueName}");
			return true;
		}
	}
}
