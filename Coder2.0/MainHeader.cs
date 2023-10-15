using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coder
{
    static class MainHeader
    {

        public static string extention = "xxxx";
        public static string version = "2.0";
        public static int num_elements;

        public static int length = 20;

        static public void WriteMainHeader(BinaryWriter writer)
        {
            writer.Write(Encoding.Default.GetBytes(extention),0,4);
            writer.Write(Encoding.Default.GetBytes(".version."),0,9);
            writer.Write(Encoding.Default.GetBytes(version),0,3);
            writer.Write(BitConverter.GetBytes(num_elements), 0, 4);
        }

        static public void ReadMainHeader(BinaryReader reader)
        {
            if (new string(reader.ReadChars(4)) != "xxxx" )
                throw new IOException("Неподходящий формат файла") ;
            if (new string(reader.ReadChars(12)) != ".version.2.0")
                throw new IOException("Неподходящая версия");
            num_elements = reader.ReadInt32();
        }
    }
}
        
        
	

    


