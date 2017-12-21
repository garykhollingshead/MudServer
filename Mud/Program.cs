using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Mud.Data.Configuration;
using Mud.Server;
using Newtonsoft.Json;

namespace Mud
{

    class Program
    {
        private static EventWaitHandle ExitRecieved;
        private static EventWaitHandle ExitFinished;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    ExitRecieved.Set();
                    ExitFinished.WaitOne();
                    break;
            }
            return false;
        }

        static void Main(string[] args)
        {
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            var appSettings = LoadSettings();
            var server = new MudServer(appSettings);
            ExitRecieved = new EventWaitHandle(false, EventResetMode.AutoReset);
            ExitFinished = new EventWaitHandle(false, EventResetMode.AutoReset);
            
            ExitRecieved.WaitOne();

            server.Shutdown();
            
            ExitFinished.Set();
        }

        private static AppSettings LoadSettings()
        {
            using (StreamReader r = new StreamReader("AppSettings.json"))
            {
                var json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
        }

    }
}
