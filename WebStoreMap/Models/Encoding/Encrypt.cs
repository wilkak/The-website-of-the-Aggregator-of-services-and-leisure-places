using System.Security.Cryptography;

namespace WebStoreMap.Models.Encoding
{
    public static class Encrypt
    {
        public static string GetMD5Hash(string Input)
        {
            using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
            {
                byte[] Text = System.Text.Encoding.UTF8.GetBytes(Input);
                Text = MD5.ComputeHash(Text);
                System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
                foreach (byte x in Text)
                {
                    _ = StringBuilder.Append(x.ToString("x2"));
                }

                return StringBuilder.ToString();
            }
        }
    }
}