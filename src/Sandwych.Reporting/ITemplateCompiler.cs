using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Sandwych.Reporting
{
    public abstract class AbstractTemplateCompiler<TDocument>
        where TDocument : IDocument, new()
    {
        public static readonly Regex OutputStatementPattern = new Regex(
            @"^\s*\{\{\s*(?:[^\{\}]*(?!\}\}))\s*(?:\}\})\s*$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public abstract Task<ITemplate> CompileAsync(TDocument doc, CancellationToken ct = default);
    }
}
