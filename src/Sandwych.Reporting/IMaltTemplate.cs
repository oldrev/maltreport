using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public interface IMaltTemplate<TDocumentTemplate> where TDocumentTemplate : IDocumentTemplate
    {
    }



    public class MaltTemplate<TDocumentTemplate> : IMaltTemplate<TDocumentTemplate> where TDocumentTemplate : IDocumentTemplate
    {

    }

}
