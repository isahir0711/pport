
namespace pport
{
    public class ProcPort
    {
        private static List<ProcessInfo> GetProcessByInode(int inode)
        {
            var dirs = Directory.EnumerateDirectories("/proc").Where(x => int.TryParse(Path.GetFileName(x), out int foo));

            List<ProcessInfo> procList = [];
            foreach (var procDir in dirs)
            {
                // Console.WriteLine(dir);
                var fdPath = Path.Combine(procDir, "fd");
                if (!Directory.Exists(fdPath))
                    continue;
                try
                {
                    var fdDirs = Directory.EnumerateFiles(Path.Combine(procDir, "fd"));
                    foreach (var fd in fdDirs)
                    {
                        var info = new FileInfo(fd).LinkTarget;
                        if (info is null)
                        {
                            continue;
                        }
                        if (info == $"socket:[{inode}]")
                        {
                            string trimmedpid = fd.Split('/')[2];
                            // Console.WriteLine($"DEBUG: PID: {pid}");
                            int pid = Convert.ToInt32(trimmedpid);
                            string ProcessName = File.ReadAllText(Path.Combine(procDir, "comm")).Replace('\0', ' ').TrimEnd();
                            string commandLine = File.ReadAllText(Path.Combine(procDir, "cmdline")).Replace('\0', ' ').TrimEnd();
                            var procInfo = new ProcessInfo(ProcessName, commandLine, pid);
                            procList.Add(procInfo);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

            }
            return procList;

        }

        public static void DisplayPortProcesses()
        {
            var procInfoList = GetPortsProcesses();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("┌─────────┬─────────┬──────────────────────────┐");
            Console.WriteLine("│ PORT    │ PID     │ PROCESS NAME             │");
            Console.WriteLine("├─────────┼─────────┼──────────────────────────┤");
            Console.ResetColor();

            foreach (var proc in procInfoList)
            {
                foreach (var info in proc)
                {
                    Console.WriteLine("│ {0,-7} │ {1,-7} │ {2,-24} │", info.Port, info.PID, info.ProcessName);
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("└─────────┴─────────┴──────────────────────────┘");
            Console.ResetColor();
        }


        public static List<List<ProcessInfo>> GetPortsProcesses()
        {
            const int localAddressColumn = 1;
            const int inodeColumn = 9;
            const int stateColumn = 3;

            var dir = "/proc/net/tcp";
            var lines = File.ReadAllLines(dir);
            List<List<ProcessInfo>> procInfoList = [];

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string state = parts[stateColumn];
                if (state != "0A")
                {
                    continue;
                }
                string localAddress = parts[localAddressColumn];
                string hexPort = localAddress.Split(':')[1];
                int port = Convert.ToInt32(hexPort, 16);
                string inode = parts[inodeColumn];

                var temptList = GetProcessByInode(Convert.ToInt32(inode));
                temptList.ForEach(x => x.Port = port);
                procInfoList.Add(temptList);
            }

            return procInfoList;
        }
    }
    public class ProcessInfo
    {
        public int Port { get; set; }
        public string ProcessName { get; }
        public string CommandLineName { get; }
        public int PID { get; }
        public ProcessInfo(string processName, string commandLine, int pid)
        {
            ProcessName = processName;
            CommandLineName = commandLine;
            PID = pid;
        }

    }

}