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

namespace Coder
{
    public class TreeNode: IComparable
    {
        public int byt;
        public int freq;
        public TreeNode bytleft;
        public TreeNode bytright;
        public TreeNode(int byt, int freq, TreeNode bytleft = null, TreeNode bytright = null)
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
            TreeNode a = (TreeNode)obj;
            if (a.freq == this.freq) return this.byt.CompareTo(a.byt);
            else return a.freq.CompareTo(this.freq);
        }
    }

    public static class Haffman
    {
        public static TreeNode[] HaffmanTree;
        public static Dictionary<byte, bool[]> MakeHaffmanTree(int[] frequency)
        {
            HaffmanTree = new TreeNode[frequency.Length];
            for (int i = 0; i < frequency.Length; i++)
            { 
                HaffmanTree[i] = new TreeNode(i, frequency[i]);
            }

            var l = HaffmanTree.ToList();
            l.RemoveAll(x => x.freq == 0);
            HaffmanTree = l.ToArray();
            

            Merge(0, ref HaffmanTree);
            Dictionary<byte, bool[]> codes = GetCodes(HaffmanTree[0]);
            
            return codes;
        }
        public static void MakeTestHaffmanTree() => MakeHaffmanTree(new int[] { 1, 0, 5, 1, 2, 3, 3, 2 });
        private static void Merge(int counter, ref TreeNode[] Tree)
        {
            int len = Tree.Length;
            if (len!=1) {
                Array.Sort(Tree);
                var buff = new TreeNode(255 + counter++, Tree[len - 2].freq + Tree[len - 1].freq, Tree[len - 2], Tree[len - 1]);
                Tree[len - 2] = buff;
                Array.Resize(ref Tree, len - 1);
                Merge(counter, ref Tree);
            }
        }
        static Dictionary<byte, bool[]> GetCodes(TreeNode root)
        {
            Dictionary<byte, bool[]> HaffmanDictionary = new Dictionary<byte, bool[]>();

            bool[] bytes = new bool[0];

            Codes(ref root, bytes, HaffmanDictionary);

            return HaffmanDictionary;
        }
        private static void Codes(ref TreeNode HaffmanTree, bool[] code, Dictionary<byte, bool[]> codes)
        {
            if (HaffmanTree.bytleft == null && HaffmanTree.bytright == null) 
            {
                codes[(byte)HaffmanTree.byt] = code;
                //Console.WriteLine(HaffmanTree.byt);
                return;
            }
            Codes(ref HaffmanTree.bytleft, code.Append<bool>(false).ToArray(), codes);  // 0
            Codes(ref HaffmanTree.bytright, code.Append<bool>(true).ToArray(), codes); // 1
        }
        public static int Traverse(ref TreeNode HaffmanTree, string code)
        {
            if (HaffmanTree.bytleft == null && HaffmanTree.bytright == null)
            {
                //Console.WriteLine(HaffmanTree.byt);
                return HaffmanTree.byt;
            }
            Traverse(ref HaffmanTree.bytleft, code + "0");  // 0
            Traverse(ref HaffmanTree.bytright, code + "1"); // 1

            return 0;
        }

    }
}
