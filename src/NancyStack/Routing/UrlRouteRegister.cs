using Nancy.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NancyStack.Routing
{

    public interface IUrlRouteRegister
    {
        void Register(Type type, string route);

        string For<TModel>(TModel routeModel);

        string For<TModel>();
    }

    public class UrlRouteRegister : IUrlRouteRegister
    {
        private readonly Regex paramRegex = new Regex(@"{(.*?)}", RegexOptions.Compiled);
        private readonly Dictionary<Type, string> routeList = new Dictionary<Type, string>();

        public string For<TModel>()
        {
            var modelType = typeof(TModel);

            if (!routeList.ContainsKey(modelType))
            {
                return "#";
            }

            var foundRoute = routeList[modelType];

            return foundRoute;
        }

        public string For<TModel>(TModel routeModel)
        {
            var modelType = typeof(TModel);

            if (!routeList.ContainsKey(modelType))
            {
                throw new RouteCouldNotBeFoundException(modelType);
            }

            var foundRoute = routeList[modelType];
            var propertiesList = GetPropertiesList<TModel>(routeModel, modelType, string.Empty);

            var queryStringBuilder = new StringBuilder();
            foreach (var prop in propertiesList)
            {
                var match = paramRegex.Match(foundRoute);

                if (match.Success)
                {
                    foundRoute = foundRoute.Replace(string.Format("{{{0}}}", prop.Name.ToLower()), HttpUtility.UrlEncode(prop.Value));
                }
                else
                {
                    queryStringBuilder.Append(HttpUtility.UrlEncode(prop.Name));
                    queryStringBuilder.Append("=");
                    queryStringBuilder.Append(HttpUtility.UrlEncode(prop.Value));
                    queryStringBuilder.Append("&");
                }
            }

            var queryString = queryStringBuilder.ToString().TrimEnd('&');

            return queryString.Length > 0 ?
                string.Format("{0}?{1}", foundRoute, queryString) :
                foundRoute;
        }

        public class PropertyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private static readonly Type[] valueTypes = new Type[]
        {
            typeof(String),
            typeof(Decimal),
            typeof(Boolean),
            typeof(long),
            typeof(Int32),
            typeof(Double),
        };

        private static ConcurrentDictionary<Type, object> typeDefaults = new ConcurrentDictionary<Type, object>();

        private object GetDefault(Type type)
        {
            return type.IsValueType ? typeDefaults.GetOrAdd(type, Activator.CreateInstance) : null;
        }

        private List<PropertyValue> GetPropertiesList<TModel>(TModel routeModel, Type modelType, string prefix)
        {
            var properties = modelType.GetProperties().Where(x => x.CanRead).ToArray();

            var propertiesList = new List<PropertyValue>();

            foreach (var property in properties)
            {
                var name = prefix + property.Name;
                var value = property.GetValue(routeModel, null);

                if (value != null)
                {
                    var valueType = value.GetType();
                    if (valueTypes.Contains(valueType) && !value.Equals(GetDefault(valueType)))
                    {
                        propertiesList.Add(new PropertyValue() { Name = name, Value = value.ToString() });
                    }
                    else if (valueType.IsArray)
                    {
                        //foreach (var v in value)
                        //{
                        //}
                    }
                    else
                    {
                        propertiesList.AddRange(GetPropertiesList(value, value.GetType(), name + "."));
                    }
                }
            }

            return propertiesList;
        }

        public void Register(Type type, string route)
        {
            if (!routeList.ContainsKey(type))
            {
                routeList.Add(type, route);
            }
        }
    }

}
