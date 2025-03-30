using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace ExternalBase
{
	static internal class Connector
	{
		static readonly int PADDING = 16;
		static readonly string CONNECTION_STRING =
			ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
		static SqlConnection connection;

		static Connector()
		{
			Console.WriteLine(CONNECTION_STRING);
			connection = new SqlConnection(CONNECTION_STRING);
		}

		public static void Select(string fields, string tables, string condition = "")
		{
			string cmd = $"SELECT {fields} FROM {tables}";
			if (condition != "") cmd += $" WHERE {condition}";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					Console.Write(reader.GetName(i).PadRight(PADDING));
				}
			}
			Console.WriteLine();

			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					Console.Write(reader[i].ToString().PadRight(PADDING));
				}
				Console.WriteLine();
			}

			connection.Close();
		}

		public static int Count(string table)
		{
			int count = 0;
			string cmd = $"SELECT COUNT(*) FROM {table}";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			count = Convert.ToInt32(command.ExecuteScalar());
			connection.Close();
			return count;
		}

		public static int ReturnID(string fields, string table, string condition)
		{
			string cmd = $"SELECT {fields} FROM {table} WHERE {condition}";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			try
			{
				object result = command.ExecuteScalar();
				connection.Close();
				return Convert.ToInt32(result);
			}
			catch (Exception)
			{
				connection.Close();
				return 0;
			}
		}

		public static int ReturnDisciplineID(string discipline_name)
		{
			return ReturnID("discipline_id", "Disciplines", $"discipline_name=N'{discipline_name}'");
		}

		public static int ReturnTeacherID(string last_name)
		{
			return ReturnID("teacher_id", "Teachers", $"last_name=N'{last_name}'");
		}
	}
}