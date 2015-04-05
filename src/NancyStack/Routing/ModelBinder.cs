using Nancy;
using Nancy.ModelBinding;
using NancyStack.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.ModelBinding
{
    public static class ModelBinderExtensions
    {
        public static TModel Bind<TModel>(this NancyStackModule module)
        {
            var model = module.BindTo(Activator.CreateInstance<TModel>());

            model = Up(module, model);

            return model;
        }

        public static TModel BindAndValidate<TModel>(this NancyStackModule module)
        {
            var model = module.BindToAndValidate(Activator.CreateInstance<TModel>());

            model = Up(module, model);

            return model;
        }

        private static TModel Up<TModel>(this NancyStackModule module, TModel model)
        {
            var modelType = model.GetType();
            var properties = modelType.GetProperties();

            //files do not get bound automatically in nancy - do this here manually
            if (module.Request.Files.Count() > 0)
            {
                var fileProperties = properties.Where(x => x.PropertyType == typeof(HttpFile));
                fileProperties.Each(x => x.SetValue(model, module.Request.Files.FirstOrDefault(v => v.Key == x.Name), null));
            }

            modelType.GetProperties().Where(x => x.PropertyType == typeof(string))
                .Each(x =>
                    {
                        if ((x.GetValue(model, null) as string) == null)
                        {
                            x.SetValue(model, string.Empty, null);
                        }
                    });

            return model;
        }
    }
}