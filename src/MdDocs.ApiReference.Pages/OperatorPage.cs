﻿using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model;
using Microsoft.Extensions.Logging;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    internal class OperatorPage : OverloadableMemberPage<OperatorDocumentation, OperatorOverloadDocumentation>
    {
        public override OutputPath OutputPath { get; }


        public OperatorPage(PageFactory pageFactory, string rootOutputPath, OperatorDocumentation model, ILogger logger)
            : base(pageFactory, rootOutputPath, model, logger)
        {
            OutputPath = new OutputPath(GetTypeDir(Model.TypeDocumentation), "Operators", $"{Model.Kind}.md");
        }


        protected override MdHeading GetPageHeading() =>
            new MdHeading($"{Model.TypeDocumentation.DisplayName}.{Model.Kind} Operator", 1);
    }
}
