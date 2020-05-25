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
			DataflowJobQueue.Instance.RegisterHandler<SendEmailEventHandler>(job => job.Execute());
			DataflowJobQueue.Instance.RegisterHandler<SendReportEventHandler>(job => job.Execute());
			DataflowJobQueue.Instance.RegisterHandler<WriteHistoryEventHandler>(job => job.Execute());

			Run().ConfigureAwait(false);

			Console.ReadKey();
		}

		static async Task Run()
		{
			for (var i = 0; i < 100; i++)
			{
				var obj = new object();
				await DataflowJobQueue.Instance.EnqueueAsync(QueueNameCollection.SendEmail, obj);

				obj = new object();
				await DataflowJobQueue.Instance.EnqueueAsync(QueueNameCollection.SendReport, obj);

				obj = new object();
				await DataflowJobQueue.Instance.EnqueueAsync(QueueNameCollection.WriteHistory, obj);

				Console.WriteLine("End one create job");
			}
		}
	}
}
