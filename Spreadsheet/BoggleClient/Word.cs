using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    class Word
    {
        public string word { get; }

        public int score { get; }

        public Word(string word, string score)
        {
            this.word = word;
            this.score = int.Parse(score);
        }

        public Word(dynamic data)
        {
            word = data.Word;
            score = data.Score;
        }
    }
}
