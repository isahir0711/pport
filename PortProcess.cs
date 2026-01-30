namespace pport
{
    public class ProcPort
    {
        private readonly static Dictionary<int, List<InodeInfo>> cache = [];

        public static void DisplayPortProcesses(PortState state)
        {
            List<ProcessInfo> procInfoList = [];

            procInfoList.AddRange(GetPortsProcesses("/proc/net/tcp", Protocol.IPv4, state));
            procInfoList.AddRange(GetPortsProcesses("/proc/net/tcp6", Protocol.IPv6, state));
            procInfoList = [.. procInfoList.OrderBy(x => x.Port)];

            if (procInfoList.Count < 1)
            {
                Console.WriteLine("No ports found");
                return;
            }

            Helper.PrintTable(procInfoList);
        }

        public static void DisplayPort(int portNumber)
        {
            List<ProcessInfo> procInfoList = [];

            procInfoList.AddRange(GetPort("/proc/net/tcp", portNumber));
            procInfoList = [.. procInfoList.OrderBy(x => x.Port)];

            if (procInfoList.Count < 1)
            {
                Console.WriteLine("No port found");
                return;
            }
            Helper.PrintTable(procInfoList);
        }

        public static List<ProcessInfo> GetPortsProcesses(string filePath, Protocol protocol, PortState state)
        {
            CreateCache();
            const int localAddressColumn = 1;
            const int inodeColumn = 9;
            const int stateColumn = 3;

            var dir = filePath;
            var lines = File.ReadAllLines(dir);
            List<ProcessInfo> result = [];

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string statestring = parts[stateColumn];
                int sHex = Convert.ToInt32(statestring, 16);

                var portState = (PortState)sHex;

                if (!portState.Equals(state))
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

                foreach (var item in hit)
                {
                    result.Add(new ProcessInfo(item.ProcessName, item.CommandLine, item.PID, port, protocol));
                }
            }
            return result;

        }

        public static List<ProcessInfo> GetPort(string filePath, int portNumber, Protocol protocol = Protocol.IPv4, PortState state = PortState.Listen)
        {
            CreateCache();
            const int localAddressColumn = 1;
            const int inodeColumn = 9;
            const int stateColumn = 3;

            var dir = filePath;
            var lines = File.ReadAllLines(dir);
            List<ProcessInfo> result = [];

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string statestring = parts[stateColumn];
                int sHex = Convert.ToInt32(statestring, 16);

                var portState = (PortState)sHex;

                if (!portState.Equals(state))
                {
                    continue;
                }
                string localAddress = parts[localAddressColumn];
                string hexPort = localAddress.Split(':')[1];
                int port = Convert.ToInt32(hexPort, 16);

                if (port != portNumber)
                {
                    continue;
                }

                string inodestring = parts[inodeColumn];
                int inode = Convert.ToInt32(inodestring);

                if (!cache.TryGetValue(inode, out List<InodeInfo>? hit) || hit == null)
                {
                    continue;
                }

                foreach (var item in hit)
                {
                    result.Add(new ProcessInfo(item.ProcessName, item.CommandLine, item.PID, port, protocol));
                }
            }
            return result;

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
                            var inodeinfo = new InodeInfo(processName, commandLine, pid);

                            if (cache[inode].Where(x => x.PID == inodeinfo.PID).ToList().Count > 0)
                            {
                                continue;
                            }

                            cache[inode].Add(inodeinfo);

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

        public Protocol Protocol { get; }
        public int PID { get; }

        public ProcessInfo(string processName, string commandLine, int pid, int port, Protocol protocol)
        {
            ProcessName = processName;
            CommandLineName = commandLine;
            PID = pid;
            Port = port;
            Protocol = protocol;
        }

    }

    public enum Protocol
    {
        IPv6,
        IPv4,
    }

    public enum PortState
    {
        Established = 0x01,
        Close = 0x07,
        Listen = 0x0A,
    }

}