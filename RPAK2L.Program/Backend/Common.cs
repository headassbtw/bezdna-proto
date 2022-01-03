using System;
using System.Diagnostics;
using System.IO;
using bezdna_proto;
using bezdna_proto.Titanfall2;
using RPAK2L.Program.Backend.Games;
using RPAK2L.Program.Dialogs;

namespace RPAK2L.Program.Backend
{
    public class PakInterface
    {
        public FileStream PakReadStream;
        public R2Pak? R2Pak;
        public bezdna_proto.Apex.RPakFile? R5Pak;
        public Game Game;
        public int Version;

        public PakInterface(string PakFile)
        {
            Stopwatch sw = Stopwatch.StartNew();
            PakReadStream = new FileStream(PakFile, FileMode.Open, FileAccess.Read);
            byte[] header = new byte[4];
            PakReadStream.Read(header, 0, 4);
            sw.Stop();
            Logger.Log.Info($"Finished reading rpak file in {sw.ElapsedMilliseconds}ms");
            if (!Utils.ValidRPakHeader(header))
            {
                throw new InvalidOperationException("Invalid RPak file");
            }

            Version = Utils.GetRPakVersion(PakReadStream);
            PakReadStream.Position = 0;
            switch (Version)
            {
                case 7:
                    Game = Game.R2;
                    Logger.Log.Debug("Loading RPak for R2");
                    R2Pak = new R2Pak(PakReadStream);
                    return;
                case 8:
                    Game = Game.R5;
                    return;
                default:
                    throw new InvalidOperationException("Unknown RPak version");
            }
            
        }
    }
}