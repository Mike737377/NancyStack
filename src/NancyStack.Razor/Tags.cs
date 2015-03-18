using HtmlTags;
using NancyTags = Nancy.ViewEngines.Razor;
using NancyStack.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Razor
{
    public interface IDisposableHtmlString : IDisposable
    {
    }

    public class AntiForgeryTokenTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public AntiForgeryTokenTag(string tokenKey, string tokenValue)
            : base("input")
        {
            this.Attr("type", "hidden")
                .Attr("name", tokenKey)
                .Attr("value", tokenValue);
        }
    }

    public class CheckboxTag : HtmlTags.CheckboxTag, NancyTags.IHtmlString
    {
        public CheckboxTag(string name, bool value)
            : base(value)
        {
            this.Attr("id", name);
            this.Attr("name", name);
        }
    }

    public class DivTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public DivTag()
            : base("div")
        { }
    }

    public class NavTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public NavTag()
            : base("nav")
        { }
    }

    public class FileUploadTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public FileUploadTag(string name)
            : base("input")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Attr("type", "file");
        }
    }

    public class FormTag<TModel> : HtmlTags.FormTag, NancyTags.IHtmlString, IDisposableHtmlString
    {
        public FormTag()
            : base(UrlRoute.For<TModel>())
        { }

        public FormTag(TModel model)
            : base(UrlRoute.For(model))
        { }

        public void Dispose()
        {
        }

        //private readonly HtmlHelpers<TModel> _helper;

        //public FormTag(HtmlHelpers<TModel> helper, string url)
        //    : base(url)
        //{
        //    _helper = helper;
        //}

        //public void Dispose()
        //{
        //    //_helper.RenderContext.Context.

        //    //using (var writer = new StreamWriter(_helper.RenderContext.Context.Response))
        //    //{
        //    //    writer.Write(_helper.Raw("</form>"));
        //    //}
        //}
    }

    public class LabelTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public LabelTag(string text, string forId)
            : base("label")
        {
            base.Text(text)
                .Attr("for", forId);
        }

        public new LabelTag Text(string text)
        {
            base.Text(text);
            return this;
        }
    }

    public class LinkTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public LinkTag(string href)
            : base("a")
        {
            this.Attr("href", href);
        }
    }

    public class ListItemTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public ListItemTag()
            : base("li")
        { }
    }

    public class PasswordTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public PasswordTag(string name)
            : base("input")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Attr("type", "password");
        }
    }

    public class SpanTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public SpanTag(string text)
            : base("span")
        {
            this.Text(text);
        }
    }

    public class SubmitTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public SubmitTag(string text)
            : base("input")
        {
            this.Attr("value", text)
                .Attr("type", "submit");
        }
    }

    public class TextAreaTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public TextAreaTag(string name, string value)
            : base("textarea")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Text(value);
        }
    }

    public class TextboxTag : HtmlTags.TextboxTag, NancyTags.IHtmlString
    {
        public TextboxTag(string name, string value)
            : base(name, value)
        {
            this.Attr("id", name);
        }
    }

    public class UnorderedListTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public UnorderedListTag()
            : base("ul")
        { }
    }

    public class ValidationMessageTag : HtmlTags.HtmlTag, NancyTags.IHtmlString
    {
        public ValidationMessageTag(string text)
            : base("span")
        {
            this.Text(text).AddClass("error");
        }
    }
}
