using Coder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    public class LZWcoder : ICoder
    {
        byte[] SourceData;
        byte[] OutputData;
        public LZWcoder(byte[] data)
        {
            this.SourceData = data;
            OutputData = LZW.Encode(data);
        }

        public long GetArchiveDataSize() { return OutputData.Length; }

        public void MakeData() { }

        public void WriteCodingInformation(BinaryWriter writer) { }

        public void WriteData(BinaryWriter writer)
        {
            writer.Write(OutputData, 0, OutputData.Length);
        }
    }
}
