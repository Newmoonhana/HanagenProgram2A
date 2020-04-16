using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //변수 선언.
            Program p = new Program();
            int a = 10;
            float b = 0.1f;
            string c = "abcd";

            Console.WriteLine("\n");
            //GetType() 사용.
            p.Get_Type(a);
            p.Get_Type(b);
            p.Get_Type(c);
        }

        void Get_Type(Object val)   //GetType 사용 함수.
        {
            Console.WriteLine("{0}", val.GetType());
        }
    }
}
