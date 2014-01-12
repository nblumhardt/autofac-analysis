using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Util;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    class TypeDiscoveredHandler : IUpdateHandler<TypeDiscoveredMessage>
    {
        readonly IActiveItemRepository<TypeData> _typeData;

        public TypeDiscoveredHandler(IActiveItemRepository<TypeData> typeData)
        {
            if (typeData == null) throw new ArgumentNullException("typeData");
            _typeData = typeData;
        }

        public void UpdateFrom(TypeDiscoveredMessage message)
        {
            var typeModel = message.TypeModel;
            var typeData = new TypeData(
                typeModel.Id,
                TypeNameParser.ParseAssemblyQualifiedTypeName(typeModel.AssemblyQualifiedName),
                typeModel.IsDisposable);
            _typeData.Add(typeData);
        }
    }
}
