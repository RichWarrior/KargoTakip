using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip
{
    public class helper
    {

        /*Async Metod İle Veri Çekme İşlemleri Tamamiyle Çözüldü!
         * Yeni Versiyon Yazarsam Editlenecek!
         * 
         * 
         * 
         * */
        public string crypto(string pass)
        {
            //Şifrelemeyi MD5 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            StringBuilder sb = new StringBuilder();
            byte[] byt = Encoding.UTF8.GetBytes(pass);
            byt = md5.ComputeHash(byt);
            foreach (byte b in byt)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            //Şifreleme Bitti
            return sb.ToString();
        }
    }
}
