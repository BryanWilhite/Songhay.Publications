global using System.Data;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.IO;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Xml.Linq;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using CloneExtensions;
global using Markdig;
global using YamlDotNet.Serialization;
global using YamlDotNet.Serialization.NamingConventions;
global using Songhay.Abstractions;
global using Songhay.Diagnostics;
global using Songhay.Extensions;
global using Songhay.Models;
global using Songhay.Publications.Abstractions;
global using Songhay.Publications.Extensions;
global using Songhay.Publications.Models;
global using Songhay.Publications.Validators;


using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Songhay.Publications.Tests")]
