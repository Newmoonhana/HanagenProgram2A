using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ConsoleApp1
{
    class MainClass
    {
        static void Main(string[] args)
        {
            //과제4 : push(), removeat(), clear(), outstring(), removeatsize(stindex, endindex).
            //프로그래밍기초_2A_과제4_이름.zip.
            //insertat(int p_index, int p_val); aa.insertat()
            //addrange(int p_index, int[] p_val);
            //count(프로퍼티 방식).
            //예외사항 발생.
            vectorint aa = new vectorint();
            aa.push(10);
            aa.push(20);
            aa.push(30);
            aa.insertat(1, 5);  //10, 5, 20, 30;
            int[] a = { 1, 2, 3 };
            aa.addrange(1, a);  //10, 1, 2, 3, 5, 20, 30;
            aa.outstring();  //10, 1, 2, 3, 5, 20, 30;
            Console.WriteLine("size : {0}", aa.count());
            aa.removeat(1);  //10, 2, 3, 5, 20, 30;
            aa.removeatsize(1, 3);  //10, 20, 30;
            aa.outstring();
            Console.WriteLine("size : {0}", aa.count());
            aa.clear();
            aa.outstring(); //empty
            Console.WriteLine("size : {0}", aa.count());
            aa.addrange(0, a);
            aa.removeat(4);  //error;
            aa.removeatsize(1, 3);  //error;
            aa.outstring(); //1.
        }
    }

    //과제 2.
    public class vectorint
    {
        private int size = 0;
        public List<int> val = new List<int>();

        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
    }

    public static class vector
    {
        public static void push(this vectorint v, int a)
        {
            v.val.Add(a);
            v.Size++;
        }

        public static void insertat(this vectorint v, int p_index, int p_val)
        {
            v.val.Insert(p_index, p_val);
            v.Size++;
        }

        public static void addrange(this vectorint v, int p_index, int[] p_val)
        {
            int n = 0;
            for (int i = p_index; n < p_val.Length; i++)
            {
                v.val.Insert(i, p_val[n++]);
                v.Size++;
            }
        }

        public static void outstring(this vectorint v)
        {
            if (v.count() == 0)
            {
                Console.WriteLine("empty");
                return;
            }
            string sum = v.val[0].ToString();
            for (int i = 1; i < v.count(); i++)
            {
                sum = sum + " " + v.val[i].ToString();
            }
            Console.WriteLine("{0}", sum);
        }

        public static void removeat(this vectorint v, int a)
        {
            if (v.count() <= a)
            {
                Console.WriteLine("error");
                return;
            }
            v.val.RemoveAt(a);
            v.Size--;
        }

        public static void removeatsize(this vectorint v, int stindex, int endindex)
        {
            int n = stindex;
            int size = v.count();
            for (int i = stindex; n <= endindex; n++)
            {
                if (size <= n)
                {
                    Console.WriteLine("index {0} error", n);
                    return;
                }
                v.val.RemoveAt(i);
                v.Size--;
            }
        }

        public static void clear(this vectorint v)
        {
            v.val.Clear();
            v.Size = 0;
        }

        public static int count(this vectorint v)
        {
            return v.Size;
        }
    }
}
