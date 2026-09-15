using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace QuantumTunnel
{
    internal static class Flash
    {
        private const string RawFlashDeviceName = @"\\.\Xvuc\Flash";

        private const string FlashDeviceName = @"\\.\Xvuc\FlashFs";
        
        private const uint BUFFER_SIZE = 1048576;

        public static async Task<int> ReadFlashImageAsync(string outputFile, IProgress<ulong> progress, CancellationToken ct, string workingDirectrory = null)
        {
            return await Flash.ReadInternalAsync(Flash.RawFlashDeviceName, outputFile,progress,ct, workingDirectrory);
        }

        public static async Task<int> ReadFlashFsFileAsync(string targetFile, string outputFile, IProgress<ulong> progresss, CancellationToken ct,string workingDirectrory = null)
        {
            return await Flash.ReadInternalAsync($@"{Flash.FlashDeviceName}\{targetFile}", outputFile,progresss,ct ,workingDirectrory);
        }

        public static async Task<int> GetFromFullDevicePathAsync(string devicePath, string outputFile, IProgress<ulong> progress, CancellationToken ct, string workingDirectory = null)
        {
            //Console.WriteLine($"device path ->{devicePath} and output file {outputFile}");
            return await ReadInternalAsync($@"{devicePath}\{outputFile}", outputFile,progress,ct, workingDirectory);
        }

        private static async Task<int> ReadInternalAsync(string devicePath, string outputFile, IProgress<ulong> progress,CancellationToken cancellationToken, string workingDirectrory = null)
        {
            System.ArgumentNullException.ThrowIfNull(devicePath);
            System.ArgumentNullException.ThrowIfNull(outputFile);

            return await Task.Run(() =>
            {    
                IntPtr pHandle = IntPtr.Zero;
            try
            {
                string currentDirectoryFullName = GetDirectoryName(workingDirectrory);
                string savedOutputFileFullPath = Path.Combine(currentDirectoryFullName, outputFile);
                Console.WriteLine($"Path : {currentDirectoryFullName}");

                    pHandle = KernelBase.CreateFile(devicePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
                    if (pHandle == IntPtr.Zero)
                    {
                        Console.WriteLine("Failed to get handle to {0}", devicePath);
                        return -1;
                    }
                    uint numBytesRead = 0U;
                    ulong bytesReadTotal = 0UL;
                    byte[] buf = new byte[BUFFER_SIZE];
                    using (FileStream fsOutputFile = new(savedOutputFileFullPath, FileMode.Create, FileAccess.Write))
                    {
                        while (KernelBase.ReadFile(pHandle, buf, (uint)buf.Length, out numBytesRead, IntPtr.Zero) || bytesReadTotal != 0UL || numBytesRead != 0U)
                        {
                            fsOutputFile.Write(buf, 0, (int)numBytesRead);
                            bytesReadTotal += (ulong)numBytesRead;
                            if (numBytesRead <= 0U)
                                goto END;
                            cancellationToken.ThrowIfCancellationRequested();
                            progress?.Report(bytesReadTotal);
                        }
                        Console.WriteLine("Failed to ReadFile {0}, error: 0x{1:X}", devicePath, KernelBase.GetLastError());
                        KernelBase.CloseHandle(pHandle);
                        return -1;
                    }
                END:
                    //Console.WriteLine("Read {0} byte(s)", bytesReadTotal);
                    Console.WriteLine($"\nFile saved : {savedOutputFileFullPath}");
                    KernelBase.CloseHandle(pHandle);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation canceled.");
                    return -1;
                }
                catch(Exception)
                {
                    return -1;
                }
                finally
                {
                    if (pHandle != IntPtr.Zero)
                        KernelBase.CloseHandle(pHandle);
                }
                return 0;
            }, cancellationToken);
        }

        private static string GetDirectoryName(string directoryName)
        {
            try {
                if (string.IsNullOrWhiteSpace(directoryName))
                    return string.Empty;
                string directory = IsSimpleName(directoryName) ? Path.Combine(AppContext.BaseDirectory, directoryName) : Path.GetDirectoryName(Path.GetFullPath(directoryName))!;
                return Directory.CreateDirectory(directory).FullName;
            }
            catch
            {
                return string.Empty;
            }
        }
        private static bool IsSimpleName(string s) => !string.IsNullOrWhiteSpace(s) 
                                             && s != "." && s != ".." 
                                            && s.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) < 0;
      
    }
}
