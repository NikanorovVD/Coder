using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    public static class LZW
    {
        public static byte[] Encode(byte[] sourceData)
        {
            Dictionary<List<byte>, int> dictionary = new Dictionary<List<byte>, int>(new ArrayComparer());
            for (int i = 0; i < 256; i++)
            {
                List<byte> e = new List<byte> { (byte)i };
                dictionary.Add(e, i);
            }
            List<byte> window = new List<byte>();
            List<int> data = new List<int>();
            foreach (var b in sourceData)
            {
                List<byte> windowChain = new List<byte>(window) { b };
                if (dictionary.ContainsKey(windowChain))
                {
                    window.Clear();
                    window.AddRange(windowChain);
                }
                else
                {
                    if (dictionary.ContainsKey(window))
                        data.Add(dictionary[window]);
                    else
                        throw new Exception("Error Encoding.");
                    dictionary.Add(windowChain, dictionary.Count);
                    window.Clear();
                    window.Add(b);
                }
            }
            if (window.Count != 0)
            {
                data.Add(dictionary[window]);
            }
            return GetBytes(data.ToArray());
        }
        public static byte[] Decode(byte[] Bufi)
        {
            var iBufi = Ia(Bufi);
            List<int> iBuf = new List<int>(iBufi);
            Dictionary<int, List<byte>> dictionary = new Dictionary<int, List<byte>>();
            for (int i = 0; i < 256; i++)
            {
                List<byte> e = new List<byte> { (byte)i };
                dictionary.Add(i, e);
            }
            List<byte> window = dictionary[iBuf[0]];
            iBuf.RemoveAt(0);
            List<byte> oBuf = new List<byte>(window);
            foreach (var k in iBuf)
            {
                List<byte> entry = new List<byte>();
                if (dictionary.ContainsKey(k))
                    entry.AddRange(dictionary[k]);
                else if (k == dictionary.Count)
                    entry.AddRange(Add(window.ToArray(), new[] { window.ToArray()[0] }));
                if (entry.Count > 0)
                {
                    oBuf.AddRange(entry);
                    dictionary.Add(dictionary.Count, new List<byte>(Add(window.ToArray(), new[] { entry.ToArray()[0] })));
                    window = entry;
                }
            }
            return oBuf.ToArray();
        }
        private static byte[] GetBytes(int[] value)
        {
            if (value == null)
                throw new Exception("GetBytes (int[]) object cannot be null.");
            var numArray = new byte[value.Length * 4];
            Buffer.BlockCopy(value, 0, numArray, 0, numArray.Length);
            return numArray;
        }
        private static byte[] Add(byte[] left, byte[] right)
        {
            var l1 = left.Length;
            var l2 = right.Length;
            var ret = new byte[l1 + l2];
            Buffer.BlockCopy(left, 0, ret, 0, l1);
            Buffer.BlockCopy(right, 0, ret, l1, l2);
            return ret;
        }
        private static int[] Ia(byte[] ba)
        {
            var bal = ba.Length;
            var int32Count = bal / 4 + (bal % 4 == 0 ? 0 : 1);
            var arr = new int[int32Count];
            Buffer.BlockCopy(ba, 0, arr, 0, bal);
            return arr;
        }
        [SecuritySafeCritical]
        private static unsafe bool Compare(byte[] a1, byte[] a2)
        {
            if (a1 == null && a2 == null)
                return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                var Len = a1.Length;
                byte* x1 = p1, x2 = p2;
                while (Len > 7)
                {
                    if (*(long*)x2 != *(long*)x1)
                        return false;
                    x1 += 8;
                    x2 += 8;
                    Len -= 8;
                }
                switch (Len % 8)
                {
                    case 0:
                        break;
                    case 7:
                        if (*(int*)x2 != *(int*)x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*(short*)x2 != *(short*)x1)
                            return false;
                        x1 += 2;
                        x2 += 2;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 6:
                        if (*(int*)x2 != *(int*)x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*(short*)x2 != *(short*)x1)
                            return false;
                        break;
                    case 5:
                        if (*(int*)x2 != *(int*)x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 4:
                        if (*(int*)x2 != *(int*)x1)
                            return false;
                        break;
                    case 3:
                        if (*(short*)x2 != *(short*)x1)
                            return false;
                        x1 += 2;
                        x2 += 2;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 2:
                        if (*(short*)x2 != *(short*)x1)
                            return false;
                        break;
                    case 1:
                        if (*x2 != *x1)
                            return false;
                        break;
                }
                return true;
            }
        }
        private class ArrayComparer : IEqualityComparer<List<byte>>
        {
            public bool Equals(List<byte> left, List<byte> right)
            {
                if (left == null || right == null)
                    return false;
                return Compare(left.ToArray(), right.ToArray());
            }
            public unsafe int GetHashCode(List<byte> obj)
            {
                var obj1 = obj.ToArray();
                var cbSize = obj1.Length;
                var hash = 0x811C9DC5;
                fixed (byte* pb = obj1)
                {
                    var nb = pb;
                    while (cbSize >= 4)
                    {
                        hash ^= *(uint*)nb;
                        hash *= 0x1000193;
                        nb += 4;
                        cbSize -= 4;
                    }
                    switch (cbSize & 3)
                    {
                        case 3:
                            hash ^= *(uint*)(nb + 2);
                            hash *= 0x1000193;
                            goto case 2;
                        case 2:
                            hash ^= *(uint*)(nb + 1);
                            hash *= 0x1000193;
                            goto case 1;
                        case 1:
                            hash ^= *nb;
                            hash *= 0x1000193;
                            break;
                    }
                }
                return (int)hash;
            }
        }
    }
}
