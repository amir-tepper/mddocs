﻿using Grynwald.MdDocs.Common.Configuration;

namespace Grynwald.MdDocs.ApiReference.Configuration
{
    public class ApiReferenceConfiguration : IConfigurationWithMarkdownPresetSetting
    {
        [ConvertToFullPath]
        public string OutputPath { get; set; } = "";

        [ConvertToFullPath]
        public string AssemblyPath { get; set; } = "";

        public MarkdownPreset MarkdownPreset { get; set; } = MarkdownPreset.Default;
    }
}
