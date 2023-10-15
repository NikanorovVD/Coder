using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    class FanonaShenonaDecoder : IDecoder
    {
        int[] freqs;
        public byte[] sourceData;
        byte[] endData;

        public FanonaShenonaDecoder(int end_data_len)
        {
            freqs = new int[256];
            endData = new byte[end_data_len];
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
            Dictionary<byte, bool[]> codes = FanonaShenon.MakeFanonaShenonTree(freqs);

            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = (byte)((sourceData[i] * 0x0202020202 & 0x010884422010) % 1023);
            }
            BitArray bits = new BitArray(sourceData);

            TreeFanonaShenon[] current = FanonaShenon.FanonaShenonTree;

            int k = 0;
            int j = 0;
            while(k != endData.Count())
            {
                bool bit = bits[j];
                j++;
                if (bit)
                {
                    if (current[0].bytright != null)
                    {
                        current = current[0].bytright;
                    }
                }
                else
                {
                    if (current[0].bytleft != null)
                    {
                        current = current[0].bytleft;
                    }
                }

                if (current[0].bytleft == null && current[0].bytright == null)
                {
                    endData[k++] = ((byte)current[0].byt);
                    current = FanonaShenon.FanonaShenonTree;
                }
            }
            writer.Write(endData, 0, k);
        }
    }
}
