using RPAK2L.Program;

namespace RPAK2L
{
    public class Wrapper
    {
        public static void Main(string[] args)
        {
            string rtn = Program.Program.Main(args);
            
            
            if(File.Exists(rtn))
                Console.WriteLine("is a file");
            if(Directory.Exists(rtn))
                Console.WriteLine("is a directory");
            
            Console.WriteLine(rtn);
        }
    }
}