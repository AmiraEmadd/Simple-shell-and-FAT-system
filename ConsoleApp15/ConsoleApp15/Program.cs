using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace ConsoleApp15
{
    class Program
    {
        public static Directory current;
        public static string currentPath;

        static bool isFullPathd(string arg)
        {
            if ((arg.Contains(":") || arg.Contains("\\")) && !arg.Contains('.'))
            {
                return true;
            }
            return false;
        }
        static bool isFullPathf(string arg)
        {
            if ((arg.Contains(":") || arg.Contains("\\")) && arg.Contains('.'))
            {
                return true;
            }
            return false;
        }
        static bool isFileName(string arg)
        {
            if (arg.Contains('.'))
            {
                return true;
            }
            return false;
        }


        static void cd(string parameter1 = " ")
        {
            if (parameter1 == " ")
            {
                Console.WriteLine(currentPath);
            }
            else
            {
                if (parameter1.Contains('\\'))
                {

                    if (parameter1[0] == '.')
                    {
                        int parentcount = parameter1.Count(f => (f == '\\'));
                        current = current.parent;
                        string temp = "";
                        char[] delimiters = { '\\', ':' };
                        string[] fullpath = currentPath.Split(delimiters);
                        if (fullpath.Length == 3) return;
                        for (int i = 0; i < fullpath.Length - (2 * (parentcount + 1)); i++)
                        {
                            temp += fullpath[i];
                            if (i == 0)
                                temp += ":\\";
                            else
                                temp += '\\';
                        }
                        currentPath = temp;
                        currentPath = currentPath.Trim(new char[] { '\0', ' ' });
                    }
                    else if (!isFullPathd(parameter1))
                    {
                        Console.WriteLine("ERROR : this path \" " + parameter1 + " \" is not a valid path!");
                    }
                    else
                    {

                        string[] dirs = parameter1.Split('\\');
                        Directory temp = current;
                        temp.Dir_name = dirs[0].ToCharArray();
                        string newPath = "";
                        for (int i = 1; i < dirs.Length; i++)
                        {
                            if (temp.searchDirectory(dirs[i]) != -1)
                            {
                                temp.parent = temp;
                                int index = temp.searchDirectory(dirs[i]);

                                temp.Dir_name = temp.DirOrFiles[index].Dir_name;
                                temp.dir_Firstcluster = temp.DirOrFiles[index].dir_Firstcluster;

                                newPath += temp.Dir_name.ToString();
                            }
                            else
                            {
                                Console.WriteLine("ERROR : this path \" " + parameter1 + " \" does not exist!");
                                return;
                            }
                        }
                        if (!currentPath.EndsWith('\\')) currentPath += "\\";
                        string temp0 = new string(temp.Dir_name);
                        currentPath = parameter1;
                        currentPath = currentPath.Trim(new char[] { '\0', ' ' });
                        currentPath += '\\';

                    }
                }
                else if (parameter1 == ".")
                {

                }
                else if (parameter1 == "..")
                {
                    current = current.parent;
                    string temp = "";
                    char[] delimiters = { '\\', ':' };
                    string[] fullpath = currentPath.Split(delimiters);
                    if (fullpath.Length == 3) return;
                    for (int i = 0; i < fullpath.Length - 2; i++)
                    {
                        temp += fullpath[i];
                        if (i == 0)
                            temp += ':';
                        else
                            temp += '\\';
                    }
                    currentPath = temp;
                    currentPath = currentPath.Trim(new char[] { '\0', ' ' });
                }
                else if (current.searchDirectory(parameter1) != -1)
                {
                    ////Console.WriteLine("exist!!!!");
                    //int index = current.searchDirectory(parameter1);
                    //current.parent = current;
                    //current.Dir_name = current.DirOrFiles[index].Dir_name;
                    //current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                    //if (!currentPath.EndsWith('\\')) currentPath += "\\";
                    //string temp = new string(current.Dir_name);
                    //currentPath += temp;
                    //currentPath = currentPath.Trim(new char[] { '\0', ' ' });
                    //currentPath += '\\';
                    int index = Program.current.searchDirectory(parameter1);//بسيرش علي الدايريكتوري الي انا عايز اروحله 

                    if (index != -1)
                    {
                        byte attribute = current.DirOrFiles[index].dir_attr;
                        if (attribute == 0x10)
                        {
                            //current=current.DirOrFiles[index]
                            //int FirstCluster = current.DirOrFiles[index].dir_Firstcluster; //علشان اشوف الفولدر موجود ولا
                            //Directory dir = new Directory(parameter1, 0x10, FirstCluster, Program.current,0);//بديلة معلومات الي الدايريكتوري الي عايز اروحله
                            //Program.current = dir;  //هنا خليته يشاور ع الدايريكتوري الي عايز اروحله
                            current = (Directory)current.DirOrFiles[index];
                            if (!currentPath.EndsWith('\\')) currentPath += "\\";
                            Program.currentPath = Program.currentPath  + parameter1+ "\\";   //غيرت الباس
                            Program.current.readDirectory();
                        }
                        else
                        {
                            Console.WriteLine("Specified folder is not exist. Its a file..");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR : this path \" " + parameter1 + " \" does not exists!");
                }
            }

        }
        static void Cls()
        {
            Console.Clear();
        }
        static void Dir(string parameter1 = "")
        {
            int file_counter = 0;
            int dir_counter=0;
            int sizecounter=0;
            int freespace =0;
            Directory temp = current;
            if (current.searchDirectory(parameter1) != -1)
            {
                //Console.WriteLine("exist!!!!");
                int index = current.searchDirectory(parameter1);
                current.parent = current;

                current.Dir_name = current.DirOrFiles[index].Dir_name;
                current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                current.dir_attr = current.DirOrFiles[index].dir_attr;
            }
            Console.WriteLine("Directory of " + currentPath);
            for (int i = 0; i< current.DirOrFiles.Count; i++)
               {
                 if (current.DirOrFiles[i].dir_attr == 0x0)
                    {
                    file_counter++;
                    sizecounter += sizecounter + current.DirOrFiles[i].dir_filesize;
                    Console.Write("\t"+current.DirOrFiles[i].dir_filesize);
                    Console.Write("     ");
                    for(int a=0;a< current.DirOrFiles[i].Dir_name.LongCount();a++)
                        Console.Write(current.DirOrFiles[i].Dir_name[a]);
                    //Console.WriteLine(current.dir_attr);
                    Console.WriteLine();
                        
                    }
                 else if (current.DirOrFiles[i].dir_attr == 0x10)
                {
                    Console.Write("<DIR>          "); 
                    Console.WriteLine(current.DirOrFiles[i].Dir_name);
                    dir_counter++;
                    //Console.WriteLine(current.dir_attr);
                }
            }
            freespace = Virtual_Disk.getFreeSpace();
            Console.WriteLine("              "+file_counter + " Files(s) " + sizecounter +" bytes"); 
            Console.Write("              " + dir_counter+ " Dir(s) "); Console.WriteLine(freespace +" bytes free");

            //current = temp;
        }
        static void Help(string parameter1 = " ")
        {


            if (parameter1 == "cd")
            {
                Console.WriteLine("Displays the name of or changes the current directory.");
            }
            else if (parameter1 == "cls")
            {
                Console.WriteLine("Clear the screen");
            }
            else if (parameter1 == "dir")
            {
                Console.WriteLine("Displays a list of files and subdirectories in a directory.");
            }
            else if (parameter1 == "help")
            {
                Console.WriteLine("Provides help information for Windows commands.");
            }
            else if (parameter1 == "exit")
            {
                Console.WriteLine("Quits the CMD.EXE program (command interpreter) or the current batch script.");
            }
            else if (parameter1 == "copy")
            {
                Console.WriteLine("Copies one or more files to another location.");
            }
            else if (parameter1 == "del")
            {
                Console.WriteLine("Deletes one or more files.");
            }
            else if (parameter1 == "md")
            {
                Console.WriteLine("Creates a directory.");
            }
            else if (parameter1 == "rd")
            {
                Console.WriteLine("Removes (deletes) a directory.");
            }
            else if (parameter1 == "rename")
            {
                Console.WriteLine("Renames a file or files.");
            }
            else if (parameter1 == "type")
            {
                Console.WriteLine("Displays the contents of a text file or files.");
            }
            else if (parameter1 == "import")
            {
                Console.WriteLine("import text file(s) from your computer.");
            }
            else if (parameter1 == "export")
            {
                Console.WriteLine("export text file(s) to your computer.");
            }
            if (parameter1 == " ")
            {
                Console.WriteLine("CD\t:Displays the name of or changes the current directory.\n");
                Console.WriteLine("CLS\t:Clear the screen.\n");
                Console.WriteLine("DIR\tDisplays a list of files and subdirectories in a directory.");
                Console.WriteLine("HELP\tProvides help information for Windows commands.");
                Console.WriteLine("EXIT\tQuits the CMD.EXE program (command interpreter) or the current batch script.");
                Console.WriteLine("COPY\tCopies one or more files to another location.");
                Console.WriteLine("DEL\tDeletes one or more files.");
                Console.WriteLine("MD\tCreates a directory.");
                Console.WriteLine("RD\tRemoves (deletes) a directory.");
                Console.WriteLine("RENAME\tRenames a file or files.");
                Console.WriteLine("TYPE\tDisplays the contents of a text file or files.");
                Console.WriteLine("IMPORT\timport text file(s) from your computer.");
                Console.WriteLine("EXPORT\texport text file(s) to your computer.");
            }
        }

        static void copy(string name, string dest)
        {
            if (dest.Contains('\\'))
            {
                if (!isFullPathd(dest))
                {
                    Console.WriteLine("ERROR : this path \" " + dest + " \" is not a valid path!");
                    return;
                }

                string[] dirs = dest.Split('\\');
                int len = dirs.Length;
                current.Dir_name = dirs[0].ToCharArray();
                current.parent = null;
                current.dir_Firstcluster = current.searchDirectory(dirs[0]);
                for (int i = 1; i < len - 2; i++)
                {
                    current.parent = current;
                    int index = current.searchDirectory(dirs[i]);
                    current.Dir_name = current.DirOrFiles[index].Dir_name;
                    current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                }


                Directory neww = new Directory(dirs[len - 1], 0x10, current.dir_Firstcluster, current, 0);

                current.DirOrFiles.Add(neww);
                File_Entry f = new File_Entry(name, 0X0, current.dir_Firstcluster, 0, neww, "");
                Directory d = new Directory(name, 0x0, current.dir_Firstcluster, neww, 0);
                //current.DirOrFiles.Add(d);
                neww.DirOrFiles.Add(d);


            }

            else
            {
                if(current.searchDirectory(name)!=-1&&current.searchDirectory(dest)!=-1)
                {
                    int indexFile = current.searchDirectory(name);
                    int indexDir = current.searchDirectory(dest);
                    File_Entry temp = (File_Entry)current.DirOrFiles[indexFile];
                    current = (Directory)current.DirOrFiles[indexDir];
                    File_Entry newfile = temp;
                    current.DirOrFiles.Add(newfile);
                    current.UpdateContent(newfile);
                    current = current.parent;
                }
                else
                {
                    Console.WriteLine("no");
                }
                //Directory neww = new Directory(dest, 0x10, current.dir_Firstcluster, current, 0);
                //current.DirOrFiles.Add(neww);
                ////Console.WriteLine(current.searchDirectory(paramter));
                //File_Entry f = new File_Entry(name, 0X0, current.dir_Firstcluster, 0, neww, "");
                //f.writeFileContent();
                //neww.DirOrFiles.Add(f);
                //int index = current.searchDirectory(dest);
                //current.DirOrFiles[index] = neww;
                //neww.deleteDirectory();
                //Directory d = new Directory(name, 0x0, current.dir_Firstcluster, neww, 0);
                ////current.DirOrFiles.Add(d);               

            }



        }
        static void del(string paramter)
        {
            int index = Program.current.searchDirectory(paramter);
            if (index != -1)
            {
                current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                current.dir_filesize = current.DirOrFiles[index].dir_filesize;
                string content = current.DirOrFiles[index].content;
                File_Entry f = new File_Entry(paramter, 0x0, current.dir_Firstcluster, current.dir_filesize, current, content);
                f.deleteFile();
            }
            else
            {
                Console.WriteLine("File does not exist");
            }
        }
        static void md(string paramter)
        {
            if (paramter.Contains('\\'))
            {
                if (!isFullPathd(paramter))
                {
                    Console.WriteLine("ERROR : this path \" " + paramter + " \" is not a valid path!");
                    return;
                }
                Directory temp = current;
                string[] dirs = paramter.Split('\\');
                int len = dirs.Length;
                current.Dir_name = dirs[0].ToCharArray();
                current.parent = null;
                current.dir_Firstcluster = current.searchDirectory(dirs[0]);
                for (int i = 1; i < len - 2; i++)
                {
                    current.parent = current;
                    int index = current.searchDirectory(dirs[i]);
                    current.Dir_name = current.DirOrFiles[index].Dir_name;
                    current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                }
                if (current.searchDirectory(dirs[len - 1]) != -1)
                {
                    Console.WriteLine("ERROR : this directory \" " + dirs[len - 1] + " \" already exists!");
                }
                else
                {
                    Directory neww = new Directory(dirs[len - 1], 0x10, FAT_Table.getAvalableCluster(), current,0);
                    current.DirOrFiles.Add(neww);
                }
                current = temp;
            }
            else
            {
                Directory temp = current;
                if (current.searchDirectory(paramter) != -1)
                    Console.WriteLine("ERROR : this directory \" " + paramter + " \" already exists!");
                else
                {
                    Directory neww = new Directory(paramter, 0x10, FAT_Table.getAvalableCluster(), current,0);
                    current.DirOrFiles.Add(neww);                   
                    //Console.WriteLine(current.searchDirectory(paramter));
                }
                
                current = temp;
                //for (int i = 0; i < current.DirOrFiles.Count; i++)
                //    Console.WriteLine(current.DirOrFiles[i].Dir_name);
            }
            //current.writeDirectory();
            
        }
        static void rd(string pararmtr)
        {
            int index = Program.current.searchDirectory(pararmtr);
            if (index != -1)
            {
                int FirstCluster = Program.current.DirOrFiles[index].dir_Firstcluster; //علشان اشوف الفولدر موجود ولا
                Directory dir = new Directory(pararmtr, 0x10, FirstCluster, Program.current,0);
                dir.deleteDirectory();
            }
            else
            {
                Console.WriteLine("Folder not exist");
            }
        }
        static void renamee(string oldname, string newname)
        {
            int index = current.searchDirectory(oldname);
            int index1 = current.searchDirectory(newname);
            if (index != -1)
            {
                if (index1 == -1)
                {
                    string[] temp = newname.Split('.');
                    current.DirOrFiles[index].assignFileName(temp[0].ToCharArray(),temp[1].ToCharArray());// = newname.ToCharArray();
                    DirectoryEntry d;
                    //current.DirOrFiles[index].oldname;
                }
                else
                {
                    Console.WriteLine("doublicate file name exists or file name can not be found ");
                }
            }
            else { Console.WriteLine("the system can not find the file specfied"); }
        }
        static void type(string paramter)
        {
            string[] tempArr = paramter.Split('.');
            string temp=tempArr[0]+'.'+tempArr[1];
            for (int i = paramter.Length; i < 11; i++)
                temp += ' ';
            //Console.WriteLine("("+paramter+")");
            int index = current.searchDirectory(temp);

            if (index != -1)
            {
                //Console.WriteLine("Ana hena ahooo");
                current.parent = current;
                current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                current.dir_filesize = current.DirOrFiles[index].dir_filesize;
                string content = null;
                File_Entry f = new File_Entry(paramter, 0x0, current.dir_Firstcluster, current.dir_filesize, current, content);
                f.readFileContent();
                Console.WriteLine(current.DirOrFiles[index].content);
            }
            else
            {
                Console.WriteLine("this File is not exist");
            }
        }
        static void import(string paramter)
        {
            char[] delimiters = { '\\', ':' };
            string[] fullpath = paramter.Split(delimiters);
            string content = File.ReadAllText(paramter);
            string name = fullpath[fullpath.Length - 1];
            int size = content.Length;
            //Console.WriteLine(size);            
            int firstcluster;
            if (size > 0)
            {
                firstcluster = FAT_Table.getAvalableCluster();
            }
            else
            {
                firstcluster = 0;
            }
            if (current.searchDirectory(name) == -1)
            {
                File_Entry f = new File_Entry(name, 0X0, firstcluster, size, current, content);
                f.writeFileContent();
                Directory d = new Directory(name, 0x0, firstcluster, current,size);
                //current.DirOrFiles.Add(d);
                current.DirOrFiles.Add(f);
                //current.UpdateContent(d);
                current.UpdateContent(f);
                //Console.WriteLine(f.content);
                //Console.WriteLine(f.dir_attr);

            }
            else
            {
                Console.WriteLine("The File Is Already Exists");
            }

        }
        public static void export(string source, string dest)
        {
            if (current.searchDirectory(source) != -1)
            {

                int index = current.searchDirectory(source);
                current.dir_Firstcluster = current.DirOrFiles[index].dir_Firstcluster;
                current.dir_filesize = current.DirOrFiles[index].dir_filesize;
                string content = current.DirOrFiles[index].content;
                File_Entry f = new File_Entry(source, 0x0, current.dir_Firstcluster, current.dir_filesize, current, content);
                f.readFileContent();
                string temp = dest +'\\'+ source;
                if (System.IO.Directory.Exists(dest))
                {
                    using StreamWriter sw = File.CreateText(temp);
                    //StreamWriter s = new StreamWriter(dest);
                    sw.Write(f.content);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("the system can not find the file ");
                }
            }
            else
            {
                Console.WriteLine("This File Is Not Exists In The Virtual Disk");
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to OS_PROJECT_VIRTUAL_DISK_SHELL\n\n");
            Virtual_Disk.initalize("virtualDisk.txt");
            FAT_Table.createFAT();
            //FAT_Table.printFAT();
            //Console.WriteLine(newdirctory.parent);
            currentPath = new string(current.Dir_name);
            currentPath = currentPath.Trim(new char[] { '\0', ' ' });
            currentPath += "\\";
            for (int j = 0; ; j++)
            {
                Console.Write(currentPath + ">");
                //current.readDirectory();
                /*for (int i = 0; i < current.DirOrFiles.Count; i++)
                {
                    Console.WriteLine(new string(current.DirOrFiles[i].Dir_name) + $" fc={current.DirOrFiles[i].dir_Firstcluster}");

                }*/
                string[] arr = { "cd", "cls", "dir", "help", "exit", "copy", "del", "md", "rd", "rename", "type", "import", "export" };
                string[] token = Console.ReadLine().Split(" ");
                string input = token[0];

                //transform(input.begin(), input.end(), input.begin(), tolower);

                if (input == arr[0])
                {
                    if (token.Length == 1)
                    {
                        cd();

                    }
                    else if (token.Length == 2)
                    {
                        cd(token[1]);

                    }
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");


                }
                else if (input == arr[1])
                {
                    if (token.Length != 1)
                    {
                        Console.WriteLine("ERROR : cls command syntax is \n\tcls\n\tfunction: Clear the screen.");
                    }
                    else
                        Cls();


                }
                else if (input == arr[2])
                {
                    if (token.Length == 1)
                        Dir();
                    else if (token.Length == 2)
                    {
                        Dir(token[1]);
                    }
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == arr[3])
                {
                    if (token.Length == 1)
                        Help();
                    else if (token.Length == 2)
                    {
                        Help(token[1]);
                    }
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");

                }
                else if (input == arr[4])
                {
                    if (token.Length != 1)
                    {
                        Console.WriteLine("ERROR : exit command syntax is \n\texit\n\tfunction: Exit the shell.");
                    }
                    else
                        break;
                }
                else if (input == arr[5])
                {
                    if (token.Length == 3)
                        copy(token[1], token[2]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == arr[6])
                {
                    if (token.Length == 2)
                        del(token[1]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == arr[7])
                {
                    if (token.Length == 2)
                        md(token[1]);
                    else
                        Console.WriteLine("ERROR : md command syntax is\n\tmd [directory]\n\t[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
                }
                else if (input == arr[8])
                {
                    if (token.Length == 2)
                        rd(token[1]);
                    else
                        Console.WriteLine("ERROR : rd command syntax is\n\trd [directory]\n\t[directory] can be a directory name or fullpath of a directory\nRemoves a directory.");
                }
                else if (input == arr[9])
                {
                    if (token.Length == 3)
                        renamee(token[1], token[2]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == arr[10])
                {
                    if (token.Length == 2)
                        type(token[1]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == arr[11])
                {
                    if (token.Length == 2)
                        import(token[1]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if(input==arr[12])
                {
                    if (token.Length == 3)
                        export(token[1], token[2]);
                    else
                        Console.WriteLine("The syntax of the command is incorrect.");
                }
                else if (input == "")
                {

                }
                else
                {

                    Console.WriteLine(token[0] + " is not recognized as an internal or external command,operable program or batch file.");
                }
            }

        }
    }
}

