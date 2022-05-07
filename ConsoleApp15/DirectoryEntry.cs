using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp15
{
    
    public class DirectoryEntry
    {
        public char[] Dir_name = new char[11];
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_Firstcluster;
        public int dir_filesize;
        public string content;
        public DirectoryEntry() { }
        public DirectoryEntry(string name , byte dir_attr , int dir_Firstcluster , int dir_filesize,string content="") {
            this.dir_attr = dir_attr;
            this.content = content;
            if(dir_attr == 0x0)
            {
                this.dir_filesize = dir_filesize;
                string[] fileName = name.Split('.');
                assignFileName(fileName[0].ToCharArray(), fileName[1].ToCharArray());
            }
            else if (dir_attr == 0x10)
            {
                assignDirName(name.ToCharArray());
            }
            this.dir_Firstcluster = dir_Firstcluster;
        }
        public void assignFileName(char[] name , char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for(int i =0; i<name.Length; i++)
                {
                    j++;
                    this.Dir_name[i] = name[i];
                }
                //j++;
                this.Dir_name[j] = '.';
                for (int i =0; i <extension.Length; i++)
                {
                    j++;
                    this.Dir_name[j] = extension[i];
                }
                for (int i = ++j; i < Dir_name.Length; i++)
                {
                    this.Dir_name[i] = ' ';
                }

            }
            else
            {
                for(int i =0; i<name.Length; i++)
                {
                    this.Dir_name[i] = name[i];
                }
                this.Dir_name[name.Length-2] = '.';
                for (int i = 0 ,j = name.Length; i < extension.Length-1; j++, i++)
                {
                    this.Dir_name[j] = extension[i];
                }
            }
        }
        
        public void assignDirName(char[] name) 
        {
            if(name.Length<=11)
            {
                int j = 0;
                for(int i=0;i<name.Length;i++)
                {
                    j++;
                    this.Dir_name[i] = name[i];
                }
                for (int i = j++; i < Dir_name.Length; i++)
                {
                    this.Dir_name[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for(int i=0;i<11;i++)
                {
                    j++;
                    this.Dir_name[i] = name[i];
                }
            }
        }
      

    }
}
