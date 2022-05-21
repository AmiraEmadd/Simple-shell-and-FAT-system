using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;

namespace ConsoleApp15
{
    public class Directory : DirectoryEntry
    {

        public List<DirectoryEntry> DirOrFiles;
        public Directory parent;

        public Directory(string name, byte dir_attr, int dir_firstcluster, Directory pa,int file_size=0 )
        {
            this.dir_filesize = file_size;
            base.dir_attr = dir_attr;
            if (dir_attr == 0x0)
            {
                string[] fileName = name.Split('.');
                assignFileName(fileName[0].ToCharArray(), fileName[1].ToCharArray());
            }
            else if (dir_attr == 0x10)
            {
                assignDirName(name.ToCharArray());
            }
            base.dir_Firstcluster = dir_Firstcluster;
            DirOrFiles = new List<DirectoryEntry>();
            DirectoryEntry me = this.GetDirectoryEntry();
            //parent.DirOrFiles.Add(me);
            //DirOrFiles.Add(me);

            if (pa != null)
            {
                parent = pa;
                //parent.DirOrFiles.Add(this.parent.GetDirectoryEntry());
                //parent.DirOrFiles.Add(me);
            }
        }

        public void UpdateContent(DirectoryEntry d)
        {
            int index = searchDirectory(new string(d.Dir_name));
            if (index != -1)
            {
                DirOrFiles.RemoveAt(index);
                DirOrFiles.Insert(index, d);
            }
        }
        public DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry me = new DirectoryEntry(new string(this.Dir_name), this.dir_attr, this.dir_Firstcluster,this.dir_filesize);
            return me;
        }
        public void writeDirectory()
        {
            byte[] dirsorfilesBYTES = new byte[DirOrFiles.Count * 32];

            for (int i = 0; i < DirOrFiles.Count; i++)
            {
                byte[] b = Converter.Directory_EntryToBytes(this.DirOrFiles[i]);

                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                {
                    dirsorfilesBYTES[j] = b[k];
                }
            }
            List<byte[]> bytesls = Converter.SplitBytes(dirsorfilesBYTES);
            int clusterFatIndex;
            if (this.dir_Firstcluster != 0)
            {
                clusterFatIndex = this.dir_Firstcluster;
            }
            else
            {
                clusterFatIndex = FAT_Table.getAvalableCluster();
                this.dir_Firstcluster = clusterFatIndex;
            }
            int lastCluster = -1;
            for (int i = 4; i < bytesls.Count; i++)
            {
                if (clusterFatIndex != -1)
                {
                    Virtual_Disk.write_cluster(bytesls[i], clusterFatIndex, bytesls[i].Length);
                    FAT_Table.setClusterPointer(clusterFatIndex, -1);
                    if (lastCluster != -1)
                    {
                        FAT_Table.setClusterPointer(lastCluster, clusterFatIndex);
                    }
                    lastCluster = clusterFatIndex;
                    clusterFatIndex = FAT_Table.getAvalableCluster();
                }
            }
            if (this.parent != null)
            {
                this.parent.UpdateContent(this.GetDirectoryEntry());
                this.parent.writeDirectory();
            }
            FAT_Table.writeFAT();
        }
        public void readDirectory()
        {
            if (this.dir_Firstcluster != 0)
            {
                DirOrFiles = new List<DirectoryEntry>();
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
                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                    {
                        break;
                    }
                    DirOrFiles.Add(Converter.BytesToDirectory_Entry(b));
                }
            }
        }
        public void deleteDirectory()
        {
            if (this.dir_Firstcluster != 0)
            {
                int cluster = this.dir_Firstcluster;
                int next = FAT_Table.getClusterPointer(cluster);
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
            if (this.parent != null)
            {
                int index = this.parent.searchDirectory(new string(this.Dir_name));
                if (index != -1)
                {
                    this.parent.DirOrFiles.RemoveAt(index);
                    this.parent.writeDirectory();

                }
                if (Program.current == this)
                {
                    if (this.parent != null)
                    {
                        Program.current = this.parent;
                        Program.currentPath = Program.currentPath.Substring(0, Program.currentPath.LastIndexOf('\\'));
                        Program.current.readDirectory();
                    }
                }
                FAT_Table.writeFAT();
            }
        }
        public int searchDirectory(string name)
        {
            if (name.Length < 11)
            {
                //name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                {
                    name += " ";
                }
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < DirOrFiles.Count; i++)
            {
                string n = new string(DirOrFiles[i].Dir_name);
                if (n.Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
