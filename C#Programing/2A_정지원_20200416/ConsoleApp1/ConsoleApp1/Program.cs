using System;

//*****cs 코드를 두개 써서 하나 빌드하려면 다른 코드를 프로젝트에서 제외해야됨.

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

            //GetType() 사용.
            p.Get_Type(a);
            p.Get_Type(b);
            p.Get_Type(c);
        }

        void Get_Type(Object val)   //GetType 사용 함수.
        {
            string valS;
            switch(val.GetType().ToString())
            {
                case "System.Int32":
                    valS = "int";
                    break;
                case "System.Single":
                    valS = "float";
                    break;
                case "System.String":
                    valS = "string";
                    break;
                default:
                    valS = "*ERROR*";
                    break;
            }
            Console.WriteLine("{0}", valS);
        }
    }
}
