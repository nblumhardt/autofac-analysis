using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Engine.Application;
using Sprache;

namespace Autofac.Analysis.Engine.Util
{
    public static class TypeNameParser
    {
        static readonly Parser<char> Comma = Parse.Char(',');

        static readonly Parser<char> TypeSimpleNameChar =
            Parse.LetterOrDigit.Or(Parse.Char(c => "._-<>".Contains(c), "type name char"));

        static readonly Parser<string> ArrayModifiers =
            Parse.Char('[').Or(Parse.Char(']')).AtLeastOnce().Text();

        static readonly Parser<char> AssemblyNameChar =
            Parse.LetterOrDigit.Or(Parse.Char('.'));

        public static readonly Parser<string> TypeSimpleName =
            TypeSimpleNameChar.AtLeastOnce().Text();

        static readonly Parser<string> AssemblyName =
            AssemblyNameChar.AtLeastOnce().Text();

        static readonly Parser<string> NestedTypeName =
            Parse.Char('+').Then(_ => TypeSimpleName);

        static readonly Parser<TypeIdentifier> GenericArgument =
            from openDelim in Parse.Char('[')
                                                // ReSharper disable StaticFieldInitializersReferesToFieldBelow
            from argTypeName in Parse.Ref(() => CompleteTypeName)
                                                // ReSharper restore StaticFieldInitializersReferesToFieldBelow
            from closeDelim in Parse.Char(']')
            select argTypeName;

        static readonly Parser<IEnumerable<TypeIdentifier>> GenericArgumentList =
            from openDelimiter in Parse.Char('[')
            from firstArg in GenericArgument
            from remainingArgs in Comma.Token().Then(_ => GenericArgument).Many()
            from closeDelimiter in Parse.Char(']')
            select new[] {firstArg}.Concat(remainingArgs);

        static Parser<T> Attribute<T>(string name, Parser<T> value)
            where T : class
        {
            return
                (from leadingComman in Comma.Token()
                from n in Parse.String(name)
                from equ in Parse.Char('=')
                from val in value
                select val).Or(Parse.Return((T)null));
        }

        static readonly Parser<int> Integer =
            Parse.Digit.AtLeastOnce().Text().Select(int.Parse);

        static readonly Parser<char> Dot = Parse.Char('.');

        static readonly Parser<Version> Version =
            from major in Integer
            from dot1 in Dot
            from minor in Integer
            from dot2 in Dot
            from build in Integer
            from dot3 in Dot
            from rev in Integer
            select new Version(major, minor, build, rev);

        static readonly Parser<string> Culture =
            Parse.LetterOrDigit.Or(Parse.Char('-')).AtLeastOnce().Text();

        static readonly Parser<string> PublicKeyToken =
            Parse.LetterOrDigit.AtLeastOnce().Text();

        public static readonly Parser<int> GenericArgumentCount =
            from backtick in Parse.Char('`')
            from nargs in Parse.Digit.AtLeastOnce().Text()
            select int.Parse(nargs);

        static readonly Parser<TypeIdentifier> CompleteTypeName =
            from simpleName in TypeSimpleName
            from argCount in GenericArgumentCount.XOr(Parse.Return(0))
            from args in GenericArgumentList.Or(Parse.Return(Enumerable.Empty<TypeIdentifier>()))
            from nestedTypeName in NestedTypeName.XOr(Parse.Return(""))
            from mods in ArrayModifiers.XOr(Parse.Return(""))
            from comma in Comma.Token()
            from assemblyName in AssemblyName
            from version in Attribute("Version", Version)
            from culture in Attribute("Culture", Culture)
            from publicKeyToken in Attribute("PublicKeyToken", PublicKeyToken)
            let fullName = simpleName + (nestedTypeName == "" ? "" : ("+" + nestedTypeName))
            select new TypeIdentifier(fullName, assemblyName, version, culture, publicKeyToken, argCount, args, mods);

        public static TypeIdentifier ParseAssemblyQualifiedTypeName(string assemblyQualifiedTypeName)
        {
            if (assemblyQualifiedTypeName == null) throw new ArgumentNullException("assemblyQualifiedTypeName");
            return CompleteTypeName.Parse(assemblyQualifiedTypeName);
        }

        public static string FormatAssemblyQualifiedTypeName(TypeIdentifier typeIdentifier)
        {
            return "";
        }
    }
}
