namespace Null.MciPlayer
{
    public class MciEventCode
    {
        public const int MM_MCINOTIFY = 0x3B9;
        public const int MCI_NOTIFY_SUCCESSFUL = 0x01;
        public const int MCI_NOTIFY_SUPERSEDED = 0x02;
        public const int MCI_NOTIFY_ABORTED = 0x04;
        public const int MCI_NOTIFY_FAILURE = 0x08;
    }
}
