using System;
using System.Text;
using System.Collections.Generic;
using Suru.Common.EncryptionLibrary;

namespace Suru.InsertGenerator.ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test_Asymetric_Encryption_Class();
            //Test_Symetric_Encryption_Class();
            Test_Salt_and_Pass_Generator();

            Console.WriteLine();
            Console.WriteLine("Press ENTER key to close.");
            Console.ReadLine();
        }

        //Test Symetric encryption / decryption features
        private static void Test_Symetric_Encryption_Class()
        {
            //El método soporta más de 255 caracteres...
            String TextToEncrypt = "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.SymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Symetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Symetric Decrypted text: " + Encryption.SymetricDecrypt(EncryptedText));
        }

        //Test Asymetric encryption / decryption features
        private static void Test_Asymetric_Encryption_Class()
        {
            String TextToEncrypt = "PassPassPass";

            Console.WriteLine("Text to process: " + TextToEncrypt);

            String EncryptedText = Encryption.AsymetricEncrypt(TextToEncrypt);

            Console.WriteLine("Asymetric Encrypted text: " + EncryptedText);

            Console.WriteLine("Asymetric Decrypted text: " + Encryption.AsymetricDecrypt(EncryptedText));
        }

        private static void Test_Salt_and_Pass_Generator()
        {
            Encryption.ResetCryptoKeys();
        }
    }
}
