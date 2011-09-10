using System;
using System.Collections.Generic;
using System.Text;

namespace Malt.Reporting.Demo
{
	public class Employee
	{
		public Employee (string name, string address, int age)
		{
			this.Name = name;
			this.Address = address;
			this.Age = age;
		}

		public string Name { get; set; }

		public string Address { get; set; }

		public int Age { get; set; }
	}
}
