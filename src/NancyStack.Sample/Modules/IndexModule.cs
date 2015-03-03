using Nancy;
using NancyStack.Modules;
using NancyStack.Sample.Handlers;

namespace NancyStack.Sample.Modules
{
    public class IndexModule : NancyStackModule
    {
        public IndexModule()
        {
            GetRoute<IndexQueryModel>("/")
                .Returning<IndexViewModel>()
                .OnSuccess(x => View["Index", x]);
        }
    }
}