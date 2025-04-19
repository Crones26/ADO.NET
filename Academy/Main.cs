﻿using System;
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
using System.Collections;

namespace Academy
{
	public partial class Main : Form
	{
		Connector connector;

		Dictionary<string, int> d_directions;
		Dictionary<string, int> d_groups;

		DataGridView[] tables;

		Query[] queries = new Query[]
		{
			new Query(
				"last_name,first_name,middle_name,birth_date,group_name,direction_name",
				"Students JOIN Groups ON Students.[group] = Groups.group_id " +
				"JOIN Directions ON Groups.direction = Directions.direction_id"
			),
			new Query(
				"group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
				"Groups,Directions",
				"direction=direction_id"
			),
			new Query(
				"direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
				"Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
				"",
				"direction_name"
			),
			new Query("*", "Disciplines"),
			new Query("*", "Teachers")
		};

		string[] status_messages = new string[]
		{
			"Количество студентов: ",
			"Количество групп: ",
			"Количество направлений: ",
			"Количество дисциплин: ",
			"Количество преподавателей: ",
		};

		public Main()
		{
			InitializeComponent();

			tables = new DataGridView[]
			{
				dgvStudents,
				dgvGroups,
				dgvDirections,
				dgvDisciplines,
				dgvTeachers
			};

			connector = new Connector(
				ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString
			);

			d_directions = connector.GetDictionary("*", "Directions");
			d_groups = connector.GetDictionary("group_id,group_name", "Groups");

			cbStudentsGroup.Items.AddRange(d_groups.Select(g => g.Key).ToArray());
			cbGroupsDirection.Items.AddRange(d_directions.Select(d => d.Key).ToArray());
			cbStudentsDirection.Items.AddRange(d_directions.Select(d => d.Key).ToArray());

			cbStudentsGroup.Items.Insert(0, "Все группы");
			cbStudentsDirection.Items.Insert(0, "Все направления");

			cbStudentsGroup.SelectedIndex = 0;
			cbStudentsDirection.SelectedIndex = 0;

			dgvStudents.DataSource = connector.Select(
				"last_name,first_name,middle_name,birth_date,group_name,direction_name",
				"Students,Groups,Directions",
				"[group]=group_id AND direction=direction_id"
			);

			toolStripStatusLabelCount.Text = $"Количество студентов: {CountRecordsInDGV(dgvStudents)}.";

			LoadPage(0);
		}

		private void LoadPage(int i, Query query = null)
		{
			if (query == null)
				query = queries[i];

			tables[i].DataSource = connector.Select(query.Columns, query.Tables, query.Condition, query.Group_by);

			if (i == 0)
				toolStripStatusLabelCount.Text = $"Количество студентов: {CountRecordsInDGV(tables[i])}.";
			else
				toolStripStatusLabelCount.Text = status_messages[i] + CountRecordsInDGV(tables[i]);
		}

		private int CountRecordsInDGV(DataGridView dgv)
		{
			return dgv.RowCount == 0 ? 0 : dgv.RowCount - 1;
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = tabControl.SelectedIndex;

			if (index == 2 && cbShowEmptyDirections != null)
			{
				cbShowEmptyDirections_CheckedChanged(cbShowEmptyDirections, EventArgs.Empty);
			}
			else
			{
				LoadPage(index);
			}
		}

		private void cbGroupsDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			dgvGroups.DataSource = connector.Select(
				"group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
				"Groups,Directions",
				$"direction=direction_id AND direction = N'{d_directions[cbGroupsDirection.SelectedItem.ToString()]}'"
			);

			toolStripStatusLabelCount.Text = $"Количество групп: {CountRecordsInDGV(dgvGroups)}.";
		}

		private void cbStudentsDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbStudentsDirection.SelectedItem == null)
				return;

			string selectedDirection = cbStudentsDirection.SelectedItem.ToString();
			int directionIndex = cbStudentsDirection.SelectedIndex;

			// Получаем группы по направлению (или все)
			Dictionary<string, int> filteredGroups = connector.GetDictionary(
				"group_id,group_name",
				"Groups",
				directionIndex == 0 ? "" : $"direction = {d_directions[selectedDirection]}"
			);

			cbStudentsGroup.Items.Clear();
			cbStudentsGroup.Items.Add("Все группы");
			cbStudentsGroup.Items.AddRange(filteredGroups.Select(g => g.Key).ToArray());
			cbStudentsGroup.SelectedIndex = 0;

			// Обновляем таблицу
			Query query = new Query(queries[0]);
			if (directionIndex > 0)
			{
				query.Condition = $"direction = {d_directions[selectedDirection]}";
			}

			LoadPage(0, query);
			toolStripStatusLabelCount.Text = $"Количество студентов: {CountRecordsInDGV(dgvStudents)}.";
		}

		private void cbStudentsGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbStudentsGroup.SelectedItem == null)
				return;

			string selectedGroup = cbStudentsGroup.SelectedItem.ToString();
			string selectedDirection = cbStudentsDirection.SelectedItem?.ToString();

			Query query = new Query(queries[0]);

			if (cbStudentsDirection.SelectedIndex > 0 && !string.IsNullOrEmpty(selectedDirection))
			{
				query.Condition = $"direction = {d_directions[selectedDirection]}";
			}

			if (selectedGroup != "Все группы")
			{
				if (!string.IsNullOrEmpty(query.Condition))
					query.Condition += " AND ";

				query.Condition += $"group_name = N'{selectedGroup}'";
			}

			LoadPage(0, query);
			toolStripStatusLabelCount.Text = $"Количество студентов: {CountRecordsInDGV(dgvStudents)}.";
		}

		private void cbShowEmptyDirections_CheckedChanged(object sender, EventArgs e)
		{
			Query query;

			if (cbShowEmptyDirections.Checked)
			{
				query = new Query(
					"direction_name, COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
					"Directions LEFT JOIN Groups ON direction=direction_id LEFT JOIN Students ON [group]=group_id",
					"",
					"direction_name"
				);
			}
			else
			{
				query = queries[2]; 
			}

			LoadPage(2, query);
		}
	}
}