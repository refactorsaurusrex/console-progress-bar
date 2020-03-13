using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Luna.ConsoleProgressBar
{
    /// <summary>
    /// Represents a text-based progress bar, for display in C# console applications. 
    /// </summary>
    public class ConsoleProgressBar : IDisposable, IProgress<double>
    {
        private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private string _currentText = string.Empty;

        /// <summary>
        /// Gets or sets the current percentage completed, expressed in decimal format. That is, values must be between 0 and 1.
        /// </summary>
        private double _currentProgress;

        /// <summary>
        /// 
        /// </summary>
        protected Timer Timer;

        /// <summary>
        /// Creates a new instance of the ConsoleProgressBar type.
        /// </summary>
        public ConsoleProgressBar()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Timer = new Timer(TimerHandler);
            RestartTimer();
        }

        /// <summary>
        /// Gets the current percent complete, expressed as a decimal between 0 and 1.
        /// </summary>
        protected double CurrentProgress => _currentProgress;

        /// <summary>
        /// Gets whether the current instances has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the number of progress bar blocks to display. The default is 10.
        /// </summary>
        public int NumberOfBlocks { get; set; } = 10;

        /// <summary>
        /// Gets or sets the opening bracket for the progress bar. The default is '['.
        /// </summary>
        public string StartBracket { get; set; } = "[";

        /// <summary>
        /// Gets or sets the closing bracket for the progress bar. The default is ']'.
        /// </summary>
        public string EndBracket { get; set; } = "]";

        /// <summary>
        /// Gets or sets the completed block indicator for the progress bar. The default is '#'.
        /// </summary>
        public string CompletedBlock { get; set; } = "#";

        /// <summary>
        /// Gets or sets the incomplete block indicator for the progress bar. The default is '-'.
        /// </summary>
        public string IncompleteBlock { get; set; } = "-";

        /// <summary>
        /// Gets or sets the animation sequence. The default is <c>UniversalProgressAnimations.Default</c>. See <see cref="UniversalProgressAnimations"/> and
        /// <see cref="MacOnlyProgressAnimations"/>.
        /// </summary>
        public string AnimationSequence { get; set; } = UniversalProgressAnimations.Default;

        /// <summary>
        /// True to display progress bar blocks. False to turn off. The default is true.
        /// </summary>
        public bool DisplayBars { get; set; } = true;

        /// <summary>
        /// True to display the percent complete. False to turn off.  The default is true.
        /// </summary>
        public bool DisplayPercentComplete { get; set; } = true;

        /// <summary>
        /// True to display progress animation. False to turn off.  The default is true.
        /// </summary>
        public bool DisplayAnimation { get; set; } = true;

        /// <summary>
        /// Gets or sets the index for the current frame of the animation sequence. 
        /// </summary>
        protected int AnimationIndex { get; set; }

        /// <summary>
        /// Gets or sets the progress bar's foreground color. The default is <c>Console.ForegroundColor</c>.
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;

        /// <summary>
        /// Gets or sets the progress bar's background color. The default is <c>Console.BackgroundColor</c>.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);
        }

        /// <summary>
        /// Updates the progress bar by writing the specified text to the console.
        /// </summary>
        /// <param name="text">The text rendering which constitutes the progress bar.</param>
        protected void UpdateText(string text)
        {
            // Get length of common portion
            var commonPrefixLength = 0;
            var commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
                commonPrefixLength++;

            // Backtrack to the first differing character
            var outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            var overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.Write(outputBuilder);
            Console.ResetColor();
            _currentText = text;
        }

        /// <summary>
        /// Starts or restarts the progress bar's timer. If the console's output is redirected to a file, this method is a no-op. 
        /// </summary>
        /// <remarks>
        /// A progress bar is only for temporary display in a console window.
        /// If the console output is redirected to a file, draw nothing.
        /// Otherwise, we'll end up with a lot of garbage in the target file.
        /// </remarks>
        protected void RestartTimer()
        {
            if (Console.IsOutputRedirected)
                return;

            lock (Timer)
            {
                Timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// Disposes resources consumed by the current instance. 
        /// </summary>
        /// <param name="isDisposing">Should always be true if called explicitly by user code.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            lock(Timer)
            {
                if (!isDisposing || IsDisposed)
                    return;

                Timer.Dispose();
                IsDisposed = true;
            }
        }

        private string GetProgressBarText(double currentProgress)
        {
            const string singleSpace = " ";

            var numBlocksCompleted = (int)(currentProgress * NumberOfBlocks);

            var completedBlocks = Enumerable.Range(0, numBlocksCompleted)
                .Aggregate(string.Empty, (current, _) => current + CompletedBlock);

            var incompleteBlocks = Enumerable.Range(0, NumberOfBlocks - numBlocksCompleted)
                .Aggregate(string.Empty, (current, _) => current + IncompleteBlock);

            var progressBar = $"{StartBracket}{completedBlocks}{incompleteBlocks}{EndBracket}";
            var percent = $"{currentProgress:P0}".PadLeft(4, '\u00a0');
            var animationFrame = AnimationSequence[AnimationIndex++ % AnimationSequence.Length];
            var animation = $"{animationFrame}";
            progressBar = DisplayBars ? progressBar + singleSpace : string.Empty;
            percent = DisplayPercentComplete ? percent + singleSpace : string.Empty;

            if (!DisplayAnimation || currentProgress is 1)
                animation = string.Empty;

            return (progressBar + percent + animation).TrimEnd();
        }

        private void TimerHandler(object state)
        {
            lock (Timer)
            {
                if (IsDisposed)
                    return;
                UpdateText(GetProgressBarText(CurrentProgress));
                RestartTimer();
            }
        }
    }
}