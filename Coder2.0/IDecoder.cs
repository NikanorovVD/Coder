using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coder
{
    interface IDecoder
    {
        void ReadCodingInformation(BinaryReader reader);
        void WriteDecodedData(BinaryWriter writer, byte[] sourceData);
    }
}
