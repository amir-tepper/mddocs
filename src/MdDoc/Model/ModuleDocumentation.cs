﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.Utilities.Collections;
using MdDoc.Model.XmlDocs;
using Mono.Cecil;

namespace MdDoc.Model
{
    public class ModuleDocumentation : IDocumentation
    {
        private readonly IDictionary<TypeId, TypeDocumentation> m_Types;
        private readonly IDictionary<string, NamespaceDocumentation> m_Namespaces;
        private readonly IXmlDocsProvider m_XmlDocsProvider;


        public AssemblyDocumentation AssemblyDocumentation { get; }

        public IReadOnlyCollection<TypeDocumentation> Types { get; }

        public IReadOnlyCollection<NamespaceDocumentation> Namespaces { get; }


        internal ModuleDefinition Definition { get; }


        internal ModuleDocumentation(AssemblyDocumentation assemblyDocumentation, ModuleDefinition definition, IXmlDocsProvider xmlDocsProvider)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            m_XmlDocsProvider = xmlDocsProvider ?? throw new ArgumentNullException(nameof(xmlDocsProvider));

            AssemblyDocumentation = assemblyDocumentation ?? throw new ArgumentNullException(nameof(assemblyDocumentation));


            m_Types = new Dictionary<TypeId, TypeDocumentation>();
            m_Namespaces = new Dictionary<string, NamespaceDocumentation>();

            foreach (var typeDefinition in Definition.Types.Where(t => t.IsPublic))
            {
                var namespaceDocumentation = m_Namespaces.GetOrAdd(
                    typeDefinition.Namespace,
                    () => new NamespaceDocumentation(this, typeDefinition.Namespace)
                );

                var typeDocumentation = new TypeDocumentation(this, namespaceDocumentation, typeDefinition, m_XmlDocsProvider);

                m_Types.Add(typeDocumentation.TypeId, typeDocumentation);
                namespaceDocumentation.AddType(typeDocumentation);
            }
            
            Types = ReadOnlyCollectionAdapter.Create(m_Types.Values);
            Namespaces = ReadOnlyCollectionAdapter.Create(m_Namespaces.Values);
        }


        public IDocumentation TryGetDocumentation(MemberId member)
        {
            switch (member)
            {
                case TypeId typeId:
                    return m_Types.GetValueOrDefault(typeId);

                case TypeMemberId typeMemberId:
                    return m_Types.GetValueOrDefault(typeMemberId.DefiningType)?.TryGetDocumentation(member);

                default:
                    return null;
            }
        }
    }
}
