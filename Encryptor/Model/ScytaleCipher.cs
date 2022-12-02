/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |           Scytale Cipher            |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System.Collections.Generic;
using System.Text;

namespace Encryptor.Model;

/// <summary>
/// Class <c>ScytaleCipher</c> provides configurable Scytale Cipher algorithm
/// </summary>
public class ScytaleCipher : EncryptionAlgorithm
{
    private readonly int _scytaleSides;

    public ScytaleCipher(int sides) { _scytaleSides = sides; }
    
    public override string Process(string defaultString)
    {
        List<StringBuilder> scytale = new List<StringBuilder>(_scytaleSides);

        for (int i = 0; i < _scytaleSides; ++i)
            scytale.Add(new StringBuilder());

        for (int i = 0; i < defaultString.Length; ++i)
            scytale[i % _scytaleSides].Append(defaultString[i]);

        StringBuilder resulter = new StringBuilder(defaultString.Length);
        foreach (var str in scytale)
            resulter.Append(str);
        
        return resulter.ToString();
    }
}