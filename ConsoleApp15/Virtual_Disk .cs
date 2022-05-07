using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



namespace ConsoleApp15
{
    public static class Virtual_Disk
    {
        public static FileStream Disk;
        public static void Create_or_Open_Disk(string path)
        {
            Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
        public static int getFreeSpace()
        {
            return (1024 * 1024) - (int)Disk.Length;    
        }
        public static void initalize(string path)
        {
            //try
            //{
                if(!File.Exists(path))
                {
                    Create_or_Open_Disk(path);
                    byte[] b = new byte[1024];
                    for(int i=0;i<b.Length;i++)
                    {
                        b[i] = 0;
                    }
                    write_cluster(b, 0);
                    FAT_Table.createFAT();
                    Directory root = new Directory("H:", 0x10, 5, null);
                    root.writeDirectory();
                    FAT_Table.setClusterPointer(5, -1);
                    Program.current = root;
                    FAT_Table.writeFAT();
                }
                else
                {
                    Create_or_Open_Disk(path);
                    FAT_Table.readFAT();
                    Directory root = new Directory("H:", 0x10, 5, null);
                    root.readDirectory();
                    Program.current = root;
                }
            //}catch(IOException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

       public static void write_cluster(byte[] cluster , int cluster_index , int offset=0 , int count =1024)
        {
            Disk.Seek(cluster_index * 1024, SeekOrigin.Begin);
            Disk.Write(cluster, offset, count);
            Disk.Flush();
        }

        public static byte[] read_cluster(int cluster_index)
        {
            Disk.Seek(cluster_index * 1024 , SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            return bytes;
        }

    }
}
