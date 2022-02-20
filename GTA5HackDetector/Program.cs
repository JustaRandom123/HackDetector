using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GTA5HackDetector
{
    class Program
    {
        public static string path = string.Empty;
        public static bool md5YesOrNo = false;
        public static ArrayList possibleHacks = new ArrayList();
        public static ArrayList foundhacksbymd5 = new ArrayList();
        public static string blacklistfilePath = AppDomain.CurrentDomain.BaseDirectory + "blacklist.txt";
        public static string hashlistfilePath = AppDomain.CurrentDomain.BaseDirectory + "hashlist.txt";



        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        static void Main(string[] args)
        {
            ShowWindow(ThisConsole, MAXIMIZE);

            Console.WriteLine("Durch die benutzung von diesem Tool stimmts du zu das dieses Programm berechtigt ist auf sämtliche dateien bzw ordner zuzugreifen und diese auf modding tools zu überprüfen! Das tool kopiert keine dateien oder sammelt Persönliche informationen\nDrücke die 'Enter' taste um dies zu bestätigen");
            var pressedKey = Console.ReadKey();
            if (pressedKey.Key != ConsoleKey.Enter)
            {
                return;
            }
            Console.Clear();
         
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("Hack detector by JustaRandom");
            Console.WriteLine("All rights to the Creator of the program");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------------");



            if (!File.Exists(blacklistfilePath)) { Console.WriteLine("Error! Bitte lege eine blacklist.txt datei an im selben ordner wo das program selber liegt!"); Console.ReadKey(); return; }
            if (!File.Exists(hashlistfilePath)) { Console.WriteLine("Error! Bitte lege eine hashlist.txt datei an im selben ordner wo das program selber liegt!"); Console.ReadKey(); return; }


            var b = new BuildFileList();
            var sw = new Stopwatch();
            Console.WriteLine("Gib bitte eine festplatte an die durchsucht werden soll zb 'C:\\'");
            path = Console.ReadLine();
            Console.WriteLine("Sollen die dateien auf MD5 hash mit unserer Datenbank abgeglichen werden? j/n");
          
            if (Console.ReadLine() == "n")
            {
                md5YesOrNo = false;
                Console.WriteLine("MD5 hash check deaktiviert");
            }
            else
            {
                md5YesOrNo = true;
                Console.WriteLine("MD5 hash check aktiviert");
            }

            Console.WriteLine("Laden der dateipfade... Bitte warten! (Dies kann ein paar Minuten dauern)");
            sw.Start();
            var files = b.GetFiles();      
            sw.Stop();

            foreach(var fileName in files)
            {
                if (md5YesOrNo)
                {
                    try
                    {
                        foreach (string item in File.ReadAllLines(hashlistfilePath))
                        {
                            if (CalculateMD5(fileName.FullName) == item)
                            {
                                foundhacksbymd5.Add(fileName.FullName + " MD5: " + item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "error.txt", e.ToString());
                    }

                    Console.WriteLine(fileName.FullName + " Hash: " + CalculateMD5(fileName.FullName));
                }
                else
                {
                    try
                    {
                        foreach (string item in File.ReadAllLines(blacklistfilePath))
                        {
                            if (fileName.FullName.Contains(item))
                            {
                                possibleHacks.Add(fileName.FullName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "error.txt", e.ToString());
                    }
                    Console.WriteLine(fileName.FullName);
                }
            }

            Console.WriteLine("\n\n\n{0} dateien in {1} sekunden geladen auf festplatte {2}", files.Count, sw.Elapsed.TotalSeconds, path);


            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n\n\n\n\n\n\nPotenzielle hacks gefunden {0}", possibleHacks.Count);           
            foreach (string pHack in possibleHacks)
            {

                Console.WriteLine(pHack);
            }


            Console.WriteLine("\n\n\n\n\n\n\n\nMD5 check hacks gefunden {0}", foundhacksbymd5.Count);
            foreach (string pHack in foundhacksbymd5)
            {

                Console.WriteLine(pHack);
            }

            Console.WriteLine("\n\n\nDas program ist fertig!");
            Console.ReadLine();
        }


        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);              
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }


        public class BuildFileList
        {
            public List<FileInfo> GetFiles()
            {
                var di = new DirectoryInfo(path);
                var directories = di.GetDirectories();
                var files = new List<FileInfo>();
                foreach (var directoryInfo in directories)
                {
                    try
                    {                      
                        GetFilesFromDirectory(directoryInfo.FullName, files);                       
                    }
                    catch (Exception)
                    {
                    }
                }
                return files;
            }

            private void GetFilesFromDirectory(string directory, List<FileInfo> files)
            {
                var di = new DirectoryInfo(directory);
                var fs = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                files.AddRange(fs);
                var directories = di.GetDirectories();
                foreach (var directoryInfo in directories)
                {
                    try
                    {                    
                        GetFilesFromDirectory(directoryInfo.FullName, files);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
