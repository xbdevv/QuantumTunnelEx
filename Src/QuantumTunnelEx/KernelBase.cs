using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace QuantumTunnel
{
    internal static class KernelBase
    {
        const string DllName = "kernelbase.dll";
        [DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPWStr)] string filename, [MarshalAs(UnmanagedType.U4)] FileAccess access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(DllName, SetLastError = true)]
        public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport(DllName)]
        public static extern uint GetLastError();

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport(DllName, SetLastError = true)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, [Out] StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        public static string GetLastErrorMessage(int errorCode)
        {
            StringBuilder messageBuffer = new StringBuilder(512);
            if (KernelBase.FormatMessage(4608, IntPtr.Zero, errorCode, 0, messageBuffer, messageBuffer.Capacity, IntPtr.Zero) == 0)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Unknown error code: ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(errorCode);
                return defaultInterpolatedStringHandler.ToStringAndClear();
            }
            return messageBuffer.ToString().Trim();
        }

        private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;
    }
}
