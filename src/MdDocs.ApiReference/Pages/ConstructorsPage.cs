﻿using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model;
using Microsoft.Extensions.Logging;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    public sealed class ConstructorsPage : OverloadableMemberPage<ConstructorDocumentation, ConstructorOverloadDocumentation>
    {
        internal ConstructorsPage(ILinkProvider linkProvider, ConstructorDocumentation model, ILogger logger)
            : base(linkProvider, model, logger)
        { }


        protected override MdHeading GetPageHeading() =>
            new MdHeading($"{Model.TypeDocumentation.DisplayName} Constructors", 1);

        //No "Returns" subsection for constructors
        protected override void AddReturnsSubSection(MdContainerBlock block, ConstructorOverloadDocumentation overload, int headingLevel)
        { }
    }
}