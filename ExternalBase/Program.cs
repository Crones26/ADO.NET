using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalBase
{
	class Program
	{
		static void Main(string[] args)
		{
			Connector.Select("*", "Disciplines");
		}
	}
}