using System;
using Syn.Bot.Oscova;
using Syn.Bot.Siml;
using System.IO;
using Syn.Bot.Siml.Interfaces;
using System.Xml.Linq;

namespace OscovaConsoleBot
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var chatbot = new SimlBot();
            chatbot.PackageManager.LoadFromString(File.ReadAllText("Knowledge.simlpk"));

            //chatbot.MainUser.ResponseReceived += (sender, eventArgs) =>
            //{
            //    Console.WriteLine(eventArgs.Result.LastResponse);
            //};

            var bot = new OscovaBot();
            bot.Dialogs.Add(new AppDialog());
            //bot.Language.LexicalDatabase = new LexicalDatabase();
            bot.Train();

            bot.MainUser.ResponseReceived += (sender, eventArgs) =>
            {
               Console.WriteLine(eventArgs.Response);
            };

            while (true)
            {
                var request = Console.ReadLine();
                var evaluationResult = chatbot.Chat(request);
                evaluationResult.BotMessage();
                var serialized = evaluationResult.Serialize();

                Console.WriteLine(serialized);
            }

            while (true)
            {
                var request = Console.ReadLine();
                var evaluationResult = bot.Evaluate(request);
                evaluationResult.Invoke();
                var serialized = evaluationResult.Serialize();

                Console.WriteLine(serialized);
            }
        }
    }


    class ExecuteApp : IAdapter
    {
        public bool IsRecursive
        {
            get
            {
                return true;
            }
        }

        public XName TagName
        {
            get
            {
                XNamespace ns = "http:\\contoso.com";
                return ns + "ExecuteApp";
            }
        }

        public string Evaluate(Syn.Bot.Siml.Context context)
        {
            throw new NotImplementedException();
        }
    }
}