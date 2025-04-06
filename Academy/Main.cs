using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;

namespace Academy
{
	public partial class Main : Form
	{

		Connector connector;
		public Main()
		{
			InitializeComponent();

			connector = new Connector
				(
					ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString
				);
			//dgv - DataGridView
			dgvStudents.DataSource = connector.Select
					(
						"last_name,first_name,middle_name,birth_date,group_name,direction_name",
						"Students,Groups,Directions",
						"[group]=group_id AND direction=direction_id"
					);
			toolStripStatusLabelCount.Text = $"Количество студентов:{dgvStudents.RowCount - 1}.";
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex)
			{
				case 0:
					dgvStudents.DataSource = connector.Select
				(
					"last_name,first_name,middle_name,birth_date,group_name,direction_name",
					"Students,Groups,Directions",
					"[group]=group_id AND direction=direction_id"
				);
					toolStripStatusLabelCount.Text = $"Количество студентов: {dgvStudents.RowCount - 1}.";
					break;

				case 1:
					dgvGroups.DataSource = connector.Select("*", "Groups");
					toolStripStatusLabelCount.Text = $"Количество групп: {dgvGroups.RowCount - 1}.";
					break;

				case 2:
					//dgvDirections.DataSource = connector.Select
					//	(
					//	"direction_name,COUNT(DISTINCT group_id) AS N'Количество групп' , COUNT(stud_id) AS N'Количество студентов'", 
					//	"Students,Groups,Directions",
					//	"[group]=group_id AND direction=direction_id",
					//	"direction_name"
					//	);

					dgvDirections.DataSource = connector.Select
						(
							"direction_name,COUNT(DISTINCT group_id) AS N'Количество групп' , COUNT(stud_id) AS N'Количество студентов'",
							"Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
							"",
							"direction_name"
						);
					toolStripStatusLabelCount.Text = $"Количество направлений: {dgvDirections.RowCount - 1}.";
					break;

				case 3:
					dgvDisciplines.DataSource = connector.Select("*", "Disciplines");
					toolStripStatusLabelCount.Text = $"Количество дисциплин: {dgvDisciplines.RowCount - 1}.";
					break;

				case 4:
					dgvTeachers.DataSource = connector.Select("*", "Teachers");
					toolStripStatusLabelCount.Text = $"Количество преподавателей: {dgvTeachers.RowCount - 1}.";
					break;
			}
		}
	}
}