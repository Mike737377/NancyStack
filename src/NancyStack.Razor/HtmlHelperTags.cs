using FubuCore.Reflection;
using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NancyStack.Razor
{
    public static class HtmlHelperTags
    {

        public static IDisposableHtmlString BeginForm<TModel, TInputModel>(this HtmlHelpers<TModel> helper, TInputModel inputModel)
        {
            return BeginForm(helper, inputModel, false);
        }

        public static IDisposableHtmlString BeginForm<TModel, TInputModel>(this HtmlHelpers<TModel> helper, TInputModel inputModel, bool multipartFormData)
        {
            var token = helper.RenderContext.GetCsrfToken();
            var form = new FormTag<TInputModel>(inputModel);
            form.Children.Add(new AntiForgeryTokenTag(token.Key, token.Value));
            form.NoClosingTag();

            if (multipartFormData)
            {
                form.Attr("enctype", "multipart/form-data");
            }

            return form;
        }

        public static IHtmlString EndForm<TModel>(this HtmlHelpers<TModel> helper)
        {
            return helper.Raw("</form>");
        }

        public static string IdFor<TModel, TProperty>(this HtmlHelpers<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return ReflectionHelper.GetAccessor(expression).FieldName;
        }

        public static IHtmlString Input<TModel, TProperty>(this HtmlHelpers<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return HtmlConventions.Instance.ApplyConventions((TModel)helper.Model, ReflectionHelper.GetAccessor(expression));
        }

        //public static HtmlTag Display<TModel, TProperty>(this HtmlHelpers<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        //{
        //    var tag = new TagGenerator(HtmlConventionFactory.HtmlConventions);
        //    return tag.GenerateDisplayFor(helper.ViewContext, expression);
        //}
        public static IHtmlString Label<TModel, TProperty>(this HtmlHelpers<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return new LabelTag(ReflectionHelper.GetAccessor(expression).FieldName, ReflectionHelper.GetAccessor(expression).FieldName);
        }
    }

}