using System;
using System.Threading;

namespace QuantumTunnel
{
    internal class QuantumTunnelEx
    {
        const string DefaultXBFSFileName = "flash.bin";
        
        static readonly string ProgramName = nameof(QuantumTunnelEx);
        static readonly string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        private static int Main(string[] args)
        {
            bool rawDump = false;
            bool listFiles = false;
            bool target = false;
            bool isDisplayUsage = false;
            string output = null;
            string resource = null;
            string workignDirectory = null;
            string deviceFullPath = null;
            
            //debug
            //args = new string[] { "-r" };
            //args = new string[] { "-t", "certkeys.bin" };
            //args = new string[] { "-t", "certkeys.bin","-o","certkeys_dump.bin" };
            //args = new string[] { "-l" };
            Console.WriteLine("QuantumTunnel - C# FlashFS Reader and more");

            void ParseCommandLine(string[] args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-r":
                        case "--rawdump":
                            rawDump = true;
                            break;
                        case "-d":
                        case "--devicefullpath":
                            listFiles = true;
                            if (++i < args.Length && !string.IsNullOrWhiteSpace(args[i]))
                                deviceFullPath = args[i];
                            else
                            {
                                isDisplayUsage = true;
                                return;
                            }
                            break;
                        case "-t":
                        case "--target":
                            target = true;
                            if (++i < args.Length && !string.IsNullOrWhiteSpace(args[i]))
                                resource = args[i];
                            else
                            {
                                isDisplayUsage = true;
                                return;
                            }
                            break;
                        case "-o":
                        case "--output":
                            if (++i < args.Length && !string.IsNullOrWhiteSpace(args[i]))
                                output = args[i];
                            else
                            {
                                isDisplayUsage = true;
                                return;
                            }
                            break;
                        case "-wd":
                        case "--workingdirectory":
                            if (++i < args.Length && !string.IsNullOrWhiteSpace(args[i]))
                                workignDirectory = args[i];
                            else
                                workignDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            break;
                        default:
                            isDisplayUsage = true;
                            return;
                    }
                }
            }
           
            ParseCommandLine(args);
            if (args.Length == 0 || isDisplayUsage || (listFiles && string.IsNullOrWhiteSpace(resource)))
            {
                Usage();
                return 0;
            }

            if (rawDump || listFiles || target)
            {
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                    Console.WriteLine("\nCancellation......");
                };
                var progress = new Progress<ulong>(bytesRead =>
                {
                    Console.Write($"\rcurrent byte(s) read: {FormatSize(bytesRead, "[{0:0.##} {1} ({2})]")}   ");
                });

                if (rawDump)
                    return Flash.ReadFlashImageAsync(output ?? DefaultXBFSFileName, progress, cts.Token, workignDirectory).GetAwaiter().GetResult();

                if (listFiles)
                    return Flash.GetFromFullDevicePathAsync(deviceFullPath, resource, progress, cts.Token, workignDirectory).GetAwaiter().GetResult();

                if (target)
                    return Flash.ReadFlashFsFileAsync(resource, output ?? resource, progress, cts.Token, workignDirectory).GetAwaiter().GetResult();
            }
            return 0;
        }
        
        static readonly string strUsage =
                "Usage:\n\n" +
                $"\t{ProgramName} -r [-o|--output <custom name dump.bin> -wd|--workingdirectory <directory to save file>]\n" +
                $"\t{ProgramName} -r|-rawdump [-o|--output <output filename>]\n" +
                $"\t{ProgramName} -t|--target <resource name> [-o|--output <custom output filename>]\n\n" +
                $"\t{ProgramName} -d|--deviceFullPath <device path> -t|--target <resouce name> [-o|--output <custom output filename>]\n\n" +
                "Example:\n" +
                $@"\t{ProgramName} -d \\.\Xvux\FlashFs -t devkit.ini -wd DUMP_FLASH\n\n" +
                $@"\t{ProgramName} -d \\.\Xvux\FlashFs -t devkit.ini -o customdevkit.ini\n\n" +
                $"\t{ProgramName} -r (will output a filename flash.bin)\n" +
                $"\t{ProgramName} -r -o XBFS.bin\n" +
                $"\t{ProgramName} -t certkeys.bin\n" +
                $"\t{ProgramName} -t certkeys.bin -o certkeys_dump.bin\n"
            ;
        static void Usage() => Console.WriteLine(strUsage);

        static string FormatSize(ulong bytes, string format = "{0:0.##} {1}")
        {
            double value = bytes;
            int i = 0;
            while (value >= 1000 && i < units.Length - 1)
            {
                value /= 1000;
                i++;
            }
            return string.Format(format,value, units[i],bytes);
        }
    }

}
