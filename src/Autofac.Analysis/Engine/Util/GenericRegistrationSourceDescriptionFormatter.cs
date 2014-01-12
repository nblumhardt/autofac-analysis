using System.Text;
using Sprache;

namespace Autofac.Analysis.Engine.Util
{
    static class GenericRegistrationSourceDescriptionFormatter
    {
        static readonly Parser<string> TypeName =
            from simpleName in TypeNameParser.TypeSimpleName
            from argCount in TypeNameParser.GenericArgumentCount
            select FormatGenericTypeName(simpleName, argCount);

        static readonly Parser<string> GenericRegistrationSourceDescription =
            from component in TypeName
            from providing in Parse.String(" providing ").Text()
            from firstService in TypeName
            from remaining in Parse.String(", ").Then(comma => TypeName.Select(tn => comma + tn)).Many()
            select component + providing + firstService + string.Concat(remaining);

        public static bool TryFormat(string description, out string genericDescription)
        {
            var d = GenericRegistrationSourceDescription.TryParse(description);
            if (d.WasSuccessful)
            {
                genericDescription = d.Value;
                return true;
            }

            genericDescription = null;
            return false;
        }

        static readonly string[] ArgNames = { "T", "U", "V", "W", "X", "Y", "Z" };

        static string FormatGenericTypeName(string simpleName, int argCount)
        {
            var result = new StringBuilder();
            result.Append(simpleName);
            result.Append("<");
            var first = true;
            for (var i = 0; i < argCount; ++i)
            {
                if (first)
                    first = false;
                else
                    result.Append(", ");

                if (i > ArgNames.Length - 2)
                {
                    result.Append(ArgNames[ArgNames.Length - 1]);
                    result.Append(i - (ArgNames.Length - 1));
                }
                else
                {
                    result.Append(ArgNames[i]);
                }
            }
            result.Append(">");
            return result.ToString();
        }
    }
}
