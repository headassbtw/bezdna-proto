namespace System.IO
{
    public class FileExtras
    {
        public static void DeleteIfExists(string path){ if (File.Exists(path)) File.Delete(path); }
    }
}