using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface ITemplate<TDocument>
        where TDocument : IDocument
    {
        Task<TDocument> RenderAsync(TemplateContext context);

        TDocument Render(TemplateContext context);
    }
}