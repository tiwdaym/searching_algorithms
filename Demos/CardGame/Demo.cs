using System;
using System.IO;
using System.Reflection;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class Demo
    {

        public void Run()
        {
            Json testData = GetTestData();

        }

        public Json GetTestData()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Demos\CardGame\Data\demoDataJson.json");
            string jsonRawData = File.ReadAllText(path);
            Console.WriteLine(jsonRawData);

            return Json.DecodeJsonFromString(jsonRawData);
        }
    }
}
