using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    class BitWriter
    {
        private Dictionary<byte, bool[]> dict;
        private bool[] codedData;
        private byte[] sourceData;
        private List<int> endData;
        public int position;

        public long EndSize {
            get => 4 * endData.LongCount();
        }
        public BitWriter(Dictionary<byte, bool[]> dict, byte[] sourceData)
        {
            this.dict = dict;
            this.sourceData = sourceData;
            this.endData = new List<int>();
            this.position = 0;
        }
        public BitWriter(bool[] codedData)
        {
            this.codedData = codedData;
            this.endData = new List<int>();
            this.position = 0;
        }

        public void Code()
        {
            int i = 0;
            endData.Add(0);

            foreach(byte b in sourceData)
            {
                CodeByte(b);
            }
            while (position != 32)
            {
                endData[i] = endData[i] << 1;
                position++;
            }

            void CodeByte(byte b)
            {
                bool[] code = dict[b];

                foreach (bool s in code)
                {
                    endData[i] = endData[i] << 1;
                    endData[i] = endData[i] | (s?1:0);
                    
                    position++;

                    if(position == 32)
                    {
                        position = 0;
                        endData.Add(0);
                        i++;
                    }
                }
            }
        }
        public void Cod()
        {
            int i = 0;
            endData.Add(0);

            foreach (bool b in codedData)
            {
                endData[i] = endData[i] << 1;
                endData[i] = endData[i] | (b ? 1 : 0);

                position++;

                if (position == 32)
                {
                    position = 0;
                    endData.Add(0);
                    i++;
                }
            }
            while (position != 32)
            {
                endData[i] = endData[i] << 1;
                position++;
            }

        }
        public void Write(BinaryWriter writer)
        {
            foreach (int x in endData)
            {
                byte[] bytes = BitConverter.GetBytes(x);
                Array.Reverse(bytes);
                writer.Write(bytes, 0, 4);
            }
        }
    }
}
