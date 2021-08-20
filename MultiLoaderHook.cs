using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Cirnix.JassNative.Runtime.Windows;
using Cirnix.JassNative.Runtime.Utilities;

namespace Cirnix.JassNative.MultiLoader
{
    public static class MultiLoaderHook
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate bool sub_6F0BD0A0Prototype(IntPtr a1, string a2);

        private static sub_6F0BD0A0Prototype sub_6F0BD0A0;

        public static void Initialize()
        {
            try
            {
                IntPtr gameModule = Kernel32.GetModuleHandle("game.dll");
                sub_6F0BD0A0 = Memory.InstallHook(gameModule + 0x0BD0A0, new sub_6F0BD0A0Prototype(sub_6F0BD0A0Hook), true, false);
            }
            catch
            {
                Trace.WriteLine("MultiLoaderHook Initialization Failed!");
            }
        }

        private static bool sub_6F0BD0A0Hook(IntPtr a1, string a2)
        {
            sub_6F0BD0A0(a1, a2);
            return true;
        }
    }
}
