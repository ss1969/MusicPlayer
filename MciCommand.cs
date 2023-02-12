namespace Null.MciPlayer
{
    public struct MciCommand
    {
        public string command;
        public string? buffer;
        public int bufferSize;
        public IntPtr callback;
        public AutoResetEvent? are;

        public MciCommand(string command,
                          string? buffer,
                          int bufferSize,
                          IntPtr callback,
                          AutoResetEvent? are = null)
        {
            this.command = command;
            this.buffer = buffer;
            this.bufferSize = bufferSize;
            this.callback = callback;
            this.are = are;
        }
    }
}
