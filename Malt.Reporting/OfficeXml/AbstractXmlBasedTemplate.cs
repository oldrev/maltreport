//作者：李维
//创建时间：2010-09-09
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Malt.Reporting.OfficeXml
{
	public abstract class AbstractXmlBasedTemplate : ITemplate
	{
		private byte[] data;

		public void Load (Stream inStream)
		{
			if (inStream == null) {
				throw new ArgumentNullException ("inStream");
			}

			var size = (int)inStream.Length;
			this.data = new byte[size];
			var nread = inStream.Read (data, 0, size);

			if (nread != size) {
				throw new IOException ();
			}
		}

		public void Save (Stream outStream)
		{
			Debug.Assert (this.data != null);

			if (outStream == null) {
				throw new ArgumentNullException ("outStream");
			}

			outStream.Write (this.data, 0, this.data.Length);
		}

		public abstract void Compile ()
;
		public abstract IDocument Render (IDictionary<string, object> context);

        #region IDocument 成员


		public void Save (string path)
		{
			using (var fs = File.Open(path, FileMode.Create, FileAccess.Write)) {
				this.Save (fs);
			}
		}

		public void Load (string path)
		{
			using (var fs = File.OpenRead(path)) {
				this.Load (fs);
			}
		}

		public byte[] GetBuffer ()
		{
			return this.data;
		}

        #endregion

        #region ICloneable 成员

		public virtual object Clone ()
		{
			var o = (AbstractXmlBasedTemplate)Activator.CreateInstance (this.GetType ());
			o.PutBuffer (this.data);
			return o;
		}

        #endregion

		internal XmlDocument GetXmlDocument ()
		{
			Debug.Assert (this.data != null);

			var xmldoc = new XmlDocument ();
			using (var ms = new MemoryStream(this.data, false)) {
				xmldoc.Load (ms);
			}
			return xmldoc;
		}

		internal void PutBuffer (byte[] buffer)
		{
			Debug.Assert (buffer != null);

			this.data = buffer;
		}

       
	}
}
