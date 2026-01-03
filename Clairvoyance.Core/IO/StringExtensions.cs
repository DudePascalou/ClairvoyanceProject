namespace Clairvoyance.Core.IO;

public static class StringExtensions
{
    private static readonly char[] _InvalidFileNameChars =
    [
        '\\','<','>','|','\0',':','*','?','/',
        '\a','\b','\t','\n','\v','\f','\r',
        '\u0001','\u0002','\u0003','\u0004','\u0005','\u0006',
        '\u000e','\u000f','\u0010','\u0011','\u0012','\u0013',
        '\u0014','\u0015','\u0016','\u0017','\u0018','\u0019',
        '\u001a','\u001b','\u001c','\u001d','\u001e','\u001f'
    ];

    /// <summary>
    /// Replaces invalid file name characters with underscores.
    /// </summary>
    /// <param name="fileName">A file name.</param>
    /// <returns>A string without invalid file name characters.</returns>
    /// <remarks>In order for this function to be deterministic
    /// over Windows and Linux platforms, the invalid characters are those
    /// invalid in one or the other platform.</remarks>
    public static string ReplaceInvalidFileNameChars(this string fileName)
    {
        return string.Join("_", fileName.Split(_InvalidFileNameChars));
    }

    /// <summary>
    /// Cleans the file name by replacing invalid characters with underscores.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <returns>Cleaned file name.</returns>
    public static string CleanFileName(this string fileName)
    {
        return FileName.CleaningRegex().Replace(fileName, "_");
    }
}
