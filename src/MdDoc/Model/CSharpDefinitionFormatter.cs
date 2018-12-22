﻿using System.Linq;
using System.Text;
using Mono.Cecil;

namespace MdDoc.Model
{
    static class CSharpDefinitionFormatter
    {

        public static string GetDefinition(PropertyDefinition property)
        {
            var hasGetter = property.GetMethod?.IsPublic == true;
            var hasSetter = property.SetMethod?.IsPublic == true;

            var definitionBuilder = new StringBuilder();
            definitionBuilder.Append("public ");

            if(!property.HasThis)
            {
                definitionBuilder.Append("static ");
            }

            definitionBuilder.Append(property.PropertyType.ToTypeId().DisplayName);
            definitionBuilder.Append(" ");


            if (property.HasParameters)
                definitionBuilder.Append("this");
            else
                definitionBuilder.Append(property.Name);

            if (property.HasParameters)
            {
                definitionBuilder.Append("[");

                definitionBuilder.AppendJoin(
                    ", ",
                    property.Parameters.Select(x => $"{x.ParameterType.ToTypeId().DisplayName} {x.Name}")
                );

                definitionBuilder.Append("]");
            }


            definitionBuilder.Append(" ");
            definitionBuilder.Append("{ ");

            if (hasGetter)
                definitionBuilder.Append("get;");


            if (hasSetter)
            {
                if (hasGetter)
                    definitionBuilder.Append(" ");

                definitionBuilder.Append("set;");
            }

            definitionBuilder.Append(" }");

            return definitionBuilder.ToString();

        }

        public static string GetDefinition(FieldDefinition field)
        {            
            var definitionBuilder = new StringBuilder();
            definitionBuilder.Append("public ");
            
            if(field.IsStatic && !field.HasConstant)
            {
                definitionBuilder.Append("static ");
            }

            if(field.HasConstant)
            {
                definitionBuilder.Append("const ");
            }

            if(field.Attributes.HasFlag(FieldAttributes.InitOnly))
            {
                definitionBuilder.Append("readonly ");
            }

            definitionBuilder.Append(field.FieldType.ToTypeId().DisplayName);
            definitionBuilder.Append(" ");


            definitionBuilder.Append(field.Name);

            definitionBuilder.Append(";");

            return definitionBuilder.ToString();
        }

    }
}
