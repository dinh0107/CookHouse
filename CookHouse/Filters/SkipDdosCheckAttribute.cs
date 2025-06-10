using System;

namespace CookHouse.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipDdosCheckAttribute : Attribute { }
}