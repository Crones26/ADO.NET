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

		Cache GroupsRelatedData;
		public MainForm()
		{
			InitializeComponent();
			AllocConsole();

			GroupsRelatedData = new Cache();
			GroupsRelatedData.AddTable("Directions", "direction_id,direction_name");
			GroupsRelatedData.AddTable("Groups", "group_id,group_name,direction");
			GroupsRelatedData.AddRelation("GroupsDirections", "Groups,direction", "Directions,direction_id");
			GroupsRelatedData.Load();
			GroupsRelatedData.Print("Directions");
			GroupsRelatedData.Print("Groups");

			//Загружаем направления из Базы в ComboBox:
			//1) Направления обучения уже загружены в таблицу в DataSet, и эту таблицу мы указываем как истоник данных:
			cbDirections.DataSource = GroupsRelatedData.Set.Tables["Directions"];
			//2) Из множества полей таблицы нужно указать какое поле буде отображаться в выпадающем списке, 
			cbDirections.DisplayMember = "direction_name";
			//3) и какое поле возвращаться при выборе элемента ComboBox
			cbDirections.ValueMember = "direction_id";

			cbGroups.DataSource = GroupsRelatedData.Set.Tables["Groups"];
			cbGroups.DisplayMember = "group_name";
			cbGroups.ValueMember = "group_id";
		}

		private void cbDirections_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Console.WriteLine(GroupsRelatedData.Tables["Directions"].ChildRelations);
			//DataRow row = GroupsRelatedData.Tables["Directions"].Rows.Find(cbDirections.SelectedValue);
			//cbGroups.DataSource = row.GetChildRows("GroupsDirections");
			//cbGroups.DisplayMember = "group_name";
			//cbGroups.ValueMember = "group_id";
			GroupsRelatedData.Set.Tables["Groups"].DefaultView.RowFilter = $"direction={cbDirections.SelectedValue}";
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}