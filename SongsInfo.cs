using System.Drawing.Imaging;

namespace MusicPlayer
{
    public class SongsInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Filesize { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public Image AlbumImage { get; set; }
        public string Year { get; set; }
        public string OriginName { get; set; }
        public string Duration { get; set; }
        public string ByteRate { get; set; }
        public Image SmallAblum { get; set; }

        public SongsInfo(string? fPath)
        {
            if (fPath == null) return;
            SetSongInfo(fPath);
            SetAlbumArt(fPath);
        }

        private void SetSongInfo(string strPath)
        {
            try
            {
                Shell32.ShellClass sh = new Shell32.ShellClass();
                Shell32.Folder dir = sh.NameSpace(Path.GetDirectoryName(strPath));
                Shell32.FolderItem item = dir.ParseName(Path.GetFileName(strPath));

                FileName = dir.GetDetailsOf(item, 0).Split('.')[0];
                FilePath = strPath;

                Filesize = dir.GetDetailsOf(item, 1);
                if (Filesize == string.Empty)
                    Filesize = "未知";

                Artist = dir.GetDetailsOf(item, 13);
                if (Artist == string.Empty)
                    Artist = "未知艺术家";

                Album = dir.GetDetailsOf(item, 14);
                if (Album == string.Empty)
                    Album = "未知";

                Year = dir.GetDetailsOf(item, 15);
                if (Year == string.Empty)
                    Year = "未知";

                OriginName = dir.GetDetailsOf(item, 21);
                if (OriginName == string.Empty)
                    OriginName = FileName;

                Duration = dir.GetDetailsOf(item, 27);
                if (Duration == string.Empty)
                    Duration = "未知";

                ByteRate = dir.GetDetailsOf(item, 28);
                if (ByteRate == string.Empty)
                    ByteRate = "未知";

                //for (int i = -1; i < 50; i++)
                //{
                //    string str = dir.GetDetailsOf(item, i);
                //    Console.WriteLine(i + " && " + str);
                //}
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SetAlbumArt(string strPath)
        {
            if(strPath != "" && strPath != null)
            {
                TagLib.File File = TagLib.File.Create(strPath);
                if (File.Tag.Pictures.Length > 0)
                {
                    var bin = (byte[])(File.Tag.Pictures[0].Data.Data);
                    AlbumImage = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(900, 900, null, IntPtr.Zero);
                    AlbumImage = Cut((Bitmap)AlbumImage, 20, 215, 877, 530);
                    //albumImage = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(640, 360, null, IntPtr.Zero);
                    SmallAblum = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(64, 64, null, IntPtr.Zero);
                    return;
                }
            }

            AlbumImage = Properties.Resources.defaultAlbum;
            SmallAblum = Properties.Resources.defaultSmallAlbum;
        }

        public static Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                using Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }
    }
}
