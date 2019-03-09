﻿using System.Collections.Generic;
using System.Linq;
using Grynwald.Utilities.Collections;
using Mono.Cecil;

namespace Grynwald.MdDocs.ApiReference.Model.XmlDocs
{
    class XmlDocsProvider : IXmlDocsProvider
    {
        private readonly IReadOnlyDictionary<MemberId, MemberElement> m_Members;


        public XmlDocsProvider(string xmlDocsPath, AssemblyDefinition assembly)
        {
            var model = XmlDocsReader.Read(xmlDocsPath);

            m_Members = model.ToDictionary(m => m.MemberId);
        }


        public MemberElement TryGetDocumentationComments(MemberId id) => m_Members.GetValueOrDefault(id);
    }
}
