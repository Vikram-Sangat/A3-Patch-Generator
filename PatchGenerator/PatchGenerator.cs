using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;


namespace PatchGenerator
{
    class PatchGenerator
    {
        public string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public String readFile(String filename)
        {
            // Read the file as one string.
            System.IO.StreamReader myFile =
               new System.IO.StreamReader(filename);
            string myString = myFile.ReadToEnd();
            myFile.Close();
            return myString;

        }
        public void CreateZip(string path) {
            String data = readFile(Directory.GetCurrentDirectory()+@"\"+path);
            try
            {
                FileStream destinationFile;
                if (path.Contains(@"\"))
                {
                    String paths = path.Replace(@"\", "/");
                    String[] newPath = paths.Split('/');
                    String dst = newPath[0];
                    Directory.CreateDirectory(@"Compressed\" + dst);
                    destinationFile = File.Create(@"Compressed\" + path);
                }
                else
                {
                    destinationFile = File.Create(@"Compressed\" + path);
                }
               
                DeflateStream deflate = new DeflateStream(destinationFile, CompressionMode.Compress);

                StreamWriter writer = new StreamWriter(deflate);

                writer.Write(data.ToString());
                writer.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void CompressFile(string path)
        {
            Directory.CreateDirectory("Compressed");
            FileStream sourceFile = File.OpenRead(path);
            FileStream destinationFile;
            if (path.Contains(@"\"))
            {
                String paths = path.Replace(@"\", "/");
                String[] newPath = paths.Split('/');
                String dst = newPath[0];
                Directory.CreateDirectory(@"Compressed\"+dst);
                destinationFile = File.Create(@"Compressed\" + path);
            }
            else 
            {
                destinationFile = File.Create(@"Compressed\" + path);
            }
           

            byte[] buffer = new byte[sourceFile.Length];
            sourceFile.Read(buffer, 0, buffer.Length);

            using (GZipStream output = new GZipStream(destinationFile,
                CompressionMode.Compress, true))
            {
               // Console.WriteLine("Compressing {0} to {1}.", sourceFile.Name,destinationFile.Name, false);
                //output.w
                output.Write(buffer, 0, buffer.Length);
            }

            // Close the files.
            sourceFile.Close();
            destinationFile.Close();
        }

        static void Main(string[] args)
        {
          
            PatchGenerator ts = new PatchGenerator();
            try
            {
                // Only get files that begin with the letter "c." 
                string path = Directory.GetCurrentDirectory();
                string output = @"" + path + @"\Patch.ini";

                string[] dirs = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                DirectoryInfo DI = new DirectoryInfo(path);
                int size = 0;
                //Console.WriteLine("The number of Files are {0}.", dirs.Length - 1);
                ArrayList data = new ArrayList();

                for (int i = 0; i < dirs.Length; i++)
                {

                    System.IO.FileInfo fi = new System.IO.FileInfo(dirs[i]);
                    if (dirs[i] != System.Reflection.Assembly.GetExecutingAssembly().Location && dirs[i] != output&&!dirs[i].Contains("Compressed"))
                    {
                        string file = dirs[i].Replace(path + @"\", "");
                        string hash = ts.GetMD5HashFromFile(dirs[i]);
                        //dirs[i] = "File Name:" + file + " and size is : " + fi.Length + "byts and hash is :" + hash;
                        //Console.WriteLine(dirs[i]);
                        size = size + Convert.ToInt32(fi.Length);
                        data.Add(file + "|" + fi.Length + "|" + hash);

                    }
                    else { }



                }
               
                StreamWriter Output1;
                Output1 = new StreamWriter(output);
                Output1.Write(data.Count);
                foreach (string d in data) Output1.Write(";"+d);
               
                Output1.Close();
               
                Console.WriteLine("Total size is {0}", size);
                Console.WriteLine("Patch Succefully Generated!!");
                Console.WriteLine("Would you like to generate zip files also ?Enter Y/N");
                String Comp = Console.ReadLine();
                if (Comp == "Y"||Comp=="y") {
                    
                    Console.WriteLine("Compressing " + data.Count+" No of files.");

                    for (int i = 0; i < dirs.Length; i++)
                    {


                        if (dirs[i] != System.Reflection.Assembly.GetExecutingAssembly().Location && dirs[i] != output && !dirs[i].Contains("Compressed"))
                        {
                            string file = dirs[i].Replace(path + @"\", "");
                            Console.WriteLine("Compressing " + file);
                            CompressFile(file);
                        }
                        else { }



                    }
                    Console.WriteLine("Compressing of " + data.Count + " files is successfully Done !!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            Console.ReadKey();
        }
    }
}
