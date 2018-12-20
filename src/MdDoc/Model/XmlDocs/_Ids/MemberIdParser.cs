﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MdDoc.Model.XmlDocs
{
    /// <summary>
    /// Parser for XML Docs member ids
    /// </summary>
    /// <remarks>
    /// A parser that can pare member ids in XML documentation docs generated by the C# compiler as documented 
    /// here https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file
    /// </remarks>
    //TODO: Array types
    internal class MemberIdParser
    {
        private readonly string m_Text;
        private readonly MemberIdLexer m_Lexer;
        private IReadOnlyList<Token> m_Tokens;
        private int m_Position;


        private Token Current => m_Position >= m_Tokens.Count ? m_Tokens[m_Tokens.Count - 1] : m_Tokens[m_Position];

        private Token Next => m_Position + 1 >= m_Tokens.Count ? m_Tokens[m_Tokens.Count - 1] : m_Tokens[m_Position + 1];


        public MemberIdParser(string text)
        {
            m_Text = text;
            m_Lexer = new MemberIdLexer(text);
        }

        
        public MemberId Parse()
        {
            try
            {
                m_Tokens = m_Lexer.GetTokens();
            }
            catch (MemberIdLexerException e)
            {
                // rethrow lexer errors as parser errors
                throw new MemberIdParserException("Failed to parse input because of an lexer error", e);
            }
            m_Position = 0;

            // all ids strat with a single char that indicates what type if identifier it is (type, method...)
            var kind = MatchToken(TokenKind.IdentifierType);
            MatchToken(TokenKind.Colon);

            switch (kind)
            {
                case "F":
                    return ParseFieldId();

                case "E":
                    return ParseEventId();

                case "T":
                    return ParseTypeId();

                case "M":
                    return ParseMethodId();

                case "P":
                    return ParsePropertyId();

                default:
                    // should not happen as for unknown tokens, MatchToken should already throw
                    throw new NotImplementedException();
            }

        }


        private TypeId ParseTypeId()
        {
            // id always starts with a name            
            if (Current.Kind != TokenKind.Name)
                throw UnexpectedToken(TokenKind.Name);

            var nameSegments = new List<string>();
            var arity = 0;

            // consume all name and dot tokens at the start of the text
            while (Current.Kind == TokenKind.Name)
            {
                nameSegments.Add(MatchToken(TokenKind.Name));

                if (Current.Kind == TokenKind.Dot)
                    MatchToken(TokenKind.Dot);
            }

            // optional part: number of type arguments for generic types
            if(Current.Kind == TokenKind.Backtick)
            {
                MatchToken(TokenKind.Backtick);
                arity = int.Parse(MatchToken(TokenKind.Number));                
            }

            var namespaceName = String.Join(".", nameSegments.Take(nameSegments.Count - 1));
            var typeName = nameSegments[nameSegments.Count - 1];
            var type = CreateTypeId(namespaceName, typeName, arity);

            // if the type if followed by square brackets, the id refers to an array type
            // arrays of arrays are allowed, too
            while(Current.Kind == TokenKind.OpenSquareBracket)
            {
                MatchToken(TokenKind.OpenSquareBracket);

                var dimensions = ParseArrayDimensions();

                MatchToken(TokenKind.CloseSquareBracket);

                // wrap type into an array
                type = new ArrayTypeId(type, dimensions);
            }

            // all token should be parsed now
            MatchToken(TokenKind.Eof);

            return type;
        }

        private int ParseArrayDimensions()
        {
            var dimensions = 1;

            // optional part: lower bound and size for each dimension, separated by commas
            while (Current.Kind != TokenKind.CloseSquareBracket)
            {
                switch (Current.Kind)
                {
                    case TokenKind.Comma:
                        MatchToken(TokenKind.Comma);
                        dimensions++;
                        break;

                    // lower bound and size are optional
                    // if neither lower bound nor size is known, the colon is omitted as well
                    // this means the following sequences are possible
                    // - lowerBound:size
                    // - lowerBound:
                    // - :size                        
                    // Both lower bound and size are ignored as they are not required
                    // for identifying a type

                    case TokenKind.Colon:
                        MatchToken(TokenKind.Colon);
                        MatchToken(TokenKind.Number);
                        break;

                    case TokenKind.Number:
                        MatchToken(TokenKind.Number);
                        MatchToken(TokenKind.Colon);
                        if (Current.Kind == TokenKind.Number)
                        {
                            MatchToken(TokenKind.Number);
                        }
                        break;

                    default:
                        throw UnexpectedToken(TokenKind.Comma, TokenKind.Colon, TokenKind.Number);
                }
            }

            return dimensions;
        }

        private FieldId ParseFieldId()
        {
            var (definingType, name) = ParseFieldOrEventId();
            return new FieldId(definingType, name);
        }

        private EventId ParseEventId()
        {
            var (definingType, name) = ParseFieldOrEventId();
            return new EventId(definingType, name);
        }

        private (TypeId definingType, string name) ParseFieldOrEventId()
        {
            // id always starts with a name            
            if (Current.Kind != TokenKind.Name)
                throw UnexpectedToken(TokenKind.Name);

            var nameSegments = new List<string>();
            var typeArity = 0;

            // consume all name and dot tokens at the start of the text
            while (Current.Kind == TokenKind.Name)
            {
                nameSegments.Add(MatchToken(TokenKind.Name));
                
                if(Current.Kind == TokenKind.Dot)
                    MatchToken(TokenKind.Dot);
            }

            switch (Current.Kind)
            {
                // for non-generic types, we have already read all dot and name tokens
                case TokenKind.Eof:
                    break;

                // for generic types, the number of type parameters is encoded using a backtick + the number of parameters
                case TokenKind.Backtick:

                    MatchToken(TokenKind.Backtick);
                    var arityString = MatchToken(TokenKind.Number);
                    typeArity = int.Parse(arityString);

                    // the field or event name must follow after the number of type parameters
                    MatchToken(TokenKind.Dot);
                    nameSegments.Add(MatchToken(TokenKind.Name));
                    break;
                
                default:
                    throw UnexpectedToken(TokenKind.Backtick, TokenKind.Eof);
            }

            // there shouldn't be anythig left after that
            MatchToken(TokenKind.Eof);


            // field id and event id need at least two name segments (type name + field/event name)
            if (nameSegments.Count < 2)
                throw new MemberIdParserException("Invalid input, method id requires name of defining type and method");

            var namespaceName = String.Join(".", nameSegments.Take(nameSegments.Count - 2));
            var typeName = nameSegments[nameSegments.Count - 2];
            var name = nameSegments[nameSegments.Count - 1];

            return (CreateTypeId(namespaceName, typeName, typeArity), name);
        }

        //TODO: type parameters in parameter list
        private MethodId ParseMethodId()
        {
            // id always starts with a name            
            if (Current.Kind != TokenKind.Name)
                throw UnexpectedToken(TokenKind.Name);

            var nameSegments = new List<string>();
            var typeArity = 0;
            var methodArity = 0;
            var methodParameters = default(IReadOnlyList<TypeId>);
            var methodReturnType = default(TypeId);

            // consume all name and dot tokens at the start of the text
            while (Current.Kind == TokenKind.Name)
            {
                nameSegments.Add(MatchToken(TokenKind.Name));

                if (Current.Kind == TokenKind.Dot)
                    MatchToken(TokenKind.Dot);
            }
            
            // end of names reached
            
            // optional part: backtick + number => arity of generic type
            if(Current.Kind == TokenKind.Backtick)
            {
                MatchToken(TokenKind.Backtick);
                var arityString = MatchToken(TokenKind.Number);
                typeArity = int.Parse(arityString);

                // the type' arity is followed by the method name
                MatchToken(TokenKind.Dot);
                nameSegments.Add(MatchToken(TokenKind.Name));
            }
            
            // optional part: double backtick + number => arity of generic method
            if(Current.Kind == TokenKind.DoubleBacktick)
            {
                MatchToken(TokenKind.DoubleBacktick);
                var arityString = MatchToken(TokenKind.Number);
                methodArity = int.Parse(arityString);
            }

            // optional part: parameter list
            if(Current.Kind == TokenKind.OpenParenthesis)
            {
                MatchToken(TokenKind.OpenParenthesis);

                methodParameters = ParseTypeNameList();

                MatchToken(TokenKind.CloseParenthesis);
            }

            // optional part: return type (only used for overloads of implicit and explicit conversion)
            if(Current.Kind == TokenKind.Tilde)
            {
                MatchToken(TokenKind.Tilde);
                methodReturnType = ParseTypeName();
            }
            
            // ensure we parsed all tokens
            MatchToken(TokenKind.Eof);

            // method id needs at least two name segments (type name + method name)
            if (nameSegments.Count < 2)
                throw new MemberIdParserException("Invalid input, method id requires name of defining type and method");


            var namespaceName = String.Join(".", nameSegments.Take(nameSegments.Count - 2));
            var typeName = nameSegments[nameSegments.Count - 2];
            var definingType = CreateTypeId(namespaceName, typeName, typeArity);

            var methodName = nameSegments[nameSegments.Count - 1];

            return new MethodId(definingType, methodName, methodArity, methodParameters ?? Array.Empty<TypeId>(), methodReturnType);

        }

        //TODO: type parameters in parameter list
        private PropertyId ParsePropertyId()
        {
            // id always starts with a name            
            if (Current.Kind != TokenKind.Name)
                throw UnexpectedToken(TokenKind.Name);

            var nameSegments = new List<string>();
            var typeArity = 0;
            var parameters = default(IReadOnlyList<TypeId>);
            
            // consume all name and dot tokens at the start of the text
            while (Current.Kind == TokenKind.Name)
            {
                nameSegments.Add(MatchToken(TokenKind.Name));

                if (Current.Kind == TokenKind.Dot)
                    MatchToken(TokenKind.Dot);
            }

            // end of names reached

            // optional part: backtick + number => arity of generic type
            if (Current.Kind == TokenKind.Backtick)
            {
                MatchToken(TokenKind.Backtick);
                var arityString = MatchToken(TokenKind.Number);
                typeArity = int.Parse(arityString);

                // the type' arity is followed by the method name
                MatchToken(TokenKind.Dot);
                nameSegments.Add(MatchToken(TokenKind.Name));
            }


            // optional part: parameter list (used for indexers)
            if (Current.Kind == TokenKind.OpenParenthesis)
            {
                MatchToken(TokenKind.OpenParenthesis);

                parameters = ParseTypeNameList();

                MatchToken(TokenKind.CloseParenthesis);
            }

            // ensure we parsed all tokens
            MatchToken(TokenKind.Eof);

            // property id needs at least two name segments (type name + property name)
            if (nameSegments.Count < 2)
                throw new MemberIdParserException("Invalid input, property id requires name of defining type and property");


            var namespaceName = String.Join(".", nameSegments.Take(nameSegments.Count - 2));
            var typeName = nameSegments[nameSegments.Count - 2];
            var definingType = CreateTypeId(namespaceName, typeName, typeArity);

            var methodName = nameSegments[nameSegments.Count - 1];

            return new PropertyId(definingType, methodName, parameters ?? Array.Empty<TypeId>());
        }


        private IReadOnlyList<TypeId> ParseTypeNameList()
        {        
            var parameters = new List<TypeId>();

            while(Current.Kind == TokenKind.Name)
            {
                parameters.Add(ParseTypeName());
                if(Current.Kind == TokenKind.Comma)
                {
                    MatchToken(TokenKind.Comma);
                }
            }


            if (parameters.Count == 0)
                throw new MemberIdParserException("Parameter list cannot be empty");

            return parameters;
        }

        private TypeId ParseTypeName()
        {
            if (Current.Kind != TokenKind.Name)
                throw UnexpectedToken(TokenKind.Name);

            var nameSegments = new List<string>();
            var typeArguments = default(IReadOnlyList<TypeId>);

            nameSegments.Add(MatchToken(TokenKind.Name));

            var arrayDimensions = new List<int>();
            var done = false;
            while(!done)
            {
                switch (Current.Kind)
                {
                    case TokenKind.Dot:
                        MatchToken(TokenKind.Dot);
                        nameSegments.Add(MatchToken(TokenKind.Name));
                        break;

                    case TokenKind.OpenBrace:
                        MatchToken(TokenKind.OpenBrace);
                        typeArguments = ParseTypeNameList();
                        MatchToken(TokenKind.CloseBrace);
                        break;

                    case TokenKind.CloseBrace:
                    case TokenKind.Comma:
                    case TokenKind.CloseParenthesis:
                    case TokenKind.Eof:
                        done = true;
                        break;

                    case TokenKind.OpenSquareBracket:
                        MatchToken(TokenKind.OpenSquareBracket);
                        arrayDimensions.Add(ParseArrayDimensions());
                        MatchToken(TokenKind.CloseSquareBracket);
                        break;

                    default:
                        throw UnexpectedToken(
                            TokenKind.Dot, 
                            TokenKind.OpenBrace, 
                            TokenKind.CloseBrace, 
                            TokenKind.Comma, 
                            TokenKind.CloseParenthesis,
                            TokenKind.OpenSquareBracket,
                            TokenKind.CloseSquareBracket,
                            TokenKind.Eof
                        );
                }

            }

            var namespaceName = String.Join(".", nameSegments.Take(nameSegments.Count - 1));
            var typeName = nameSegments[nameSegments.Count - 1];

            TypeId type;
            if(typeArguments != null)
            {
                type = new GenericTypeInstanceId(namespaceName, typeName, typeArguments);
            }
            else
            {
                type = new SimpleTypeId(namespaceName, typeName);
            }

            for(int i = 0; i < arrayDimensions.Count; i++)
            {
                type = new ArrayTypeId(type, arrayDimensions[i]);
            }
            
            return type;
        }

        private string MatchToken(TokenKind kind)
        {
            if(Current.Kind == kind)
            {
                var value = Current.Value;
                m_Position++;
                return value;
            }
            else
            {
                throw UnexpectedToken(kind);
            }
        }

        private MemberIdParserException UnexpectedToken(params TokenKind[] expected)
        {
            return new MemberIdParserException($"Unexpected token. Expected {String.Join(",", expected)} but was {Current.Kind}");
        }

        private static TypeId CreateTypeId(string namespaceName, string typeName, int arity)
        {
            if (arity > 0)
            {
                return new GenericTypeId(namespaceName, typeName, arity);
            }
            else
            {
                return new SimpleTypeId(namespaceName, typeName);
            }
        }
    }
}
