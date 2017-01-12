using System.Collections.Generic;
using System.IO;
using Syn.Bot.Oscova.Interfaces;
using Syn.WordNet;

namespace OscovaConsoleBot
{
    class LexicalDatabase : ILexicalDatabase
    {
        public LexicalDatabase()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "WordNet");

            var dataAdj = new ReaderInfo(new StreamReader(Path.Combine(directory, "data.adj")), PartOfSpeech.Adjective);
            var dataAdv = new ReaderInfo(new StreamReader(Path.Combine(directory, "data.adv")), PartOfSpeech.Adverb);
            var dataNoun = new ReaderInfo(new StreamReader(Path.Combine(directory, "data.noun")), PartOfSpeech.Noun);
            var dataVerb = new ReaderInfo(new StreamReader(Path.Combine(directory, "data.verb")), PartOfSpeech.Verb);

            var indexAdj = new ReaderInfo(new StreamReader(Path.Combine(directory, "index.adj")), PartOfSpeech.Adjective);
            var indexAdv = new ReaderInfo(new StreamReader(Path.Combine(directory, "index.adv")), PartOfSpeech.Adverb);
            var indexNoun = new ReaderInfo(new StreamReader(Path.Combine(directory, "index.noun")), PartOfSpeech.Noun);
            var indexVerb = new ReaderInfo(new StreamReader(Path.Combine(directory, "index.verb")), PartOfSpeech.Verb);

            var dataList = new List<ReaderInfo> { dataAdj, dataAdv, dataNoun, dataVerb };
            var indexList = new List<ReaderInfo> { indexAdj, indexAdv, indexNoun, indexVerb };

            WordNetEngine = new WordNetEngine();
            WordNetEngine.Load(dataList,indexList);
        }

        WordNetEngine WordNetEngine { get; }

        public double GetWordSimilarity(string sourceWord, string targetWord)
        {
            var semanticSimilarity = new SemanticSimilarity(WordNetEngine);
            return semanticSimilarity.GetWordSimilarity(sourceWord, targetWord);
        }
    }
}