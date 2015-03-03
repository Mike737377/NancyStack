using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NancyStack.Sample.Handlers
{
    public class IndexHandler : IHandler<IndexQueryModel, IndexViewModel>
    {
        public IndexViewModel Execute(HandlerContext context, IndexQueryModel message)
        {
            return new IndexViewModel { CurrentTime = DateTime.Now };
        }
    }

    public class IndexQueryModel { }

    public class IndexViewModel
    {
        public DateTime CurrentTime { get; set; }
    }


}