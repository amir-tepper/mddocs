﻿using System.Linq;
using Grynwald.MdDocs.ApiReference.Configuration;
using Grynwald.MdDocs.ApiReference.Model;
using Grynwald.MdDocs.ApiReference.Model.XmlDocs;
using Grynwald.MdDocs.ApiReference.Pages;
using Microsoft.Extensions.Logging.Abstractions;

namespace Grynwald.MdDocs.ApiReference.Test.Pages
{
    /// <summary>
    /// Tests for <see cref="MethodPage" />
    /// </summary>
    public class MethodPageTest : PageTestBase<MethodDocumentation, MethodPage>
    {
        protected override MethodPage CreatePage(MethodDocumentation model, ApiReferenceConfiguration configuration)
        {
            return new MethodPage(NullLinkProvider.Instance, configuration, model, NullLogger.Instance);
        }

        protected override MethodDocumentation CreateSampleModel()
        {
            var assembly = Compile(@"
                namespace MyNamespace
                {
                    public class Class1
                    {
                        public void Method1()
                        { }
                    }
                }
            ");

            var assemblyDocumentation = new AssemblyDocumentation(assembly, NullXmlDocsProvider.Instance, NullLogger.Instance);
            return assemblyDocumentation.MainModuleDocumentation.Types.Single().Methods.Single();
        }
    }
}