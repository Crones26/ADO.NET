using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Academy
{
	class Connector
	{
		readonly string CONNECTION_STRING;// = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
		SqlConnection connection;
		public Connector(string connection_string)
		{
			//CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
			CONNECTION_STRING = connection_string;
			connection = new SqlConnection(CONNECTION_STRING);
			AllocConsole();
			Console.WriteLine(CONNECTION_STRING);

		}
		~Connector()
		{
			FreeConsole();
		}
		public DataTable Select(string columns, string tables, string condition = "", string group_by = "")
		{
			DataTable table = null;

			string cmd = $"SELECT {columns} FROM {tables}";
			if (condition	!= "") cmd += $" WHERE {condition}";
			if (group_by	!= "") cmd += $" GROUP BY {group_by}";
			cmd += ";";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				//1) Создаем таблицу:
				table = new DataTable();
				table.Load(reader);
#if OLD
				//2) Добавляем в нее поля:
				for (int i = 0; i < reader.FieldCount; i++)
				{
					table.Columns.Add();
				}

				//3) Заполняем таблицу:
				while (reader.Read())
				{
					DataRow row = table.NewRow();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						row[i] = reader[i];
					}
					table.Rows.Add(row);
				}
#endif
			}

			reader.Close();
			connection.Close();
			return table;
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

	}
}
