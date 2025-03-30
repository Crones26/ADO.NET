using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalBase
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Connector.Select("*", "Disciplines");
			Console.WriteLine("---------------------------------");
			Console.WriteLine(Connector.ReturnDisciplineID("JavaScript"));
			Console.WriteLine(Connector.ReturnDisciplineID("NodeJS"));

			Connector.Select("*", "Teachers");
			Console.WriteLine("---------------------------------");
			Console.WriteLine(Connector.ReturnTeacherID("Ковтун"));
			Console.WriteLine(Connector.ReturnTeacherID("Глазунов"));
			Console.WriteLine("---------------------------------");
			Console.WriteLine(Connector.Count("Teachers"));
			Console.WriteLine("---------------------------------");

			Connector.Select("*", "Students");
			Console.WriteLine("---------------------------------");
			Console.WriteLine(Connector.Count("Students"));
		}
	}
}