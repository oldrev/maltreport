using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using NVelocity.Runtime;
using NVelocity.App;
using NVelocity.App.Events;
using NVelocity;
using NVelocity.Context;

namespace Bravo.Reporting
{
	internal sealed class VelocityTextTemplateEngine : ITextTemplateEngine
	{
		private string logTag;
		private VelocityEngine engine;
		private IDictionary<Type, IRenderFilter> filters = new Dictionary<Type, IRenderFilter> ();

		public VelocityTextTemplateEngine (string logTag)
		{
			if (string.IsNullOrEmpty (logTag)) {
				throw new ArgumentNullException ("logTag");
			}
			
			this.LogTag = logTag;
			
			this.engine = new VelocityEngine ();
			this.engine.Init ();
		}

		public string LogTag {
			get { return this.logTag; }
			private set { this.logTag = value; }
		}

		public void Evaluate (IDictionary<string, object> context, TextReader input, TextWriter output)
		{
			if (context == null) {
				throw new ArgumentNullException ("context");
			}
			
			if (input == null) {
				throw new ArgumentNullException ("input");
			}
			
			
			if (output == null) {
				throw new ArgumentNullException ("output");
			}
			
			var vctx = this.CreateVelocityContext (context);
			
			//执行渲染
			var successed = this.engine.Evaluate (vctx, output, "VelocityTextTemplateEngine", input);
			output.Flush ();
			
			if (!successed) {
				throw new TemplateException ("文本模板渲染失败");
			}
		}

		public void RegisterFilter (Type t, IRenderFilter filter)
		{
			if (t == null) {
				throw new ArgumentNullException ("t");
			}
			
			if (filter == null) {
				throw new ArgumentNullException ("filter");
			}
			
			if (this.filters.ContainsKey (t)) {
				throw new ArgumentException ("Duplicated type: " + t.Name);
			}
			
			this.filters[t] = filter;
		}

		public void Reset ()
		{
			this.filters.Clear ();
		}

		private VelocityContext CreateVelocityContext (IDictionary<string, object> context)
		{
			Debug.Assert (context != null);
			
			var vctx = new VelocityContext ();
			EventCartridge eventCart = new EventCartridge ();
			eventCart.ReferenceInsertion += this.OnReferenceInsertion;
			vctx.AttachEventCartridge (eventCart);
			
			//添加转义工具
			vctx.Put (VelocityEscapeTool.DefaultKey, new VelocityEscapeTool ());
			
			foreach (var item in context) {
				vctx.Put (item.Key, item.Value);
			}
			return vctx;
		}

		void OnReferenceInsertion (object sender, ReferenceInsertionEventArgs e)
		{
			if (e.OriginalValue != null) {
				this.DoFilter (e);
			} else {
				e.NewValue = string.Empty;
			}
		}

		private void DoFilter (ReferenceInsertionEventArgs e)
		{
			var t = e.OriginalValue.GetType ();
			IRenderFilter filter = null;
			var hasFilter = this.filters.TryGetValue (t, out filter);
			if (hasFilter) {
				Debug.Assert (filter != null);
				e.NewValue = filter.Filter (e.OriginalValue);
			}
		}
		
		
	}
}
