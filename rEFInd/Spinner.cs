namespace ErrorIndicateList
{
    public class SpinTasker : IDisposable
    {
        //Тело конструктора
        int SpinTaskerLine;
        string? WorkText;
        bool active;

        //Настройки SpinTasker'а
        private readonly int delay = 100;
        private int counter = 0;
        private readonly string Sequence = @"/-\|";
        private Thread? SpinThread;

        //Другие значения
        private readonly string Temp = Path.GetTempPath();

        public void Start(string Text)
        {
            WorkText = Text;
            SpinTaskerLine = Console.CursorTop;
            active = true;

            SpinThread = new Thread(Spin);
            if (!SpinThread.IsAlive)
                SpinThread.Start();
        }

        public void Stop(bool Condition = false, string? ErrorMessage = "Unexpected Error", string? OkMessage = null)
        {
            active = false;
            counter = 0;
            Thread.Sleep(100);
            Console.Write("                                               ");
            if (Condition)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("[ ");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(" ] " + WorkText + "   ");

                Console.SetCursorPosition(31, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(!string.IsNullOrWhiteSpace(OkMessage) ? OkMessage : "");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("[ ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("!ERROR!");
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(" ] " + WorkText + "   ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n    " + ErrorMessage + "\n");
                Console.ForegroundColor = ConsoleColor.White;

                if (File.Exists(@$"{Temp}\rEFInd-Bin.zip"))
                    File.Delete(@$"{Temp}\rEFInd-Bin.zip");

                if (Directory.Exists(@$"{Temp}\rEFInd"))
                    Directory.Delete(@$"{Temp}\rEFInd", true);

                Console.CursorVisible = true;

                if (rEFInd.WorkClass.Options.Wait)
                    Console.ReadLine();

                Environment.Exit(1);
            }
        }

        private void Spin()
        {
            while (active)
            {
                Console.SetCursorPosition(0, SpinTaskerLine);
                Console.Write("[ " + Sequence[++counter % Sequence.Length] + " ] " + WorkText + "...");
                Thread.Sleep(delay);
            }
        }

        public void Dispose()
        {
            Stop(false);
            GC.SuppressFinalize(this);
        }
    }
}