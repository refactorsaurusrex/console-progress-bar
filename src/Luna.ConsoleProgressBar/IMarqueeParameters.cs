namespace Luna.ConsoleProgressBar
{
    internal interface IMarqueeParameters
    {
        string IncompleteBlock { get; set; }

        string CompletedBlock { get; set; }

        int NumberOfBlocks { get; set; }

        string StartBracket { get; set; }

        string EndBracket { get; set; }
    }
}