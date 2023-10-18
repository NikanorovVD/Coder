using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    class HaffmanDecoder : IDecoder
    {
        int[] freqs;
        public byte[] sourceData;
        List<byte> endData;
        int end_data_len;

        public HaffmanDecoder(int end_data_len)
        {
            freqs = new int[256];
            endData = new List<byte>();
            this.end_data_len = end_data_len;
        }

        public void ReadCodingInformation(BinaryReader reader)
        {
            for (int j = 0; j < 256; j++)
            {
                freqs[j] = reader.ReadByte();
            }
        }

        public void WriteDecodedData(BinaryWriter writer, byte[] sourceData)
        {
            Dictionary<byte, bool[]> codes = Haffman.MakeHaffmanTree(freqs);

            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = (byte)((sourceData[i] * 0x0202020202 & 0x010884422010) % 1023);
            }
            BitArray bits = new BitArray(sourceData);

            TreeNode current = Haffman.HaffmanTree[0];
            
            int k = 0;
            int j = 0;
            bool bit;
            while (k != end_data_len)
            {
                bit = bits[j];
                j++;
                if (bit)
                {
                    if (current.bytright != null)
                    {
                        current = current.bytright;
                    }
                }
                else
                {
                    if (current.bytleft != null)
                    {
                        current = current.bytleft;
                    }
                }

                if (current.bytleft == null && current.bytright == null)
                {
                    endData.Add((byte)current.byt);
                    k++;
                    current = Haffman.HaffmanTree[0];
                }
            }
           writer.Write(endData.ToArray(), 0, k);
        }
    }
}
