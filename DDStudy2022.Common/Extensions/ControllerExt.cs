using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Extensions
{
    public static class ControllerExt
    {
        public static string? ControllerAction<T>(this IUrlHelper urlHelper, string name, object? arg)
            where T : ControllerBase
        {
            var controllerType = typeof(T);
            if (controllerType.GetMethod(name) == null)
                return null;

            var controllerName = controllerType.Name.Replace("Controller", string.Empty);
            var action = urlHelper.Action(name, controllerName,  arg);
            return action;
        }
    }
}
