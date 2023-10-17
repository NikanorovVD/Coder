using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    class SymbolInfo : IComparable
    {
        public int number;
        public int count;
        public double p;
        public double I;

        public SymbolInfo(int number)
        {
            this.number = number;
            count = 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is SymbolInfo s)
                return s.count.CompareTo(count);
            else throw new InvalidCastException("сравнение SymbolInfo с недопустимым типом");
        }
        public double GetS() => I * count;
        public override string ToString() => $"{number}\t--\t{count}\t{p}\t{I}";
    }
    class FileInformation
    {
        private SymbolInfo[] symbols;
        private double S;
        public FileInformation(byte[] data, bool print = false)
        {
            symbols = new SymbolInfo[256];

            for (int i = 0; i < 256; i++)
            {
                symbols[i] = new SymbolInfo(i);
            }

            long len = data.LongLength;

            for (int i = 0; i < len; i++)
            {
                symbols[data[i]].count++;
                //Console.WriteLine(symbols[data[i]].count);
            }
            
            S = 0;
            for (int i = 0; i < 256; i++)
            {
                symbols[i].p = (double)symbols[i].count / len;
                symbols[i].I = (symbols[i].p == 0) ? 0 : -Math.Log(symbols[i].p, 2);
                S += symbols[i].GetS();
            }

            if (print)
            {
                Console.WriteLine($"суммарное кол-во информации в битах  {S}");
                Console.WriteLine($"суммарное кол-во информации в байтах {S / 8}");
            }
        }

        public int[] GetFreqs()
        {
            int[] freqs = new int[256];
            for(int i = 0; i< freqs.Length; i++)
            {
                freqs[i] = symbols[i].count;
                //Console.WriteLine(freqs[i]);
            }
            return freqs;
        }

        public int[] GetNormalizedFreqs()
        {
            int[] freqs = GetFreqs();
            int maxFreq = freqs.Max();
            for (int i = 0; i < freqs.Length; i++)
            {
                if (freqs[i] != 0)
                    freqs[i] = (((freqs[i]-1) * (255 - 1)) / (maxFreq-1)) + 1;
            }
            return freqs;
        }
        public int[] GetRangeFreqs()
        {
            int[] freqs = GetFreqs();
            int n = freqs.Sum();
            int maxFreq = freqs.Max();
            int D = 64;
            //while (D < n)
            //    D *= 2;
            //int[] normfreqs;
            for(int i = 0; i< freqs.Length; i++)
            {
                if(freqs[i]!=0)
                freqs[i] = (freqs[i]-1) * (D-1) / n + 1;
                //Console.WriteLine(freqs[i]);
            }
            //Console.WriteLine(freqs.Sum());

            return freqs;
        }
    }
}
