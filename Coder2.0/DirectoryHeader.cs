using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coder
{
    class DirectoryHeader
    {
        public static string type = "drct";       //4
        public string name;                       //16
        public int dir_id;                        //4
        public int parent_dir_id;                 //4

        public static int length = 28;

        public DirectoryHeader(DirectoryInfo directory, int dir_id, int parent_dir_id)
        {
            name = directory.Name;
            this.dir_id = dir_id;
            this.parent_dir_id = parent_dir_id;
        }

        private DirectoryHeader(string name, int dir_id, int parent_dir_id)
        {
            this.name = name;
            this.dir_id = dir_id;
            this.parent_dir_id = parent_dir_id;
        }

        public void WriteDirectoryHeader(BinaryWriter writer)
        {
            writer.Write(Encoding.Default.GetBytes(type), 0, 4);
            writer.Write(Encoding.Default.GetBytes(name), 0, name.Length);
            writer.Seek(16 - name.Length, SeekOrigin.Current);
            writer.Write(BitConverter.GetBytes(dir_id), 0, 4);
            writer.Write(BitConverter.GetBytes(parent_dir_id), 0, 4);
        }

        public static DirectoryHeader ReadDirectoryHeader(BinaryReader reader)
        {
            char[] s = reader.ReadChars(16);
            string name = new string(s).TrimEnd('\0');
            int dir_id = reader.ReadInt32();
            int parent_dir_id = reader.ReadInt32();
            return new DirectoryHeader(name, dir_id, parent_dir_id);
        }
    }
}
