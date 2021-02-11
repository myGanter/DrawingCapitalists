using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrawingCapitalists.Models;

using Core.Expansions;
using Core.Exceptions;

namespace DrawingCapitalists.Services
{
    public class VueModelCreatorService
    {
        private readonly HttpQueryParamsService ParamsService;

        private static readonly Dictionary<string, Func<VueModelCreatorService, object>> ModelCreator;

        static VueModelCreatorService()
        {
            ModelCreator = new Dictionary<string, Func<VueModelCreatorService, object>>();

            ModelCreator.Add("_Room", s => 
            {
                var prs = s.ParamsService;
                var strParam = prs.GetParamValueOrNull("id");

                if (!strParam.IsNullOrEmpty() && long.TryParse(strParam, out long id) && id > 0)
                {
                    return new RoomVueTemplateModel() { Id = id };
                }
                else
                    throw new ClientException($"Комнаты {strParam} не существует");
            });
        }

        public VueModelCreatorService(HttpQueryParamsService paramsService)
        {
            ParamsService = paramsService;
        }

        public object CreateModelObjectOrNull(string vueTemplateName)
        {
            if (ModelCreator.TryGetValue(vueTemplateName, out Func<VueModelCreatorService, object> fabric))
            {
                return fabric(this);
            }
            else
                return null;
        }
    }
}
