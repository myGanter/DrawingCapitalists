using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public readonly struct UserStruct : IEquatable<UserStruct>
    {
        public readonly string FingerPrint;

        public readonly string Name;

        public UserStruct(string name, string fingerPrint)
        {
            Name = name;
            FingerPrint = fingerPrint;
        }

        public bool Equals(UserStruct other)
        {
            return FingerPrint == other.FingerPrint && Name == other.Name;
        }
    }
}
