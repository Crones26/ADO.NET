using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Data.SqlClient;
using System.Configuration;


namespace AcademyDataSet
{
	public partial class MainForm : Form
	{
		readonly string CONNECTION_STRING = "";
		SqlConnection connection;
		DataSet GroupsRelatedData;
		List<string> tables;
		List<string> commands;

		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
			connection = new SqlConnection(CONNECTION_STRING);
			Console.WriteLine(CONNECTION_STRING);

			tables = new List<string>();
			GroupsRelatedData = new DataSet(nameof(GroupsRelatedData));
			//LoadGroupsRelatedData();
			Check();
			//Загружаем направления из Базы в ComboBox:

			//1) Направления обучения уже загружены в таблицу в DataSet, и эту таблицу мы указываем как истоник данных:
			cbDirections.DataSource = GroupsRelatedData.Tables["Directions"];
			//2) Из множества полей таблицы нужно указать какое поле буде отображаться в выпадающем списке, 
			cbDirections.DisplayMember = "direction_name";
			//3) и какое поле возвращаться при выборе элемента ComboBox
			cbDirections.ValueMember = "direction_id";

			cbGroups.DataSource = GroupsRelatedData.Tables["Groups"];
			cbGroups.DisplayMember = "group_name";
			cbGroups.ValueMember = "group_id";
		}
		public void AddTable(string table, string columns)
		{
			string[] separated_columns = columns.Split(',');
			GroupsRelatedData.Tables.Add(table);
			for (int i = 0; i < separated_columns.Length; i++)
				GroupsRelatedData.Tables[table].Columns.Add(separated_columns[i]);
			GroupsRelatedData.Tables[table].PrimaryKey =
				new DataColumn[] { GroupsRelatedData.Tables[table].Columns[separated_columns[0]] };
			tables.Add($"{table},{columns}");
		}
		public void AddRelation(string name, string child, string parent)
		{
			GroupsRelatedData.Relations.Add
				(
				name,
				GroupsRelatedData.Tables[parent.Split(',')[0]].Columns[parent.Split(',')[1]],
				GroupsRelatedData.Tables[child.Split(',')[0]].Columns[child.Split(',')[1]]
				);
		}
		public void Load()
		{
			string[] tables = this.tables.ToArray();
			for (int i = 0; i < tables.Length; i++)
			{
				string columns = "";
				DataColumnCollection column_collection = GroupsRelatedData.Tables[tables[i].Split(',')[0]].Columns;
				foreach (DataColumn column in column_collection)
				{
					columns += $"[{column.ColumnName}],";
				}
				columns = columns.Remove(columns.LastIndexOf(','));
				Console.WriteLine(columns);
				//Console.WriteLine(GroupsRelatedData.Tables[tables[i].Split(',')[0]].Columns.ToString());
				string cmd = $"SELECT {columns} FROM {tables[i].Split(',')[0]}";
				SqlDataAdapter adapter = new SqlDataAdapter(cmd, connection);
				adapter.Fill(GroupsRelatedData.Tables[tables[i].Split(',')[0]]);
			}
		}
		void LoadGroupsRelatedData()
		{
			Console.WriteLine(nameof(GroupsRelatedData));
			//1) Создаем 'DataSet':
			//Перенесли в конструктор.

			//2) Добавляем талицы в 'DataSet':
			const string dsTable_Directions = "Directions";
			const string dst_col_direction_id = "direction_id";
			const string dst_col_direction_name = "direction_name";
			GroupsRelatedData.Tables.Add(dsTable_Directions);
			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_id, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_name, typeof(string));
			GroupsRelatedData.Tables[dsTable_Directions].PrimaryKey =
				new DataColumn[] { GroupsRelatedData.Tables[dsTable_Directions].Columns[dst_col_direction_id] };

			const string dsTable_Groups = "Groups";
			const string dst_Groups_col_group_id = "group_id";
			const string dst_Groups_col_group_name = "group_name";
			const string dst_Groups_col_direction = "direction";
			GroupsRelatedData.Tables.Add(dsTable_Groups);
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_id, typeof(int));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_name, typeof(string));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_direction, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Groups].PrimaryKey =
				new DataColumn[] { GroupsRelatedData.Tables[dsTable_Groups].Columns[dst_Groups_col_group_id] };

			//3) Строим связи между таблицами:
			string dsRelation_GroupsDirections = "GroupsDirections";
			GroupsRelatedData.Relations.Add
				(
				dsRelation_GroupsDirections,  //Relation name
				GroupsRelatedData.Tables["Directions"].Columns["direction_id"], //Parent field (Primary key)
				GroupsRelatedData.Tables["Groups"].Columns["direction"]         //Child field  (Foreign key)
				);

			//4) Загружаем данные в таблицы:
			string directions_cmd = "SELECT * FROM Directions";
			string groups_cmd = "SELECT * FROM Groups";
			SqlDataAdapter directionsAdapter = new SqlDataAdapter(directions_cmd, connection);
			SqlDataAdapter groupsAdapter = new SqlDataAdapter(groups_cmd, connection);

			connection.Open();
			directionsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Directions]);
			groupsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Groups]);
			connection.Close();

			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Directions].Rows)
			{
				Console.WriteLine($"{row[dst_col_direction_id]}\t{row[dst_col_direction_name]}");
			}
			Console.WriteLine("\n--------------------------------------------------\n");
			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Groups].Rows)
			{
				Console.WriteLine
(
$"{row[dst_Groups_col_group_id]}\t{row[dst_Groups_col_group_name]}\t{row.GetParentRow(dsRelation_GroupsDirections)[dst_col_direction_name]}"
);
			}
			Console.WriteLine("\n==================================\n");
		}
		void Print(string table)
		{
			Console.WriteLine("\n------------------------------------\n");
			Console.WriteLine(table);
			string relation_name = "No relation";
			string parent_table_name = "";
			string parent_column_name = "";
			int parent_index = -1;
			if (hasParents(table))
			{
				relation_name = GroupsRelatedData.Tables[table].ParentRelations[0].RelationName;
				parent_table_name = GroupsRelatedData.Tables[table].ParentRelations[0].ParentTable.TableName;
				parent_column_name = parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1) + "_name";
				Console.WriteLine(parent_table_name);
				//DataColumn parent_column = GroupsRelatedData.Tables[parent_table_name].Columns["direction_name"];
				//Console.WriteLine(parent_column.ColumnName);
				parent_index =
					GroupsRelatedData.Tables[table].Columns.
					IndexOf(parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1));
				Console.WriteLine(parent_index);
			}
			foreach (DataRow row in GroupsRelatedData.Tables[table].Rows)
			{
				for (int i = 0; i < row.ItemArray.Length; i++)
				{
					if (i == parent_index)
					{
						DataRow parent_row = row.GetParentRow(relation_name);
						Console.Write(parent_row[parent_column_name]);
						//Console.Write(row.GetParentRow(relation_name)[parent_column_name]);
						//GroupsRelatedData.Tables;
					}
					else
						Console.Write(row[i].ToString() + "\t");
					//Console.WriteLine(row[i].GetType());
				}
				//if (hasParents(table))
				//{
				//	DataRow parent_row = row.GetParentRow(GroupsRelatedData.Tables[table].ParentRelations[0].RelationName);
				//	//for (int j = 0; j < parent_row.ItemArray.Length; j++)
				//	//Console.Write(parent_row[j] + "\t");
				//	Console.Write(parent_row["direction_name"]);
				//}
				Console.WriteLine();
			}
			Console.WriteLine("\n------------------------------------\n");
		}
		bool hasParents(string table)
		{

			return GroupsRelatedData.Tables[table].ParentRelations.Count > 0;
			//for (int i = 0; i < GroupsRelatedData.Relations.Count; i++)
			//{
			//	if (GroupsRelatedData.Relations[i].ChildTable.TableName == table) return true;
			//}
			//return false;
		}
		void Check()
		{
			AddTable("Directions", "direction_id,direction_name");
			AddTable("Groups", "group_id,group_name,direction");
			AddTable("Students", "stud_id,last_name,first_name,middle_name,birth_date,group");
			AddRelation("GroupsDirections", "Groups,direction", "Directions,direction_id");
			AddRelation("StudentsGroups", "Students,group", "Groups,group_id");
			Load();
			Print("Directions");
			Print("Groups");
			Print("Students");
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		private void cbDirections_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Console.WriteLine(GroupsRelatedData.Tables["Directions"].ChildRelations);
			//DataRow row = GroupsRelatedData.Tables["Directions"].Rows.Find(cbDirections.SelectedValue);
			//cbGroups.DataSource = row.GetChildRows("GroupsDirections");
			//cbGroups.DisplayMember = "group_name";
			//cbGroups.ValueMember = "group_id";
			GroupsRelatedData.Tables["Groups"].DefaultView.RowFilter = $"direction={cbDirections.SelectedValue}";
		}
	}
}