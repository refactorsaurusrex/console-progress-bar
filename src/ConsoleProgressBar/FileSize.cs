using System;
using System.IO;
using System.Linq;
using System.Security;

namespace ConsoleProgressBar
{
    internal class FileSize
    {
        private readonly long _fileSizeInBytes;

        public const double OneKB = 1024;
        public const double OneMB = 1024 * 1024;
        public const double OneGB = 1024 * 1024 * 1024;

        public FileSize(long fileSizeInBytes) => _fileSizeInBytes = fileSizeInBytes;

        public static implicit operator string(FileSize fs) => fs.ToString();

        public override string ToString()
        {
            if (_fileSizeInBytes > OneGB)
                return $"{_fileSizeInBytes / OneGB:F2} GB";

            if (_fileSizeInBytes > OneMB)
                return $"{_fileSizeInBytes / OneMB:F2} MB";

            return _fileSizeInBytes > OneKB
                ? $"{_fileSizeInBytes / OneKB:F2} KB"
                : $"{_fileSizeInBytes} bytes";
        }
    }
}
