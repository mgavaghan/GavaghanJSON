using System;

namespace Gavaghan.JSON
{
    /// <summary>
    /// Extends the standards-based <code>JSONValueFactory</code> to recognized Java
    /// style line and block comments.  Comments are treated as whitespace and not
    /// stored in the model.
    /// </summary>
    public class CommentedJSONValueFactory : TypedJSONValueFactory
    {
        /// <summary>
        /// The default implementation
        /// </summary>
        static public readonly CommentedJSONValueFactory COMMENTED_DEFAULT = new CommentedJSONValueFactory();

        /// <summary>
        /// Throw away characters until the line comment is completely read.
        /// </summary>
        /// <param name="pbr">the PushbackReader to use</param>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Gavaghan.JSON.JSONException"/>
        private void SkipLineComment(PushbackReader pbr)
        {
            for (; ; )
            {
                int c = pbr.Read();

                // out of data? quit
                if (c < 0) break;

                // end of line? quit
                if ((c == '\n') || (c == '\r')) break;
            }
        }

        /// <summary>
        /// Throw away characters until the block comment is completely read.
        /// </summary>
        /// <param name="pbr">the PushbackReader to use</param>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Gavaghan.JSON.JSONException"/>
        private void SkipBlockComment(PushbackReader pbr)
        {
            bool star = false;

            for (; ; )
            {
                int c = pbr.Read();

                // out of data? throw exception
                if (c < 0) throw new JSONException("$", "Unterminated block comment at end of file");

                if (star && (c == '/')) break;

                star = (c == '*');
            }
        }

        /// <summary>
        /// Get the minimum size of the pushback buffer.
        /// </summary>
        public override int PushbackBufferSize
        {
            get
            {
                // We need an extra pushback to identify a comment.
                return Math.Max(base.PushbackBufferSize, 2);
            }
        }

        /// <summary>
        /// Skip until the first non-whitespace character where comments are
        /// also treated as whitespace.
        /// </summary>
        /// <param name="pbr"></param>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Gavaghan.JSON.JSONException"/>
        public override void SkipWhitespace(PushbackReader pbr)
        {
            for (; ; )
            {
                int c = pbr.Read();

                if (c < 0) break; // bail on EOF

                if (!IsWhitespace(c))
                {
                    // it's not whitespace, so see if it's the start of a comment
                    if (c == '/')
                    {
                        int next = pbr.Read();

                        // is it a line comment?
                        if (next == '/')
                        {
                            SkipLineComment(pbr);
                            continue;
                        }
                        // is it a block comment?
                        else if (next == '*')
                        {
                            SkipBlockComment(pbr);
                            continue;
                        }

                        // else, unread - it's the end of the whitespace
                        pbr.Unread(c);
                    }

                    pbr.Unread(c);
                    break;
                }
            }
        }
    }
}
