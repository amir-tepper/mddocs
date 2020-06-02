# API Reference Configuration

**Applies to:** Version 0.4 an later

## Available Settings

- [Assembly Path](#assembly-path)
- [Output Path](#output-path)
- [Markdown Preset](#markdown-preset)

## Assembly Path

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>mddocs:apireference:outputPath</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td><code>assembly</code></td>
    </tr>
    <tr>
        <td><b>MSBuild Property</b></td>
        <td>Determined automatically</td>
    </tr>
</table>

The Assembly Path setting sets the path to the assembly to load in order to generate documentation.

**ℹ️ Note:** When using [MdDocs MSBuild integration](../../msbuild-integration.md), setting the assembly path has **no effect** because the MSBuild targets use the target path of the current project.

## Output Path

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>mddocs:apireference:outputPath</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td><code>outdir</code></td>
    </tr>
    <tr>
        <td><b>MSBuild Property</b></td>
        <td><code>ApiReferenceDocumentationOutputPath</code></td>
    </tr>
</table>

The Output Path settings defines the path of the directory the generated documentation is written to.

## Markdown Preset

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>mddocs:apireference:markdownPreset</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td><code>markdown-preset</code></td>
    </tr>
    <tr>
        <td><b>MSBuild Property</b></td>
        <td><code>MdDocsMarkdownPreset</code></td>
    </tr>
    <tr>
        <td><b>Default value</b></td>
        <td><code>Default</code></td>
    </tr>
    <tr>
        <td><b>Allowed values</b></td>
        <td>
            <ul>
                <li><code>Default</code></li>
                <li><code>MkDocs</code></li>
            </ul>
        </td>
    </tr>
</table>

The *Markdown Preset (Default Template)* customizes serialization of Markdown.

Supported values are:

- `default`: Produces Markdown that should work in most environments, including
  GitHub and GitLab
- `MkDocs`: Produces Markdown optimized for being rendered by Python-Markdown
  and [MkDocs](https://www.mkdocs.org/)

For details on the differences between the presets, see also
[Markdown Generator docs](https://github.com/ap0llo/markdown-generator/blob/master/docs/apireference/Grynwald/MarkdownGenerator/MdSerializationOptions/Presets/index.md).

## See Also

- [Configuration Overview](../README.md)
- [MdDocs .NET CLI Tool](../../net-cli-tool.md)
- [MdDocs MSBuild Integration](../../msbuild-integration.md)
