using HtmlTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Tags
{
    public interface IDisposableHtmlString : IDisposable
    {
    }

    public class AntiForgeryTokenTag : HtmlTag
    {
        public AntiForgeryTokenTag(string tokenKey, string tokenValue)
            : base("input")
        {
            this.Attr("type", "hidden")
                .Attr("name", tokenKey)
                .Attr("value", tokenValue);
        }
    }

    public class CheckboxTag : HtmlTags.CheckboxTag
    {
        public CheckboxTag(string name, bool value)
            : base(value)
        {
            this.Attr("id", name);
            this.Attr("name", name);
        }
    }

    public class DivTag : HtmlTags.HtmlTag
    {
        public DivTag()
            : base("div")
        { }
    }

    public class NavTag : HtmlTags.HtmlTag
    {
        public NavTag()
            : base("nav")
        { }
    }

    public class FileUploadTag : HtmlTags.HtmlTag
    {
        public FileUploadTag(string name)
            : base("input")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Attr("type", "file");
        }
    }

    public class FormTag<TModel> : HtmlTags.FormTag, IDisposableHtmlString
    {
        private readonly HtmlHelpers<TModel> _helper;

        public FormTag(HtmlHelpers<TModel> helper, string url)
            : base(url)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            //_helper.RenderContext.Context.

            //using (var writer = new StreamWriter(_helper.RenderContext.Context.Response))
            //{
            //    writer.Write(_helper.Raw("</form>"));
            //}
        }
    }

    public class LabelTag : HtmlTags.HtmlTag
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

    public class LinkTag : HtmlTags.HtmlTag
    {
        public LinkTag(string href)
            : base("a")
        {
            this.Attr("href", href);
        }
    }

    public class ListItemTag : HtmlTags.HtmlTag
    {
        public ListItemTag()
            : base("li")
        { }
    }

    public class PasswordTag : HtmlTags.HtmlTag
    {
        public PasswordTag(string name)
            : base("input")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Attr("type", "password");
        }
    }

    public class SpanTag : HtmlTags.HtmlTag
    {
        public SpanTag(string text)
            : base("span")
        {
            this.Text(text);
        }
    }

    public class SubmitTag : HtmlTags.HtmlTag
    {
        public SubmitTag(string text)
            : base("input")
        {
            this.Attr("value", text)
                .Attr("type", "submit");
        }
    }

    public class TextAreaTag : HtmlTags.HtmlTag
    {
        public TextAreaTag(string name, string value)
            : base("textarea")
        {
            this.Attr("id", name);
            this.Attr("name", name);
            this.Text(value);
        }
    }

    public class TextboxTag : HtmlTags.TextboxTag
    {
        public TextboxTag(string name, string value)
            : base(name, value)
        {
            this.Attr("id", name);
        }
    }

    public class UnorderedListTag : HtmlTags.HtmlTag
    {
        public UnorderedListTag()
            : base("ul")
        { }
    }

    public class ValidationMessageTag : HtmlTags.HtmlTag
    {
        public ValidationMessageTag(string text)
            : base("span")
        {
            this.Text(text).AddClass("error");
        }
    }
}
