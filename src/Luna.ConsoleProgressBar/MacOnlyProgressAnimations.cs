using JetBrains.Annotations;

#pragma warning disable 1591

namespace Luna.ConsoleProgressBar
{
    /// <summary>
    /// A collection of progress bar animations that only work correctly on Macs.
    /// </summary>
    [PublicAPI]
    public static class MacOnlyProgressAnimations
    {
        public const string RotatingDot = "\u25dc\u25dd\u25de\u25df";
        public const string GrowingBarVertical = "\u2581\u2582\u2583\u2584\u2585\u2586\u2587\u2588\u2587\u2586\u2585\u2584\u2583\u2581";
        public const string RotatingPipe = "\u2524\u2518\u2534\u2514\u251c\u250c\u252c\u2510";
        public const string RotatingCircle = "\u25d0\u25d3\u25d1\u25d2";
        public const string GrowingBarHorizontal = "\u2589\u258a\u258b\u258c\u258d\u258e\u258f\u258e\u258d\u258c\u258b\u258a\u2589";
    }
}