using System;

namespace LogicTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Soal 1: Reverse String
                // Definisi variabel dengan value string yg akan di reverse
                string NamaSaya = "Ricky Ariansyah";
                // panggil method ReverseString dengan parameter NamaSaya
                string reversedString = LogicTest.ReverseString(NamaSaya);
                // print original string
                Console.WriteLine($"string Asli: {NamaSaya}");
                // print reverse string
                Console.WriteLine($"string Reverse: {reversedString}");


            // Soal 2: Palindrome Check
            // Definisi variabel dengan value string yg akan di cek apakah palindrome
                string palindromeStr = "racecar";
                Console.WriteLine($"Apakah \"{palindromeStr}\" adalah palindrome? {LogicTest.IsPalindrome(palindromeStr)}");



            // soal 3: Prime Number Generator
            // Definisi variabel dengan value int batas angka bilangan prima yang akan dicari
                 int limit = 20;
                Console.WriteLine($"bilangan prima sampai dengan {limit} adalah : {string.Join(", ", LogicTest.GeneratePrimes(limit))}");
        }
    }
}