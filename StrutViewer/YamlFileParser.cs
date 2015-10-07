using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DivyaSinghal.StrutViewer
{
    internal class YamlFileParser
    {
        private readonly string _filePath;

        private const string DescriptionTag = "description:";
        private const string ScenarioTag = "x-scenario";
        private const string WhenTag = "when:";
        private const string GivenTag = "given:";
        private const string ThenTag = "then:";
        private const string ArrayTag = "-";
        public IList<TestModel> Tests;

        public YamlFileParser(string filePath)
        {
            _filePath = filePath;
        }

        public IList<TestModel> Parse()
        {

            List<TestModel> result = new List<TestModel>();

            //// Load test from yaml file
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(_filePath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {

                    if (line.Contains(DescriptionTag))
                    {
                        var test = new TestModel();
                        test.Description = line.Split(':').LastOrDefault();
                        result.Add(test);
                    }

                    // Process line    
                   // FindTest(line,streamReader); 
                }
            }

            return result;
        }

        private TestModel FindTest(string line, StreamReader streamReader)
        {
            var result = new TestModel();
            if (string.IsNullOrEmpty(FindDescription(streamReader)))
            {
                return result;
            }

            if (FindScenario(streamReader, result))
            {
                FindTests(streamReader, result);
            }

            return result;
        }

        private void FindTests(StreamReader streamReader, TestModel result)
        {
            string line = streamReader.ReadLine(); 
            if(line != null)
            {
                switch (line)
                {
                    case "-":
                        FindUnitTestCollection(streamReader, result);
                        break;
                    case WhenTag:
                    case GivenTag:
                    case ThenTag:
                        break;
                    default:
                        throw new  Exception( String.Format("Invalid line --> {0}", line));

                }
                if (line.Contains(DescriptionTag))
                {
                    result.Description = line.Split(':').LastOrDefault();

                }
            }
        }

        private void FindUnitTestCollection(StreamReader streamReader, TestModel result)
        {
            result.Tests = new List<UnitTestModel>();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                switch (GetKeyWord(line))
                {
                    case GivenTag:

                    case WhenTag:
                    case ThenTag:
                    case "":
                        break;
                    case ArrayTag:
                        result.Tests.Add(new UnitTestModel());
                        break;
                    case DescriptionTag:
                        return;
                    case ScenarioTag:
                        return;
                }
            }
        }

        private string FindDescription(StreamReader streamReader)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Contains(DescriptionTag))
                {
                    var value = line.Split(':').LastOrDefault();
                    return value;
                }
            }

            return null;
        }

        private bool FindScenario(StreamReader streamReader, TestModel result)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Contains(ScenarioTag))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetKeyWord(string line)
        {
            if (line.StartsWith(ArrayTag))
            {
                return ArrayTag;
            }

            if (line.StartsWith(WhenTag))
            {
                return WhenTag;
            }

            if (line.StartsWith(GivenTag))
            {
                return GivenTag;
            }

            if (line.StartsWith(ThenTag))
            {
                return ThenTag;
            }

            if (line.StartsWith(ScenarioTag))
            {
                return ScenarioTag;
            }

            if (line.StartsWith(DescriptionTag))
            {
                return DescriptionTag;
            }

            return string.Empty;
        }

    }
}
