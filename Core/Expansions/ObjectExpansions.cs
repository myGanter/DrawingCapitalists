﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Models;

namespace Core.Expansions
{
    public static class ObjectExpansions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNull(this object obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj is not null;
        }

        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static UserObjectContainer CreateContainer(this User us, object obj, string requestId = null)
        {
            return new UserObjectContainer(us, obj) { RequestId = requestId };
        }

        public static UserStruct CreateUserStruct(this User user)
        {
            return new UserStruct(user.Name, user.FingerPrint);
        }
    }
}
