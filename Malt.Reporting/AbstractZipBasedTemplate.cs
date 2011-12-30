//2010-09-02
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Malt.Reporting
{
	public abstract class AbstractZipBasedTemplate : ITemplate
	{

        #region Members of ITemplate

		public abstract void Save (Stream outStream)
;
		public abstract void Load (Stream inStream);

		public virtual void Save (string path)
		{
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException ("path");
			}

			using (var fs = File.Open(path, FileMode.CreateNew, FileAccess.Write)) {
				this.Save (fs);
			}
		}

		public virtual void Load (string path)
		{
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException ("path");
			}

			using (var fs = File.Open(path, FileMode.Open, FileAccess.Read)) {
				this.Load (fs);
			}
		}

		public abstract Stream GetEntryInputStream (string entryPath);

		public abstract Stream GetEntryOutputStream (string entryPath);

		public abstract string MainContentEntryPath { get; }

        #endregion

		public virtual byte[] GetBuffer ()
		{
			using (var ms = new MemoryStream()) {
				this.Save (ms);
				return ms.ToArray ();
			}
		}

		public string ToBase64String ()
		{
			return Convert.ToBase64String (this.GetBuffer ());
		}

		internal TextReader GetEntryTextReader (string entryPath)
		{
			if (string.IsNullOrEmpty (entryPath)) {
				throw new ArgumentNullException ("entryPath");
			}

			return new StreamReader (this.GetEntryInputStream (entryPath));
		}

		internal TextWriter GetEntryTextWriter (string entryPath)
		{
			if (string.IsNullOrEmpty (entryPath)) {
				throw new ArgumentNullException ("entryPath");
			}

			return new StreamWriter (this.GetEntryOutputStream (entryPath));
		}

		internal void WriteXmlEntry (string entryPath, XmlDocument xml)
		{
			if (xml == null) {
				throw new ArgumentNullException ("xml");
			}

			if (string.IsNullOrEmpty (entryPath)) {
				throw new ArgumentNullException ("entryPath");
			}

			using (var cos = this.GetEntryOutputStream(entryPath))
			using (var writer = new XmlTextWriter(cos, Encoding.UTF8)) {
				writer.Formatting = Formatting.None; //对于 Velocity 模板，最好格式化
				xml.WriteTo (writer);
			}
		}

		internal void ReadXmlEntry (string entryPath, XmlDocument xml)
		{
			using (var contentStream = this.GetEntryInputStream(entryPath)) {
				xml.Load (contentStream);
			}
		}

		internal void WriteMainContentXml (XmlDocument xml)
		{
			this.WriteXmlEntry (this.MainContentEntryPath, xml);
		}

		internal void ReadMainContentXml (XmlDocument xml)
		{
			this.ReadXmlEntry (this.MainContentEntryPath, xml);
		}

		public abstract ICollection<string> EntryPaths { get; }

		public abstract bool EntryExists (string entryPath);

		public abstract string AddImage (Image img);

		public abstract void Compile ();

		public abstract IDocument Render (IDictionary<string, object> context);

		protected static void CopyStream (Stream src, Stream dest)
		{
			if (src == null) {
				throw new ArgumentNullException ("src");
			}

			if (dest == null) {
				throw new ArgumentNullException ("dest");
			}

			var bufSize = 2048;
			var buf = new byte[bufSize];
			int nRead = 0;
			while ((nRead = src.Read(buf, 0, bufSize)) > 0) {
				dest.Write (buf, 0, nRead);
			}
		}

		public abstract object Clone ();

		public void CopyTo (AbstractZipBasedTemplate destDoc)
		{
			if (destDoc == null) {
				throw new ArgumentNullException ("destDoc");
			}

			foreach (var item in this.EntryPaths) {
				using (var inStream = this.GetEntryInputStream(item))
				using (var outStream = destDoc.GetEntryOutputStream(item)) {
					CopyStream (inStream, outStream);
				}
			}
		}
	}
}
