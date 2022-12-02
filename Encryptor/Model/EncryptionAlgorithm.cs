/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |   Encryption Algorithm Base Class   |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

namespace Encryptor.Model;

/// <summary>
/// Abstract class <c>EncryptionAlgorithm</c> describes basic functions to be implemented in every encryption algorithm
/// </summary>
public abstract class EncryptionAlgorithm
{
    // Text processing method
    public abstract string Process(string value);
}
