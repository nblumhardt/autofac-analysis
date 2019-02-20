using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Engine.Util;

namespace Autofac.Analysis.Engine.Application
{
    public class TypeIdentifier
    {
        readonly string _fullName;
        readonly string _assemblyName;
        readonly Version _version;
        readonly string _culture;
        readonly string _publicKeyToken;
        readonly int _genericArgumentCount;
        readonly string _modifiers;
        readonly TypeIdentifier[] _genericArguments;
        readonly static ConcurrentDictionary<string, TypeIdentifier> _cache = new ConcurrentDictionary<string,TypeIdentifier>();

        static readonly IDictionary<string, string> LanguageNames = new Dictionary<string, string>
        {
            { typeof(string).FullName, "string" },
            { typeof(bool).FullName, "bool" },
            { typeof(byte).FullName, "byte" },
            { typeof(char).FullName, "char" },
            { typeof(short).FullName, "short" },
            { typeof(int).FullName, "int" },
            { typeof(long).FullName, "long" },
            { typeof(ushort).FullName, "ushort" },
            { typeof(uint).FullName, "uint" },
            { typeof(ulong).FullName, "ulong" },
            { typeof(decimal).FullName, "decimal" },
        };

        const string AnonPart1 = "DisplayClass", AnonPart2 = "<>";

        public int GenericArgumentCount
        {
            get { return _genericArgumentCount; }
        }

        public string Modifiers
        {
            get { return _modifiers; }
        }

        public string PublicKeyToken
        {
            get { return _publicKeyToken; }
        }

        public TypeIdentifier[] GenericArguments
        {
            get { return _genericArguments; }
        }

        public string Culture
        {
            get { return _culture; }
        }

        public Version Version
        {
            get { return _version; }
        }

        public string FullName
        {
            get { return _fullName; }
        }

        public string Name
        {
            get
            {
                var lastPunc = 0;
                if (FullName.Contains('.'))
                    lastPunc = FullName.LastIndexOf(".", StringComparison.Ordinal);
                if (FullName.Contains('+'))
                    lastPunc = Math.Max(FullName.LastIndexOf("+", StringComparison.Ordinal), lastPunc);
                return FullName.Substring(lastPunc + 1);
            }
        }

        public static TypeIdentifier Parse(string assemblyQualifiedTypeName)
        {
            return _cache.GetOrAdd(assemblyQualifiedTypeName, aqn =>
                TypeNameParser.ParseAssemblyQualifiedTypeName(assemblyQualifiedTypeName));
        }

        public TypeIdentifier(string fullName, string assemblyName, Version version, string culture, string publicKeyToken, int genericArgumentCount, IEnumerable<TypeIdentifier> genericArguments, string modifiers)
        {
            if (fullName == null) throw new ArgumentNullException(nameof(fullName));
            if (genericArguments == null) throw new ArgumentNullException(nameof(genericArguments));
            if (modifiers == null) throw new ArgumentNullException(nameof(modifiers));
            _fullName = fullName;
            _assemblyName = assemblyName;
            _version = version;
            _culture = culture;
            _publicKeyToken = publicKeyToken;
            _genericArgumentCount = genericArgumentCount;
            _modifiers = modifiers;
            _genericArguments = genericArguments.ToArray();
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public string AssemblyQualifiedName
        {
            get { return TypeNameParser.FormatAssemblyQualifiedTypeName(this); }
        }

        public string DisplayFullName
        {
            get { return FormatForDisplay(tn => tn.FullName); }
        }

        public string DisplayName
        {
            get { return FormatForDisplay(tn => tn.Name); }
        }

        string FormatForDisplay(Func<TypeIdentifier, string> displayNameSelector)
        {
            string languageName;
            if (LanguageNames.TryGetValue(FullName, out languageName))
                return languageName;

            if (IsAnonymous)
                return "Anonymous";

            var result = displayNameSelector(this);
            if (GenericArguments.Length > 0)
            {
                result += "<";
                result += string.Join(", ", GenericArguments.Select(a => a.FormatForDisplay(displayNameSelector)));
                result += ">";
            }
            result += Modifiers;
            return result;            
        }

        public bool IsAnonymous
        {
            get { return Name.Contains(AnonPart1) && Name.Contains(AnonPart2); }
        }
    }
}
