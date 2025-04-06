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
			dgvStudents.DataSource = connector.Select("*", "Students");
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex)
			{
				case 0:
					dgvStudents.DataSource = connector.Select("*", "Students");
					toolStripStatusLabelCount.Text = $"Количество студентов: {dgvStudents.RowCount - 1}.";
					break;

				case 1:
					dgvGroups.DataSource = connector.Select("*", "Groups");
					toolStripStatusLabelCount.Text = $"Количество групп: {dgvGroups.RowCount - 1}.";
					break;

				case 2:
					dgvDirections.DataSource = connector.Select("*", "Directions");
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