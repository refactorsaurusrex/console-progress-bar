using System;

namespace ConsoleProgressBar
{
    internal class Marquee
    {
        private readonly IMarqueeParameters _parameters;
        private int _start;
        private int _end;

        public Marquee(IMarqueeParameters parameters)
        {
            _parameters = parameters;
            
            _start = 1 - (int)Math.Round(parameters.NumberOfBlocks * 0.2);
            _end = 1;
        }

        public void Rotate()
        {
            _start++;
            _start %= _parameters.NumberOfBlocks;

            _end++;
            _end %= _parameters.NumberOfBlocks;
        }

        public string GetBlockCharacter(int index)
        {
            switch (index)
            {
                case 0:
                    return _parameters.StartBracket;
                case var i when i == _parameters.NumberOfBlocks + 1:
                    return _parameters.EndBracket;
                default:
                    return IsInCompletedWindow(index) ? _parameters.CompletedBlock : _parameters.IncompleteBlock;
            }
        }

        private bool IsInCompletedWindow(int location)
        {
            if (_end < _start)
                return location >= _start || location <= _end;

            return location >= _start && location <= _end;
        }
    }
}