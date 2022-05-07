using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace ConsoleApp15
{
    public static class Converter
    {
        public static byte[] ToBytes(int[] array)
        {
            byte[] bytes = null;
            bytes = new byte[array.Length * sizeof(int)];
            System.Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);
            return bytes;

        }
        public static int[] Toint(byte[] bytes)
        {
            int[] ints = null;
            ints = new int[bytes.Length / sizeof(int)];
            System.Buffer.BlockCopy(bytes, 0, ints, 0, bytes.Length);
            return ints;
        }
        public static List<byte[]> SplitBytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            int NumberOfArrays = bytes.Length / 1024;
            int rem = bytes.Length % 1024;
            for (int i = 0; i < NumberOfArrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                ls.Add(b);
            }
            if (rem > 0)
            {
                byte[] b1 = new byte[1024];
                for (int i = NumberOfArrays * 1024, k = 0; k < rem; i++, k++)
                {
                    b1[k] = bytes[i];
                }
                ls.Add(b1);
            }

            return ls;
        }
        public static byte[] StringToBytes(string str)
        {

            byte[] byt = new byte[str.Length];
            // converting each character into byte 
            // and store it
            for (int i = 0; i < str.Length; i++)
            {
                byt[i] = Convert.ToByte(str[i]);
            }
            return byt;
        }
        public static string BytesToString(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        public static byte[] Directory_EntryToBytes(DirectoryEntry d)
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < d.Dir_name.Length; i++)
            {
                bytes[i] = (byte)d.Dir_name[i];
            }
            bytes[11] = d.dir_attr;
            int j = 12;
            for (int i = 0; i < d.dir_empty.Length; i++)
            {
                bytes[j] = d.dir_empty[i];
                j++;
            }
            byte[] fc = BitConverter.GetBytes(d.dir_Firstcluster);
            for (int i = 0; i < fc.Length; i++)
            {
                bytes[j] = fc[i];
                j++;
            }
            byte[] sz = BitConverter.GetBytes(d.dir_filesize);
            for (int i = 0; i < sz.Length; i++)
            {
                bytes[j] = sz[i];
                j++;
            }
            return bytes;
        }
        public static DirectoryEntry BytesToDirectory_Entry(Byte[] bytes)
        {
            char[] name = new char[11];
            //convert directory name to byters
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = (char)bytes[i];

            }
            //convert directory attribute to byters
            byte attr = bytes[11];
            //convert directory empty to byters
            byte[] emp = new byte[12];
            int j = 12;
            for (int i = 0; i < emp.Length; i++)
            {
                emp[i] = bytes[j];
                j++;
            }
            //convert directory first cluster to byters
            byte[] fc = new byte[4];
            for (int i = 0; i < fc.Length; i++)
            {
                fc[i] = bytes[j];
                j++;
            }
            int firstcluster = BitConverter.ToInt32(fc, 0);
            byte[] sz = new byte[4];
            for (int i = 0; i < sz.Length; i++)
            {
                sz[i] = bytes[j];
                j++;
            }
            int filesize = BitConverter.ToInt32(sz, 0);
            // assign the new values
            DirectoryEntry d = new DirectoryEntry(new string(name), attr, firstcluster, filesize);
            d.dir_empty = emp;
            d.dir_filesize = filesize;
            return d;
        }
    }
}
