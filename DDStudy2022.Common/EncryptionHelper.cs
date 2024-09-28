using System.Security.Cryptography;
using System.Text;

namespace DDStudy2022.Common;

public class EncryptionHelper
{
    public static RSA GetRsa(string privateKeyString, string publicKeyString)
    {
        var rsa = RSA.Create();
        
        rsa.ImportRSAPrivateKey(
            source: Encoding.UTF8.GetBytes(privateKeyString), 
            bytesRead: out _
            );
        
        rsa.ImportRSAPublicKey(
            source: Encoding.UTF8.GetBytes(publicKeyString), 
            bytesRead: out _
            );

        return rsa;
    }
}