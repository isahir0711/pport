
namespace pport
{
    public class ProcPort
    {
        private readonly static Dictionary<int, List<InodeInfo>> cache = [];

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
            CreateCache();
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
                string inodestring = parts[inodeColumn];
                int inode = Convert.ToInt32(inodestring);

                if (!cache.TryGetValue(inode, out List<InodeInfo>? hit) || hit == null)
                {
                    continue;
                }

                List<ProcessInfo> tempList = [];
                foreach (var item in hit)
                {
                    tempList.Add(new ProcessInfo(item.ProcessName, item.CommandLine, item.PID, port));
                }

                procInfoList.Add(tempList);
            }

            return procInfoList;
        }

        public static void CreateCache()
        {
            var dirs = Directory.EnumerateDirectories("/proc").Where(x => int.TryParse(Path.GetFileName(x), out int foo));

            foreach (var procDir in dirs)
            {
                var fdDir = Path.Combine(procDir, "fd");

                if (!Directory.Exists(fdDir))
                {
                    continue;
                }

                try
                {
                    var fdDirs = Directory.EnumerateFiles(fdDir);

                    foreach (var fd in fdDirs)
                    {
                        var info = new FileInfo(fd).LinkTarget;
                        if (info is null)
                        {
                            continue;
                        }
                        if (info[0..6] == "socket")
                        {
                            string pidstring = fd.Split('/')[2];
                            string inodestring = info[8..^1];
                            int pid = Convert.ToInt32(pidstring);
                            int inode = Convert.ToInt32(inodestring);
                            string processName = File.ReadAllText(Path.Combine(procDir, "comm")).Replace('\0', ' ').TrimEnd();
                            string commandLine = File.ReadAllText(Path.Combine(procDir, "cmdline")).Replace('\0', ' ').TrimEnd();

                            if (!cache.ContainsKey(inode))
                            {
                                cache.Add(inode, []);
                            }

                            cache[inode].Add(new InodeInfo(processName, commandLine, pid));
                        }
                    }

                }
                catch (Exception)
                {
                    continue;
                }
            }
        }


    }

    public class InodeInfo
    {
        public string ProcessName { get; }
        public string CommandLine { get; }
        public int PID { get; }

        public InodeInfo(string processName, string commandLine, int pid)
        {
            ProcessName = processName;
            CommandLine = commandLine;
            PID = pid;
        }

        public override string ToString()
        {
            return $"{ProcessName}";
        }
    }

    public class ProcessInfo
    {
        public int Port { get; }
        public string ProcessName { get; }
        public string CommandLineName { get; }
        public int PID { get; }
        public ProcessInfo(string processName, string commandLine, int pid, int port)
        {
            ProcessName = processName;
            CommandLineName = commandLine;
            PID = pid;
            Port = port;
        }

    }

}