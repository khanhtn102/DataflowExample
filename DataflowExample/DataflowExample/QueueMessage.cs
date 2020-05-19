using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataflowExample
{
	public class QueueMessage
	{
		public Guid Id { get; internal set; }
		public string QueueName { get; set; }
		public string Payload { get; internal set; }
		public DateTime CreatedDate { get; internal set; }
		public static QueueMessage CreateQueueMessage<T>(string queueName, T message)
		{
			var queueMessage = new QueueMessage();
			queueMessage.Id = Guid.NewGuid();
			queueMessage.QueueName = queueName;
			queueMessage.Payload = JsonConvert.SerializeObject(message);
			queueMessage.CreatedDate = DateTime.UtcNow;
			return queueMessage;
		}

	}
}
