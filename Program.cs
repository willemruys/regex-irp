using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        List<Output> outputs = new List<Output>();
        using (StreamReader r = new StreamReader("./data/uniq-regexes-8.json"))
        {
            string json = r.ReadToEnd();
            List<InputObject> items = JsonConvert.DeserializeObject<List<InputObject>>(json);

            foreach (InputObject element in items)
            {
                Output output = new Output();
                if (element.regex != null)
                {
                    List<RegexInputPair> inputPairs = new List<RegexInputPair>();
                    try
                    {
                        Regex rx = new Regex(element.regex);
                        output.regex = element.regex;
                        if (element.matches.Count > 0)
                        {
                            foreach (string input in element.matches)
                            {
                                RegexInputPair inputPair = new RegexInputPair();
                                bool matches = rx.IsMatch(input);
                                inputPair.input = input;
                                inputPair.match = matches;
                                inputPairs.Add(inputPair);
                            }
                        }
                        output.inputExceptionStackTrace = element.exceptionStackTrace;
                        output.exception = false;
                        output.inputPairs = inputPairs;
                    }
                    catch (System.Exception e)
                    {
                        RegexInputPair inputPair = new RegexInputPair();

                        OutputException exceptionStackTrace = new OutputException();
                        exceptionStackTrace.regex = element.regex;
                        exceptionStackTrace.message = e.Message;
                        exceptionStackTrace.source = e.Source;

                        output.exception = true;
                        output.outputExceptionStackTrace = exceptionStackTrace;
                        output.inputExceptionStackTrace = element.exceptionStackTrace;
                        Console.WriteLine($"Error occured: {e.Message}");
                        outputs.Add(output);
                        continue;
                    }
                    outputs.Add(output);
                }
            }
        }
        File.WriteAllText("./data/output.json", JsonConvert.SerializeObject(outputs));

        // serialize JSON directly to a file
        using (StreamWriter file = File.CreateText("./data/output.json"))
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Serialize(file, outputs);
        }
    }
}

public class InputObject
{
    public List<string>? matches { get; set; }
    public string? regex { get; set; }
    public InputException? exceptionStackTrace { get; set; }
}

public class Output
{
    public Boolean? exception { get; set; }
    public string? regex { get; set; }
    public List<RegexInputPair>? inputPairs { get; set; }

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
