using System;
using System.Collections.Generic;
using System.Text;

//*****cs 코드를 두개 써서 하나 빌드하려면 다른 코드를 프로젝트에서 제외해야됨.

/*변수타입을 알수 있는것 switch를 이용해서 c#7.0 사용하지 않고 만들수있도록 하는것TestFN3(10, 20, "30", 40); <- 가변적으로 늘어날수 있음            >> 출력 : 10 + 20 + 30 + 40 = 100magotoolivesource@gmail.com2A_프로그램기초_이름.zip

 */
namespace ConsoleApp1
{
    class Guaze2
    {
        static void Sum(params Object[] val)
        {
            int sum = 0;
            for (int i = 0; i < val.Length; i++)
            {
                int sumNum;
                if (val[i].GetType() == typeof(string))
                    sumNum = int.Parse((string)val[i]);
                else
                    sumNum = (int)val[i];
                sum += sumNum;
            }
            Console.WriteLine("총 합계는 {0}입니다.\n", sum);
        }

        static void Main(string[] args)
        {
            Guaze2.Sum(10, 20, "30", 40);
        }
    }
}
