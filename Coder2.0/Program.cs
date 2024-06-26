﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Coder
{
    enum ActionType {code_one_file, code_several_files, code_directory, decode, test }
    enum CodeType {NoCoding = 0, Haffman = 1, Shannon = 2, Interval = 3, LZW = 4}
    class Program
    {
        static void Main(string[] args)
        {
            ActionType action = ActionType.test;
            CodeType type = CodeType.Shannon;

            string filename = "q.jpg";
            string folder = "C:\\МИЭТ\\Coder";
            
            

            if(action == ActionType.test)
            {
                string path = Path.Combine(folder, "files", filename);
                string coder_path = Path.Combine(folder, "out", Path.ChangeExtension(filename, MainHeader.extention));
                string decoder_path = Path.Combine(folder, "decoder_out");
                if (File.Exists(path))
                {
                    Coder(new FileInfo(path), type, coder_path);
                    Decoder(new FileInfo(coder_path), decoder_path);
                }
                else
                    Console.WriteLine($"Указанный файл: {path} не существует");
                Console.ReadLine();
                return;
            }

            //тест для двух файлов
            string[] paths = { "C:\\Coder2.0_git\\files\\qwe.txt", "C:\\Coder2.0_git\\files\\Small.txt" };
            string new_path = "C:\\Coder2.0_git\\out\\qwe+small";


            //string[] paths = { "C:\\Coder2.0_git\\out\\qwe+small.xxxx" };
            //string new_path = "C:\\Coder2.0_git\\decoder_out";

            //тест для папок
            //string[] paths = { "C:\\Coder2.0_git\\files\\Books" };
            //string new_path = "C:\\Coder2.0_git\\out\\books";

            //string[] paths = { "C:\\Coder2.0_git\\out\\books.xxxx" };
            //string new_path = "C:\\Coder2.0_git\\decoder_out";

            if (action == ActionType.code_one_file) {
                if (File.Exists(paths[0]))
                    Coder(new FileInfo(paths[0]), type, new_path);
                else
                    Console.WriteLine($"Указанный файл: {paths[0]} не существует");

            }
            else if (action == ActionType.code_several_files)
            {
                FileInfo[] files = new FileInfo[paths.Length];
                int i = 0;
                foreach (string path in paths) {
                    if (!File.Exists(path)) {
                        Console.WriteLine($"Указанный файл: {path} не существует");
                        return;
                    }
                    else
                        files[i] = new FileInfo(paths[i++]);
                }
                Coder(new_path, type, files);
            }
            else if (action == ActionType.code_directory)
            {
                if (Directory.Exists(paths[0]))
                {
                    int i = 1;
                    DirectoryInfo directory = new DirectoryInfo(paths[0]);
                    new_path = Path.ChangeExtension(new_path ?? directory.FullName, MainHeader.extention);
                    BinaryWriter writer = new BinaryWriter(File.Open(new_path, FileMode.OpenOrCreate));
                    Coder(new DirectoryInfo(paths[0]), ref i, type, 0, new_path, writer);
                    writer.Close();
                    Console.WriteLine($"Архив успешно записан в {new_path}");
                }
                else
                    Console.WriteLine($"Указанная папка: {paths[0]} не существует");
            }
            else if (action == ActionType.decode)
            {
                if (File.Exists(paths[0]))
                    Decoder(new FileInfo(paths[0]), new_path);
                else
                    Console.WriteLine($"Указанный файл: {paths[0]} не существует");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// для одного файла
        /// </summary>
        static void Coder(FileInfo file, CodeType code_type, string new_path = null)
        {
            new_path = Path.ChangeExtension(new_path ?? file.FullName, MainHeader.extention);
            MainHeader.num_elements = 1;

            byte[] data;
            using (BinaryReader reader = new BinaryReader(file.Open(FileMode.Open)))
            {
                data = reader.ReadBytes((int)file.Length);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(new_path, FileMode.OpenOrCreate)))
            {
                MainHeader.WriteMainHeader(writer);
                CodeFile(code_type, data, file, 0, writer);
            }

            Console.WriteLine($"файл успешно записан в {new_path}");
        }

        

        /// <summary>
        /// для нескольких файлов
        /// </summary>
        static void Coder(string new_path, CodeType code_type, params FileInfo[] files)
        {
            if (files.Length == 0) { Console.WriteLine("В функцию не передано ни одного файла"); return; }
            if (string.IsNullOrEmpty(new_path)) { Console.WriteLine("В функцию не передан конечный путь"); return; }
            new_path = Path.ChangeExtension(new_path, MainHeader.extention);
            MainHeader.num_elements = files.Length;

            BinaryWriter writer = new BinaryWriter(File.Open(new_path, FileMode.OpenOrCreate));
            MainHeader.WriteMainHeader(writer);

            foreach (FileInfo file in files)
            {
                byte[] data;
                using (BinaryReader reader = new BinaryReader(file.Open(FileMode.Open)))
                {
                    data = reader.ReadBytes((int)file.Length);
                }
                CodeFile(code_type, data, file, 0, writer);
            }
            writer.Close();
            Console.WriteLine($"файл успешно записан в {new_path}");
        }


        /// <summary>
        ///для иерархии папок
        /// </summary>
        static void Coder(DirectoryInfo directory, ref int id, CodeType code_type, int parent_dir_id = 0 , string new_path = null, BinaryWriter writer = null)
        {
            int this_id = id;
            byte[] data;

            MainHeader.num_elements = 1 + num_elements(directory);

            if (id == 1) MainHeader.WriteMainHeader(writer);

            DirectoryHeader dir_header = new DirectoryHeader(directory,id,parent_dir_id);
            dir_header.WriteDirectoryHeader(writer);

            foreach(FileInfo file in directory.GetFiles())
            {
                using (BinaryReader reader = new BinaryReader(file.Open(FileMode.Open)))
                {
                    data = reader.ReadBytes((int)file.Length);
                }
                CodeFile(code_type, data, file, id, writer);
            }
            foreach(DirectoryInfo dir in directory.GetDirectories())
            {
                id++;
                Coder(dir, ref id, code_type, this_id, new_path, writer);
            }
      

            int num_elements(DirectoryInfo main_directory){
                int n = 0;
                n += main_directory.GetFiles().Length;
                foreach (DirectoryInfo dir in main_directory.GetDirectories())
                {
                    n++;
                    n += num_elements(dir);
                }
                return n;
            }

        }


        static void CodeFile(CodeType code_type, byte[] data, FileInfo file, int id, BinaryWriter writer, bool intelligent = true)
        {
            ICoder coder = GetCoderByType(code_type, data);
            coder.MakeData();
            Console.WriteLine($"Размер исходного файла {data.Length}");
            Console.WriteLine($"Размер массива частот в байтах {coder.GetCodingInformationSize()}");
            Console.WriteLine($"Размер сжатых данных в байтах {coder.GetArchiveDataSize()}");
            Console.WriteLine($"Размер файла со сжатием:{coder.GetArchiveDataSize() + coder.GetCodingInformationSize()}");
            if (intelligent && ((coder.GetArchiveDataSize() + coder.GetCodingInformationSize()) > data.Length))
            {
                Console.WriteLine($"Файл {file.Name} будет записан без сжатия");
                code_type = CodeType.NoCoding;
                coder = GetCoderByType(CodeType.NoCoding, data);
            }

            FileHeader header = new FileHeader(file, coder.GetArchiveDataSize(), (int)code_type, id);
            header.WriteFileHeader(writer);
            coder.WriteCodingInformation(writer);
            coder.WriteData(writer);
        }

        static void Decoder(FileInfo file, string new_dir_path = null)
        {
            byte[] sourceData;
            int[] freq = new int[256];
            FileHeader fileHeader;
            DirectoryHeader directoryHeader;
            string type;
            string[] paths;
            using (BinaryReader reader = new BinaryReader(file.Open(FileMode.Open)))
            {
                try { MainHeader.ReadMainHeader(reader); }
                catch (IOException e) { Console.WriteLine(e.Message); return; }

                paths = new string[MainHeader.num_elements];//изменить на число папок
                paths[0] = new_dir_path ?? file.DirectoryName;
                for (int i = 0; i < MainHeader.num_elements; i++)
                {
                    type = new string(reader.ReadChars(4));
                    if (type == "file")
                    {
                        fileHeader = new FileHeader(reader);
                        string path = Path.Combine(paths[fileHeader.parent_dir_id], fileHeader.name);
                        CodeType code_type = GetTypeByCoding(fileHeader.coding);
                        IDecoder decoder = GetDecoderByType(code_type, (int)fileHeader.source_size + 4);

                        decoder.ReadCodingInformation(reader);

                        sourceData = reader.ReadBytes((int)fileHeader.archive_size);
                        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                        {
                            decoder.WriteDecodedData(writer, sourceData);
                        }

                    }
                    else if (type == "drct")
                    {
                        directoryHeader = DirectoryHeader.ReadDirectoryHeader(reader);
                        string new_path = Path.Combine(paths[directoryHeader.parent_dir_id], directoryHeader.name);
                        Directory.CreateDirectory(new_path);
                        paths[directoryHeader.dir_id] = new_path;
                    }
                    else throw new IOException("В файле не файл и не папка");
                }
            }

            Console.WriteLine($"файл успешно декодирован в {new_dir_path}");
        }

        private static IDecoder GetDecoderByType(CodeType code_type, int end_data_len)
        {
            switch (code_type)
            {
                case CodeType.Haffman: return new HaffmanDecoder(end_data_len-4);
                case CodeType.NoCoding: return new NoCodingDecoder();
                case CodeType.Shannon: return new FanonaShenonaDecoder(end_data_len-4);
                case CodeType.Interval: return new RangeDecoder(end_data_len - 4);
                case CodeType.LZW: return new LZWdecoder(end_data_len);
                default: throw new NotImplementedException("Такой метод сжатия ещё не написан");
            }
        }

        private static CodeType GetTypeByCoding(long coding)
        {
            switch (coding)
            {
                case 0: return CodeType.NoCoding;
                case 1: return CodeType.Haffman;
                case 2: return CodeType.Shannon;
                case 3: return CodeType.Interval;
                case 4: return CodeType.LZW;
                default: throw new Exception($"Неизвестный код сжатия:{coding}");
            }
        }

        static ICoder GetCoderByType(CodeType type, byte[] data)
        {
            switch (type) {
                case CodeType.Haffman: return new HaffmanCoder(data);
                case CodeType.NoCoding: return new NoCodingWriter(data);
                case CodeType.Shannon: return new FanonaShenonaCoder(data);
                case CodeType.Interval: return new RangeCoder(data);
                case CodeType.LZW: return new LZWcoder(data);
                default: throw new NotImplementedException("Такой метод сжатия ещё не написан");
            }
        }
    }
    class NoCodingWriter : ICoder
    {
        byte[] data;
        public NoCodingWriter(byte[] data) => this.data = data;

        public long GetArchiveDataSize() => data.Length;
        public int GetCodingInformationSize() => 0;
        public void MakeData() { }
        public void WriteCodingInformation(BinaryWriter writer) { }
        public void WriteData(BinaryWriter writer) => writer.Write(data, 0, data.Length);
        
    }

    class NoCodingDecoder : IDecoder
    {
        public void ReadCodingInformation(BinaryReader reader) { }
        public void WriteDecodedData(BinaryWriter writer, byte[] sourceData) => writer.Write(sourceData, 0, sourceData.Length);
    }
}
    

