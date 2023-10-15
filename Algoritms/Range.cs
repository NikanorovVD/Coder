using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.CodeDom.Compiler;
using System.Xml.Linq;
using System.Collections;
using Microsoft.Win32;
using System.Runtime.CompilerServices;

namespace Coder
{
  
    public static class Range
    {
        public static bool[] RangeEncode(int[] freq, byte[] data)
        {
            List<bool> bits = new List<bool>(0);
            int bits_to_follow = 0;
           
            long D = 0;
            long[] pfeq = new long[freq.Length + 1];
            for (int i = 0; i < freq.Length; i++)
            {
                pfeq[i] = D;
                D += (long)freq[i];
            }
            pfeq[freq.Length] = D;
            long N = 4 * D * D * 1024;
            long First = N / 4;
            long Half = 2 * First;
            long Last = 3 * First;
            Console.WriteLine(N);
            long l = 0;
            long t = N - 1;
            long delta;
            foreach (byte byt in data)
            {
                delta = t - l + 1;
                t = l + delta * pfeq[byt + 1] / D - 1; // second
                l = l + delta * pfeq[byt] / D; // first
                for(; ; )
                {
                    if (t < Half)
                        bits_plus_follow(false);
                    else if (l >= Half)
                    {
                        bits_plus_follow(true);
                        l = l - Half;
                        t = t - Half;
                    }
                    else if ((l >= First) && (t < Last))
                    {
                        bits_to_follow++;
                        l = l - First;
                        t = t - First;
                    }
                    else break;
                    l = 2 * l;
                    t = 2 * t + 1;
                }
            }
            bits_plus_follow(true);
            Console.WriteLine(bits_to_follow);

            return bits.ToArray();

            void bits_plus_follow(bool bit)
            {
                bits.Add(bit);
                for (; bits_to_follow > 0; bits_to_follow--)
                   bits.Add(!bit);
            }
        }
        public static byte[] RangeDecode(BitArray data, int[] freq, int size)
        {
            long D = 0;
            long[] pfeq = new long[freq.Length + 1];
            for (int j = 0; j < freq.Length; j++)
            {
                pfeq[j] = D;
                D += (long)freq[j];
            }
            pfeq[freq.Length] = D;
            long N = 4 * D * D * 1024;
            Console.WriteLine(N);
            long First = N / 4;
            long Half = 2 * First;
            long Last = 3 * First;
            long l = 0;
            long t = N - 1;
            long a = l;
            long b = t;
            long delta;
            long beta;
            List<byte> source = new List<byte>(0);
            int k = 0;
            int i = 0;
            bool flag = true;
            while (i != size) 
            {
                if (flag) {
                    long temp =(long)(data[k] ? 1 : 0);
                    k++;
                    beta = b - a + 1;
                    b = a + beta * (temp + 1) / 2 - 1; // second
                    a = a + beta * temp / 2; // first
                }
                delta = t - l + 1;
                int j = BinarySearch(pfeq, a, l, delta, D);
                if ((a >= (l + delta * pfeq[j - 1] / D)) && (b <= (l + delta * pfeq[j] / D - 1)))
                {
                    t = l + delta * pfeq[j] / D - 1; // second
                    l = l + delta * pfeq[j - 1] / D; // first
                    source.Add((byte)(j - 1));
                    i++;
                    flag = false;
                    for (; ; )
                    {
                        if (t < Half) { }
                        else if (l >= Half)
                        {
                            l = l - Half;
                            t = t - Half;
                            a = a - Half;
                            b = b - Half;
                        }
                        else if ((l >= First) && (t < Last))
                        {
                            l = l - First;
                            t = t - First;
                            a = a - First;
                            b = b - First;
                        }
                        else break;
                        l = 2 * l;
                        t = 2 * t + 1;
                        a = 2 * a;
                        b = 2 * b + 1;
                    }
                }
                else
                {
                    flag = true;
                }
            }

            return source.ToArray();
        }
        private static int BinarySearch(long[] pfeq, long a, long l, long delta, long D)
        {
            int left = 0;
            int right = pfeq.Length - 1;

            while (left < right)
            {
                int mid = (left + right) / 2;

                if (a < (l + pfeq[mid] * delta / D))
                {
                    right = mid;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return left;
        }
    }

}
