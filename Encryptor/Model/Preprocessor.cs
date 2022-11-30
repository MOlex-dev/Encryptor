using System;
using System.Collections.Generic;
using System.Text;

namespace Encryptor.Model;

/// <summary>
/// Class <c>Preprocessor</c> processes default text (removes whitespaces, etc...) before encryption
/// </summary>
public static class Preprocessor
{
    /// <summary>
    /// Struct, describing preprocessor settings
    /// </summary>
    public struct PreprocessorSettings
    {
        public bool RemoveWhitespaces { get; set; } = false;
        public bool RemovePunctuation { get; set; } = false;
        public bool RemoveSymbols { get; set; } = false;
        public CaseChanging ChangeCase { get; set; } = CaseChanging.NotChange;

        
        public PreprocessorSettings()
        { }
        
        /// <summary>
        /// Method, checking are settings of current instance equals to default
        /// </summary>
        public bool IsDefault() =>
            Equals(this, default(PreprocessorSettings));

        public override bool Equals(object? obj)
        {
            var other = (PreprocessorSettings)obj;
            return this.RemoveWhitespaces == other.RemoveWhitespaces &&
                   this.RemovePunctuation == other.RemovePunctuation &&
                   this.RemoveSymbols == other.RemoveSymbols &&
                   this.ChangeCase == other.ChangeCase;
        }
    }

    /// <summary>
    /// Message case changing variants
    /// </summary>
    public enum CaseChanging
    {
        NotChange,
        ToLower,
        ToUpper
    }

    private static readonly HashSet<char> _symbols = new HashSet<char>() { '~', '{', '}', '|', '_', '-', '+', '^', '[',
                                                                           ']', '@', '>', '<', '=', '/', '\\', '*', '%',
                                                                           '$', '&', '#' };
    
    private static readonly HashSet<char> _punctuation = new HashSet<char>() { '!', '\"', '\'', '(', ')', ',', '.', ':',
                                                                               ';', '?', '`' };

    /// <summary>
    /// String processing method
    /// </summary>
    /// <param name="message">text, which should be processed by preprocessor</param>
    /// <param name="settings">preprocessor settings</param>
    /// <returns>processed string</returns>
    public static string ProcessMessage(string message, PreprocessorSettings settings = default)
    {
        if (settings.IsDefault()) return message;
        
        HashSet<char> symbolsToRemove = new();
        
        if (settings.RemoveWhitespaces) symbolsToRemove.Add(' ');
        if (settings.RemoveSymbols) symbolsToRemove.UnionWith(_symbols);
        if (settings.RemovePunctuation) symbolsToRemove.UnionWith(_punctuation);

        message = RemoveCharsFromString(message, symbolsToRemove);
        message = ChangeStringCase(in message, settings.ChangeCase);
        
        return message;
    }
    
    private static string ChangeStringCase(in string str, CaseChanging changeCase) => 
        changeCase switch
        {
            CaseChanging.NotChange => str,
            CaseChanging.ToLower   => str.ToLower(),
            CaseChanging.ToUpper   => str.ToUpper(),
            _ => string.Empty
        };
    
    private static string RemoveCharsFromString(in string str, HashSet<char> symbolsSet)
    {
        StringBuilder builder = new StringBuilder(str);
        foreach (var c in symbolsSet)
        {
            if (c == '.') continue;
            builder.Replace(c, '\0');
        }
        
        // Remove points except in numbers like 12.345
        if (symbolsSet.Contains('.'))
        {
            for (int i = 0; i < builder.Length; ++i)
            {
                if (builder[i] != '.') continue;
                if (i != 0 &&
                    i != builder.Length - 1 &&
                    Char.IsDigit(builder[i - 1]) &&
                    Char.IsDigit(builder[i + 1]))
                    continue;
                builder[i] = '\0';
            }
        }

        for (int i = 0; i < builder.Length; ++i)
        {
            if (builder[i] == '\0')
                builder.Remove(i, 1);
        }

        return builder.ToString();
    }
}
