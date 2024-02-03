using System.Text;
using System.Security.Cryptography;


namespace SPTrEngine.Tools
{
    public static class HashMaker
    {
        public static string ComputeSHA256(string s)
        {
            string hash = string.Empty;

            // SHA256 해시 객체 초기화
            using (SHA256 sha256 = SHA256.Create())
            {
                // 주어진 문자열의 해시를 계산합니다.
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                // 바이트 배열을 문자열 형식으로 변환
                foreach (byte b in hashValue)
                {
                    hash += $"{b:X2}";
                }
            }

            return hash;
        }

    }
}