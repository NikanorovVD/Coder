using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    class RangeDecoder : IDecoder
    {
        int[] freqs;
        public byte[] sourceData;
        byte[] endData;
        int end;

        public RangeDecoder(int end_data_len)
        {
            freqs = new int[256];
            endData = new byte[end_data_len];
            end = end_data_len;
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
            //List<byte> sourceDataList = new List<byte>();
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = (byte)((sourceData[i] * 0x0202020202 & 0x010884422010) % 1023);
            }
            byte[] zeros = new byte[20];
            sourceData = sourceData.Concat(zeros).ToArray();
            BitArray bits = new BitArray(sourceData);
            byte[] data;
            data = Range.RangeDecode(bits, freqs, end);

            writer.Write(data, 0, data.Length);
        }
    }
}
