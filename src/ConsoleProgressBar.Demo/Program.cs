using System;
using System.Threading.Tasks;

namespace ConsoleProgressBar.Demo
{
    internal static class Program
    {
        // async Main is a C# 7.1 feature, change your project settings to the 
        // new version if this is flagged as an error
        private static async Task Main()
        {
            await ConsoleProgressBars();
            Console.WriteLine();

            await FileTransferProgressBars();
            Console.WriteLine();
        }

        private static async Task ConsoleProgressBars()
        {
            var pb1 = new ConsoleProgressBar { ForegroundColor = ConsoleColor.Cyan };
            await TestProgressBar(pb1, 1);

            var pb2 = new ConsoleProgressBar
            {
                NumberOfBlocks = 30,
                ForegroundColor = ConsoleColor.Cyan,
                StartBracket = string.Empty,
                EndBracket = string.Empty,
                CompletedBlock = "\u2022",
                IncompleteBlock = "·",
                AnimationSequence = UniversalProgressAnimations.Default
            };
            await TestProgressBar(pb2, 2);

            var pb3 = new ConsoleProgressBar
            {
                DisplayBars = false,
                AnimationSequence = UniversalProgressAnimations.RotatingTriangle,
                ForegroundColor = ConsoleColor.Cyan
            };
            await TestProgressBar(pb3, 3);
        }

        private static async Task TestProgressBar(ConsoleProgressBar progress, int num)
        {
            Console.Write($"{num}. Performing some task... ");
            using (progress)
            {
                for (var i = 0; i <= 150; i++)
                {
                    progress.Report((double)i / 150);
                    await Task.Delay(20);
                }

                progress.Report(1);
                await Task.Delay(200);
            }

            Console.WriteLine();
        }

        private static async Task FileTransferProgressBars()
        {
            const long fileSize = (long)(8 * FileSize.OneKB);
            var pb4 = new FileTransferProgressBar(fileSize, TimeSpan.FromSeconds(5))
            {
                ForegroundColor = ConsoleColor.Green,
                NumberOfBlocks = 15,
                StartBracket = "|",
                EndBracket = "|",
                CompletedBlock = "|",
                IncompleteBlock = "\u00a0",
                AnimationSequence = UniversalProgressAnimations.PulsingLine
            };
            await TestFileTransferProgressBar(pb4, fileSize, 4);

            const long fileSize2 = (long)(100 * 36 * FileSize.OneMB);
            var pb5 = new FileTransferProgressBar(fileSize2, TimeSpan.FromSeconds(5))
            {
                ForegroundColor = ConsoleColor.Green,
                DisplayBars = false,
                DisplayAnimation = false
            };
            pb5.FileTransferStalled += HandleFileTransferStalled;
            await TestFileTransferStalled(pb5, fileSize2, 5);
        }

        private static async Task TestFileTransferProgressBar(FileTransferProgressBar progress, long fileSize, int num)
        {
            Console.Write($"{num}. File transfer in progress... ");
            using (progress)
            {
                for (var i = 0; i <= 150; i++)
                {
                    progress.BytesReceived = i * (fileSize / 150);
                    progress.Report((double)i / 150);
                    await Task.Delay(20);
                }

                progress.BytesReceived = fileSize;
                progress.Report(1);
                await Task.Delay(200);
            }

            Console.WriteLine();
        }

        private static async Task TestFileTransferStalled(FileTransferProgressBar progress, long fileSize, int num)
        {
            Console.Write($"{num}. File transfer in progress... ");
            using (progress)
            {
                for (var i = 0; i <= 110; i++)
                {
                    progress.BytesReceived = i * (fileSize / 1000);
                    progress.Report((double)i / 1000);
                    await Task.Delay(2);
                }

                await Task.Delay(6000);
            }
        }

        private static void HandleFileTransferStalled(object sender, ProgressEventArgs eventArgs)
        {
            var pb = (FileTransferProgressBar)sender;
            pb.Dispose();

            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}File transfer stalled!");
            Console.WriteLine($"{pb.TimeSpanFileStalled.Seconds} seconds elapsed since last data received");
        }
    }
}