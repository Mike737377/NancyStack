using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NancyStack.Sample.Handlers
{
    public class FormQueryHandler : IHandler<FormQueryModel, FormViewModel>
    {
        public FormViewModel Execute(HandlerContext context, FormQueryModel message)
        {
            return new FormViewModel();
        }
    }

    public class FormInputHandler : IHandler<FormInputModel, FormResponseModel>
    {
        public FormResponseModel Execute(HandlerContext context, FormInputModel message)
        {
            return new FormResponseModel()
            {
                Name = message.Name
            };
        }
    }

    public class FormQueryModel { }

    public class FormViewModel
    {
        public string Name { get; set; }
    }

    public class FormInputModel
    {
        public string Name { get; set; }
    }

    public class FormResponseModel
    {
        public string Name { get; set; }
    }
}
