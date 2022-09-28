using Newtonsoft.Json;
using System.Text.RegularExpressions;
using ShellProgressBar;

class Program
{
    static void Main(string[] args)
    {
        foreach (int value in Enumerable.Range(0, 10))
        {
            List<Output> outputs = new List<Output>();
            string fileName = $"./data/input/regexlib/regexlib_egret_{value}.json";
            // string fileName = "./data/input-sample.json";
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                List<InputObject> items = JsonConvert.DeserializeObject<List<InputObject>>(json);
                ProgressBar pb = new ProgressBar(
                    items.Count,
                    $"Reading regex patterns from {fileName}"
                );
                foreach (InputObject element in items)
                {
                    Output output = new Output();
                    if (element.regex != null)
                    {
                        Console.WriteLine("Busy with Regex: " + element.regex);
                        pb.Tick();
                        List<RegexInputPair> matchingInputPairs = new List<RegexInputPair>();
                        List<RegexInputPair> nonMatchingInputPairs = new List<RegexInputPair>();
                        try
                        {
                            Regex rx = new Regex(element.regex);
                            output.regex = element.regex;

                            // iterate over matches proposed by egret
                            if (element.matches != null)
                            {
                                foreach (string input in element.matches)
                                {
                                    RegexInputPair inputPair = new RegexInputPair();
                                    bool matches = rx.IsMatch(input);
                                    inputPair.input = input;
                                    inputPair.match = matches;
                                    matchingInputPairs.Add(inputPair);
                                }
                            }

                            // iterate over non-matches proposed by egret
                            if (element.nonMatches != null)
                            {
                                foreach (string input in element.nonMatches)
                                {
                                    RegexInputPair nonMatchingInputPair = new RegexInputPair();
                                    bool matches = rx.IsMatch(input);
                                    nonMatchingInputPair.input = input;
                                    nonMatchingInputPair.match = matches;

                                    // append to nonMatching list
                                    nonMatchingInputPairs.Add(nonMatchingInputPair);
                                }
                            }
                            output.inputExceptionStackTrace = element.exceptionStackTrace;
                            output.exception = false;
                            output.matchingInputPairs = matchingInputPairs;
                            output.nonMatchingInputPairs = nonMatchingInputPairs;
                        }
                        catch (System.Exception e)
                        {
                            // Console.WriteLine(e);
                            RegexInputPair inputPair = new RegexInputPair();

                            OutputException exceptionStackTrace = new OutputException();
                            exceptionStackTrace.regex = element.regex;
                            exceptionStackTrace.message = e.Message;
                            exceptionStackTrace.source = e.Source;

                            output.exception = true;
                            output.outputExceptionStackTrace = exceptionStackTrace;
                            output.inputExceptionStackTrace = element.exceptionStackTrace;
                            // Console.WriteLine($"Error occured: {e.Message}");
                            outputs.Add(output);
                            continue;
                        }
                        outputs.Add(output);
                    }
                }
            }
            string outputFile = $"./data/output/regexlib/output_{value}.json";
            // string outputFile = $"./data/output_sample.json";

            File.WriteAllText(outputFile, JsonConvert.SerializeObject(outputs));
        }

        // serialize JSON directly to a file
        // using (StreamWriter file = File.CreateText("./data/output.json"))
        // {
        //     Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //     serializer.Serialize(file, outputs);
        // }
        // }
    }
}

public class InputObject
{
    public List<string>? matches { get; set; }
    public List<string>? nonMatches { get; set; }
    public string? regex { get; set; }
    public InputException? exceptionStackTrace { get; set; }
}

public class Output
{
    public Boolean? exception { get; set; }
    public string? regex { get; set; }
    public List<RegexInputPair>? matchingInputPairs { get; set; }

    public List<RegexInputPair>? nonMatchingInputPairs { get; set; }

    public OutputException? outputExceptionStackTrace { get; set; }

    public InputException? inputExceptionStackTrace { get; set; }
}

public class InputException
{
    public string? exceptionThrownBy { get; set; }
    public string? exception { get; set; }
}

public class OutputException
{
    public string? message { get; set; }
    public string? regex { get; set; }
    public string? source { get; set; }
}

public class RegexInputPair
{
    public string? input { get; set; }
    public Boolean? match { get; set; }
}
