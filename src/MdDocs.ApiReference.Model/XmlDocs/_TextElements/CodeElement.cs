﻿using System;

namespace Grynwald.MdDocs.ApiReference.Model.XmlDocs
{
    public sealed class CodeElement : Element
    {
        public string Content { get; }

        public string Language { get; }


        public CodeElement(string content, string language)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Language = language;
        }


        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}