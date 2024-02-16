using System;
using System.Text;

namespace LogicTest
{
    public class LogicTest
    {
        /*
        function Name : 
        ReverseString
        Param :
        string - inputStr
        return :
        string 
        */
        public static string ReverseString(string inputStr)
        {

            // validasi untuk cek string null atau kosong
            if (string.IsNullOrEmpty(inputStr))
            {
                // throw error
                throw new ArgumentNullException(nameof(inputStr), "Input tidak boleh null atau kosong.");
            }

            // membuar variabel penampung dengan StringBuilder, ini akan membuat performa lebih baik 
            // karena secara memori sudah didefinisikan/alokasikan berdasarkan panjang inputStr
            var reversedStringBuilder = new StringBuilder(inputStr.Length);

            // looping inputString dari index terakhir sampe index awal, kemudian append ke dalam variabel yang di definisikan
            for (int i = inputStr.Length - 1; i >= 0; i--)
            {
                reversedStringBuilder.Append(inputStr[i]);
            }

            // return hasil
            return reversedStringBuilder.ToString();
        }



        /*
        function Name : 
        IsPalindrome
        Param :
        string - inputStr
        return :
        boolean      
        */
        public static bool IsPalindrome(string inputStr)
        {

            // validasi untuk cek string null atau kosong
            if (string.IsNullOrEmpty(inputStr))
            {
                // throw error
                throw new ArgumentNullException(nameof(inputStr), "Input tidak boleh null atau kosong.");
            }

            // huruf dengan panjang 1 dikatakan palindrome, karena bisa dibolak balik sama
            if (inputStr.Length <= 1)
            {
                return true;
            }

            // inisialisasi pointer 
            int start = 0;
            int end = inputStr.Length - 1;

            // looping, keaadaan akan terus berjalan selama ppointer tidak bertemu / masih ada kata yang perlu dibandingkan
            while (start < end)
            {
                // abaikan yang bukan alphanumeric
                while (!char.IsLetterOrDigit(inputStr[start]) && start < end)
                {
                    start++;
                }
                while (!char.IsLetterOrDigit(inputStr[end]) && start < end)
                {
                    end--;
                }

                // bandingkan kata dengan pointer yang sedang berjalan
                if (char.ToLower(inputStr[start]) != char.ToLower(inputStr[end]))
                {
                    return false; // bukan palindrome jika berbeda
                }
                start++;
                end--;
            }
            return true; // return palindrome
        }


        /*
        function Name : 
        GeneratePrimes
        Param :
        int - limit
        return :
        list angka bilangan prima     
        */
        public static List<int> GeneratePrimes(int limit)
        {

            // validasi angka kurang dari 2
            if (limit < 2)
            {
                throw new ArgumentException("Limit harus lebih besar dari atau sama dengan 2.");
            }

            // deklarasi variabel untuk menampun angka bilangan prima
            List<int> primesList = new List<int>();


            // Looping untuk menguji setiap angka dari 2 hingga limit
            for (int num = 2; num <= limit; num++)
            {
                // deklarasi awal sebagai num awal sebagai prima
                bool isPrime = true;

                // Loop untuk mencari pembagi dari 2 hingga akar kuadrat dari angka tersebut
                for (int divisor = 2; divisor * divisor <= num; divisor++)
                {
                    // kenapa diviosr * divisor, agar tidak melakukan loop / iterasi yang tidak perlu
                    // ketika habis dibagi 0 berarti bukan prima
                    if (num % divisor == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                // jika tidak terpenuhi semua logic diatas berarti bilangan prima
                if (isPrime)
                {
                    primesList.Add(num);
                }
            }

            return primesList;
        }
    }
}
