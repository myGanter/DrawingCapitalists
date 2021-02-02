using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using Core.Expansions;

namespace Core.Services.VueApp
{
    public class VueTemplateService
    {
        private const string VueDir = "VueComponents";

        private readonly HashSet<string> Cache;

        private readonly object Locker;

        private readonly IWebHostEnvironment Env;

        public VueTemplateService(IWebHostEnvironment env)
        {
            Cache = new HashSet<string>();
            Locker = new object();
            Env = env;
        }

        public string NormalizeTemplateName(string templateName)
        {
            var sb = new StringBuilder();
            sb.Append('_');

            if (!templateName.IsNullOrEmpty())
            {
                sb.Append(char.ToUpper(templateName[0]));
                sb.Append(templateName.Skip(1).Select(x => char.ToLower(x)).ToArray());
            }

            return sb.ToString();
        }

        public string GetComponentPath(string normalizeTemplateName)
        {
            return $"~/Views/{VueDir}/{normalizeTemplateName}.cshtml";
        }

        public bool TemplateExist(string normalizeTemplateName)
        {            
            var componentPath = Path.GetFullPath($"{Env.ContentRootPath}/Views/{VueDir}/{normalizeTemplateName}.cshtml");

            lock (Locker)
            {
                if (Cache.Contains(normalizeTemplateName))
                    return true;
                else
                {
                    var isExists = File.Exists(componentPath); 
                    
                    if (isExists)
                        Cache.Add(normalizeTemplateName);

                    return isExists;
                }
            }
        }
    }
}
