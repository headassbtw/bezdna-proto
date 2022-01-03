using System.ComponentModel;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace RPAK2L.Updater
{
    public class Main
    {
        private static bool done = false;
        private static WebClient client = null;
        private static string destzip;
        private static string dest_dir;
        private static string update_tmp_dir;
        private static string fileext = "";
        private static string dl_url = "";
        public static int Update(string dir)
        {
            dest_dir = dir;
            update_tmp_dir = Path.Combine(dest_dir, ".update");
            Console.WriteLine("Updater 0.0.12");
            Directory.CreateDirectory(update_tmp_dir);
            
            if (OperatingSystem.IsWindows())
            {
                Console.WriteLine("Windows");
                fileext = "zip";
                dl_url = "https://github.com/headassbtw/rpak2l/releases/latest/download/Windows_x64_Release.zip";
            }
            else if (OperatingSystem.IsLinux())
            {
                Console.WriteLine("Linux");
                fileext = "tar.gz";
                dl_url = "https://github.com/headassbtw/rpak2l/releases/latest/download/Linux_x64_Release.tar.gz";
            }
            destzip = Path.Combine(update_tmp_dir, $"update.{fileext}");
            
            File.Delete(destzip);
            Console.WriteLine($"downloading from {dl_url}");
            Console.WriteLine($"downloading  to  {destzip}");
            client = new WebClient();
            client.DownloadFileCompleted += Complete;
            
            client.DownloadProgressChanged += (sender, args) =>
            {
                Console.WriteLine("");
            };
            
            //client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            ThreadPool.QueueUserWorkItem(task => client.DownloadFileAsync(new Uri(dl_url), destzip));

            while (!done)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        private static void Complete(object sender, AsyncCompletedEventArgs e)
        {
            client = null;
            Console.WriteLine("Complete!");


            if (fileext == "tar.gz")
            {
                string tarpath = Path.Combine(update_tmp_dir, "update.tar");
                ICSharpCode.SharpZipLib.GZip.GZip.Decompress(File.OpenRead(destzip),
                    File.Create(tarpath),true);
                var tar = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(File.OpenRead(tarpath), Encoding.ASCII);
                tar.ExtractContents(dest_dir);
            }
            else if (fileext == "zip")
            {
                ZipFile.ExtractToDirectory(destzip, dest_dir);
            }
            
            Directory.Delete(update_tmp_dir, true);
            done = true;
        }
    }
}