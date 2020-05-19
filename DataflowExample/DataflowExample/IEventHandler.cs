using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataflowExample
{
	public interface IEventHandler
	{
		Task<bool> ExecuteAsync();
	}
}
