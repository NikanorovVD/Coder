using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    public class RangeCoder : ICoder
    {
        BitWriter bitWriter;
        byte[] SourceData;
        int[] freqs;
        public RangeCoder(byte[] data)
        {
            this.SourceData = data;
            this.freqs = new FileInformation(data, true).GetRangeFreqs();
            this.bitWriter = new BitWriter(Range.RangeEncode(freqs, SourceData));
        }

        public long GetArchiveDataSize() => bitWriter.EndSize;

        public int GetCodingInformationSize() => 256;


        public void MakeData() => bitWriter.Cod();

        public void WriteCodingInformation(BinaryWriter writer)
        {
            for (int i = 0; i < freqs.Length; i++)
            {
                writer.Write(BitConverter.GetBytes(freqs[i]), 0, 1);
            }
        }

        public void WriteData(BinaryWriter writer)
        {
            bitWriter.Write(writer);
        }
    }
}
