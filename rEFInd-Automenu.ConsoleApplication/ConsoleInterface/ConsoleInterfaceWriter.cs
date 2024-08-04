namespace rEFInd_Automenu.ConsoleApplication.ConsoleInterface
{
    public static class ConsoleInterfaceWriter
    {
        private const int _OriginalMessageOffset = 50;
        public static int MessageOffset
        {
            get;
            set;
        } = _OriginalMessageOffset;

        public static void ResetOffset()
        {
            MessageOffset = _OriginalMessageOffset;
        }

        public static void WriteHeader(string Message)
        {
            //Console.WriteLine();
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(Message);
            Console.ResetColor();
            Console.WriteLine(" ]");
        }

        public static void WriteInformation(string WorkText, string? Message)
        {
            WriteInformation(Console.CursorTop, WorkText, Message);
        }

        public static void WriteWarning(string Message)
        {
            WriteWarning(Console.CursorTop, Message, null);
        }

        public static void WriteError(string Message)
        {
            WriteError(Console.CursorTop, Message, null);
        }

        public static void WriteInformation(int WorkLine, string WorkText, string? Message)
        {
            Console.SetCursorPosition(0, WorkLine);
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(0, WorkLine);
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("INFO");
            Console.ResetColor();
            Console.Write(" ] " + WorkText);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                Console.SetCursorPosition(MessageOffset, WorkLine);
                //Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" - " + Message);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        public static void WriteSuccess(int WorkLine, string WorkText, string? Message)
        {
            Console.SetCursorPosition(0, WorkLine);
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(0, WorkLine);
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK");
            Console.ResetColor();
            Console.Write(" ] " + WorkText);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                Console.SetCursorPosition(MessageOffset, WorkLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Message);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        public static void WriteError(int WorkLine, string WorkText, string? Message)
        {
            Console.SetCursorPosition(0, WorkLine);
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(0, WorkLine);
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("!ERROR!");
            Console.ResetColor();
            Console.Write(" ] " + WorkText);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(MessageOffset, WorkLine);
            Console.Write(Message);
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void WriteWarning(int WorkLine, string WorkText, string? Message)
        {
            Console.SetCursorPosition(0, WorkLine);
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(0, WorkLine);
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("!WARNING!");
            Console.ResetColor();
            Console.Write(" ] " + WorkText);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(MessageOffset, WorkLine);
                Console.Write(Message);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}
