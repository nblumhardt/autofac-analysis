using System;

namespace Autofac.Analysis.Display
{
    static class IdDisplay
    {
        public static string MakeShortId(string longGuidId)
        {
            if (longGuidId == null) throw new ArgumentNullException(nameof(longGuidId));
            return longGuidId.Substring(0, 3) + ".." + longGuidId.Substring(longGuidId.Length - 3, 3);
        }        
    }
}