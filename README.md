# Progress Bars for .NET Console Applications

A simple way to represent the progress of long-running tasks in a C# console apps. Targets .NET Standard 2.0.

## Installation

Install from [NuGet](https://www.nuget.org/packages/Luna.ConsoleProgressBar/):

```powershell
Install-Package Luna.ConsoleProgressBar
- OR - 
dotnet add package Luna.ConsoleProgressBar
```

## Features
* **ConsoleProgressBar**
  * **Implements `IProgress<double>`** The `IProgress` interface greatly simplifies the effort to report progress from an `async` method to the UI, clunky boilerplate code to ensure thread-safety is not required since the `SynchronizationContext` is captured when the progress bar is instantiated.
  * **Efficient and light-weight** `Console` can become sluggish and unresponsive when called frequently, this progress bar only performs 8 calls/second regardless of how often progress is reported.
  * **Customizable** Each component of the progress bar (start/end brackets, completed/incomplete blocks, progress animation) can be set to any string value through public properties and each item displayed (the progress bar itself, percentage complete, animation) can be shown or hidden.
* **FileTransferProgressBar**
  * **Extends** ConsoleProgressBar and adds the ability to detect when a file transfer has stalled.
  * If the time since last progress reported exceeds the `TimeSpanFileStalled` value, the `FileTransferStalled` event fires.
  * **Provides further customization** of the display with the ability to show/hide the bytes received and file size in bytes
* **MarqueeProgressBar**
  * Used for displaying progress of long running tasks of indeterminate duration. 
  * Offers the same customizations as ConsoleProgressBar and FileTransferProgressBar.
## Examples

> The marquee progress bars are only shown in the Windows demo.

### Windows

![Progress Bar_Win](https://raw.githubusercontent.com/refactorsaurusrex/console-progress-bar/master/images/demo.gif)


### Mac (VS Code)

![Progress Bar_Mac](https://s3-us-west-1.amazonaws.com/alunapublic/console_progress_bar/ProgressBar_Mac.gif)
### Ubuntu

![Progress Bar_Ubuntu](https://s3-us-west-1.amazonaws.com/alunapublic/console_progress_bar/ProgressBar_Ubuntu.gif)

## Usage
Numbers correspond to the examples shown above, full source code for examples can be found in [the demo project](https://github.com/refactorsaurusrex/console-progress-bar/blob/master/src/ConsoleProgressBar.Demo/Program.cs).
```csharp
// 1. Default behavior
var pb1 = new ConsoleProgressBar();
await TestProgressBar(pb1, 1);

// 2. Customized all progress bar components
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

// 3. Hide progress bar
var pb3 = new ConsoleProgressBar
{
    DisplayBars = false,
    AnimationSequence = UniversalProgressAnimations.RotatingTriangle,
    ForegroundColor = ConsoleColor.Cyan
};
await TestProgressBar(pb3, 3);

// 4. Customized progress bar, successful file transfer
const long fileSize = (long)(8 * FileHelper.OneKB);
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

// 5. Hide progress bar and animation, unsuccessful file transfer
const long fileSize2 = (long)(100 * 36 * FileHelper.OneMB);
var pb5 = new FileTransferProgressBar(fileSize2, TimeSpan.FromSeconds(5))
{
    ForegroundColor = ConsoleColor.Green,
    DisplayBars = false,
    DisplayAnimation = false
};
pb5.FileTransferStalled += HandleFileTransferStalled;
await TestFileTransferStalled(pb5, fileSize2, 5);

// 6. Default marquee progress bar with a yellow foreground
var bar1 = new MarqueeProgressBar
{
    BarForegroundColor = ConsoleColor.Yellow
};
var ct1 = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await bar1.Start("6. Performing some task...", ct1.Token);

// 7. Marquee progress bar with custom blocks
var bar2 = new MarqueeProgressBar
{
    BarForegroundColor = ConsoleColor.Yellow,
    IncompleteBlock = "·",
    CompletedBlock = "\u2588"
};
var ct2 = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await bar2.Start("7. Performing some task...", ct2.Token);
```

## Versioning

This project follows a slightly modified semantic versioning pattern: `major.minor.build`. Although I'd prefer sticking to the normal `major.minor.patch` pattern, nuget makes it comically difficult to retrieve the last published version number of a particular package. That, in turn, makes it challenging to create a build process that only succeeds if the version number has been properly incremented. The scope of this project is sufficiently narrow that I opted for simplicity over semantic purity. 

## Contributions, Bug Reports, and Suggestions

If you find a bug or have a suggestion for improvement, please [open an issue](https://github.com/refactorsaurusrex/console-progress-bar/issues). Code contributions are absolutely welcome, but please be sure to review [these guidelines](https://github.com/refactorsaurusrex/console-progress-bar/blob/master/CONTRIBUTING.MD) first. Thanks!

## Credits

- Progress bar icon by [freepik](https://www.flaticon.com/authors/freepik).