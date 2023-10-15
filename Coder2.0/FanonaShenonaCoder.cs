using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    public class FanonaShenonaCoder : ICoder
    {
        BitWriter bitWriter;
        byte[] SourceData;
        int[] freqs;
        public FanonaShenonaCoder(byte[] data)
        {
            this.SourceData = data;
            this.freqs = new FileInformation(data, true).GetNormalizedFreqs();
            this.bitWriter = new BitWriter(FanonaShenon.MakeFanonaShenonTree(freqs), SourceData);
        }

        public long GetArchiveDataSize() => bitWriter.EndSize;

        public void MakeData() => bitWriter.Code();

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
