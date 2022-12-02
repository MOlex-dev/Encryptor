/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |       Caesar Cipher Algorithm       |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System.Text;

namespace Encryptor.Model;

/// <summary>
/// Class <c>CaesarCipher</c> provides configurable Caesar Cipher algorithm
/// </summary>
public class CaesarCipher : EncryptionAlgorithm
{
    private int _shift;
    private readonly byte _additionalShift;

    /// <param name="shift">0 means Progressive Shift</param>
    public CaesarCipher(int shift)
    {
        _shift = shift;
        _additionalShift = (byte)(_shift == 0 ? 1 : 0);
    }
    
    public override string Process(string defaultString)
    {
        StringBuilder str = new StringBuilder(defaultString);

        for (int i = 0; i < str.Length; ++i)
        {
            _shift += _additionalShift;
            
            if (!char.IsLetter(str[i])) continue;

            if (char.IsLower(str[i]))
            {
                str[i] = (char)((str[i] - 'a' + _shift) % 26 + 'a');
                continue;
            }
            str[i] = (char)((str[i] - 'A' + _shift) % 26 + 'A');
        }

        return str.ToString();
    }
}