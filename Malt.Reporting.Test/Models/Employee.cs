using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malt.Reporting.Test.Models
{
	public sealed class Employee
	{

		public Employee ()
		{
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public string Phone { get; set; }

		public string Mobile { get; set; }

		public DateTime HireDate { get; set; }

		public DateTime BirthDate { get; set; }

		public string Note { get; set; }

	}
}
