using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clairvoyance.Core.IO;

/// <summary>
/// File name cleaning through a regular expression.
/// </summary>
/// <remarks>
/// This class must be declared as partial to allow the source generation at compile time
/// (<seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators"/>).
/// </remarks>
public static partial class FileName
{
    /// <summary>
    /// Gets the regular expression used to clean the file names:
    /// only lowercase letters, numbers, underscores and dots.
    /// </summary>
    /// <returns>The <see cref="CleaningRegex"/> to validate a file name.</returns>
    [GeneratedRegex("[^a-zA-Z0-9_.]")]
    public static partial Regex CleaningRegex();
}
