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
    class LZWdecoder : IDecoder
    {
        int[] freqs;
        public byte[] sourceData;
        byte[] endData;
        int end;

        public LZWdecoder(int end_data_len)
        {
            freqs = new int[256];
            endData = new byte[end_data_len];
            end = end_data_len;
        }

        public void ReadCodingInformation(BinaryReader reader)
        {
        }

        public void WriteDecodedData(BinaryWriter writer, byte[] sourceData)
        {
            //List<byte> sourceDataList = new List<byte>();
            byte[] data;
            data = LZW.Decode(sourceData);

            writer.Write(data, 0, data.Length);
        }
    }
}
