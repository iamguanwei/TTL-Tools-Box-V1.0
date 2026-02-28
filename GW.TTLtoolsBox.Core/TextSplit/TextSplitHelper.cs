using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.TextSplit
{
    /// <summary>
    /// 为文本拆分提供服务的静态工具类。
    /// </summary>
    public static class TextSplitHelper
    {
        #region public

        /// <summary>
        /// 按句子拆分文本
        /// </summary>
        /// <param name="originalText">原始文本</param>
        /// <param name="minLength">拆分后段落最短长度（含分隔符）</param>
        /// <param name="maxLength">拆分后段落最长长度（含分隔符）</param>
        /// <param name="mainSeparators">主拆分分隔符数组（优先级高）</param>
        /// <param name="subSeparators">次拆分分隔符数组（兜底）</param>
        /// <param name="trimSegments">是否移除拆分后段落的首尾空白符（默认false）</param>
        /// <returns>拆分后的文本（段落间用\r\n分隔）</returns>
        /// <exception cref="ArgumentException">参数不合法时抛出</exception>
        public static string SplitText(
            string originalText,
            int minLength,
            int maxLength,
            char[] mainSeparators,
            char[] subSeparators,
            bool trimSegments = false)
        {
            // 1. 参数校验（避免非法参数导致异常）
            if (string.IsNullOrEmpty(originalText)) return string.Empty;
            if (minLength <= 0) throw new ArgumentException("最短长度必须大于0", nameof(minLength));
            if (maxLength < minLength) throw new ArgumentException("最长长度不能小于最短长度", nameof(maxLength));
            mainSeparators = mainSeparators ?? Array.Empty<char>(); // 空值兜底
            subSeparators = subSeparators ?? Array.Empty<char>(); // 空值兜底

            // 2. 兼容多换行符格式（\r\n、\n、\r），统一拆分为文本块
            string[] _textBlocks = originalText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            List<string> _resultBlocks = new List<string>();

            // 3. 逐个处理每个文本块
            foreach (string block in _textBlocks)
            {
                if (string.IsNullOrEmpty(block))
                {
                    _resultBlocks.Add(block); // 保留空行
                    continue;
                }

                List<string> splitBlockResult = processSingleBlock(block, minLength, maxLength, mainSeparators, subSeparators, trimSegments);
                _resultBlocks.AddRange(splitBlockResult);
            }

            // 4. 拼接所有结果（恢复换行）
            return string.Join("\r\n", _resultBlocks);
        }

        /// <summary>
        /// 按对话拆分文本
        /// </summary>
        /// <param name="originalText">原始文本</param>
        /// <param name="mainSeparators">对话符号拆分分隔符数组（必须成对出现）</param>
        /// <param name="trimSegments">是否移除拆分后段落的首尾空白符（默认false）</param>
        /// <returns>拆分后的文本（段落间用\r\n分隔）</returns>
        /// <exception cref="ArgumentException">参数不合法时抛出</exception>
        public static string SplitText(string originalText, char[] mainSeparators, bool trimSegments = false)
        {
            // 1. 参数校验（避免非法参数导致异常）
            if (string.IsNullOrEmpty(originalText)) return string.Empty;
            mainSeparators = mainSeparators ?? Array.Empty<char>(); // 空值兜底

            // 2. 兼容多换行符格式（\r\n、\n、\r），统一拆分为文本块
            string[] _textBlocks = originalText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            List<string> _resultBlocks = new List<string>();

            // 3. 构建分隔符映射（开始分隔符到结束分隔符的映射）
            Dictionary<char, char> _separatorMap = new Dictionary<char, char>();
            for (int i = 0; i < mainSeparators.Length; i += 2)
            {
                if (i + 1 < mainSeparators.Length)
                {
                    _separatorMap[mainSeparators[i]] = mainSeparators[i + 1];
                }
            }

            // 4. 逐个处理每个文本块
            foreach (string block in _textBlocks)
            {
                if (string.IsNullOrEmpty(block))
                {
                    _resultBlocks.Add(block); // 保留空行
                    continue;
                }

                List<string> splitBlockResult = processDialogBlock(block, _separatorMap, trimSegments);
                _resultBlocks.AddRange(splitBlockResult);
            }

            // 5. 拼接所有结果（恢复换行）
            return string.Join("\r\n", _resultBlocks);
        }

        #endregion

        #region private

        /// <summary>
        /// 处理单个对话文本块（无换行的纯文本）
        /// </summary>
        private static List<string> processDialogBlock(
            string block,
            Dictionary<char, char> separatorMap,
            bool trimSegments)
        {
            List<string> _splitResult = new List<string>();
            int _currentPos = 0;
            int _totalLength = block.Length;

            // 构建开始分隔符集合（提升查找效率）
            HashSet<char> _startSepSet = new HashSet<char>(separatorMap.Keys);

            // 核心规则1：无任何分隔符 → 原样返回，不拆分
            if (!block.Any(c => _startSepSet.Contains(c)))
            {
                string segment = trimSegments ? block.Trim() : block;
                _splitResult.Add(segment);
                return _splitResult;
            }

            // 有分隔符时，使用状态机处理
            while (_currentPos < _totalLength)
            {
                // 查找当前位置后的第一个开始分隔符
                int _startSepPos = -1;
                for (int i = _currentPos; i < _totalLength; i++)
                {
                    if (_startSepSet.Contains(block[i]))
                    {
                        _startSepPos = i;
                        break;
                    }
                }

                // 没找到开始分隔符 → 剩余文本作为一段
                if (_startSepPos == -1)
                {
                    string _segment = block.Substring(_currentPos);
                    _splitResult.Add(trimSegments ? _segment.Trim() : _segment);
                    break;
                }

                // 找到开始分隔符，提取开始分隔符前的文本（如果有）
                if (_startSepPos > _currentPos)
                {
                    string _prefixSegment = block.Substring(_currentPos, _startSepPos - _currentPos);
                    _splitResult.Add(trimSegments ? _prefixSegment.Trim() : _prefixSegment);
                }

                // 记录开始分隔符和对应的结束分隔符
                char _startSep = block[_startSepPos];
                char _endSep = separatorMap[_startSep];

                // 从开始分隔符后开始寻找结束分隔符
                int _endSepPos = -1;
                for (int i = _startSepPos + 1; i < _totalLength; i++)
                {
                    if (block[i] == _endSep)
                    {
                        _endSepPos = i;
                        break;
                    }
                }

                // 找到结束分隔符 → 将整个对话作为一段
                if (_endSepPos != -1)
                {
                    string _dialogSegment = block.Substring(_startSepPos, _endSepPos - _startSepPos + 1);
                    _splitResult.Add(trimSegments ? _dialogSegment.Trim() : _dialogSegment);
                    _currentPos = _endSepPos + 1;
                }
                else
                {
                    // 没找到结束分隔符 → 剩余文本作为一段
                    string _segment = block.Substring(_startSepPos);
                    _splitResult.Add(trimSegments ? _segment.Trim() : _segment);
                    break;
                }
            }

            return _splitResult;
        }

        /// <summary>
        /// 处理单个文本块（无换行的纯文本）
        /// </summary>
        private static List<string> processSingleBlock(
            string block,
            int minLength,
            int maxLength,
            char[] mainSeparators,
            char[] subSeparators,
            bool trimSegments)
        {
            List<string> _splitResult = new List<string>();
            int _currentPos = 0;
            int _totalLength = block.Length;

            // 构建分隔符集合（提升查找效率）
            HashSet<char> _mainSepSet = new HashSet<char>(mainSeparators);
            HashSet<char> _subSepSet = new HashSet<char>(subSeparators);

            // 核心规则1：无任何分隔符 → 原样返回，不拆分、不截断
            if (!block.Any(c => _mainSepSet.Contains(c) || _subSepSet.Contains(c)))
            {
                string segment = trimSegments ? block.Trim() : block;
                _splitResult.Add(segment);
                return _splitResult;
            }

            // 有分隔符时，循环拆分（确保每段≤maxLength，且只按分隔符拆分）
            while (_currentPos < _totalLength)
            {
                // 计算当前可拆分的最大结束位置（确保段落总长度≤maxLength）
                int _maxEndPos = _currentPos + maxLength - 1;
                _maxEndPos = Math.Min(_maxEndPos, _totalLength - 1);

                // 步骤1：在[currentPos, maxEndPos]范围内找最后一个主分隔符
                int _splitPos = findLastSeparatorInRange(block, _currentPos, _maxEndPos, _mainSepSet);

                // 步骤2：没找到主分隔符 → 找最后一个次分隔符
                if (_splitPos == -1)
                {
                    _splitPos = findLastSeparatorInRange(block, _currentPos, _maxEndPos, _subSepSet);
                }

                // 步骤3：找到分隔符 → 拆分
                if (_splitPos != -1)
                {
                    string _segment = block.Substring(_currentPos, _splitPos - _currentPos + 1);
                    _splitResult.Add(trimSegments ? _segment.Trim() : _segment);
                    _currentPos = _splitPos + 1;
                }
                else
                {
                    // 步骤4：当前范围无分隔符 → 向后找第一个分隔符（确保不截断文本）
                    int _nextSepPos = findFirstSeparatorAfterPos(block, _maxEndPos + 1, _mainSepSet, _subSepSet);

                    if (_nextSepPos != -1)
                    {
                        // 按找到的分隔符拆分（即使这段略超maxLength，也不截断）
                        string _segment = block.Substring(_currentPos, _nextSepPos - _currentPos + 1);
                        _splitResult.Add(trimSegments ? _segment.Trim() : _segment);
                        _currentPos = _nextSepPos + 1;
                    }
                    else
                    {
                        // 兜底：理论上不会走到这里（已校验有分隔符）
                        string _segment = block.Substring(_currentPos);
                        _splitResult.Add(trimSegments ? _segment.Trim() : _segment);
                        _currentPos = _totalLength;
                    }
                }
            }

            return _splitResult;
        }

        /// <summary>
        /// 在指定区间内从后往前找最后一个分隔符的位置
        /// </summary>
        private static int findLastSeparatorInRange(string text, int startPos, int endPos, HashSet<char> separators)
        {
            for (int i = endPos; i >= startPos; i--)
            {
                if (separators.Contains(text[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从指定位置往后找第一个分隔符（先主后次）
        /// </summary>
        private static int findFirstSeparatorAfterPos(string text, int startPos, HashSet<char> mainSep, HashSet<char> subSep)
        {
            // 先找主分隔符
            for (int i = startPos; i < text.Length; i++)
            {
                if (mainSep.Contains(text[i])) return i;
            }
            // 再找次分隔符
            for (int i = startPos; i < text.Length; i++)
            {
                if (subSep.Contains(text[i])) return i;
            }
            return -1;
        }

        #endregion
    }
}