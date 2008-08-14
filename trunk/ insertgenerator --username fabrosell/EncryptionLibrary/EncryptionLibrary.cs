using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Suru.Common.EncryptionLibrary
{
    public class EncryptionClass
    {

        public static String Encrypt(String sToEncript)
        {
            // Create a new Rijndael object.
            Rijndael RijndaelAlg = Rijndael.Create();

            RijndaelAlg.

            //RijndaelAlg.CreateEncryptor(Key, IV)

            return sToEncript;
        }

        public static String Decrypt(String sEncripted)
        {
            return sEncripted;
        }
    }
}
