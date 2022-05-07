using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp15
{
    public class File_Entry : DirectoryEntry
    {
        
        public Directory parent;
        public File_Entry(string name, byte dir_attr, int dir_firstcluster , int dir_filesize, Directory pa,string content) : base(name, dir_attr, dir_firstcluster , dir_filesize,content )
        {
            this.content=content;
            //content = string.Empty;
            if (pa != null)
            {
                parent = pa;
            }

        }
        public DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry me = new DirectoryEntry(new string(this.Dir_name), this.dir_attr, this.dir_Firstcluster ,this.dir_filesize);
            return me;
        }





        public void writeFileContent()
        {
            byte[] contentBytes = Converter.StringToBytes(content);
            List<byte[]> bytesls = Converter.SplitBytes(contentBytes);
            int clusterFatIndex;
            if(this.dir_Firstcluster!=0)
            {
                clusterFatIndex = this.dir_Firstcluster;
            }
            else
            {
                clusterFatIndex = FAT_Table.getAvalableCluster();
                this.dir_Firstcluster = clusterFatIndex;
            }
            int lastcluster = -1;
            for(int i=0;i<bytesls.Count;i++)
            {
                Virtual_Disk.write_cluster(bytesls[i], clusterFatIndex, 0, bytesls[i].Length);
                FAT_Table.setClusterPointer(clusterFatIndex, -1);
                if(lastcluster!=-1)
                {
                    FAT_Table.setClusterPointer(lastcluster,clusterFatIndex);

                }
                lastcluster = clusterFatIndex;
                clusterFatIndex = FAT_Table.getAvalableCluster();
            }
        }
        public void readFileContent()
        {
            if(this.dir_Firstcluster!=0)
            {
                content = string.Empty;
                int cluster = this.dir_Firstcluster;
                int next = FAT_Table.getClusterPointer(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.read_cluster(cluster));
                    cluster = next;
                    if (cluster != -1)
                    {
                        next = FAT_Table.getClusterPointer(cluster);
                    }
                } while (next != -1);
                content = Converter.BytesToString(ls.ToArray());
            }
        }
        public void deleteFile()
        {
            if(this.dir_Firstcluster!=0)
            {
                int cluster = this.dir_Firstcluster;
                int next= FAT_Table.getClusterPointer(cluster);
                do
                {
                    FAT_Table.setClusterPointer(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                    {
                        next = FAT_Table.getClusterPointer(cluster);
                    }
                } while (cluster != -1);
            }
            if(this.parent!=null)
            {
                int index = this.parent.searchDirectory(new string(this.Dir_name));
                if(index!=-1)
                {
                    this.parent.DirOrFiles.RemoveAt(index);
                    this.parent.writeDirectory();
                    FAT_Table.writeFAT();
                }
            }
        }
    }
}
