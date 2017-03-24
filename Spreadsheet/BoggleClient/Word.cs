using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    /// <summary>
    /// Helper class for containing data for a Word
    /// </summary>
    public class Word
    {
        /// <summary>
        /// The word
        /// </summary>
        public string word { get; }

        /// <summary>
        /// the score of the word
        /// </summary>
        public int score { get; }

        /// <summary>
        /// Constructs a Word from a word and score
        /// </summary>
        /// <param name="word"></param>
        /// <param name="score"></param>
        public Word(string word, int score)
        {
            this.word = word;
            this.score = score;
        }

        /// <summary>
        /// Constructs a word from JSON
        /// </summary>
        /// <param name="data"></param>
        public Word(dynamic data)
        {
            word = data.Word;
            score = data.Score;
        }
    }
}
