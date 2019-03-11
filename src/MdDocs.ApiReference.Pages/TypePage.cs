﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model;
using Microsoft.Extensions.Logging;

using static Grynwald.MarkdownGenerator.FactoryMethods;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    internal class TypePage : PageBase<TypeDocumentation>
    {
        private readonly ILogger m_Logger;

        public override OutputPath OutputPath { get; }


        public TypePage(PageFactory pageFactory, string rootOutputPath, TypeDocumentation model, ILogger logger)
            : base(pageFactory, rootOutputPath, model)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            OutputPath = new OutputPath(GetTypeDir(Model), "Type.md");
        }


        public override void Save()
        {
            m_Logger.LogInformation($"Saving page '{OutputPath}'");

            var document = new MdDocument(
                Heading($"{Model.DisplayName} {Model.Kind}", 1)
            );

            AddObsoleteWarning(document.Root, Model);

            AddDefinitionSection(document.Root);

            AddTypeParametersSection(document.Root);

            AddRemarksSection(document.Root);

            //TODO: Skip constructors when it is compiler-generated, i.e. only the implicit default constructor
            AddOverloadableMembersSection(
                document.Root,
                "Constructors",
                (IEnumerable<OverloadDocumentation>)Model.Constructors?.Overloads ?? Array.Empty<OverloadDocumentation>()
            );

            AddSimpleMembersSection(document.Root, "Fields", Model.Fields);

            AddSimpleMembersSection(document.Root, "Events", Model.Events);

            AddSimpleMembersSection(document.Root, "Properties", Model.Properties);

            AddOverloadableMembersSection(document.Root, "Indexers", Model.Indexers.SelectMany(m => m.Overloads));

            AddOverloadableMembersSection(document.Root, "Methods", Model.Methods.SelectMany(m => m.Overloads));

            AddOverloadableMembersSection(document.Root, "Operators", Model.Operators.SelectMany(x => x.Overloads));

            //TODO: Explicit interface implementations

            //TODO: Extension methods

            AddExampleSection(document.Root);

            AddSeeAlsoSection(document.Root);

            AddFooter(document.Root);

            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            document.Save(OutputPath);
        }


        private void AddDefinitionSection(MdContainerBlock block)
        {
            // Add Namespace            
            block.Add(
                Paragraph(Bold("Namespace:"), " ", GetMdSpan(Model.NamespaceDocumentation.NamespaceId))
            );

            // Add Assembly
            block.Add(
                Paragraph(Bold("Assembly:"), " " + Model.AssemblyName)
            );

            if (Model.Summary != null)
            {
                block.Add(ConvertToBlock(Model.Summary));
            }

            // Definition as code
            block.Add(CodeBlock(Model.CSharpDefinition, "csharp"));

            // Add list of base types            
            if (Model.InheritanceHierarchy.Count > 1)
            {
                block.Add(
                    Paragraph(
                        Bold("Inheritance:"),
                        " ",
                        Model.InheritanceHierarchy.Select(GetMdSpan).Join(" → ")
                ));
            }

            // Add class attributes
            if (Model.Attributes.Count > 0)
            {
                block.Add(
                    Paragraph(
                        Bold("Attributes:"),
                        " ",
                        Model.Attributes.Select(GetMdSpan).Join(",")
                ));
            }

            // Add list of implemented interfaces
            if (Model.ImplementedInterfaces.Count > 0)
            {
                block.Add(
                    Paragraph(
                        Bold("Implements:"),
                        " ",
                        Model.ImplementedInterfaces.Select(GetMdSpan).Join(","))
                );
            }
        }

        private void AddRemarksSection(MdContainerBlock block)
        {
            if (Model.Remarks != null)
            {
                block.Add(Heading(2, "Remarks"));
                block.Add(ConvertToBlock(Model.Remarks));
            }
        }

        private void AddOverloadableMembersSection(MdContainerBlock block, string sectionHeading, IEnumerable<OverloadDocumentation> overloads)
        {
            if (overloads.Any())
            {
                var table = Table(Row("Name", "Description"));

                foreach (var ctor in overloads.OrderBy(x => x.Signature))
                {
                    table.Add(
                        Row(
                            CreateLink(ctor.MemberId, ctor.Signature),
                            ConvertToSpan(ctor.Summary)
                    ));
                }

                block.Add(
                    Heading(sectionHeading, 2),
                    table
                );
            }
        }

        private void AddSimpleMembersSection(MdContainerBlock block, string sectionHeading, IEnumerable<SimpleMemberDocumentation> members)
        {
            if (members.Any())
            {
                block.Add(Heading(sectionHeading, 2));

                var table = Table(Row("Name", "Description"));
                foreach (var member in members.OrderBy(x => x.Name))
                {
                    table.Add(
                        Row(
                            CreateLink(member.MemberId, member.Name),
                            ConvertToSpan(member.Summary)
                    ));
                }
                block.Add(table);
            }
        }

        private void AddExampleSection(MdContainerBlock block)
        {
            if (Model.Example == null)
                return;

            block.Add(Heading("Example", 2));
            block.Add(ConvertToBlock(Model.Example));
        }

        private void AddSeeAlsoSection(MdContainerBlock block)
        {
            if (Model.SeeAlso.Count > 0)
            {
                block.Add(Heading(2, "See Also"));
                block.Add(
                    BulletList(
                        Model.SeeAlso.Select(seeAlso => ListItem(ConvertToSpan(seeAlso)))
                ));
            }
        }

        private void AddTypeParametersSection(MdContainerBlock block)
        {
            if (Model.TypeParameters.Count == 0)
                return;


            block.Add(Heading("Type Parameters", 2));

            foreach (var typeParameter in Model.TypeParameters)
            {
                block.Add(
                    Paragraph(CodeSpan(typeParameter.Name)
                ));

                if (typeParameter.Description != null)
                {
                    block.Add(ConvertToBlock(typeParameter.Description));
                }
            }
        }
    }
}