namespace RPAK2L.ViewModels.FileView.Types
{
    public class File
    {
        public string Name { get; set; }
        public string Path { get; set; }
        
        public string Size { get; set; }

        public File(string path = null)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }
        public File(string path = null, string discardPath = null)
        {
            Name = System.IO.Path.GetFileName(path);
        }
    }
}