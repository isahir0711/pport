const int localAddressColumn = 1;
const int inodeColumn = 9;
const int stateColumn = 3;

var dir = "/proc/net/tcp";
var lines = File.ReadAllLines(dir);

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

    var procInfo = DisplayPortProcess(Convert.ToInt32(inode));
    if (procInfo is null)
    {
        continue;
    }
    Console.WriteLine("Port {0,6} Process Name: {1,6}", port, procInfo.ProcessName);
    // Console.WriteLine($"CommandLine {procInfo.CommandLineName}");

}

//TODO: 1 inode doesnt mean just 1 process (return a list of ProcessInfo)
//TODO: allow check just for one port or process
//TODO: Check for tcp6/ipv6 too
//TODO: we could create a map/dictionary traverse the while /proc/*/fd looking for socket:[x] and then query it
static ProcessInfo DisplayPortProcess(int inode)
{
    var dirs = Directory.EnumerateDirectories("/proc").Where(x => int.TryParse(Path.GetFileName(x), out int foo));

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
                    string ProcessName = File.ReadAllText(Path.Combine(procDir, "comm")).Replace('\0', ' ').TrimEnd();
                    string commandLine = File.ReadAllText(Path.Combine(procDir, "cmdline")).Replace('\0', ' ').TrimEnd();
                    var procInfo = new ProcessInfo(ProcessName, commandLine);
                    return procInfo;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            continue;
        }

    }
    return null;

}

class ProcessInfo
{
    public string ProcessName { get; }
    public string CommandLineName { get; }
    public ProcessInfo(string processName, string commandLine)
    {
        ProcessName = processName;
        CommandLineName = commandLine;
    }
}
