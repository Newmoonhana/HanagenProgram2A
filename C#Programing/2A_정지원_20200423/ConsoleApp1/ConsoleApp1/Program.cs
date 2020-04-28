using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class MainClass
    {
        static void Main(string[] args)
        {
            //과제 1.
            string tempstr = null;
            string tempstr2 = "";
            bool isflag1 = tempstr.isempty();	//true나 false 호출.
            bool isflag2 = tempstr2.isempty();	//true나 false 호출.
            Console.WriteLine("tempstr = {0}", isflag1);
            Console.WriteLine("tempstr2 = {0}\n", isflag2);

            //과제 2.
            vectorint aa = new vectorint();
            aa.push(10);
            aa.push(20);
            aa.push(30);
            aa.outstring(); //10, 20, 30
            aa.removeat(1);
            aa.outstring(); //10, 30
            aa.clear();
            aa.outstring(); //empty
        }
    }

    public static class Program
    {
        //과제 1.
        public static bool isempty(this string s)
        {
            if (s == null)
                return true;
            else
                return false;
        }
    }

    //과제 2.
    public class vectorint
    {
        public List<int> val = new List<int>();
        public int size = 0;
    }

    public static class vector
    {
        public static void push(this vectorint v, int a)
        {
            v.val.Add(a);
            v.size++;
        }

        public static void outstring(this vectorint v)
        {
            if (v.size == 0)
            {
                Console.WriteLine("empty");
                return;
            }
            string sum = v.val[0].ToString();
            for (int i = 1; i < v.size; i++)
            {
                sum = sum + " " + v.val[i].ToString();
            }
            Console.WriteLine("{0}", sum);
        }

        public static void removeat(this vectorint v, int a)
        {
            v.val.RemoveAt(a);
            v.size--;
        }

        public static void clear(this vectorint v)
        {
            v.val.Clear();
            v.size = 0;
        }
    }
}
