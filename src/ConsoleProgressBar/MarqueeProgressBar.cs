using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    /// <summary>
    /// Represents a text-based marquee-style progress bar, for display in C# console applications. 
    /// </summary>
    public sealed class MarqueeProgressBar : IMarqueeParameters
    {
        private int _numberOfBlocks = Math.Max((int)(Console.BufferWidth * 0.35), 25);

        /// <summary>
        /// Gets or sets the foreground color for the progress bar. The default is <c>Console.ForegroundColor</c>.
        /// </summary>
        public ConsoleColor BarForegroundColor { get; set; } = Console.ForegroundColor;

        /// <summary>
        /// Gets or sets the background color for the progress bar. The default is <c>Console.BackgroundColor</c>.
        /// </summary>
        public ConsoleColor BarBackgroundColor { get; set; } = Console.BackgroundColor;

        /// <summary>
        /// Gets or sets the foreground color for the header text. The default is <c>Console.ForegroundColor</c>.
        /// </summary>
        public ConsoleColor HeaderForegroundColor { get; set; } = Console.ForegroundColor;

        /// <summary>
        /// Gets or sets the background color for the header text. The default is <c>Console.BackgroundColor</c>.
        /// </summary>
        public ConsoleColor HeaderBackgroundColor { get; set; } = Console.BackgroundColor;

        /// <summary>
        /// The delay in milliseconds between redrawing the progress bar. The default is 50. Set to a higher number for a slower
        /// loop speed, or a lower number for a higher loop speed.
        /// </summary>
        public int Delay { get; set; } = 50;

        /// <summary>
        /// Gets or sets the incomplete block indicator for the progress bar. The default is '-'.
        /// </summary>
        public string IncompleteBlock { get; set; } = "-";

        /// <summary>
        /// Gets or sets the completed block indicator for the progress bar. The default is '#'.
        /// </summary>
        public string CompletedBlock { get; set; } = "#";

        /// <summary>
        /// Gets or sets the opening bracket for the progress bar. The default is '['.
        /// </summary>
        public string StartBracket { get; set; } = "[";

        /// <summary>
        /// Gets or sets the closing bracket for the progress bar. The default is ']'.
        /// </summary>
        public string EndBracket { get; set; } = "]";

        /// <summary>
        /// Gets or sets the number of progress bar blocks to display. The default is 35% of the current console buffer width,
        /// with a minimum of 25. 
        /// </summary>
        public int NumberOfBlocks
        {
            get => _numberOfBlocks;
            set => _numberOfBlocks = Math.Max(25, Math.Min(Console.BufferWidth, value));
        }

        /// <summary>
        /// Displays the specified header text and runs the marquee in an indefinite loop until cancelled.
        /// </summary>
        /// <param name="header">The header text to display next to the progress bar.</param>
        /// <param name="token">A cancellation token, used to end the progress bar loop.</param>
        /// <returns></returns>
        public async Task Start(string header, CancellationToken token)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.BackgroundColor = HeaderBackgroundColor;
            Console.ForegroundColor = HeaderForegroundColor;
            Console.Write($"{header}  ");
            Console.BackgroundColor = BarBackgroundColor;
            Console.ForegroundColor = BarForegroundColor;
            var startPosition = header.Length + 2; // 2 spaces
            var progressBarText = $"{StartBracket}{IncompleteBlock.Repeat(NumberOfBlocks)}{EndBracket}";
            Console.Write(progressBarText);

            try
            {
                var marquee = new Marquee(this);
                var builder = new StringBuilder();

                while (true)
                {
                    builder.Clear();
                    for (var i = 0; i < progressBarText.Length; i++)
                    {
                        var c = marquee.GetBlockCharacter(i);
                        builder.Append(c);
                    }

                    Console.CursorLeft = startPosition;
                    progressBarText = builder.ToString();
                    Console.Write(progressBarText);
                    marquee.Rotate();
                    await Task.Delay(Delay, token);
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                Console.CursorTop++;
                Console.CursorLeft = 0;
                Console.CursorVisible = true;
                Console.ResetColor();
            }
        }
    }
}
