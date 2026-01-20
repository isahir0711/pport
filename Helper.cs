using System.Text.Json;

namespace pport
{
    public class Helper
    {
        public static void DisplayHelp()
        {
            Console.WriteLine("PPORT - List listening network ports and their processes");
            Console.WriteLine();
            Console.WriteLine("Usage: pport [COMMAND] [OPTION]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  --help              Display this help message");
            Console.WriteLine("  --watch <interval>  Continuously refresh at specified interval (e.g., 1s, 5s)");
            Console.WriteLine("  --port <port>       Filter results by a specific port number");
            Console.WriteLine("  --state <state>     Filter by port state (e.g., LISTEN, ESTABLISHED)");
            Console.WriteLine("  --csv               Export results in CSV format");
            Console.WriteLine("  --json              Export results in JSON format");
            Console.WriteLine("  --kill <pid|name>   Kill a process by PID or process name");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  pport                 List all listening ports");
            Console.WriteLine("  pport --watch 2s      Refresh every 2 seconds");
            Console.WriteLine("  pport --port 8080     Show only port 8080");
            Console.WriteLine("  pport --kill nginx    Kill all nginx processes");
            Console.WriteLine();
        }

        public static void CreateCSV()
        {
            var procInfoList = ProcPort.GetPortsProcesses();

            string outputDir = "output";
            Directory.CreateDirectory(outputDir);
            string csvFilePath = Path.Combine(outputDir, "pport_output.csv");

            try
            {
                using (var writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("PORT,PROCESS_NAME,COMMAND_LINE");

                    foreach (var procList in procInfoList)
                    {
                        foreach (var info in procList)
                        {
                            string escapedCommandLine = info.CommandLineName.Replace("\"", "\"\"");
                            if (escapedCommandLine.Contains(",") || escapedCommandLine.Contains("\"") || escapedCommandLine.Contains("\n"))
                            {
                                escapedCommandLine = $"\"{escapedCommandLine}\"";
                            }

                            writer.WriteLine($"{info.Port},{info.ProcessName},{escapedCommandLine}");
                        }
                    }
                }

                Console.WriteLine($"CSV file created successfully: {csvFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating CSV file: {ex.Message}");
            }
        }

        public static void CreateJSON()
        {
            var procInfoList = ProcPort.GetPortsProcesses();

            string outputDir = "output";
            Directory.CreateDirectory(outputDir);
            string jsonFilePath = Path.Combine(outputDir, "pport_output.json");

            try
            {
                var flatList = new List<object>();

                foreach (var procList in procInfoList)
                {
                    foreach (var info in procList)
                    {
                        flatList.Add(new
                        {
                            Port = info.Port,
                            PID = info.PID,
                            ProcessName = info.ProcessName,
                            CommandLine = info.CommandLineName
                        });
                    }
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(flatList, options);

                File.WriteAllText(jsonFilePath, jsonContent);

                Console.WriteLine($"JSON file created successfully: {jsonFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating JSON file: {ex.Message}");
            }
        }
    }
}