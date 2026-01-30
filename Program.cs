using System.ComponentModel;
using System.Diagnostics;
using pport;

if (args.Length == 0)
{
    ProcPort.DisplayPortProcesses(PortState.Listen);
    return;
}

if (args.Length == 1)
{
    switch (args[0])
    {
        case "--csv":
            Helper.CreateCSV();
            break;
        case "--json":
            Helper.CreateJSON();
            break;
        case "--help":
            Helper.DisplayHelp();
            break;
    }
    return;
}

if (args.Length > 2)
{
    Helper.DisplayHelp();
    return;
}

//try to parse the args
//--port portnumbertoquery

string option = args[0];
string param = args[1];
switch (option)
{
    case "--watch":
        string sub = param[0..^1];
        if (!int.TryParse(sub, out int timeinseconds))
        {
            Console.WriteLine("Wrong input format for time");
            return;
        }
        while (true)
        {
            Console.Clear();
            ProcPort.DisplayPortProcesses(PortState.Listen);
            Thread.Sleep(timeinseconds * 1000);
        }

    case "--state":
        var res = Enum.TryParse(param, ignoreCase: true, out PortState state) && Enum.IsDefined(state);
        if (!res)
        {
            Console.WriteLine("Bad state");
            return;
        }
        ProcPort.DisplayPortProcesses(state);
        break;

    case "--kill":
        if (int.TryParse(param, out int pid))
        {
            Process process = Process.GetProcessById(pid);
            if (process is null)
            {
                Console.WriteLine("not found");
                return;
            }
            process.Kill();
            return;

        }
        Process[] processes = Process.GetProcessesByName(param);
        if (processes.Length == 0)
        {
            Console.WriteLine("No process found with that name");
            return;
        }

        foreach (Process process in processes)
        {
            try
            {
                process.Kill();
                return;
            }
            catch (Win32Exception)
            {
                Console.WriteLine("Not permitted");
                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Not permitted");
                return;
            }
        }
        break;

    case "--port":
        if (!int.TryParse(param, out int portNumber))
        {
            Console.WriteLine("bad port number");
            return;
        }
        ProcPort.DisplayPort(portNumber);
        break;


    default:
        Helper.DisplayHelp();
        break;
}

