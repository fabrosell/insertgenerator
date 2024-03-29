using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security;

namespace Suru.Common.EncryptionLibrary
{

    //Symetric methods: to read / write encrypted data
    //Asymetric methods: to store salt and password to use with symetric methods.

    public class Encryption
    {
        /// <summary>
        /// Encrypts data with symetric key (Rijndael algorithm).
        /// </summary>
        /// <param name="sToEncript">Text to encrypt.</param>
        /// <returns>Encripted Base64 encrypted String.</returns>
        public static String SymetricEncrypt(String sToEncript)
        {
            String Salt = AsymetricDecrypt(ConfigurationManager.AppSettings.Get("Salt"));
            String Pass = AsymetricDecrypt(ConfigurationManager.AppSettings.Get("Pass"));

            // Create the password key
            Byte[] saltValueBytes = Encoding.ASCII.GetBytes(Salt);
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes(Pass, saltValueBytes);

            // Create the algorithm and specify the key and IV
            RijndaelManaged alg = new RijndaelManaged();
            alg.Key = passwordKey.GetBytes(alg.KeySize / 8);
            alg.IV = passwordKey.GetBytes(alg.BlockSize / 8);


            MemoryStream inFile = new MemoryStream(Encoding.ASCII.GetBytes(sToEncript));
            inFile.Flush();                         

            //FileStream inFile = new FileStream(inFilename, FileMode.Open, FileAccess.Read);
            Byte[] fileData = new byte[inFile.Length];
            inFile.Read(fileData, 0, (Int32)inFile.Length);

            //Just ended using inFile. Close & Dispose it.
            inFile.Close();
            inFile.Dispose();

            ICryptoTransform encryptor = alg.CreateEncryptor();

            MemoryStream outFile = new MemoryStream();
            CryptoStream encryptStream = new CryptoStream(outFile, encryptor, CryptoStreamMode.Write);            

            encryptStream.Write(fileData, 0, fileData.Length);
            encryptStream.FlushFinalBlock();

            Byte[] cipherTextBytes = outFile.ToArray();

            outFile.Close();
            outFile.Dispose();
            encryptStream.Close();
            encryptStream.Dispose();

            return Convert.ToBase64String(cipherTextBytes);
        }
        
        /// <summary>
        /// Decrypts data encrypted in above method (Rijndael algorithm)
        /// </summary>
        /// <param name="sEncripted">Encripted Base64 String.</param>
        /// <returns>Decrypted text.</returns>
        public static String SymetricDecrypt(String sEncripted)
        {
            String Salt = AsymetricDecrypt(ConfigurationManager.AppSettings.Get("Salt"));
            String Pass = AsymetricDecrypt(ConfigurationManager.AppSettings.Get("Pass"));

            // Create the password key
            Byte[] saltValueBytes = Encoding.ASCII.GetBytes(Salt);
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes(Pass, saltValueBytes);

            // Create the algorithm and specify the key and IV
            RijndaelManaged alg = new RijndaelManaged();
            alg.Key = passwordKey.GetBytes(alg.KeySize / 8);
            alg.IV = passwordKey.GetBytes(alg.BlockSize / 8);                

            // Read the encrypted file Int32o fileData
            ICryptoTransform decryptor = alg.CreateDecryptor();
            
            MemoryStream inFile = new MemoryStream(Convert.FromBase64String(sEncripted));
            inFile.Flush();

            //FileStream inFile = new FileStream(inFilename, FileMode.Open, FileAccess.Read);
            CryptoStream decryptStream = new CryptoStream(inFile, decryptor, CryptoStreamMode.Read);
            Byte[] fileData = new Byte[inFile.Length];

            Int32 DecryptedByteCount = decryptStream.Read(fileData, 0, (Int32)inFile.Length);

            //Byte[] uncipheredText = outFile.ToArray();

            decryptStream.Close();
            decryptStream.Dispose();
            inFile.Close();
            inFile.Dispose();

            return Encoding.UTF8.GetString(fileData, 0, DecryptedByteCount);
        }

        /// <summary>
        /// Initialize the RSA algorithm parameters. 
        /// NET framework automatically handles creation / search for named CSP container,
        /// the same RSACryptoServiceProvided will be returned every time.
        /// </summary>
        /// <returns>RSA crypto service provider for encrypt / decrypt.</returns>
        private static RSACryptoServiceProvider InitializeRSAParameters()
        {
            // Create a CspParameters object
            CspParameters persistantCsp = new CspParameters();

            persistantCsp.KeyContainerName = "Insert_Generator_CSP_Parameters_by_Suru";
            
            // Create an instance of the RSA algorithm object
            RSACryptoServiceProvider RSAEncryption = new RSACryptoServiceProvider(persistantCsp);

            // Specify that the private key should be stored in the CSP
            RSAEncryption.PersistKeyInCsp = true;

            // Create a new RSAParameters object with the private key
            RSAParameters privateKey = RSAEncryption.ExportParameters(true);

            return RSAEncryption;
        }

        /// <summary>
        /// Encrypts using asymetric encription (RSA).
        /// </summary>
        /// <param name="sToEncript">String to encrypt</param>
        /// <returns>Encripted String</returns>
        public static String AsymetricEncrypt(String sToEncript)
        {
            RSACryptoServiceProvider RSAProvider = InitializeRSAParameters();

            byte[] messageBytes = Encoding.Unicode.GetBytes(sToEncript); 
            byte[] encryptedMessage = RSAProvider.Encrypt(messageBytes, false);            

            return Convert.ToBase64String(encryptedMessage);
        }

        /// <summary>
        /// Decrypts text encrypted with above asymetric encription (RSA)
        /// </summary>
        /// <param name="sEncripted">Encripted text to decrypt.</param>
        /// <returns>Unencrypted string.</returns>
        public static String AsymetricDecrypt(String sEncripted)
        {
            RSACryptoServiceProvider RSAProvider = InitializeRSAParameters();

            byte[] messageBytes = Convert.FromBase64String(sEncripted);
            byte[] decryptedBytes = RSAProvider.Decrypt(messageBytes, false);

            return Encoding.Unicode.GetString(decryptedBytes);
        }

        /// <summary>
        /// This method generates and save two random Salt and Password keys for Symetric Encryption uses.
        /// </summary>
        public static void ResetCryptoKeys()
        {
            try
            {
                Random RandGen = new Random();
                Int32 Rand1, Rand2;

                StringBuilder sb_Salt = new StringBuilder();
                StringBuilder sb_Pass = new StringBuilder();

                #region Generate Random Salt and Pass 32 chars long with alfanumeric ascii codes

                for (Int32 i = 0; i < 32; i++)
                {
                    Rand1 = 0;
                    Rand2 = 0;

                    //Omit symbols
                    while ((Rand1 > 57 && Rand1 < 65) ||
                           (Rand1 > 90 && Rand1 < 97) || Rand1 == 0)
                    {
                        Rand1 = RandGen.Next(74);
                        Rand1 += 48;
                    }

                    while ((Rand2 > 57 && Rand2 < 65) ||
                           (Rand2 > 90 && Rand2 < 97) || Rand2 == 0)
                    {
                        Rand2 = RandGen.Next(74);
                        Rand2 += 48;
                    }

                    sb_Salt.Append(Char.ConvertFromUtf32(Rand1));
                    sb_Pass.Append(Char.ConvertFromUtf32(Rand2));
                }

                #endregion

                //Encrypt and save new Salt and Pass
                String ConfigName = null;
                DirectoryInfo dInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);                

                foreach (FileInfo f in dInfo.GetFiles())
                {
                    if (f.Extension == ".config")
                    {
                        ConfigName = f.Name;
                        break;
                    }
                }

                if (ConfigName == null)
                    throw new ApplicationException("Can't found configuration file.");

                //Getting Configuration file is not as easy as in application
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

                fileMap.ExeConfigFilename = ConfigName;

                Configuration ConfigurationFile = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                String NewSalt = AsymetricEncrypt(sb_Salt.ToString());
                String NewPass = AsymetricEncrypt(sb_Pass.ToString());

                ConfigurationManager.AppSettings.Set("Salt", NewSalt);
                ConfigurationFile.AppSettings.Settings["Salt"].Value = NewSalt;

                ConfigurationManager.AppSettings.Set("Pass", NewPass);
                ConfigurationFile.AppSettings.Settings["Pass"].Value = NewPass;

                ConfigurationFile.Save();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in ResetCryptoKeys: " + ex.Message);
            }
        }
    }
}
