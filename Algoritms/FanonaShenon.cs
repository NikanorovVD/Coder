using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TreeFanonaShenon : IComparable
    {
        public int byt;
        public int freq;
        public TreeFanonaShenon[] bytleft;
        public TreeFanonaShenon[] bytright;
        public TreeFanonaShenon(int byt, int freq, TreeFanonaShenon[] bytleft = null, TreeFanonaShenon[] bytright = null)
        {
            this.byt = byt;
            this.bytleft = bytleft;
            this.bytright = bytright;
            this.freq = freq;
        }
        public int CompareTo(object obj)
        {
            /*
            Порядок сортировки по умолчанию:
            S2<S1<255<...<2<1<0
            */
            TreeFanonaShenon a = (TreeFanonaShenon)obj;
            if (a.freq == this.freq) return this.byt.CompareTo(a.byt);
            else return a.freq.CompareTo(this.freq);
        }
    }
    public static class FanonaShenon
    {
        public static TreeFanonaShenon[] FanonaShenonTree;
        public static Dictionary<byte, bool[]> MakeFanonaShenonTree(int[] frequency)
        {
            FanonaShenonTree = new TreeFanonaShenon[frequency.Length];
            for (int i = 0; i < frequency.Length; i++)
            {
                FanonaShenonTree[i] = new TreeFanonaShenon(i, frequency[i]);
            }
            var l = FanonaShenonTree.ToList();
            l.RemoveAll(x => x.freq == 0);
            FanonaShenonTree = l.ToArray();
            Array.Sort(FanonaShenonTree);

            Dictionary<byte, bool[]> FanonaShenonDictionary = new Dictionary<byte, bool[]>();
            bool[] bytes = new bool[0];
            Codes(ref FanonaShenonTree, bytes, FanonaShenonDictionary);

            return FanonaShenonDictionary;
        }
        private static void Codes(ref TreeFanonaShenon[] FanonaShenonTree, bool[] code, Dictionary<byte,
                                    bool[]> codes)
        {
            if (FanonaShenonTree.Count() == 1)
            {
                FanonaShenonTree[0] = new TreeFanonaShenon(FanonaShenonTree[0].byt, FanonaShenonTree[0].freq);
                codes[(byte)FanonaShenonTree[0].byt] = code;
                return; 
            }
            int i = 1;
            while (true) {
                int s1 = 0;
                int s2 = 0;
                for (int j = 0; j< FanonaShenonTree.Count(); j++)
                {
                    if (j < i)
                    {
                        s1 += FanonaShenonTree[j].freq;
                    }
                    else
                    {
                        s2 += FanonaShenonTree[j].freq;
                    }
                }
                if (s1 >= s2)
                {
                    break;
                }
                else i++;
            }
            TreeFanonaShenon[] FanonaShenonTree1 = new TreeFanonaShenon[i];
            TreeFanonaShenon[] FanonaShenonTree2 = new TreeFanonaShenon[FanonaShenonTree.Count()-i];
            for (int j = 0; j < FanonaShenonTree.Count(); j++)
            {
                if (j < i)
                {
                    FanonaShenonTree1[j] = FanonaShenonTree[j];
                }
                else
                {
                    FanonaShenonTree2[j-i] = FanonaShenonTree[j];
                }
            }
            FanonaShenonTree[0] = new TreeFanonaShenon(0,0, FanonaShenonTree1, FanonaShenonTree2);
            Codes(ref FanonaShenonTree1, code.Append<bool>(false).ToArray(), codes);  // 0
            Codes(ref FanonaShenonTree2, code.Append<bool>(true).ToArray(), codes); // 1
        }
    }
}
