using FubuCore;
using FubuCore.Reflection;
using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NancyStack.Razor
{
    public interface ITagBuilderConstructor
    {
        void BuildBy(Func<RequestData, IHtmlString> builder);
    }

    public class HtmlBuilder
    {
        public HtmlBuilder(Func<RequestData, bool> expression, TagBuilder builder)
        {
            Condition = expression;
            Builder = builder;
        }

        public TagBuilder Builder { get; private set; }
        public Func<RequestData, bool> Condition { get; private set; }
    }

    public class HtmlConventions
    {
        public static readonly HtmlConventions Instance = new HtmlConventions();
        private readonly List<HtmlBuilder> _inputBuilders = new List<HtmlBuilder>();

        public HtmlConventions()
        {
            InputFor(x => x.Accessor.PropertyType == typeof(string))
                .BuildBy(r => new TextboxTag(r.Accessor.FieldName, (r.Accessor.GetValue(r.Model) ?? string.Empty).ToString()));

            InputFor(x => x.Accessor.PropertyType == typeof(string) && x.Accessor.FieldName.EqualsIgnoreCase("password"))
                .BuildBy(r => new PasswordTag(r.Accessor.FieldName));

            //InputFor(x => x.Accessor.PropertyType == typeof(string) && x.Accessor.HasAttribute<TextAreaAttribute>())
            //    .BuildBy(r => new TextAreaTag(r.Accessor.FieldName, (r.Accessor.GetValue(r.Model) ?? string.Empty).ToString()));

            InputFor(x => x.Accessor.PropertyType == typeof(bool))
                .BuildBy(r => new CheckboxTag(r.Accessor.FieldName, (r.Accessor.GetValue(r.Model) as bool?) ?? false));

            InputFor(x => x.Accessor.PropertyType == typeof(DateTime))
                .BuildBy(r => new TextboxTag(r.Accessor.FieldName, (r.Accessor.GetValue(r.Model) ?? string.Empty).ToString()));

            InputFor(x => x.Accessor.PropertyType == typeof(DateTime?))
                .BuildBy(r => new TextboxTag(r.Accessor.FieldName, (r.Accessor.GetValue(r.Model) ?? string.Empty).ToString()));

            InputFor(x => x.Accessor.PropertyType == typeof(Nancy.HttpFile))
                .BuildBy(r => new FileUploadTag(r.Accessor.FieldName));
        }

        public IHtmlString ApplyConventions<T>(T model, Accessor accessor)
        {
            var req = new RequestData(model, accessor);
            var matchingBuilder = _inputBuilders.LastOrDefault(x => x.Condition(req));

            if (matchingBuilder == null)
            {
                throw new NoConventionFoundException();
            }

            var htmlTag = matchingBuilder.Builder.BuildTag(req);
            return htmlTag;
        }

        private ITagBuilderConstructor InputFor(Func<RequestData, bool> expression)
        {
            var tagBuilder = new TagBuilder();
            _inputBuilders.Add(new HtmlBuilder(expression, tagBuilder));
            return tagBuilder;
        }
    }

    public class NoConventionFoundException : Exception
    {
    }

    public class TagBuilder : ITagBuilderConstructor
    {
        private Func<RequestData, IHtmlString> builder;

        public void BuildBy(Func<RequestData, IHtmlString> builder)
        {
            this.builder = builder;
        }

        public IHtmlString BuildTag(RequestData requestData)
        {
            return this.builder(requestData);
        }
    }

    public class RequestData
    {
        public RequestData(object model, Accessor accessor)
        {
            Accessor = accessor;
            Model = model;
        }

        protected RequestData()
        {
        }

        public Accessor Accessor { get; protected set; }
        public object Model { get; protected set; }
    }

    public class RequestData<T> : RequestData
    {
        public RequestData(T model, Accessor accessor)
        {
            Accessor = accessor;
            Model = model;
        }

        public new T Model
        {
            get
            {
                return (T)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
    }
}
