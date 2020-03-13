using System;
using System.Linq;
using System.Threading;

namespace Luna.ConsoleProgressBar
{
    /// <summary>
    /// Represents a text-based progress bar for tracking file transfers in C# console applications. 
    /// </summary>
    public class FileTransferProgressBar : ConsoleProgressBar
    {
        private long _lastReportTicks;

        /// <summary>
        /// Creates a new instances of the FileTransferProgressBar type.
        /// </summary>
        /// <param name="fileSizeInBytes">The size of the file to transfer, in bytes.</param>
        /// <param name="timeout">The period of time which indicates the file transfer has stalled if no data is received.</param>
        public FileTransferProgressBar(long fileSizeInBytes, TimeSpan timeout)
        {
            _lastReportTicks = DateTime.Now.Ticks;

            FileSizeInBytes = fileSizeInBytes;
            BytesReceived = 0;
            TimeSpanFileStalled = timeout;

            Timer = new Timer(TimerHandler);
            RestartTimer();
        }

        /// <summary>
        /// Gets the size of the file being transferred, in bytes.
        /// </summary>
        public long FileSizeInBytes { get; }

        /// <summary>
        /// Gets or sets the number of bytes that have been transferred. 
        /// </summary>
        public long BytesReceived { get; set; }

        /// <summary>
        /// Gets the timeout period which determines whether a file transfer has stalled.
        /// </summary>
        public TimeSpan TimeSpanFileStalled { get; }

        /// <summary>
        /// True to display the number of bytes transferred. False to turn off. The default is true. 
        /// </summary>
        public bool DisplayBytes { get; set; } = true;

        /// <summary>
        /// When raised, indicates that the file transfer has stalled.
        /// </summary>
        public event EventHandler<ProgressEventArgs> FileTransferStalled;

        /// <inheritdoc />
        public override void Report(double value)
        {
            var ticks = DateTime.Now.Ticks;
            Interlocked.Exchange(ref _lastReportTicks, ticks);
            base.Report(value);
        }

        private void TimerHandler(object state)
        {
            lock (Timer)
            {
                if (IsDisposed) 
                    return;

                var elapsedTicks = DateTime.Now.Ticks - _lastReportTicks;
                var elapsed = TimeSpan.FromTicks(elapsedTicks);

                UpdateText(GetProgressBarText());
                RestartTimer();

                if (elapsed < TimeSpanFileStalled) 
                    return;

                FileTransferStalled?.Invoke(this, new ProgressEventArgs(new DateTime(_lastReportTicks), DateTime.Now));
            }
        }

        private string GetProgressBarText()
        {
            const string singleSpace = " ";

            var numBlocksCompleted = (int)(CurrentProgress * NumberOfBlocks);

            var completedBlocks = Enumerable.Range(0, numBlocksCompleted)
                .Aggregate(string.Empty, (current, _) => current + CompletedBlock);

            var incompleteBlocks = Enumerable.Range(0, NumberOfBlocks - numBlocksCompleted)
                .Aggregate(string.Empty, (current, _) => current + IncompleteBlock);

            var progressBar = $"{StartBracket}{completedBlocks}{incompleteBlocks}{EndBracket}";
            var percent = $"{CurrentProgress:P0}".PadLeft(4, '\u00a0');

            string fileSizeInBytes = new FileSize(FileSizeInBytes);
            var padLength = fileSizeInBytes.Length;
            var bytesReceived = new FileSize(BytesReceived).ToString().PadLeft(padLength, '\u00a0');
            var bytes = $"{bytesReceived} of {fileSizeInBytes}";

            var animationFrame = AnimationSequence[AnimationIndex++ % AnimationSequence.Length];
            var animation = $"{animationFrame}";
            progressBar = DisplayBars ? progressBar + singleSpace : string.Empty;
            percent = DisplayPercentComplete ? percent + singleSpace : string.Empty;
            bytes = DisplayBytes ? bytes + singleSpace : string.Empty;

            if (!DisplayAnimation || CurrentProgress is 1)
                animation = string.Empty;

            return progressBar + bytes + percent + animation;
        }
    }
}
