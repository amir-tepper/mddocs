﻿using Grynwald.MdDocs.Common.Configuration;

namespace Grynwald.MdDocs.CommandLineHelp.Configuration
{
    public class CommandLineHelpConfiguration
    {
        public enum TemplateName
        {
            Default
        }

        public class DefaultTemplateConfiguration : IConfigurationWithMarkdownPresetSetting
        {
            public const string s_DefaultApplicationMdFileName = "index.md";

            public bool IncludeVersion { get; set; }

            public bool IncludeAutoGeneratedNotice { get; set; }

            public MarkdownPreset MarkdownPreset { get; set; } = MarkdownPreset.Default;

            public string ApplicationMdFileName { get; set; } = s_DefaultApplicationMdFileName;

        }

        public class TemplateConfiguration
        {
            public TemplateName Name { get; set; }

            public DefaultTemplateConfiguration Default { get; set; } = new DefaultTemplateConfiguration();
        }


        [ConvertToFullPath]
        public string OutputPath { get; set; } = "";

        [ConvertToFullPath]
        public string AssemblyPath { get; set; } = "";

        public bool ShouldDeleteOutputPathBeforeGeneration { get; set; } = true;

        public TemplateConfiguration Template { get; set; } = new TemplateConfiguration();
    }
}
