using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Avalonia.Threading;
using RPAK2L;
using RPAK2L.Backend;
using RPAK2L.Dialogs;
using RPAK2L.ViewModels.FileView.Views;

// this is utterly retarded... (C) VALVe
using Wasmtime;

namespace bezdna_proto
{
    public class Utils
    {
        public const int HEADER_SIZE7 = 88;
        public const int HEADER_SIZE8 = 0x80;

        public static bool ValidRPakHeader(byte[] file)
        {
            return (file[0] == 'R') && (file[1] == 'P') && (file[2] == 'a') && (file[3] == 'k');
        }

        public static int GetRPakVersion(byte[] file)
        {
            if (file.Length < 88)
                return 0;

            return file[5] + (file[6] << 8);
        }

        private static ProgressableTask _task;

        public static int GetRPakVersion(FileStream file)
        {
            if (file.Length < 88)
                return 0;

            file.Position = 4;
            var b5 = file.ReadByte();

            return b5 + (file.ReadByte() << 8);
        }

        public static ulong Hash(string toHash)
        {
            var bytes = Encoding.ASCII.GetBytes(toHash + "\0\0\0\0");
            var reader = new BinaryReader(new MemoryStream(bytes));

            var firstDWord = reader.ReadUInt32();

            ulong v2 = 0;
            uint v3 = 0;
            uint v4 = (firstDWord - 45 * ((~(firstDWord ^ 0x5C5C5C5Cu) >> 7) & (((firstDWord ^ 0x5C5C5C5Cu) - 0x1010101) >> 7) & 0x1010101)) & 0xDFDFDFDF;
            
            uint v8;
            ulong i;
            
            for (i = ~firstDWord & (firstDWord - 0x1010101) & 0x80808080; i == 0; i = v8 & 0x80808080)
            {
                ulong v6 = v4;
                var v7 = reader.ReadUInt32();
                v3 += 4;
                v2 = ((((0xFB8C4D96501u * v6) >> 24) + 0x633D5F1u * v2) >> 61) ^ (((0xFB8C4D96501u * v6) >> 24) + 0x633D5F1u * v2);
                v8 = ~v7 & (v7 - 0x1010101);
                v4 = (v7 - 45 * ((~(v7 ^ 0x5C5C5C5Cu) >> 7) & (((v7 ^ 0x5C5C5C5Cu) - 0x1010101) >> 7) & 0x1010101)) & 0xDFDFDFDF;
            }

            uint v10 = (uint)((i & (uint.MaxValue - i + 1)) - 1);
            uint v9 = 31u - (uint)System.Numerics.BitOperations.LeadingZeroCount(v10); // _BitScanReverse, should also do -1 if it's 0

            return 0x633D5F1 * v2
                + ((0xFB8C4D96501u * (v4 & v10)) >> 24)
                - 0xAE502812AA7333u * (v3 + v9 / 8);
        }

        public static int prog;
        public static byte[] Decompress(FileStream file, ulong expectedDSize, int headerSize)
        {
            
            file.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[file.Length];
            file.Read(bytes);
            
            using var engine = new Engine();
            using var module = Module.FromBytes(engine, "decomp", Properties.Resources.decomp);
            //using var module = Module.FromFile(engine, @"D:\Projects\wasi-sdk-12.0\decomp.wasm");

            using var host = new Host(engine);
            var Memory = host.DefineMemory("env", "memory", 2);
            using dynamic instance = host.Instantiate(module);

            if (!Memory.Grow((uint)(bytes.Length + 0x100 + Memory.PageSize - 1) / Memory.PageSize))
                throw new Exception("OOM @ Initial write");

            const int parameters = Memory.PageSize * 2;
            const int startOffset = parameters + 0x100;
            int offset = startOffset;
            
            for (var i = 0; i < bytes.Length; i++)
            {
                // I can feel the retardation
                Memory.WriteByte(offset, bytes[i]);
                offset++;
            }
            var dSize = instance.get_decompressed_size(parameters, startOffset, -1, bytes.Length, 0, headerSize);

            if ((ulong)dSize != expectedDSize)
                throw new Exception("dSize != header.decompressedSize");

            if (!Memory.Grow((uint)(dSize + Memory.PageSize - 1) / Memory.PageSize))
                throw new Exception("OOM @ dSize grow");

            Memory.WriteInt64(parameters + 8, offset);
            Memory.WriteInt64(parameters + 8 * 3, -1);

            var ret = instance.decompress_rpak(parameters, bytes.Length, dSize);
            if (ret != 1)
                throw new Exception("Invalid compressed data!");

            Stopwatch sw = Stopwatch.StartNew();
            var outb = new byte[dSize];
            ProgressableTask _task = new ProgressableTask(0, outb.Length);
            _task.Init("Decompressing");
            ThreadPool.QueueUserWorkItem(async => {
                while (prog < outb.Length)
                {
                    Dispatcher.UIThread.Post(() => {DirectoryTreeViewModel.TaskProgress = prog; _task.IncrementBar(prog);});
                    Thread.Sleep(100);
                }
            });
            for (var i = 0; i < dSize; i++)
            {
                // Once again, this is retarded
                outb[i] = Memory.ReadByte(offset + i);
                prog = i;
            }
            sw.Stop();
            Logger.Log.Info($"Did dumb memory shit in {sw.ElapsedMilliseconds}ms");
            _task.Finish();
            return outb;
        }
    }
}
