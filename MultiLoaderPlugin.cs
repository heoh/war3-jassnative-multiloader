using System.Diagnostics;

using Cirnix.JassNative.Runtime.Plugin;

namespace Cirnix.JassNative.MultiLoader
{
    public class MultiLoaderPlugin : IPlugin
    {
        public void Initialize() { }

        public void OnGameLoad()
        {
            Trace.WriteLine("Initializing MultiLoader...");
            Trace.Indent();
            MultiLoaderHook.Initialize();
            NetworkHook.Initialize();
            Trace.WriteLine("Done!");
            Trace.Unindent();
        }

        public void OnMapEnd() { }

        public void OnMapLoad() { }

        public void OnProgramExit() { }

    }
}
