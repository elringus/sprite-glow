// Copyright 2012-2017 Elringus (Artyom Sovetnikov). All Rights Reserved.

namespace UnityCommon
{
    using System;
    using UnityEngine;
    
    public static class StringUtils
    {
        /// <summary>
        /// Whether compared string is literally-equal (independent of case).
        /// </summary>
        public static bool LEquals (this string content, string comparedString)
        {
            Debug.Assert(content != null);
            return content.Equals(comparedString, StringComparison.OrdinalIgnoreCase);
        }
    
        /// <summary>
        /// More performant version of string.EndsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool EndsWithFast (this string content, string match)
        {
            int ap = content.Length - 1;
            int bp = match.Length - 1;
    
            while (ap >= 0 && bp >= 0 && content[ap] == match[bp])
            {
                ap--;
                bp--;
            }
    
            return (bp < 0 && content.Length >= match.Length) || (ap < 0 && match.Length >= content.Length);
        }
    
        /// <summary>
        /// More performant version of string.StartsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool StartsWithFast (this string content, string match)
        {
            int aLen = content.Length;
            int bLen = match.Length;
            int ap = 0, bp = 0;
    
            while (ap < aLen && bp < bLen && content[ap] == match[bp])
            {
                ap++;
                bp++;
            }
    
            return (bp == bLen && aLen >= bLen) || (ap == aLen && bLen >= aLen);
        }
    
        /// <summary>
        /// Attempts to extract content between the specified matches (on first occurence).
        /// </summary>
        public static string GetBetween (this string content, string startMatchString, string endMatchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(startMatchString) && content.Contains(endMatchString))
            {
                var startIndex = content.IndexOf(startMatchString) + startMatchString.Length;
                var endIndex = content.IndexOf(endMatchString, startIndex);
                return content.Substring(startIndex, endIndex - startIndex);
            }
            else return null;
        }
    
        /// <summary>
        /// Attempts to extract content before the specified match (on first occurence).
        /// </summary>
        public static string GetBefore (this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var endIndex = content.IndexOf(matchString);
                return content.Substring(0, endIndex);
            }
            else return null;
        }
    
        /// <summary>
        /// Attempts to extract content before the specified match (on last occurence).
        /// </summary>
        public static string GetBeforeLast (this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var endIndex = content.LastIndexOf(matchString);
                return content.Substring(0, endIndex);
            }
            else return null;
        }
    
        /// <summary>
        /// Attempts to extract content after the specified match (on last occurence).
        /// </summary>
        public static string GetAfter (this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.LastIndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }
    
        /// <summary>
        /// Attempts to extract content after the specified match (on first occurence).
        /// </summary>
        public static string GetAfterFirst (this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.IndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }
    
        /// <summary>
        /// Splits the string using new line symbol as a separator.
        /// Will split by all type of new lines, independant of environment.
        /// </summary>
        public static string[] SplitByNewLine (this string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
    
            // "\r\n"   (\u000D\u000A)  Windows
            // "\n"     (\u000A)        Unix
            // "\r"     (\u000D)        Mac
            // Not using Environment.NewLine here, as content could've been produced 
            // in not the same environment we running the program in.
            return content.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        }
    
        /// <summary>
        /// Checks whether string is null, empty or consists of whitespace chars.
        /// </summary>
        public static bool IsNullEmptyOrWhiteSpace (string content)
        {
            if (String.IsNullOrEmpty(content))
                return true;
    
            return String.IsNullOrEmpty(content.Trim());
        }
    
        /// <summary>
        /// Removes mathing trailing string.
        /// </summary>
        public static string TrimEnd (this string source, string value)
        {
            if (!source.EndsWithFast(value))
                return source;
    
            return source.Remove(source.LastIndexOf(value));
        }
    }
    
}
