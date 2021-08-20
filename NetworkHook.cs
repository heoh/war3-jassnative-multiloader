using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Cirnix.JassNative.Runtime.Windows;
using Cirnix.JassNative.Runtime.Utilities;

namespace Cirnix.JassNative.MultiLoader
{
    public static class NetworkHook
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SockAddr  // bigendian
        {
            public short sin_family;
            public ushort sin_port;
            public IPAddr sin_addr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sin_zero;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IPAddr
        {
            public byte s_b1;
            public byte s_b2;
            public byte s_b3;
            public byte s_b4;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Winsock_bindPrototype(IntPtr s, ref SockAddr name, int namelen);

        private static Winsock_bindPrototype Winsock_bind;

        private static Lazy<bool> MultiWindowMode = new Lazy<bool>(() => CountGameApplications() > 1);
        private static Lazy<IPAddr> BindedIP = new Lazy<IPAddr>(() => CreateRandomIP());

        public static void Initialize()
        {
            try
            {
                IntPtr wsModule = Kernel32.GetModuleHandle("Ws2_32.dll");
                Winsock_bind = Memory.InstallHook(Kernel32.GetProcAddress(wsModule, "bind"), new Winsock_bindPrototype(Winsock_bindHook), true, false);
            }
            catch
            {
                Trace.WriteLine("NetworkHook Initialization Failed!");
            }
        }

        private static int Winsock_bindHook(IntPtr s, ref SockAddr name, int namelen)
        {
            if (MultiWindowMode.Value)
            {
                name.sin_addr = BindedIP.Value;
                Trace.WriteLine($"Winsock_bind: IP: {name.sin_addr.s_b1}.{name.sin_addr.s_b2}.{name.sin_addr.s_b3}.{name.sin_addr.s_b4}");
            }

            return Winsock_bind(s, ref name, namelen);
        }

        private static int CountGameApplications()
        {
            Process[] ProcessList = Process.GetProcessesByName("War3");
            return ProcessList.Length;
        }

        private static IPAddr CreateRandomIP()
        {
            Random rand = new Random();
            return new IPAddr()
            {
                s_b1 = 127,
                s_b2 = (byte)rand.Next(0, 256),
                s_b3 = (byte)rand.Next(0, 256),
                s_b4 = (byte)rand.Next(1, 255),
            };
        }
    }
}
