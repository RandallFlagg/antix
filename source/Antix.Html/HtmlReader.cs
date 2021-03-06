﻿using System;

namespace Antix.Html
{
    public class HtmlReader : IHtmlReader
    {
        readonly string _html;
        int _index;

        public HtmlReader(
            string html)
        {
            _html = html;
            _index = 0;
        }

        public bool Any()
        {
            return _html.Length - _index > 0;
        }

        public char Peek()
        {
            return
                _index >= _html.Length
                    ? (char) 0
                    : _html[_index];
        }

        public string Peek(
            int count)
        {
            if (_index + count > _html.Length)
                throw new InvalidOperationException();

            return _html.Substring(_index, count);
        }

        public char Consume()
        {
            return _html[_index++];
        }

        public string Consume(int count)
        {
            var consumed = Peek(count);
            _index += count;

            return consumed;
        }

        public void Consume(
            Func<char, bool> match)
        {
            while (match(Peek()))
                _index++;
        }

        public bool TryConsume(
            string target, bool consumeTarget)
        {
            if (_index + target.Length > _html.Length) return false;

            var i = 0;
            while (i < target.Length
                   && _html[_index + i] == target[i])
                i++;

            if (i == target.Length)
            {
                if (consumeTarget) _index += target.Length;
                return true;
            }

            return false;
        }

        public bool TryConsume(
            string target, bool seek, bool consumeTarget)
        {
            if (_index + target.Length > _html.Length) return false;

            if (TryConsume(target, consumeTarget))
            {
                return true;
            }

            if (seek)
            {
                var targetIndex = _html
                    .IndexOf(target, _index + 1, StringComparison.OrdinalIgnoreCase);
                if (targetIndex > 0)
                {
                    _index = consumeTarget
                        ? targetIndex + target.Length
                        : targetIndex;

                    return true;
                }
            }

            return false;
        }

        public bool TryConsume(
            string target, bool seek, bool consumeTarget, out string consumed)
        {
            var startIndex = _index;
            if (TryConsume(target, seek, consumeTarget))
            {
                var endIndex = consumeTarget ? _index - target.Length : _index;
                consumed = _html.Substring(startIndex, endIndex - startIndex);
                return true;
            }

            consumed = null;
            return false;
        }
    }
}