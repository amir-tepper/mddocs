﻿using System;
using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model.XmlDocs;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    class TextBlockToMarkdownConverter
    {
        class ConvertToBlockVisitor : IVisitor
        {
            private MdParagraph m_CurrentParagraph = new MdParagraph();
            private readonly IMdSpanFactory m_SpanFactory;


            public MdContainerBlock Result { get; } = new MdContainerBlock();


            public ConvertToBlockVisitor(IMdSpanFactory spanFactory)
            {
                m_SpanFactory = spanFactory ?? throw new ArgumentNullException(nameof(spanFactory));
            }


            public void Visit(ParamRefElement element)
            {
                m_CurrentParagraph.Add(new MdCodeSpan(element.Name));
            }

            public void Visit(TypeParamRefElement element)
            {
                m_CurrentParagraph.Add(new MdCodeSpan(element.Name));
            }

            public void Visit(CElement element)
            {
                if (!String.IsNullOrEmpty(element.Content))
                {
                    m_CurrentParagraph.Add(new MdCodeSpan(element.Content));
                }
            }

            public void Visit(CodeElement element)
            {
                // end the current paragraph
                PushParagraph();

                // add a new code block (can be added directory to Result as CodeElement has no child-elements)
                Result.Add(new MdCodeBlock(element.Content, GetInfoString(element.Language)));
            }

            public void Visit(TextElement element)
            {
                m_CurrentParagraph.Add(new MdTextSpan(element.Content));
            }

            public void Visit(SeeElement element)
            {
                m_CurrentParagraph.Add(m_SpanFactory.GetMdSpan(element.MemberId));
            }

            public void Visit(ParaElement element)
            {
                Visit(element.Text);
            }

            public void Visit(TextBlock textBlock)
            {
                // end the current paragraph
                PushParagraph();

                // visit text elements
                foreach (var element in textBlock.Elements)
                {
                    element.Accept(this);
                }

                // end the paragraph to make sure all content gets added to Result
                PushParagraph();
            }


            private void PushParagraph()
            {
                Result.Add(m_CurrentParagraph);
                m_CurrentParagraph = new MdParagraph();

            }

            /// <summary>
            /// Gets the Markdown info string for the specified language if available.
            /// </summary>
            /// <remarks>
            /// Uses the language names as supported by GitHub, see
            /// https://github.com/github/linguist/blob/master/lib/linguist/languages.yml
            /// </remarks>
            private string GetInfoString(CodeLanguage language)
            {
                // Uses
                switch (language)
                {
                    case CodeLanguage.CSharp:
                        return "csharp";
                    case CodeLanguage.CPlusPlus:
                        return "cpp";
                    case CodeLanguage.C:
                        return "c";
                    case CodeLanguage.FSharp:
                        return "fsharp";
                    case CodeLanguage.Javascript:
                        return "js";
                    case CodeLanguage.VisualBasic:
                        return "vbnet";
                    case CodeLanguage.XML:
                        return "xml";
                    case CodeLanguage.HTML:
                        return "html";
                    case CodeLanguage.XAML:
                        return "xaml";
                    case CodeLanguage.SQL:
                        return "sql";
                    case CodeLanguage.Python:
                        return "python";
                    case CodeLanguage.Powershell:
                        return "powershell";
                    case CodeLanguage.Batch:
                        return "batch";                   
                    default:
                        return null;                        
                }

            }
        }

        class ConvertToSpanVisitor : IVisitor
        {
            private readonly IMdSpanFactory m_SpanFactory;


            public MdCompositeSpan Result { get; } = new MdCompositeSpan();


            public ConvertToSpanVisitor(IMdSpanFactory spanFactory)
            {
                m_SpanFactory = spanFactory ?? throw new ArgumentNullException(nameof(spanFactory));
            }


            public void Visit(ParamRefElement element)
            {
                Result.Add(new MdCodeSpan(element.Name));
            }

            public void Visit(TypeParamRefElement element)
            {
                Result.Add(new MdCodeSpan(element.Name));
            }

            public void Visit(CElement element)
            {
                if (!String.IsNullOrEmpty(element.Content))
                {
                    Result.Add(new MdCodeSpan(element.Content));
                }
            }

            public void Visit(CodeElement element)
            {
                // <code></code> cannot be converted to a span => ignore element
            }

            public void Visit(TextElement element)
            {
                Result.Add(new MdTextSpan(element.Content));
            }

            public void Visit(SeeElement element)
            {
                Result.Add(m_SpanFactory.GetMdSpan(element.MemberId));
            }

            public void Visit(TextBlock text)
            {
                foreach (var element in text.Elements)
                {
                    element.Accept(this);
                }
            }

            public void Visit(ParaElement element)
            {
                // a single span cannot contain multiple paragraphs, but we can at least add a line break
                Result.Add("\r\n");

                // visit text block in paragraph
                element.Text.Accept(this);
            }
        }


        public static MdBlock ConvertToBlock(TextBlock text, IMdSpanFactory spanFactory)
        {
            if (text.IsEmpty)
            {
                return MdEmptyBlock.Instance;
            }

            var visitor = new ConvertToBlockVisitor(spanFactory);
            text.Accept(visitor);

            return visitor.Result;
        }

        public static MdSpan ConvertToSpan(TextBlock text, IMdSpanFactory spanFactory)
        {
            var visitor = new ConvertToSpanVisitor(spanFactory);
            text.Accept(visitor);

            return visitor.Result;
        }
    }
}
