using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Malt.Reporting
{
	public interface ITemplate : ICloneable
	{
		void Save (Stream outStream);
		
		void Save (string path);

		void Load (Stream inStream);
		
		void Load (string path);

		byte[] GetBuffer ();

		ITemplate Compile ();
		
		void Render (IDictionary<string, object> context);
	}
}
