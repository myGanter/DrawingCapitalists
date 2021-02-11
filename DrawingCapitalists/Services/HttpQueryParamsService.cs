using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace DrawingCapitalists.Services
{
    public class HttpQueryParamsService
    {
        private readonly HttpRequest Request;

        private IQueryCollection Query => Request.Query;

        private QueryString QueryString => Request.QueryString;

        public HttpQueryParamsService(IHttpContextAccessor httpContextAccessor)
        {
            Request = httpContextAccessor.HttpContext.Request;
        }

        public string GetParamValueOrNull(string param)
        {
            if (Query.TryGetValue(param, out StringValues strVal))
            {
                var val = strVal.ToString();
                return val;
            }
            else
                return null;
        }

        public string GetQueryStr(params string[] excludedValues)
        {
            excludedValues = excludedValues.Select(x => x.ToLower()).ToArray();

            var q = Query
                .Where(x => !excludedValues.Contains(x.Key.ToLower()))
                .Aggregate(new StringBuilder(), (acc, val) => 
                {
                    acc.Append(val.Key);
                    acc.Append('=');
                    acc.Append(val.Value.ToString());
                    acc.Append('&');
                    return acc;
                });

            if (q.Length > 0)
            {
                q.Remove(q.Length - 1, 1);
                return "?" + q.ToString();
            }

            return "";
        }
    }
}
