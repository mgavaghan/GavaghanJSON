using System;
using System.IO;

namespace Gavaghan.JSON
{
    /// <summary>
    /// Give a TextReader the ability to unread characters.
    /// FIXME needs test cases
    /// </summary>
    public class PushbackReader : TextReader
    {
        /// <summary>
        /// The underlying reader.
        /// </summary>
        private readonly TextReader mReader;

        /// <summary>
        /// The pushback buffer.
        /// </summary>
        private readonly int[] mBuffer;

        /// <summary>
        /// The index into the pushback buffer;
        /// </summary>
        private int mIndex;

        /// <summary>
        /// Create a new PushbackReader.
        /// </summary>
        /// <param name="reader">the underlying reader</param>
        /// <param name="size">the pushback buffer size</param>
        public PushbackReader(TextReader reader, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", "size may not be negative");

            mReader = reader;
            mBuffer = new int[size];
            mIndex = size;
        }

        /// <summary>
        /// Unread a character.
        /// </summary>
        /// <param name="c">character to unread</param>
        /// <exception cref="System.IO.IOException">if pushback buffer is full</exception>
        public void Unread(int c)
        {
            if (mIndex == 0) throw new IOException("Pushback buffer overflow");

            mIndex--;

            mBuffer[mIndex] = c;
        }

        /// <summary>
        /// Reads the next character from the input stream and advances the character
        /// position by one character.
        /// </summary>
        /// <returns>The next character from the input stream, or -1 if no more characters are available.</returns>
        /// <exception cref="System.IO.Exception"/>
        public override int Read()
        {
            int c;

            if (mIndex < mBuffer.Length)
            {
                c = mBuffer[mIndex];
                if (c >= 0) mIndex++;
                else c = -1;
            }
            else
            {
                c = mReader.Read();
            }

            return c;
        }

        /// <summary>
        /// Reads a maximum of count characters from the current stream and writes the
        /// data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the
        ///     values between index and (index + count - 1) replaced by the characters read
        ///     from the current source.</param>
        /// <param name="index">The place in buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>The number of characters that have been read.</returns>
        public override int Read(char[] buffer, int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index", "index may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count may not be negative");

            int read;

            // if no buffered data
            if (mIndex == mBuffer.Length)
            {
                read = mReader.Read(buffer, index, count);
            }
            else
            {
                int newIndex = index;
                int newCount = count;
                int c = 0;

                read = 0;

                while ((mIndex < mBuffer.Length) && (newCount > 0))
                {
                    c = mBuffer[mIndex];

                    if (c < 0) break;

                    buffer[newIndex] = (char)mBuffer[mIndex];

                    mIndex++;

                    newIndex++;
                    newCount--;

                    read++;
                }

                if (c >= 0) read += mReader.Read(buffer, newIndex, newCount);
            }

            return read;
        }
    }
}
