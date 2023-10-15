using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coder
{
    public interface ICoder
    {
        void MakeData();
        long GetArchiveDataSize();
        void WriteCodingInformation(BinaryWriter writer);
        void WriteData(BinaryWriter writer);
    }
}
