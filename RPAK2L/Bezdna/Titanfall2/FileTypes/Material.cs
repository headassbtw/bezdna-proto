using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bezdna_proto.Titanfall2.FileTypes
{
    public class Material
    {
        // pad 0-8
        public ulong GUID { get; private set; }
        public DataDescriptor NameDesc { get; private set; }
        public uint NameOffset { get; private set; }

        // TODO ???-0xd0

        public string Name { get; private set; }
        public string MaterialName { get; private set; }

        public ulong[] TextureReferences { get; private set; }

        // Is this even valid?
        public static readonly string[] TextureRefName =
        {
            "_col",
            "_nml",
            "_gls",
            "_spc",
            "_ilm",
            "UNK5",
            "UNK6",
            "UNK7",
            "_bm", // ???
            "UNK9",
            "UNK10",
            "_ao",
            "_cav",
            "_opa",
        };

        public Material(RPakFile rpak, FileEntryInternal file)
        {
            if (rpak.MinDataChunkID > file.Description.id)
            {
                Name = "OOB";
                return;
            }

            var description = file.Description;
            var descOff = rpak.DataChunkSeeks[description.id] + description.offset;
            rpak.reader.BaseStream.Seek(descOff, System.IO.SeekOrigin.Begin);

            // WTF?
            var pad0 = rpak.reader.ReadUInt64();
            var pad8 = rpak.reader.ReadUInt64();

            if (pad0 != 0)
                throw new Exception("pad0 wasn't 0!!!");
            if (pad8 != 0)
                throw new Exception("pad8 wasn't 0!!!");

            GUID = rpak.reader.ReadUInt64();
            DataDescriptor d;
            d.id = rpak.reader.ReadUInt32();
            d.offset = rpak.reader.ReadUInt32();
            NameDesc = d;

            if (rpak.MinDataChunkID > d.id)
            {
                Name = "OOB2";
                return;
            }

            var backup = rpak.reader.BaseStream.Position;
            rpak.reader.BaseStream.Seek(rpak.DataChunkSeeks[d.id] + d.offset, System.IO.SeekOrigin.Begin);
            Name = rpak.reader.ReadNTString();
            rpak.reader.BaseStream.Position = backup;

            // TODO: figure out wtf is everything else...
            d.id = rpak.reader.ReadUInt32();
            d.offset = rpak.reader.ReadUInt32();
            backup = rpak.reader.BaseStream.Position;
            rpak.reader.BaseStream.Seek(rpak.DataChunkSeeks[d.id] + d.offset, System.IO.SeekOrigin.Begin);
            MaterialName = rpak.reader.ReadNTString();
            rpak.reader.BaseStream.Position = backup;

            rpak.reader.BaseStream.Position = descOff + 0x98; // 0x60 in Apex???
            d.id = rpak.reader.ReadUInt32();
            d.offset = rpak.reader.ReadUInt32();
            var wtfOff = rpak.DataChunkSeeks[d.id] + d.offset;
            d.id = rpak.reader.ReadUInt32();
            d.offset = rpak.reader.ReadUInt32();
            var wtf2Off = rpak.DataChunkSeeks[d.id] + d.offset; // end
            rpak.reader.BaseStream.Position = wtfOff;

            var refcnt = (wtf2Off - wtfOff) / 8;
            TextureReferences = new ulong[refcnt];
            for (var i = 0; i < refcnt; i++)
                TextureReferences[i] = rpak.reader.ReadUInt64();
            //var textureRefs = new List<ulong>();

            /*var texture_guid = rpak.reader.ReadUInt64();
            do
            {
                textureRefs.Add(texture_guid);
                texture_guid = rpak.reader.ReadUInt64();
            } while (texture_guid != 0);

            TextureReferences = textureRefs.ToArray();*/

            // 0x90 - shader set GUID
            // 0xC0 - flag bullshit
        }
    }
}
