using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataflowExample.EventHandler
{
	public static class EventHandlerFactory
	{
		public static IEventHandler GetEventHandlerInstance(string queueName, QueueMessage queueMessage)
		{
			switch (queueName)
			{
				case QueueNameCollection.SendEmail:
					return new SendEmailEventHandler(queueMessage);
				case QueueNameCollection.SendReport:
					return new SendReportEventHandler(queueMessage);
				case QueueNameCollection.WriteHistory:
					return new WriteHistoryEventHandler(queueMessage);
				default: throw new NotImplementedException();
			}
		}
	}
}
