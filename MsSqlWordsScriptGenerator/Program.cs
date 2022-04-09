using System;
using System.IO;
using System.Text;

namespace MsSqlWordsScriptGenerator
{
    class Program
    {
        static readonly StringBuilder Sb = new StringBuilder();

        static void Main(string[] args)
        {
            WriteStart();

            WriteWordCollection();

            var words = File.ReadAllLines("words.txt");
            foreach (var w in words)
            {
                if (string.IsNullOrEmpty(w))
                    continue;

                Console.WriteLine(w);
                WriteWord(w);
            }

            WriteEnd();

            File.WriteAllText("MsSqlWordCollectionsInit.sql", Sb.ToString());

            Console.WriteLine("\nСкрипт сгенерирован");
        }

        static void WriteStart()
        {
            Sb.Append("begin transaction;" + Environment.NewLine);
        }

        static void WriteWordCollection()
        {
            Sb.Append("insert WordCollections values ('Базовый набор слов', null, null)" + Environment.NewLine);
            Sb.Append("declare @baseWordCollectionId integer" + Environment.NewLine);
            Sb.Append("set @baseWordCollectionId = @@IDENTITY" + Environment.NewLine + Environment.NewLine);
        }

        static void WriteWord(string word)
        {
            Sb.Append($"insert Words values ('{word}', @baseWordCollectionId)" + Environment.NewLine);
        }

        static void WriteEnd()
        {
            Sb.Append("commit;");
        }
    }
}
