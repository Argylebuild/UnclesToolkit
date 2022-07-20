using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Argyle.Utilities
{
    /// <summary>
    /// This is an incomplete code. Not sure best way to store the key or encrypt the data.
    /// </summary>
    public static class CryptKeeper 
    {
        static byte[] Key => new byte[8] {4,7,3,9,1,0,4,3};
        
        public static string EnCrypt(this string text)
        {
            string result = null;
            
            if (!String.IsNullOrEmpty(text))
            {
                byte[] plaintextBytes = Encoding.ASCII.GetBytes(text);
                //byte[] textBytesLength = BitConverter.GetBytes(plaintextBytes.Length); //padding to encryprion

                SymmetricAlgorithm symmetricAlgorithm = DES.Create();
                symmetricAlgorithm.Key = Key;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        //cryptoStream.Write(textBytesLength, 0, 8);
                        cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                    }

                    result = Encoding.ASCII.GetString(memoryStream.ToArray());
                }
            }

            return result;
        }

        public static string Decrypt(this string text)
        {
            string result = null;

            if (!String.IsNullOrEmpty(text))
            {
                byte[] encryptedBytes = Encoding.ASCII.GetBytes(text);

                SymmetricAlgorithm symmetricAlgorithm = DES.Create();
                symmetricAlgorithm.Key = Key;
                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] decryptedBytes = new byte[encryptedBytes.Length];
                        cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                        result = Encoding.ASCII.GetString(decryptedBytes);
                    }
                }
            }

            return result;
        }
        
        
        public static void Main()
        {
            try
            {

                string original = "Here is some data to encrypt!";

                // Create a new instance of the Rijndael
                // class.  This generates a new key and initialization
                // vector (IV).
                using (Rijndael myRijndael = Rijndael.Create())
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes(original, myRijndael.Key, myRijndael.IV);

                    // Decrypt the bytes to a string.
                    string roundtrip = DecryptStringFromBytes(encrypted, myRijndael.Key, myRijndael.IV);

                    //Display the original data and the decrypted data.
                    Console.WriteLine("Original:   {0}", original);
                    Console.WriteLine("Round Trip: {0}", roundtrip);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
