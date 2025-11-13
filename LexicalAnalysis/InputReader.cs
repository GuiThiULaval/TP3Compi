using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class InputReader
    {
        public int BufferSize { get; init; }
        public char CurrentSymbol { get; private set; }

        private readonly FileStream _fileStream;
        private readonly StreamReader _streamReader;

        private readonly char[] _previousBuffer;
        private readonly char[] _currentBuffer;
        private int _beginPointer;
        private int _forwardPointer;

        public InputReader(int bufferSize, string filePath)
        {
            // Initialize streams.
            _fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            _streamReader = new(_fileStream, Encoding.UTF8);

            // Setup buffers and pointers.
            BufferSize = bufferSize;
            _previousBuffer = new char[BufferSize];
            _currentBuffer = new char[BufferSize];
            _beginPointer = BufferSize * 2;
            _forwardPointer = _beginPointer;

            // Start reading.
            LoadNextBuffer();
            UpdateCurrentSymbol();
        }

        public void CloseFile()
        {
            _streamReader.Close();
            _fileStream.Close();
        }

        public void AdvancePointer()
        {
            _forwardPointer++;

            if (_forwardPointer == BufferSize * 2)
            {
                LoadNextBuffer();
            }

            UpdateCurrentSymbol();
        }

        public void RetractPointer()
        {
            // Forward pointer should always be ahead or at same position of begin pointer.
            if (_forwardPointer == _beginPointer)
            {
                throw new Exception($"Preceding symbol has already been consumed.");
            }

            _forwardPointer--;
            UpdateCurrentSymbol();
        }

        public string ConsumeLexeme()
        {
            // Compute bounds before moving pointers.
            int headStart = Math.Min(_beginPointer, BufferSize);
            int headEnd = Math.Min(_forwardPointer + 1, BufferSize);
            int tailStart = Math.Max(0, _beginPointer - BufferSize);
            int tailEnd = Math.Max(0, _forwardPointer - BufferSize + 1);

            // Replace pointers.
            _beginPointer = _forwardPointer + 1;
            AdvancePointer();

            // Create lexeme.
            string head = new(_previousBuffer[headStart..headEnd]);
            string tail = new(_currentBuffer[tailStart..tailEnd]);
            return head + tail;
        }

        private void UpdateCurrentSymbol()
        {
            if (_forwardPointer < BufferSize)
            {
                CurrentSymbol = _previousBuffer[_forwardPointer];
            }
            else
            {
                CurrentSymbol = _currentBuffer[_forwardPointer - BufferSize];
            }
        }

        private void LoadNextBuffer()
        {
            // Loading next buffer will corrupt current lexeme.
            if (_beginPointer < BufferSize)
            {
                throw new Exception($"Current lexeme of length={_forwardPointer - _beginPointer} does not fit in buffer of size={BufferSize}.");
            }

            // Rotate buffers.
            _currentBuffer.CopyTo(_previousBuffer, 0);
            _beginPointer -= BufferSize;
            _forwardPointer -= BufferSize;

            // Load next buffer.
            Array.Clear(_currentBuffer);
            int charsRead = _streamReader.Read(_currentBuffer, 0, BufferSize);
            if (charsRead < BufferSize)
            {
                _currentBuffer[charsRead] = Alphabet.EndOfFile;
            }
        }
    }
}
