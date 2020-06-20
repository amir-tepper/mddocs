﻿using ApprovalTests;
using ApprovalTests.Reporters;
using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.CommandLineHelp.Configuration;
using Grynwald.MdDocs.CommandLineHelp.Model;
using Grynwald.MdDocs.CommandLineHelp.Templates.Default;
using Grynwald.MdDocs.Common.Configuration;
using Grynwald.MdDocs.TestHelpers;
using Xunit;

namespace Grynwald.MdDocs.CommandLineHelp.Test.Templates.Default
{
    /// <summary>
    /// Tests for <see cref="MultiCommandApplicationPage"/>
    /// </summary>
    [Trait("Category", "SkipWhenLiveUnitTesting")]
    [UseReporter(typeof(DiffReporter))]
    public class MultiCommandApplicationPageTest
    {
        [Fact]
        public void GetDocument_returns_expected_document_01()
        {
            var model = new MultiCommandApplicationDocumentation("TestApp", version: null);
            Approve(model);
        }

        [Fact]
        public void GetDocument_returns_expected_document_02()
        {
            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: "1.2.3-beta");
            Approve(model);
        }

        [Fact]
        public void GetDocument_returns_expected_document_03()
        {
            var model = new MultiCommandApplicationDocumentation("TestApp", version: null)
                .WithCommand(name: "command1", helpText: "Documentation for command 1")
                .WithCommand(name: "command2");

            Approve(model);
        }

        [Fact]
        public void GetDocument_returns_expected_document_04()
        {
            // commands must be ordered by name
            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: null)
                .WithCommand(name: "commandXYZ")
                .WithCommand(name: "commandAbc");

            Approve(model);
        }

        [Fact]
        public void GetDocument_returns_expected_document_05()
        {
            // commands must be ordered by name
            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: "4.5.6")
            {
                Usage = new[] { "usage line 1", "usage line 2", "usage line 3" }
            };

            Approve(model);
        }

        [Fact]
        public void GetDocument_returns_expected_document_06()
        {
            var configuration = new ConfigurationProvider().GetDefaultCommandLineHelpConfiguration();
            configuration.Template.Default.IncludeVersion = false;

            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: "4.5.6")
            {
                Usage = new[] { "usage line 1", "usage line 2", "usage line 3" }
            };

            Approve(model, configuration);
        }

        [Fact]
        public void GetDocument_returns_expected_Markdown_for_default_settings()
        {
            var configuration = new ConfigurationProvider().GetDefaultCommandLineHelpConfiguration();

            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: "4.5.6")
            {
                Usage = new[] { "usage line 1", "usage line 2", "usage line 3" }
            };

            Approve(model, configuration);
        }

        [Fact]
        public void GetDocument_does_not_include_AutoGenerated_notice_if_the_includeAutoGeneratedNotice_setting_is_false()
        {
            var configuration = new ConfigurationProvider().GetDefaultCommandLineHelpConfiguration();
            configuration.Template.Default.IncludeAutoGeneratedNotice = false;

            var model = new MultiCommandApplicationDocumentation(name: "TestApp", version: "4.5.6")
            {
                Usage = new[] { "usage line 1", "usage line 2", "usage line 3" }
            };

            Approve(model, configuration);
        }


        private void Approve(MultiCommandApplicationDocumentation model, CommandLineHelpConfiguration? configuration = null)
        {
            var pathProvider = new DefaultCommandLineHelpPathProvider();
            var documentSet = new DocumentSet<IDocument>();

            // add dummy pages for all commands
            foreach (var command in model.Commands)
            {
                documentSet.Add(pathProvider.GetPath(command), new TextDocument());
            }

            configuration ??= new ConfigurationProvider().GetDefaultCommandLineHelpConfiguration();

            var applicationPage = new MultiCommandApplicationPage(documentSet, pathProvider, model, configuration);
            documentSet.Add(pathProvider.GetPath(model), applicationPage);

            var doc = applicationPage.GetDocument();

            Assert.NotNull(doc);
            var writer = new ApprovalTextWriter(doc.ToString());
            Approvals.Verify(writer, new ApprovalNamer(relativeOutputDirectory: "../../../_referenceResults"), Approvals.GetReporter());
        }
    }
}
