namespace RPAK2L.UI
{
    public class ExitTypeArgs : EventArgs
    {
        public int Type;
        public string Parameters;

        public ExitTypeArgs(int type)
        {
            Type = type;
        }
        public ExitTypeArgs(int type, string param)
        {
            Type = type;
            Parameters = param;
        }
    }
    
    public class Funcs
    {
        public static event EventHandler<ExitTypeArgs> Exiting;

        public static void Exit(int type)
        {
            var e = new ExitTypeArgs(type);
            Exiting.Invoke(null, e);
        }
        public static void Exit(int type, string param)
        {
            var e = new ExitTypeArgs(type, param);
            Exiting.Invoke(null, e);
        }
    }
}