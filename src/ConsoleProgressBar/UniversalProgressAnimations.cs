using JetBrains.Annotations;
#pragma warning disable 1591

namespace ConsoleProgressBar
{
    /// <summary>
    /// A collection of progress bar animations that work correctly on both Mac and Windows machines.
    /// </summary>
    [PublicAPI]
    public static class UniversalProgressAnimations
    {
        public const string Default = @"|/-\-";
        public const string BouncingBall = ".oO\u00b0Oo.";
        public const string Explosion = ".oO@*";
        public const string RotatingTriangle = "\u25b2\u25ba\u25bc\u25c4";
        public const string RotatingArrow = "\u2190\u2191\u2192\u2193";
        public const string PulsingLine = "\u2212\u003d\u2261\u039e\u2261\u003d\u2212";
        public const string Circles = "\u25cb\u263c\u00a4\u2219";
    }
}
