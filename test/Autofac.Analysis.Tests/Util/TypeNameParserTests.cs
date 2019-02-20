using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Util;
using Xunit;

namespace Autofac.Analysis.Tests.Util
{
    public class TypeNameParserTests
    {
        public class WhenParsingANameWithSimpleQualification
        {
            readonly TypeIdentifier _parsed;

            public WhenParsingANameWithSimpleQualification()
            {
                _parsed = TypeNameParser.ParseAssemblyQualifiedTypeName("Namespace.Class, Company.Product");
            }

            [Fact]
            public void TheNameIsParsed()
            {
                Assert.Equal("Namespace.Class", _parsed.FullName);                
            }

            [Fact]
            public void TheAssemblyNameIsParsed()
            {
                Assert.Equal("Company.Product", _parsed.AssemblyName);
            }
        }

        public class WhenParsingANameWithGenericArguments
        {
            readonly TypeIdentifier _parsed;

            public WhenParsingANameWithGenericArguments()
            {
                _parsed = TypeNameParser.ParseAssemblyQualifiedTypeName(
                    "Namespace.Class`2[[A, B],[C, D]], Company.Product");
            }

            [Fact]
            public void TheNameIsParsed()
            {
                Assert.Equal("Namespace.Class", _parsed.FullName);
            }

            [Fact]
            public void TheAssemblyNameIsParsed()
            {
                Assert.Equal("Company.Product", _parsed.AssemblyName);
            }

            [Fact]
            public void TheGenericArgumentsAreParsed()
            {
                Assert.Equal(2, _parsed.GenericArguments.Length);
            }

            [Fact]
            public void TheFirstArgIsNamedCorrectly()
            {
                Assert.Equal("A", _parsed.GenericArguments[0].FullName);
            }
        }

        public class WhenParsingANameWithNestedGenerics
        {
            readonly TypeIdentifier _parsed;

            public WhenParsingANameWithNestedGenerics()
            {
                _parsed = TypeIdentifier.Parse("Autofac.Features.Metadata.Meta`1[[System.Func`1[[Autofac.Features.OwnedInstances.Owned`1[[Company.App.ITask, Company.App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Autofac, Version=2.4.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]][], Autofac, Version=2.4.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da");
            }

            [Fact]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.Equal("Autofac.Features.Metadata.Meta", _parsed.FullName);
            }

            [Fact]
            public void TheFirstNestedTypeNameIsCorrect()
            {
                Assert.Equal("System.Func", _parsed.GenericArguments[0].FullName);
            }

            [Fact]
            public void TheSecondNestedTypeNameIsCorrect()
            {
                Assert.Equal("Autofac.Features.OwnedInstances.Owned", _parsed.GenericArguments[0].GenericArguments[0].FullName);
            }

            [Fact]
            public void TheThirdNestedTypeNameIsCorrect()
            {
                Assert.Equal("Company.App.ITask", _parsed.GenericArguments[0].GenericArguments[0].GenericArguments[0].FullName);
            }

            [Fact]
            public void TheTypeFullNameIsDescribedCorrectly()
            {
                Assert.Equal("Autofac.Features.Metadata.Meta<System.Func<Autofac.Features.OwnedInstances.Owned<Company.App.ITask>>>[]", _parsed.DisplayFullName);
            }

            [Fact]
            public void TheTypeIsDescribedCorrectly()
            {
                Assert.Equal("Meta<Func<Owned<ITask>>>[]", _parsed.DisplayName);
            }
        }

        public class WhenParsingAGenericThatHasANestedPrivate
        {
            readonly TypeIdentifier _parsed;

            public WhenParsingAGenericThatHasANestedPrivate()
            {
                _parsed = TypeIdentifier.Parse("UserNamespace.Submodule.Class`1+NestedSubclass, UserNamespace.Submodule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }

            [Fact]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.Equal("UserNamespace.Submodule.Class+NestedSubclass", _parsed.FullName);
            }
        }

        public class WhenTypeUsesContravariance
        {
            readonly TypeIdentifier _parsed;

            public WhenTypeUsesContravariance()
            {
                _parsed = TypeIdentifier.Parse("Autofac.Features.Variance.ContravariantRegistrationSource+<>c__DisplayClass8+<>c__DisplayClassa, Autofac, Version=3.0.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da");
            }

            [Fact(Skip = "Need to look into what this type name actually means; see #16")]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.Equal("Autofac.Features.Variance.ContravariantRegistrationSource", _parsed.FullName);
            }
        }
    }
}
