using System;

namespace R8Assembler
{
    internal class Program
    {
        readonly static Dictionary<string, string> INSTRUCTIONS = new Dictionary<string, string>()
        {
            {"NOP", "00000000"},
            {"LDA", "00000001"},
            {"STA", "00000010"},
            {"LDI", "00000011"},
            {"ADD", "00000100"},
            {"SUB", "00000101"},
            {"JMP", "00000110"},
            {"JMZ", "00000111"},
        };

        static void Main(string[] args)
        {
            string? readFilePath = "";
            string? outputFileName = "";
            string[]? lines = null;

            // Collects user input
            do
            {
                Console.Write("Enter source code file: ");
                readFilePath = Console.ReadLine();

                if (string.IsNullOrEmpty(readFilePath)) continue;

                try
                {
                    lines = System.IO.File.ReadAllLines(readFilePath);
                }
                catch (System.IO.FileNotFoundException) { continue; }

            } while (lines == null);

            Console.Write("Enter output file name: ");
            outputFileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputFileName)) { outputFileName = "a"; }

            List<string> filteringLines = new List<string>();
            Dictionary<string, int> labels = new Dictionary<string, int>();

            int byteCounter = 0;

            // Filters the lines
            for (int i = 0; i < lines.Length; i++)
            {
                // Removing empty lines
                if (string.IsNullOrWhiteSpace(lines[i])) { continue; }

                // Removing comments
                if (lines[i].Contains(';'))
                {
                    int index = lines[i].IndexOf(';');
                    lines[i] = lines[i].Substring(0, index);
                }

                // Removing whitespace around value
                lines[i] = lines[i].Trim();

                // Counting bytes
                if (INSTRUCTIONS.ContainsKey(lines[i].Split()[0])) { byteCounter += 2; }
                else if (lines[i].Split()[0] == "db") { byteCounter += 1; }
                else if (lines[i].Split()[0] == "halt") { byteCounter += 1; }

                // Listing and removing labels
                if (lines[i].EndsWith(':'))
                {
                    labels.Add(lines[i].Remove(lines[i].Length - 1, 1), byteCounter);
                    lines[i] = lines[i].Remove(lines[i].Length - 1, 1);
                }

                filteringLines.Add(lines[i]);
            }

            string[] filteredLines = filteringLines.ToArray();
            List<string> outputLines = new List<string>();

            // Creates binary output
            for (int i = 0; i < filteredLines.Length; i++)
            {
                string[] tokens = filteredLines[i].Split();

                // If it's an instruction
                if (INSTRUCTIONS.ContainsKey(tokens[0]))
                {
                    outputLines.Add(INSTRUCTIONS[tokens[0]]);

                    if (tokens.Length != 2) { tokens = new string[] { tokens[0], "0b00000000" }; }

                    string? byteArg = filterByte(tokens[1]);

                    if (byteArg == null && labels.ContainsKey(tokens[1]))
                    {
                        if (tokens[0] == "JMP" || tokens[0] == "JMZ")
                        {
                            byteArg = intToBinary((labels[tokens[1]] - 1).ToString());
                        }
                        else { byteArg = intToBinary(labels[tokens[1]].ToString()); }
                    }
                    if (byteArg == null) { Console.WriteLine($"{tokens[1]} is not a valid byte."); Console.Read(); return; }

                    outputLines.Add(byteArg);
                }

                // If it's a define byte
                if (tokens[0] == "db")
                {
                    if (tokens.Length != 2) { tokens = new string[] { tokens[0], "0b00000000" }; }

                    string? byteArg = filterByte(tokens[1]);
                    if (byteArg == null) { Console.WriteLine($"{tokens[1]} is not a valid byte."); Console.Read(); return; }
                    outputLines.Add(byteArg);
                }

                // If it's halt
                if (tokens[0] == "halt")
                {
                    outputLines.Add("11111000");
                }
            }

            if (outputLines.Count > 64) { Console.WriteLine("Warning: output file longer than 64 bytes."); }

            File.WriteAllLines($"{Path.GetDirectoryName(readFilePath)}/{outputFileName}.R8bin", outputLines.ToArray());
        }

        static string? filterByte(string input)
        {
            if (input.StartsWith("0b")) return input.Substring(2);
            else if (input.StartsWith("0x")) return hexToBinary(input.Substring(2));
            else if (int.TryParse(input, out _)) return intToBinary(input);
            else if (input.StartsWith("'") && input.EndsWith("'")) return intToBinary(((int)input[1]).ToString());
            else return null;
        }

        static string? hexToBinary(string input)
        {
            try
            {
                string result = Convert.ToString(Convert.ToInt32(input, 16), 2).PadLeft(8, '0');
                return result;
            }
            catch (System.FormatException)
            {
                Console.WriteLine($"0x{input} is not a valid hexadecimal number.");
                Console.Read();
                return null;
            }
        }

        static string? intToBinary(string input)
        {
            try
            {
                string result = Convert.ToString(Convert.ToInt32(input), 2).PadLeft(8, '0');
                return result;
            }
            catch (System.FormatException)
            {
                Console.WriteLine($"{input} is not a valid integer.");
                Console.Read();
                return null;
            }
        }
    }
}