using synosscamera.core.Diagnostics;
using synosscamera.utilities.Compression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace synosscamera.utilities.Extensions
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Compares the string with another one ignoring case.
        /// </summary>
        /// <param name="str">base string</param>
        /// <param name="strToCompare">string to compare</param>
        /// <returns>true or false</returns>
        [DebuggerStepThrough]
        public static bool IsEqualNoCase(this string str, string strToCompare)
        {
            return string.Equals(str, strToCompare, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares the string with a list of other string (ignoring case) and returns true if one of them is equal.
        /// </summary>
        /// <param name="str">base string</param>
        /// <param name="strsToCompare">List of strings to compare with</param>
        /// <returns>true or false</returns>
        [DebuggerStepThrough]
        public static bool IsEqualNoCase(this string str, params string[] strsToCompare)
        {
            return strsToCompare.Any(s => string.Equals(str, s.ToString(), System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Compares the string with another one but in contrast to <see cref="string.Equals(string)"/> treats NULL and string.Empty
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strToCompare"></param>
        /// <returns></returns>
        public static bool IsEqualTreatNullAsEmpty(this string str, string strToCompare)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.IsNullOrEmpty(strToCompare);
            }
            else
            {
                return string.Equals(str, strToCompare);
            }
        }

        /// <summary>
        /// Compares the string with another one but in contrast to <see cref="string.Equals(string)"/> treats NULL and string.Empty
        /// ignoring case
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strToCompare"></param>
        /// <returns></returns>
        public static bool IsEqualTreatNullAsEmptyNoCase(this string str, string strToCompare)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.IsNullOrEmpty(strToCompare);
            }
            else
            {
                return str.IsEqualNoCase(strToCompare);
            }
        }

        /// <summary>
        /// Replaces the format item in the string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">String containing format items</param>
        /// <param name="args">Objects to replace the format items with</param>
        /// <returns>A formatted string</returns>
        [DebuggerStepThrough]
        public static string FormatWith(this string format, params object[] args)
        {
            format.CheckArgumentNull(nameof(format));

            return string.Format(format, args);
        }
        /// <summary>
        /// Replaces the format item in the string with the string representation of a corresponding object in a specified array.
        /// A paramter supplies culture specific formatting information
        /// </summary>
        /// <param name="format">String containing format items</param>
        /// <param name="provider">Culture specific format information</param>
        /// <param name="args">Objects to replace the format items with</param>
        /// <returns>A formatted string</returns>
        [DebuggerStepThrough]
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            format.CheckArgumentNull(nameof(format));

            return string.Format(provider, format, args);
        }
        /// <summary>
        /// Truncate a text at the previous space of a given index and adds an ellipses character to the text "..."
        /// </summary>
        /// <param name="text">Text tu truncate</param>
        /// <param name="length">Index to start searching the next space backwards</param>
        /// <returns>A truncated text with ellipses if the text length is greater than length</returns>
        [DebuggerStepThrough]
        public static string TruncateAtWord(this string text, int length)
        {
            if (text == null || text.Length < length || length <= 0)
                return text;
            var nextSpace = text.LastIndexOf(" ", length, StringComparison.Ordinal);
            return string.Format("{0}...", text.Substring(0, (nextSpace > 0) ? nextSpace : length).Trim());
        }
        /// <summary>
        /// Converts an input string to an integer value. 
        /// If not possible, a default value will be returned
        /// </summary>
        /// <param name="str">Inputstring to convert</param>
        /// <param name="defaultValue">Default value which should be used</param>
        /// <returns>An integer value representing the string</returns>
        [DebuggerStepThrough]
        public static int ToInt(this string str, int defaultValue = 0)
        {

            return int.TryParse(str, out int result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts an input string to an nullable integer value. 
        /// If not possible, a null value will be returned
        /// </summary>
        /// <param name="str">Inputstring to convert</param>
        /// <returns>An integer value representing the string or null if not possible</returns>
        [DebuggerStepThrough]
        public static int? ToNullableInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (int.TryParse(str, out int i)) return i;
            return null;
        }
        /// <summary>
        /// Converts an input string to an boolean value. 
        /// If not possible, a default value will be returned
        /// </summary>
        /// <param name="str">Inputstring to convert</param>
        /// <param name="defaultValue">Default value which should be used</param>
        /// <returns>A boolean value representing the string</returns>
        [DebuggerStepThrough]
        public static bool ToBool(this string str, bool defaultValue = false)
        {

            if (str.IsEqualNoCase("yes", "1", "true", "ja", "ui", "si", "t"))
            {
                return true;
            }
            if (str.IsEqualNoCase("no", "0", "false", "nein", "no", "f"))
            {
                return false;
            }

            return bool.TryParse(str, out bool result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts an input string to an nullable boolean value. 
        /// If not possible, a null value will be returned
        /// </summary>
        /// <param name="str">Inputstring to convert</param>
        /// <returns>A boolean value representing the string or null if not possible</returns>
        [DebuggerStepThrough]
        public static bool? ToNullableBool(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (bool.TryParse(str, out bool i)) return i;

            if (str.IsEqualNoCase("yes", "1", "true", "ja", "ui", "si", "t"))
            {
                return true;
            }
            if (str.IsEqualNoCase("no", "0", "false", "nein", "no", "f"))
            {
                return false;
            }

            return null;
        }
        /// <summary>
        /// Applies an SQL-Like LIKE pattern to the string and validates the string against this pattern
        /// </summary>
        /// <param name="source">String which should be checked</param>
        /// <param name="pattern">LIKE pattern which should be used</param>
        /// <returns>Returns true if the input string matches the LIKE pattern, otherwise false</returns>
        [DebuggerStepThrough]
        public static bool IsLike(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(pattern))
                return true;

            if ((string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern)) && String.CompareOrdinal(pattern, "[]") != 0)
                return false;

            RegexOptions options = RegexOptions.Singleline;
            string regexString = ConvertLikeExpression(pattern);
            Regex regexpr = new Regex(regexString, options);

            return regexpr.IsMatch(source);
        }

        /// <summary>
        /// Checks if a string is null or contains only whitespace characters
        /// </summary>
        /// <param name="value">String value to check</param>
        /// <returns>Returns true if null or only whitespace characters are in the string</returns>
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Checks if a string is null, contains only whitespace characters or exceeds a maxlength
        /// </summary>
        /// <param name="value">String value to check</param>
        /// <param name="maxLength">Max length allowed for non-whitespace strings</param>
        /// <returns>Returns true if null, only whitespace characters are in the string or the string length exceeds <paramref name="maxLength"/> paramter</returns>
        [DebuggerStepThrough]
        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            if (value.Length > maxLength)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a string contains any non whitspace characters
        /// </summary>
        /// <param name="value">String value to check</param>
        /// <returns>Returns true if the string contains any non whitespace character</returns>
        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static string ConvertLikeExpression(string expression)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("^");

            for (int pos = 0; pos < expression.Length; pos++)
            {
                switch (expression[pos])
                {
                    case '?':
                        sb.Append(".");
                        break;

                    case '*':
                        sb.Append(".*");
                        break;

                    case '#':
                        sb.Append(@"\d");
                        break;

                    case '[':
                        StringBuilder gsb = ConvertGroupSubexpression(expression, ref pos);

                        // Skip groups of form [], i.e. empty strings
                        if (gsb.Length > 2)
                            sb.Append(gsb);
                        break;
                    default:
                        sb.Append(Regex.Escape(expression[pos].ToString()));
                        break;
                }
            }

            sb.Append("$");

            return sb.ToString();
        }



        private static StringBuilder ConvertGroupSubexpression(string expression, ref int pos)
        {
            StringBuilder sb = new StringBuilder();

            // Pattern is only a "["
            if (pos++ == expression.Length)
                throw new ArgumentException("Argument 'Pattern' is not a valid value.");

            sb.Append("[");

            // Negate if needed
            if (expression[pos] == '!')
            {
                sb.Append("^");
                pos++;
            }

            while (pos < expression.Length && expression[pos] != ']')
                sb.Append(Regex.Escape(expression[pos++].ToString()));

            // "]" not found in string
            if (pos == expression.Length)
                throw new ArgumentException("Argument 'Pattern' is not a valid value.");

            return sb.Append("]");
        }
        /// <summary>
        /// Applies a mask to a given string. The mask-character is '.' (dot).
        /// <para>
        ///     Comparing the string positions of the input string and the mask string.
        ///     If the mask string has a ., the input strings character will be used, otherwise it will be removed!
        ///     </para>
        /// </summary>
        /// <param name="value">Inputstring to apply the mask</param>
        /// <param name="mask">Mask which should be applied</param>
        /// <returns>The masked version of the input string</returns>
        public static string ApplyMask(this string value, string mask)
        {
            if (string.IsNullOrEmpty(mask))
                return value;

            string result = "";
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask.Substring(i, 1).Equals(".") && i <= value.Length - 1)
                    result = $"{result}{value.Substring(i, 1)}";
            }

            return result;
        }
        /// <summary>
        /// May be used to mask fields in an inputstring by replacing its current value with the mask
        /// </summary>
        /// <param name="value">Inputstring to apply the field mask</param>
        /// <param name="field">Field which should be masked</param>
        /// <param name="mask">Mask which should be applied</param>
        /// <returns>A masked version of the string</returns>
        public static string MaskField(this string value, string field, string mask = "***")
        {
            if (string.IsNullOrEmpty(mask))
                return value;

            var separators = ",;";
            var sb = new StringBuilder();

            foreach (var keyValue in Regex.Split(value, $"(?<=[{separators}])"))
            {
                var temp = keyValue;
                var index = keyValue.IndexOf("=");
                if (index > 0)
                {
                    var key = keyValue.Substring(0, index);
                    if (string.Compare(key.Trim(), field.Trim(), true) == 0)
                    {
                        var end = separators.Contains(keyValue.Last()) ? keyValue.Last().ToString() : "";
                        temp = key + "=" + mask + end;
                    }
                }

                sb.Append(temp);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Converts a given file name to a unique filename including a specifyable name prefix and
        /// by adding the current system ticks
        /// </summary>
        /// <param name="filePath">Filename to convert in a unique name</param>
        /// <param name="nameprefix">Name prefix which should be used</param>
        /// <returns></returns>
        public static string ToUniqueFileName(this string filePath, string nameprefix)
        {
            string file = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            if (string.IsNullOrEmpty(extension))
            {
                return Path.Combine(Path.GetDirectoryName(filePath), "{0}{1}{2}".FormatWith(file, nameprefix, DateTime.Now.Ticks));
            }

            return Path.Combine(Path.GetDirectoryName(filePath), "{0}{1}{2}{3}".FormatWith(file, nameprefix, DateTime.Now.Ticks, extension));
        }

        /// <summary>
        /// Validates if an input string matches at least ONE of the supplied patterns
        /// </summary>
        /// <param name="input">String to validate</param>
        /// <param name="patterns">Patters which should match</param>
        /// <returns></returns>
        public static bool ValidateAgainstPatterns(this string input, string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                if (input.IsLike(pattern))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Split a string which is given by pascal case notation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SplitPascalCaseText(this string text)
        {
            return Regex.Replace(text, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Returns null if the string is null or empty
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static string NullIfEmpty(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            return input;
        }

        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        /// <param name="hex">hexadecimal string</param>
        /// <returns>Byte array represented by the string. Throws exception if invalid characters are found</returns>
        [DebuggerStepThrough]
        public static byte[] HexToByteArray(this string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// removes all curly brackets from a string
        /// </summary>
        /// <param name="source">Input string</param>
        /// <returns></returns>
        public static string RemoveCurlyBrackets(this string source)
        {
            if (source.IsPresent())
            {
                return source.Replace("{", "").Replace("}", "");
            }

            return source;
        }

        /// <summary>
        /// removes all invalid characters which are not allowed to be used within a property name
        /// </summary>
        /// <param name="source">Input string which should be used to convert into a valid property name</param>
        /// <returns>A valid property name</returns>
        public static string SanitizePropertyName(this string source)
        {
            if (source.IsPresent())
            {
                return String.Join("", source.AsEnumerable()
                                        .Select(chr => Char.IsLetter(chr) || Char.IsDigit(chr) || chr == '_'
                                                       ? chr.ToString()      // valid symbol
                                                       : string.Empty) // remove invalid symbol
                                  );
            }

            return source;
        }

        /// <summary>
        /// Extract the build number from a bamboo build version string
        /// </summary>
        /// <remarks>
        /// Build number format is something like:
        ///     AD-AJ2-15
        ///     
        /// Then 15 will be returned as build number.
        /// </remarks>
        /// <param name="buildVersion">Bamboo build version string</param>
        /// <returns>The build number as integer. 0 is the default value.</returns>
        public static int ExtractBuildNumber(this string buildVersion)
        {
            var build = 0;

            if (buildVersion.IsPresent() && buildVersion.LastIndexOf("-") >= 0)
            {
                int.TryParse(buildVersion.Substring(buildVersion.LastIndexOf("-") + 1), out build);
            }
            else if (buildVersion.IsPresent())
            {
                int.TryParse(buildVersion, out build);
            }
            return build;
        }

        /// <summary>
        /// Set the first character in a string to lower
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>Converted string</returns>
        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
            {
                return str;
            }

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Set the first character in a string to lower
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>Converted string</returns>
        public static string FirstCharacterToUpper(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
            {
                return str;
            }

            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Split a string in equal chink sizes
        /// </summary>
        /// <param name="str">String to create chunks from</param>
        /// <param name="chunkLength">Chunk size</param>
        /// <returns>A list with the chunks extracted</returns>
        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            str.CheckArgumentNullOrEmpty(nameof(str));
            if (chunkLength < 1) throw new ArgumentException(nameof(chunkLength));

            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }
        /// <summary>
        /// Check if a given input string is base64 formatted
        /// </summary>
        /// <param name="data">String data to check</param>
        /// <returns>True if the string is base64 encoded.</returns>
        [DebuggerStepThrough]
        public static bool IsBase64(this string data)
        {
            if (data.IsPresent())
            {
                Span<byte> buffer = new Span<byte>(new byte[data.Length]);
                return Convert.TryFromBase64String(data, buffer, out int bytesParsed);
            }

            return false;
        }


        /// <summary>
        /// Zips a string and returns a base64 representation
        /// </summary>
        /// <param name="data">String data to zip</param>
        /// <returns>A base64 encoded, zip compressed version of the zip</returns>
        [DebuggerStepThrough]
        public static string ZipToBase64(this string data)
        {
            if (String.IsNullOrEmpty(data))
                return null;

            return Convert.ToBase64String(ZipUtility.Zip(data));
        }

        /// <summary>
        /// Unzip a base64 encoded compressed string
        /// </summary>
        /// <param name="data">Base64 encoded compressed data</param>
        /// <returns>The unzipped string</returns>
        [DebuggerStepThrough]
        public static string UnZipFromBase64(this string data)
        {
            if (String.IsNullOrEmpty(data))
                return null;

            var bytes = Convert.FromBase64String(data);
            return ZipUtility.Unzip(bytes);
        }

        /// <summary>
        /// Check if an input string represents a valid json object or array
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsValidJson(this string stringValue)
        {
            if (!stringValue.IsPresent())
                return false;

            stringValue = stringValue.Trim();
            if ((stringValue.StartsWith("{") && stringValue.EndsWith("}")) //For object  
                 ||
                 (stringValue.StartsWith("[") && stringValue.EndsWith("]")) //For array
               )
            {
                try
                {
                    JsonDocument.Parse(Encoding.UTF8.GetBytes(stringValue));
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Try to parse a json string and return the object of type
        /// </summary>
        /// <typeparam name="T">Type of object which should be parsed</typeparam>
        /// <param name="this">string instance</param>
        /// <param name="result">Objet receiving parsing results</param>
        /// <returns>True if succeed, otherwise false</returns>
        [DebuggerStepThrough]
        public static bool TryParseJson<T>(this string @this, out T result)
        {
            if (@this.IsMissing())
            {
                result = default(T);
                return false;
            }

            bool success = true;
            try
            {
                result = JsonSerializer.Deserialize<T>(@this);
            }
            catch (JsonException)
            {
                success = false;
                result = default(T);
            }

            return success;
        }
    }
}
