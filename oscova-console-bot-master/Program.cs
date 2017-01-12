using System;
using Syn.Bot.Oscova;
using Syn.Bot.Siml;
using System.IO;
using Syn.Bot.Siml.Interfaces;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace OscovaConsoleBot
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "WordNet");

            var chatbot = new SimlBot();
            chatbot.PackageManager.LoadFromString(File.ReadAllText(Path.Combine(directory, "jarvis.simlpk")));
            chatbot.Adapters.Add(new ExecuteApp());

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
                Console.WriteLine(evaluationResult.BotMessage);
            }

            //while (true)
            //{
            //    var request = Console.ReadLine();
            //    var evaluationResult = bot.Evaluate(request);
            //    evaluationResult.Invoke();
            //    var serialized = evaluationResult.Serialize();

            //    Console.WriteLine(serialized);
            //}
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
                XNamespace ns = "http://example.com/class#adapter";
                return ns + "ExecuteApp";
            }
        }

        public string Evaluate(Syn.Bot.Siml.Context context)
        {
            Task.Factory.StartNew(() => 
            {
                var peticion = RecuperarParametros(context.Element.Value).SingleOrDefault(t => t.Item1 == "-url").Item2.Replace(" ", "+");
                var chrome = new ProcessStartInfo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                chrome.Arguments = string.Format("https://www.google.es/#q={0}", peticion);
                chrome.RedirectStandardOutput = true;
                chrome.UseShellExecute = false;

                var proc = Process.Start(chrome);
                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                proc.WaitForExit();
            });

            return string.Empty;
        }

        public List<Tuple<string, string>> RecuperarParametros(string valor)
        {
            var result = new List<Tuple<string, string>>();
            string key = string.Empty;
            string value = string.Empty;
            var dividirXespacios = valor.Split(' ').ToList();

            while (dividirXespacios.Count() > 0)
            {
                var elementoExtraido = dividirXespacios.FirstOrDefault();
                dividirXespacios = dividirXespacios.Skip(1).ToList();

                if (string.IsNullOrWhiteSpace(elementoExtraido))
                    continue;

                //se trata de una key
                if (elementoExtraido.FirstOrDefault() == '-')
                {
                    //ya hay una key anterior
                    if (!string.IsNullOrEmpty(key))
                    {
                        result.Add(new Tuple<string, string>(key, value.Trim()));
                        key = string.Empty;
                        value = string.Empty;
                    }

                    key = elementoExtraido.Trim();
                }
                //se trata de un valor
                else
                {
                    value += string.Format(" {0}", elementoExtraido);
                }
            }

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                result.Add(new Tuple<string, string>(key, value.Trim()));
            }

            return result;
        }

        public bool ComprobarServicio()
        {
            var servicio = "http://mamutpre.cinconet.local/ServicioMamut.svc";


            return false;
        }
    }
}