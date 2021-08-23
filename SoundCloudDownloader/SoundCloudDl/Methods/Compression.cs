using System.IO.Compression;

namespace SoundCloudDl.Methods
{
    public class Compression
    {
        public void CompressFolder(string toBeCompressedFolder, string targetDirectory)
        {
            ZipFile.CreateFromDirectory(toBeCompressedFolder, targetDirectory);
        }
    }
}
