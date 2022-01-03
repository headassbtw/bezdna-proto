using System;
using bezdna_proto.Titanfall2;
using bezdna_proto.Titanfall2.FileTypes;
using RPAK2L.Program.ViewModels.FileView.Types;

namespace RPAK2L.Program.Backend
{
    public enum Game
    {
        R2,
        R5
    }


    public static class Extensions
    {
        public static Type GetType(this FileEntryInternal file)
        {
            string extension = file.ShortName;

            switch (extension)
            {
                case "txtr":
                    return typeof(Texture);
                case "matl":
                    return typeof(Material);
                case "shdr":
                    return typeof(Shader);
                case "dtbl":
                    return typeof(DataTables);
                default:
                    return typeof(FileEntryInternal);
            }
        }

        public static FileType Type(this FileEntryInternal file)
        {
            string extension = file.ShortName;

            switch (extension)
            {
                case "txtr":
                    return FileType.Texture;
                case "matl":
                    return FileType.Material;
                case "shdr":
                    return FileType.Shader;
                case "dtbl":
                    return FileType.DataTables;
                default:
                    return FileType.Misc;
            }
        }
    }
    
    public enum FileType
    {
        Texture,
        Material,
        Shader,
        DataTables,
        Misc
    }
}