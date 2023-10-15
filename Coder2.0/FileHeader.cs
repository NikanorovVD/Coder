using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coder
{
    class FileHeader
    {
        public static string type = "file";       //4
        public string name;                       //16
        public long source_size;                  //8
        public long archive_size;                 //8
        public int coding;                        //4
        public int parent_dir_id;                 //4

        public static int length = 44;

        public FileHeader(FileInfo file, long archive_size, int coding, int dir_id = 0)
            : this(file.Name, file.Length, archive_size, coding, dir_id) { }
       

        private FileHeader(string name, long source_size, long archive_size, int coding, int dir_id)
        {
            this.name = name;
            this.source_size = source_size;
            this.archive_size = archive_size;
            this.coding = coding;
            this.parent_dir_id = dir_id;
        }

        public void WriteFileHeader(BinaryWriter writer)
        {
            writer.Write(Encoding.Default.GetBytes(type), 0, 4);
            writer.Write(Encoding.Default.GetBytes(name), 0, name.Length);//нельзя ли просто поставить 16?
            writer.Seek(16 - name.Length, SeekOrigin.Current);

            writer.Write(BitConverter.GetBytes(source_size), 0, 8);
            writer.Write(BitConverter.GetBytes(archive_size), 0, 8);
            writer.Write(BitConverter.GetBytes(coding), 0, 4);
            writer.Write(BitConverter.GetBytes(parent_dir_id), 0, 4);
        }

        public FileHeader(BinaryReader reader)
        {
            char[] s = reader.ReadChars(16);
            name = new string(s).TrimEnd('\0');
            source_size = reader.ReadInt64();
            archive_size = reader.ReadInt64();
            coding = reader.ReadInt32();
            parent_dir_id = reader.ReadInt32();
        }
    }
}
