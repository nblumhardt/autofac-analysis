using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Util;
using NUnit.Framework;

namespace Autofac.Analysis.Tests.Util
{
    public class TypeNameParserTests
    {
        [TestFixture]
        public class WhenParsingANameWithSimpleQualification
        {
            TypeIdentifier _parsed;

            [SetUp]
            public void SetUp()
            {
                _parsed = TypeNameParser.ParseAssemblyQualifiedTypeName("Namespace.Class, Company.Product");
            }

            [Test]
            public void TheNameIsParsed()
            {
                Assert.AreEqual("Namespace.Class", _parsed.FullName);                
            }

            [Test]
            public void TheAssemblyNameIsParsed()
            {
                Assert.AreEqual("Company.Product", _parsed.AssemblyName);
            }
        }

        [TestFixture]
        public class WhenParsingANameWithGenericArguments
        {
            TypeIdentifier _parsed;

            [SetUp]
            public void SetUp()
            {
                _parsed = TypeNameParser.ParseAssemblyQualifiedTypeName(
                    "Namespace.Class`2[[A, B],[C, D]], Company.Product");
            }

            [Test]
            public void TheNameIsParsed()
            {
                Assert.AreEqual("Namespace.Class", _parsed.FullName);
            }

            [Test]
            public void TheAssemblyNameIsParsed()
            {
                Assert.AreEqual("Company.Product", _parsed.AssemblyName);
            }

            [Test]
            public void TheGenericArgumentsAreParsed()
            {
                Assert.AreEqual(2, _parsed.GenericArguments.Length);
            }

            [Test]
            public void TheFirstArgIsNamedCorrectly()
            {
                Assert.AreEqual("A", _parsed.GenericArguments[0].FullName);
            }
        }

        [TestFixture]
        public class WhenParsingANameWithNestedGenerics
        {
            TypeIdentifier _parsed;

            [SetUp]
            public void SetUp()
            {
                _parsed = TypeIdentifier.Parse("Autofac.Features.Metadata.Meta`1[[System.Func`1[[Autofac.Features.OwnedInstances.Owned`1[[Company.App.ITask, Company.App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Autofac, Version=2.4.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]][], Autofac, Version=2.4.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da");
            }

            [Test]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.AreEqual("Autofac.Features.Metadata.Meta", _parsed.FullName);
            }

            [Test]
            public void TheFirstNestedTypeNameIsCorrect()
            {
                Assert.AreEqual("System.Func", _parsed.GenericArguments[0].FullName);
            }

            [Test]
            public void TheSecondNestedTypeNameIsCorrect()
            {
                Assert.AreEqual("Autofac.Features.OwnedInstances.Owned", _parsed.GenericArguments[0].GenericArguments[0].FullName);
            }

            [Test]
            public void TheThirdNestedTypeNameIsCorrect()
            {
                Assert.AreEqual("Company.App.ITask", _parsed.GenericArguments[0].GenericArguments[0].GenericArguments[0].FullName);
            }

            [Test]
            public void TheTypeFullNameIsDescribedCorrectly()
            {
                Assert.AreEqual("Autofac.Features.Metadata.Meta<System.Func<Autofac.Features.OwnedInstances.Owned<Company.App.ITask>>>[]", _parsed.DisplayFullName);
            }

            [Test]
            public void TheTypeIsDescribedCorrectly()
            {
                Assert.AreEqual("Meta<Func<Owned<ITask>>>[]", _parsed.DisplayName);
            }
        }

        [TestFixture]
        public class WhenParsingAGenericThatHasANestedPrivate
        {
            TypeIdentifier _parsed;

            [SetUp]
            public void SetUp()
            {
                _parsed = TypeIdentifier.Parse("UserNamespace.Submodule.Class`1+NestedSubclass, UserNamespace.Submodule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }

            [Test]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.AreEqual("UserNamespace.Submodule.Class+NestedSubclass", _parsed.FullName);
            }
        }

        [TestFixture]
        public class WhenTypeUsesContravariance
        {
            TypeIdentifier _parsed;

            [SetUp]
            public void SetUp()
            {
                _parsed = TypeIdentifier.Parse("Autofac.Features.Variance.ContravariantRegistrationSource+<>c__DisplayClass8+<>c__DisplayClassa, Autofac, Version=3.0.0.0, Culture=neutral, PublicKeyToken=17863af14b0044da");
            }

            [Test, Ignore("Need to look into what this type name actually means; see #16")]
            public void TheOuterTypeNameIsCorrect()
            {
                Assert.AreEqual("Autofac.Features.Variance.ContravariantRegistrationSource", _parsed.FullName);
            }
        }
    }
}
