using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReplacePictures
{
    class Program
    {
        public static string path = Environment.CurrentDirectory;
        private static List<customTable> charactersName = new List<customTable>();
        public static long header = 0;
        public static string fileName = "vol1";

        static void Main(string[] args)
        {

            if(args.Length > 0)
            {
                fileName = args[0];
            }

            if(fileName.EndsWith(".dat"))
            {
                if (!File.Exists(path + "\\" + fileName))
                {
                    Console.WriteLine("File " + fileName + " not found. (0x001)");
                    Environment.Exit(-1);
                    return;
                } 
                Extract();
            }
            else
            {
                if (!File.Exists(path + "\\" + fileName + ".dat"))
            {
                Console.WriteLine("File " + fileName + ".dat not found. (0x001)");
                Environment.Exit(-1);
                return;
            } 
                Insert();
            }

        }

        static void Extract()
        {
            if(Directory.Exists(path + "\\" + fileName + "\\"))
            {
                Console.WriteLine("The folder " + fileName + " already exist. Export cancelled.");
                Environment.Exit(0);
            }

            string finalPath = path + "\\" + fileName.Replace(".dat", "") + "\\";

            Directory.CreateDirectory(finalPath);

            FileStream fileStream = new FileStream(path + "\\" + fileName, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);
            MemoryStream memoire = new MemoryStream();

            long nbFiles = ReadBytes(fileStream, 4);
            long i = 0;

            Console.WriteLine(nbFiles + " files found.");

            for(i = nbFiles ; i > 0 ; i--)
            {
                fileStream.Seek(0x4, SeekOrigin.Current); // The 4th bytes are unknown, so we skip them

                byte[] offset = new byte[4]; // Current image address
                fileStream.Read(offset, 0, 4);
                byte[] pictureLength = new byte[4]; // Current image length
                fileStream.Read(pictureLength, 0, 4);
                long currentPosition = fileStream.Position; // We save the position, and return to it after

                fileStream.Seek(BitConverter.ToInt32(offset), SeekOrigin.Begin);
                byte[] picture = new byte[BitConverter.ToInt32(pictureLength)];
                fileStream.Read(picture, 0, BitConverter.ToInt32(pictureLength));
                string ext = "bin";
                if(picture[0] == 137) // The file is an image
                    ext = "png";
                else if(picture[0] == 79) // The file is a font
                    ext = "otf";
                else if(picture[0] == 68) // The file is a font (TrueType)
                    ext = "ttf";
                else if(picture[0] == 208 || picture[0] == 161) // the file is a thumbs.db (?)
                    ext = "thumbs.db";
                else if(picture[0] == 102 || picture[0] == 109) // the file is a video
                    ext = "mp4";

                File.WriteAllBytes(finalPath + "\\" + (nbFiles - i) + "." + ext, picture);
                Console.WriteLine((nbFiles - i) + "." + ext + " saved!");

                fileStream.Seek(currentPosition, SeekOrigin.Begin);
            }

        }

        static void Insert()
        {
            string pattern = @"^\d+\.(png|ttf|otf|db|mp4|thumbs\.db)$"; // We get only MMBNLC's files, so you can place any "edit" files like .psd into your folder!

            string[] fichiers = Directory.GetFiles(path + "\\" + fileName + "\\")
            .Where(fichier => Regex.IsMatch(Path.GetFileName(fichier), pattern, RegexOptions.IgnoreCase))
            .Select(file => file) // Unnecessary, but added for clarity
            .ToArray();

            FileStream fileStream = new FileStream(path + "\\" + fileName + ".dat", FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);
            MemoryStream memoire = new MemoryStream();

            long nbMessage = ReadBytes(fileStream, 4);

            if(nbMessage != fichiers.Length){
                Console.WriteLine("The number of files in " + fileName + ".dat (" + nbMessage + ") is different from the number of files in the \"" + fileName + "\" (" + fichiers.Length + ") folder. Import cancelled.");
                Environment.Exit(0);
            }

            fileStream.Position = 0;
            byte[] nbImages = new byte[4];
            fileStream.Read(nbImages, 0, 4);
            memoire.Write(nbImages);
            long lengthTotal = 0;

            header = (nbMessage * 0xC) + 4;

            Console.WriteLine("Start of reintegration of " + fileName + ".dat.");

            foreach (var file in fichiers)
            {
                byte[] octets = File.ReadAllBytes(path + "\\" + fileName + "\\" + Path.GetFileName(file));
                charactersName.Add(new customTable(Int32.Parse(Path.GetFileName(file).Replace(".png", "").Replace(".thumbs.db", "").Replace(".ttf", "").Replace(".mp4", "").Replace(".otf", "")), octets, octets.Length));
            }

            long i = 0;

            Console.WriteLine("Loaded " + fichiers.Length + " files.");

            for(i = nbMessage ; i > 0 ; i--)
            {
                byte[] unknow = new byte[4];
                fileStream.Read(unknow, 0, 4);
                memoire.Write(unknow);

                var characterNameFound = charactersName.Find(characterName => characterName.Id == (nbMessage - i));
                long octet = header + lengthTotal;
                byte[] offset = new byte[4];
                Array.Copy(BitConverter.GetBytes(octet), offset, Math.Min(BitConverter.GetBytes(octet).Length, offset.Length));

                byte[] pictureLength = new byte[4];
                Array.Copy(BitConverter.GetBytes(characterNameFound.Length), pictureLength, Math.Min(BitConverter.GetBytes(characterNameFound.Length).Length, pictureLength.Length));

                memoire.Write(offset);
                memoire.Write(pictureLength);

                fileStream.Seek(8, SeekOrigin.Current);
                lengthTotal += characterNameFound.Length;

            }
            Console.WriteLine("Header updated.");

            for(i = nbMessage ; i > 0 ; i--)
            {
                var picturesData = charactersName.Find(characterName => characterName.Id == (nbMessage - i));
                byte[] picture = picturesData.Name;
                memoire.Write(picture);
            }

            using (FileStream fileStreamFinal = new FileStream(path + "\\" + fileName + " NEW.dat", FileMode.Create))
            {
                Console.WriteLine("File " + fileName + " NEW.dat saved !");
                memoire.WriteTo(fileStreamFinal);
            }
        }

        static int ReadBytes(FileStream file, int size, long offset = 0)
        {
            byte[] bytes = new byte[size];
            file.Seek(0 + offset, SeekOrigin.Current);
            file.Read(bytes, 0, size);
            if (size == 4)
                return BitConverter.ToInt32(bytes, 0);
            if (size == 2)
                return BitConverter.ToUInt16(bytes, 0);
            else
                return bytes[0];
        }
    }
}