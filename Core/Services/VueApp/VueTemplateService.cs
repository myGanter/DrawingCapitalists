using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;

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
            return "_" + templateName;
        }

        public string GetComponentPath(string normalizeTemplateName)
        {
            return $"~/Views/{VueDir}/{normalizeTemplateName}.cshtml";
        }

        public bool TemplateExist(string normalizeTemplateName)
        {
            var componentPath = $"{Env.ContentRootPath}\\Views\\{VueDir}\\{normalizeTemplateName}.cshtml";
            
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
