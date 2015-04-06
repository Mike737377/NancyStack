using Nancy;
using NancyStack.Modules;
using NancyStack.Sample.Handlers;

namespace NancyStack.Sample.Modules
{
    public class IndexModule : NancyStackModule
    {
        public IndexModule()
        {
            AddRoute.Get<IndexQueryModel>("/")
                .Returning<IndexViewModel>()
                .OnSuccess(x => x.RenderView("Index"));

            AddRoute.Get<FormQueryModel>("/form")
                .Returning<FormViewModel>()
                .OnSuccess(x => x.RenderView("Form"));

            AddRoute.Post<FormInputModel>("/form")
                .Returning<FormResponseModel>()
                .OnValidationError<FormQueryModel>()
                .OnSuccess(x => x.RenderView("FormResult"));
        }
    }
}