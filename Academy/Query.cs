using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
	class Query
	{
		public string Columns { get; }
		public string Tables { get; }
		public string Condition { get; set; }
		public string Group_by { get; }
		public Query(string columns, string tables, string condition = "", string group_by = "")
		{
			Columns = columns;
			Tables = tables;
			Condition = condition;
			Group_by = group_by;
		}
		public Query(Query other)
		{
			this.Columns = other.Columns;
			this.Tables = other.Tables;
			this.Condition = other.Condition;
			this.Group_by = other.Group_by;
		}
	}
}