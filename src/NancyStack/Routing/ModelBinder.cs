using Nancy;
using Nancy.ModelBinding;
using NancyStack.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.ModelBinding
{
    public interface IModelBinder
    {
        TModel Bind<TModel>(INancyModule module);
        TModel BindAndValidate<TModel>(INancyModule module);
    }

    public class ModelBinder : IModelBinder
    {
        public TModel Bind<TModel>(INancyModule module)
        {
            var model = module.BindTo(Activator.CreateInstance<TModel>());

            model = ApplyMissingBindings(module, model);

            return model;
        }

        public TModel BindAndValidate<TModel>(INancyModule module)
        {
            var model = module.BindToAndValidate(Activator.CreateInstance<TModel>());

            model = ApplyMissingBindings(module, model);

            return model;
        }

        private TModel ApplyMissingBindings<TModel>(INancyModule module, TModel model)
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

    public static class ModelBinderExtensions
    {
        public static TModel Bind<TModel>(this INancyModule module)
        {
            return new ModelBinder().Bind<TModel>(module);
        }

        public static TModel BindAndValidate<TModel>(this INancyModule module)
        {
            return new ModelBinder().BindAndValidate<TModel>(module);
        }
    }
}