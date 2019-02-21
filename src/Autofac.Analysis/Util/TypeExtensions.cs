using System;

namespace Autofac.Analysis.Util
{
    static class TypeExtensions
    {
        public static bool IsDisposable(this Type type) => typeof(IDisposable).IsAssignableFrom(type);
    }
}
