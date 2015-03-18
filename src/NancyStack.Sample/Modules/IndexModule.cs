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

            GetRoute<FormQueryModel>("/form")
                .Returning<FormViewModel>()
                .OnSuccess(x => View["Form", x]);

            PostRoute<FormInputModel>("/form")
                .Returning<FormResponseModel>()
                .OnSuccess(x => View["FormResult", x]);
        }
    }
}