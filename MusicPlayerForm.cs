using KControlsLib;
using Null.MciPlayer;
using System;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace MusicPlayer
{
    enum PlayMode
    {
        ListLoop,
        Shuffle,
        SingleLoop
    }

    enum CurrentPage
    {
        Local,
        Favourite
    }

    public partial class MusicPlayerForm : Form
    {
        private CurrentPage currentPage = CurrentPage.Local;
        private PlayMode playMode = PlayMode.ListLoop;

        private SongsInfo currSelectedSong = new(null);       //用于右键菜单
        private MciPlayer? player = null;
        private MciPlayer? playerStore = null;
        private bool playerStoreValid = false;                // 是否已经保存了一个有效的 mci player

        private string localSongsFilePath = Application.StartupPath + "\\songListHistory.txt";
        private string favoriteSongsFilePath = Application.StartupPath + "\\favoriteSongsHistory.txt";
        private List<SongsInfo> searchList = new List<SongsInfo>();                 //用于搜索功能
        private List<SongsInfo> localSongsList = new List<SongsInfo>();             //本地音乐List
        private List<SongsInfo> favoriteSongsList = new List<SongsInfo>();          //收藏音乐List
        private int[] randomList = new int[1];                                      // 随机播放序列
        private int randomListIndex = 0;                                            // 序列索引
        private int jumpSongIndex;                                                  // 手动选中需要在随机过程中跳过的歌曲
        private int currentSongTotalMS = 0;                                         // 当前歌曲总长度毫秒，非MCI取得

        #region 构造函数、事件绑定
        public MusicPlayerForm()
        {
            InitializeComponent();

            this.Load += MusicPlayerForm_Load;
            this.Shown += MusicPlayerForm_Shown;
            this.FormClosed += this.MusicPlayerForm_FormClosed;

            this.btnPrev.ImageNormal = Properties.Resources.prev;
            this.btnPrev.ImageDown = Properties.Resources.prevh;
            this.btnPrev.HoverText = "上一首";
            this.btnPrev.Click += BtnPrev_Click;

            this.btnPlayPause.ImageNormal = Properties.Resources.play;
            this.btnPlayPause.ImageNormalHover = Properties.Resources.playh;
            this.btnPlayPause.ImageDown = Properties.Resources.pause;
            this.btnPlayPause.ImageDownHover = Properties.Resources.pauseh;
            this.btnPlayPause.HoverText = "播放";
            this.btnPlayPause.HoverTextToggle = "暂停";
            this.btnPlayPause.ToggleMode = true;
            this.btnPlayPause.Click += BtnPlayPause_Click;

            this.btnNext.ImageNormal = Properties.Resources.next;
            this.btnNext.ImageDown = Properties.Resources.nexth;
            this.btnNext.HoverText = "下一首";
            this.btnNext.Click += BtnNext_Click;

            this.btnLoop.ImageNormal = Properties.Resources.sequence;
            this.btnLoop.HoverText = "循环方式";
            this.btnLoop.Click += BtnLoop_Click;

            this.btnVolume.ImageNormal = Properties.Resources.volume;
            this.btnVolume.ImageDown = Properties.Resources.mute;
            this.btnVolume.HoverText = "静音";
            this.btnVolume.HoverTextToggle = "解除静音";
            this.btnVolume.ToggleMode = true;
            this.btnVolume.Click += BtnVolume_Click;
            this.btnVolume.MouseEnter += BtnVolume_MouseEnter;
            this.btnVolume.MouseLeave += BtnVolume_MouseLeave;

            this.btnMinium.ImageNormal = Properties.Resources.min;
            this.btnMinium.ImageNormalHover = Properties.Resources.minh;
            this.btnMinium.HoverText = "最小化";
            this.btnMinium.Click += BtnMinium_Click;

            this.btnClose.ImageNormal = Properties.Resources.close;
            this.btnClose.ImageNormalHover = Properties.Resources.closeh;
            this.btnClose.HoverText = "关闭";
            this.btnClose.Click += BtnClose_Click;

            this.paTitle.MouseDown += PaTitle_MouseDown;
            this.paTitle.MouseUp += PaTitle_MouseUp;
            this.paTitle.MouseMove += PaTitle_MouseMove;
            this.lblTitle.MouseDown += PaTitle_MouseDown;
            this.lblTitle.MouseUp += PaTitle_MouseUp;
            this.lblTitle.MouseMove += PaTitle_MouseMove;

            this.txtSearchSongName.Enter += TxtSearchSongName_Enter;
            this.txtSearchSongName.Leave += TxtSearchSongName_Leave;
            this.txtSearchSongName.TextChanged += TxtSearchSongName_TextChanged;

            SetPropertyInfo(this.lvSongList, "DoubleBuffered", true);
            SetPropertyInfo(this.lvSongList, "OptimizedDoubleBuffer ", true);
            SetPropertyInfo(this.lvSongList, "AllPaintingInWmPaint", true);
            this.lvSongList.DrawColumnHeader += LvSongList_DrawColumnHeader;
            this.lvSongList.DrawSubItem += LvSongList_DrawSubItem;
            this.lvSongList.DoubleClick += LvSongList_DoubleClick;
            this.lvSongList.MouseDown += LvSongList_MouseDown;

            // 因为MCI对某些mp3取总长度、取当前位置都有问题，进度条拖动功能关闭
            //this.tkbProgress.MouseDown += TkbProgress_MouseDown;
            //this.tkbProgress.MouseUp += TkbProgress_MouseUp;

            this.tkbVol.MouseHover += TkbVol_MouseHover;
            this.tkbVol.MouseLeave += BtnVolume_MouseLeave;
            this.tkbVol.Scroll += TkbVol_Scroll;

            this.timerPlay.Tick += TimerPlay_Tick;
        }

        private static void SetPropertyInfo(System.Windows.Forms.Control control, string property, bool value)
        {
            System.Reflection.PropertyInfo? controlProperty = typeof(System.Windows.Forms.Control)
                .GetProperty(property, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (controlProperty == null) return;
            controlProperty.SetValue(control, value, null);
        }
        #endregion

        #region 外部调用函数
        public bool StandAloneMode { get; set; } = true;
        public bool Invaded { get; set; } = false;
        public void Play(string file, bool repeat = false)
        {
            player = new MciPlayer(file);
            player.Open();
            player.Volume(tkbVol.Value);
            player.Play(repeat);
        }
        public void Stop()
        {
            player?.Stop();
        }
        public void Pause()
        {
            player?.Pause();
        }
        public void Resume()
        {
            player?.Resume();
        }
        public void ControlsEnable(bool enable)
        {
            if (this.InvokeRequired)
            {
                Action<bool> d = new Action<bool>(ControlsEnable);
                this.Invoke(d, enable);
            }
            else
            {
                if (enable)
                {
                    // 关闭定时器
                    this.timerPlay.Stop();
                    // 禁止按钮、显示Banner
                    this.tkbProgress.Enabled = false;
                    this.btnPrev.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnPlayPause.Enabled = false;
                    this.lvSongList.DoubleClick -= LvSongList_DoubleClick;
                    this.lblInvade.Visible = true;
                }
                else
                {
                    // 允许按钮、隐藏Banner
                    this.tkbProgress.Enabled = true;
                    this.btnPrev.Enabled = true;
                    this.btnNext.Enabled = true;
                    this.btnPlayPause.Enabled = true;
                    this.lvSongList.DoubleClick += LvSongList_DoubleClick;
                    this.lblInvade.Visible = false;
                    // 仅当之前player有效，才开启定时器
                    if (playerStoreValid) this.timerPlay.Start();
                }
            }
        }
        public void InvadeMode(bool enable)
        {
            // 操作控件
            this.ControlsEnable(enable);
            // 外部控制模式开关
            if (enable)
            {
                // 保存player
                playerStoreValid = false;
                if (player?.GetState() == MciPlayer.PlaybackState.Playing)
                {
                    this.playerStore = player;
                    player.Pause();
                    playerStoreValid = true;
                }
            }
            else
            {
                // 退出Invade模式，先停止当前player
                player?.Stop();
                player?.Close();
                // 如果之前存储player有效，恢复过来，并恢复播放
                if (playerStoreValid)
                {
                    this.player = this.playerStore;
                    this.player?.Resume();
                    playerStoreValid = false;
                }
            }
            // 标记
            this.Invaded = enable;
        }
        #endregion

        #region 标题栏拖动
        Point downPoint;

        private void PaTitle_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X, this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void PaTitle_MouseUp(object? sender, MouseEventArgs e)
        {
            lblTitle.ForeColor = Color.Gray;
        }

        private void PaTitle_MouseDown(object? sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
            lblTitle.ForeColor = Color.White;
        }
        #endregion

        #region 顶部菜单
        private void lblPlaylist_Click(object sender, EventArgs e)
        {
            currentPage = CurrentPage.Local;
            lblPlaylist.ForeColor = Color.White;
            lblFavourite.ForeColor = Color.Gray;

            AddSongsToListView(localSongsList);

            tsmiFavorite.Visible = true;
        }

        private void lblFavourite_Click(object sender, EventArgs e)
        {
            currentPage = CurrentPage.Favourite;
            lblPlaylist.ForeColor = Color.Gray;
            lblFavourite.ForeColor = Color.White;

            AddSongsToListView(favoriteSongsList);

            tsmiFavorite.Visible = false;
        }
        private void lblOpen_Click(object sender, EventArgs e)
        {
            this.odlgFile.Filter = "媒体文件|*.mp3;*.wav;*.wma;*.avi;*.mpg;*.asf;*.wmv";
            this.odlgFile.Multiselect = true;

            if (odlgFile.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < odlgFile.FileNames.Length; i++)
                {
                    string path = odlgFile.FileNames[i];
                    if (!HavePath(localSongsList, path))
                        localSongsList.Add(new SongsInfo(path));
                }
            }

            AddSongsToListView(localSongsList);
            SaveSongsListHistory(localSongsFilePath, localSongsList);
            UpdateSearchList();
        }
        private void lblClear_Click(object sender, EventArgs e)
        {
            switch (currentPage)
            {
                case CurrentPage.Local:
                    localSongsList.Clear();
                    SaveSongsListHistory(localSongsFilePath, localSongsList);
                    AddSongsToListView(localSongsList);
                    break;
                case CurrentPage.Favourite:
                    favoriteSongsList.Clear();
                    SaveSongsListHistory(favoriteSongsFilePath, favoriteSongsList);
                    AddSongsToListView(favoriteSongsList);
                    break;
            }
        }
        private bool HavePath(List<SongsInfo> songList, string path)
        {
            for (int i = 0; i < songList.Count; i++)
            {
                if (path == songList[i].FilePath)
                    return true;
            }
            return false;
        }
        private void AddSongsToListView(List<SongsInfo> songList)
        {
            lvSongList.BeginUpdate();
            lvSongList.Items.Clear();
            foreach (SongsInfo song in songList)
            {
                string[] songAry = new string[6];
                int currCount = lvSongList.Items.Count + 1;
                if (currCount < 10)
                    songAry[0] = "0" + currCount;
                else
                    songAry[0] = "" + currCount;
                songAry[1] = song.FileName;
                songAry[2] = song.Artist;
                songAry[3] = song.Album;
                songAry[4] = song.Duration;
                songAry[5] = song.Filesize;
                ListViewItem lvItem = new ListViewItem(songAry);
                lvItem.SubItems.Add(song.FilePath);
                lvSongList.Items.Add(lvItem);
                //WMPLib.IWMPMedia media = AxWmp.newMedia(song.FilePath);
                //AxWmp.currentPlaylist.appendItem(media);
            }
            lvSongList.EndUpdate();

            // 不让显示底部Scrollbar，以后再改，kiilii
            if (lvSongList.Items.Count > 21)
                lvSongList.Columns[0].Width = 62 - SystemInformation.VerticalScrollBarWidth;
            else
                lvSongList.Columns[0].Width = 62;
        }
        #endregion

        #region 播放列表右键菜单(收藏音乐、删除音乐、打开文件位置)
        /*
         * 菜单——收藏音乐
         */
        private void tsmiFavorite_Click(object sender, EventArgs e)
        {
            foreach (SongsInfo song in favoriteSongsList)
            {
                if (currSelectedSong.FilePath == song.FilePath)
                    return;
            }

            favoriteSongsList.Add(new SongsInfo(currSelectedSong.FilePath));
            SaveSongsListHistory(favoriteSongsFilePath, favoriteSongsList);
        }

        /*
         * 菜单——删除音乐
         */
        private void tsmiRemoveSongFromList_Click(object sender, EventArgs e)
        {
            DeleteSongFormList delForm = new DeleteSongFormList(currSelectedSong.FilePath);
            delForm.StartPosition = FormStartPosition.CenterParent;
            if (delForm.ShowDialog() == DialogResult.OK)
            {
                int removeIndex = lvSongList.SelectedItems[0].Index;
                if (currentPage == CurrentPage.Local)
                {
                    localSongsList.RemoveAt(removeIndex);
                    SaveSongsListHistory(localSongsFilePath, localSongsList);
                    AddSongsToListView(localSongsList);
                }
                else if (currentPage == CurrentPage.Favourite)
                {
                    favoriteSongsList.RemoveAt(removeIndex);
                    SaveSongsListHistory(favoriteSongsFilePath, favoriteSongsList);
                    AddSongsToListView(favoriteSongsList);
                }

                UpdateSearchList();
            }
        }

        /*
         * 菜单——打开文件位置
         */
        private void tsmiOpenFilePath_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"Explorer.exe", "/select,\"" + currSelectedSong.FilePath + "\"");
        }
        #endregion

        #region 读写播放列表历史记录
        /*
         * 保存历史记录到本地文件
         */
        private void SaveSongsListHistory(string savePath, List<SongsInfo> songsList)
        {
            string saveString = "";
            for (int i = 0; i < songsList.Count; i++)
            {
                saveString += songsList[i].FilePath + "};{";
            }
            File.WriteAllText(savePath, saveString);
        }

        /*
         * 读取历史记录到本地文件
         */
        private List<SongsInfo> ReadHistorySongsList(string filePath)
        {
            List<SongsInfo> resSongList = new List<SongsInfo>();
            string readString = "";
            if (File.Exists(filePath))
            {
                readString = File.ReadAllText(filePath);
                if (readString != "")
                {
                    string[] arr = readString.Split(new string[] { "};{" }, StringSplitOptions.None);
                    foreach (string path in arr)
                    {
                        if (path != null && path != "" && File.Exists(path))
                        {
                            resSongList.Add(new SongsInfo(path));
                        }
                    }
                }
            }
            else
                File.Create(filePath);

            return resSongList;
        }
        #endregion

        #region 播放列表
        /*
         * 播放列表重绘
         */
        private void LvSongList_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            int index = e.ColumnIndex;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(27, 27, 25)), e.Bounds);
            TextRenderer.DrawText(e.Graphics, lvSongList.Columns[index].Text, new Font("微软雅黑", 9, FontStyle.Regular), e.Bounds, Color.Gray, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            Pen pen = new Pen(Color.FromArgb(34, 35, 39), 2);
            Point p = new Point(e.Bounds.Left - 1, e.Bounds.Top + 1);
            Size s = new Size(e.Bounds.Width, e.Bounds.Height - 2);
            Rectangle r = new Rectangle(p, s);
            e.Graphics.DrawRectangle(pen, r);
        }
        private void LvSongList_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            //Console.WriteLine("DrawSub: {0}, state {1}", e.ItemIndex, e.ItemState);
            if (e.ItemIndex == -1) return;
            // 隔行底色略不同
            if (e.ItemIndex % 2 == 0)
            {
                e.SubItem!.BackColor = Color.FromArgb(27, 29, 32);
                e.DrawBackground();
            }
            // 音乐标题白色，其他灰色
            e.SubItem!.ForeColor = e.ColumnIndex == 1 ? Color.White : Color.Gray;
            // 选中换底色
            if ((e.ItemState & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(46, 47, 51)))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }
            // 绘制文字
            if (!string.IsNullOrEmpty(e.SubItem.Text))
            {
                this.DrawText(e, e.Graphics, e.Bounds, 2);
            }
        }
        private void DrawText(DrawListViewSubItemEventArgs e, Graphics g, Rectangle r, int paddingLeft)
        {
            TextFormatFlags flags = GetFormatFlags(e.Header!.TextAlign);

            r.X += 1 + paddingLeft;//重绘图标时，文本右移
            TextRenderer.DrawText(g,
                                  e.SubItem!.Text,
                                  e.SubItem.Font,
                                  r,
                                  e.SubItem.ForeColor,
                                  flags);
        }
        private TextFormatFlags GetFormatFlags(HorizontalAlignment align)
        {
            TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
            switch (align)
            {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    break;
            }
            return flags;
        }
        private void UpdateCurrentSongToUI(int index)
        {
            // 设定ListView
            lvSongList.Items[index].Focused = true;
            lvSongList.Items[index].EnsureVisible();
            lvSongList.Items[index].Selected = true;
            lvSongList.Refresh();
            // 设定头像、名字等
            List<SongsInfo> lst = currentPage switch
            {
                CurrentPage.Local => localSongsList,
                CurrentPage.Favourite => favoriteSongsList,
                _ => throw new NotImplementedException(),
            };
            pbAlbum.Image = lst[index].SmallAblum;
            lblArtistName.Text = lst[index].Artist;
            lblMusicName.Text = lst[index].OriginName;

            // 不用MCI取总长度，VBR的会有问题
            var si = new SongsInfo(lvSongList.Items[index].SubItems[6].Text);
            Console.WriteLine("Duration {0} bit {1}", si.Duration, si.ByteRate);
            var parts = si.Duration.Split(':');
            lblEndTime.Text = si.Duration[3..];
            currentSongTotalMS = Convert.ToInt32(parts[0]) * 3600000 + Convert.ToInt32(parts[1]) * 60000 + Convert.ToInt32(parts[2]) * 1000;
            // 进度条还是用MCI取的值
            tkbProgress.Maximum = player!.GetLength();
        }
        /*
         * 播放列表双击事件
         */
        private void LvSongList_DoubleClick(object? sender, EventArgs e)
        {
            Console.WriteLine(lvSongList.SelectedItems[0].Index);

            int currIndex = lvSongList.SelectedItems[0].Index;

            string songFilePath = lvSongList.Items[currIndex].SubItems[6].Text;
            Console.WriteLine("LvSongList_DoubleClick " + songFilePath);
            if (player?.DevicePath == songFilePath)
            {
                //选中歌曲为正在播放的歌曲
                this.PlayPauseSwitch();
            }
            else
            {
                //选中的为其他歌曲
                this.BuildRandomList(lvSongList.Items.Count);
                jumpSongIndex = currIndex;
                this.PlayNewIndex(currIndex);
                this.UpdateCurrentSongToUI(currIndex);
            }
        }
        private void LvSongList_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem lvi = lvSongList.GetItemAt(e.X, e.Y);
                if (lvi != null)
                {
                    cmsSongListMenu.Visible = true;
                    currSelectedSong = new SongsInfo(lvi.SubItems[6].Text);
                    cmsSongListMenu.Show(Cursor.Position);
                }
                else
                    cmsSongListMenu.Close();
            }
        }
        #endregion

        #region 列表搜索
        private void TxtSearchSongName_Enter(object? sender, EventArgs e)
        {
            if (txtSearchSongName.Text == "输入要搜索的歌曲名")
                txtSearchSongName.Text = "";
        }

        private void TxtSearchSongName_Leave(object? sender, EventArgs e)
        {
            if (txtSearchSongName.Text == "")
                txtSearchSongName.Text = "输入要搜索的歌曲名";
        }

        private void TxtSearchSongName_TextChanged(object? sender, EventArgs e)
        {
            //初始化
            if (txtSearchSongName.Text == "输入要搜索的歌曲名")
            {
                switch (currentPage)
                {
                    case CurrentPage.Local:
                        AddSongsToListView(localSongsList);
                        break;
                    case CurrentPage.Favourite:
                        AddSongsToListView(favoriteSongsList);
                        break;
                }
                return;
            }
            else if (txtSearchSongName.Text != "")
            {
                List<SongsInfo> resultList = new List<SongsInfo>();

                Dictionary<string, SongsInfo> resultDic = new Dictionary<string, SongsInfo>();
                Regex r = new Regex(Regex.Escape(txtSearchSongName.Text), RegexOptions.IgnoreCase);

                for (int i = 0; i < localSongsList.Count; i++)  // 本地列表内查找
                {
                    Match m = r.Match(localSongsList[i].FileName);
                    if (m.Success)
                    {
                        resultDic.Add(localSongsList[i].FilePath, localSongsList[i]);
                    }
                }

                for (int i = 0; i < favoriteSongsList.Count; i++)   // 收藏列表内查找
                {
                    Match m = r.Match(favoriteSongsList[i].FileName);
                    if (m.Success && !resultDic.ContainsKey(favoriteSongsList[i].FilePath))
                    {
                        resultDic.Add(favoriteSongsList[i].FilePath, favoriteSongsList[i]);
                    }
                }

                if (resultDic.Count > 0)    // 如果找到，放入
                {
                    List<SongsInfo> resList = new List<SongsInfo>();
                    foreach (SongsInfo song in resultDic.Values)
                    {
                        resList.Add(song);
                    }
                    AddSongsToListView(resList);
                }
                else
                {
                    lvSongList.Items.Clear();
                }
            }
        }

        private void UpdateSearchList()
        {
            searchList = new List<SongsInfo>();
            for (int i = 0; i < lvSongList.Items.Count; i++)
            {
                searchList.Add(new SongsInfo(lvSongList.Items[i].SubItems[6].Text));
            }
        }
        #endregion

        #region 播放核心,MCIPlayer操作

        /// <summary>
        /// 切换播放状态，如果确实切换了，返回true
        /// </summary>
        /// <returns></returns>
        private bool PlayPauseSwitch()
        {
            if (player?.GetState() == MciPlayer.PlaybackState.Playing)
            {
                player.Pause();
                timerPlay.Stop();
                btnPlayPause.Down = false;
                return true;
            }
            else if (player?.GetState() == MciPlayer.PlaybackState.Paused)
            {
                player.Resume();
                timerPlay.Start();
                btnPlayPause.Down = true;
                return true;
            }
            return false;
        }

        /*
         * 播放指定的歌曲
         * index：播放列表中，歌曲的序号
         */
        private void PlayNewIndex(int index)
        {
            Console.WriteLine("PlayNewIndex {0}:{1}", index, lvSongList.Items[index].SubItems[6].Text);
            string filePath = lvSongList.Items[index].SubItems[6].Text;
            // 直接重开播放
            player?.Close();
            player = new MciPlayer(filePath, this);
            player.Open();
            player.Volume(tkbVol.Value);
            player.Play();
            timerPlay.Start();

            // 按钮
            btnPlayPause.Down = true;
        }

        /*
         * 重置播放器状态信息
         */
        private void ReloadStatus()
        {
            //设置专辑封面为默认
            pbAlbum.Image = Properties.Resources.defaultSmallAlbum;
            lblMusicName.Text = "未知歌曲";
            lblArtistName.Text = "未知艺术家";
            lblCurrTime.Text = "00:00";
            lblEndTime.Text = "00:00";
            tkbVol.Value = tkbVol.Maximum / 2;
            tkbProgress.Value = 0;
            if (lvSongList.Items.Count > 0 && lvSongList.SelectedItems.Count == 0)
            {
                lvSongList.Items[0].Selected = true;//设定选中            
                lvSongList.Items[0].EnsureVisible();//保证可见
                lvSongList.Items[0].Focused = true;
            }
        }

        private void EndOfPlayCallback()     //播放歌曲结束回调
        {
            /* 列表没文件直接退 */
            if (lvSongList.Items.Count == 0) return;

            /* 根据播放文件名计算index */
            int currIndex = 0;
            for (int i = 0; i < lvSongList.Items.Count; i++)
            {
                if (player!.DevicePath == lvSongList.Items[i].SubItems[6].Text)
                {
                    currIndex = i;
                    break;
                }
            }
            /* 根据播放模式选择下一首 */
            switch (playMode)
            {
                case PlayMode.ListLoop:
                    if (currIndex != lvSongList.Items.Count - 1)
                        currIndex += 1;
                    else
                        currIndex = 0;
                    break;
                case PlayMode.SingleLoop:
                    //do nothing
                    break;
                case PlayMode.Shuffle:
                    if (randomList[randomListIndex] == jumpSongIndex)   //匹配到需要跳过的歌曲
                    {
                        randomListIndex++;
                    }
                    if (randomListIndex == randomList.Length)
                    {
                        this.BuildRandomList(lvSongList.Items.Count);   //重新生成随机序列
                        jumpSongIndex = -1;                             //第二轮开始 播放所有歌曲 不跳过
                    }
                    currIndex = randomList[randomListIndex++];
                    if (randomListIndex == randomList.Length)
                    {
                        this.BuildRandomList(lvSongList.Items.Count);   //重新生成随机序列
                        jumpSongIndex = -1;                             //第二轮开始 播放所有歌曲 不跳过
                    }
                    break;
            }
            /* 开始播放 */
            this.PlayNewIndex(currIndex);
            this.UpdateCurrentSongToUI(currIndex);
        }
        private void BuildRandomList(int songListCount)
        {
            randomListIndex = 0;
            randomList = new int[songListCount];
            //初始化序列
            for (int i = 0; i < songListCount; i++)
            {
                randomList[i] = i;
            }
            //随机序列
            for (int i = songListCount - 1; i >= 0; i--)
            {
                Random r = new Random(Guid.NewGuid().GetHashCode());
                int j = r.Next(0, songListCount - 1);
                (randomList[i], randomList[j]) = (randomList[j], randomList[i]);
            }
            //输出序列
            //for (int i = 0; i < songListCount; i++)
            //{
            //    Console.Write(randomList[i] + " ");
            //}
            //Console.WriteLine(" ");
        }
        #endregion

        #region 按键方法
        private void BtnPrev_Click(object? sender, EventArgs e)
        {
            if (lvSongList.SelectedItems.Count == 0) return;
            int currIndex = lvSongList.SelectedItems[0].Index;
            if (currIndex > 0)
            {
                currIndex -= 1;
            }
            else
            {
                currIndex = lvSongList.Items.Count - 1;
            }
            this.PlayNewIndex(currIndex);
            this.UpdateCurrentSongToUI(currIndex);
        }
        private void BtnNext_Click(object? sender, EventArgs e)
        {
            if (lvSongList.SelectedItems.Count == 0) return;

            int currIndex = lvSongList.SelectedItems[0].Index;
            if (currIndex < lvSongList.Items.Count - 1)
            {
                currIndex += 1;
            }
            else
            {
                currIndex = 0;
            }
            this.PlayNewIndex(currIndex);
            this.UpdateCurrentSongToUI(currIndex);
        }
        private void BtnPlayPause_Click(object? sender, EventArgs e)
        {
            if (lvSongList.Items.Count == 0) return;
            // 尝试暂停、恢复；如果成功则退出
            if (this.PlayPauseSwitch()) return;
            // 当前没有播放，重新开始
            // 有选中，按选中的开始; 无选中，按设定播放模式来
            int newIndex = 0;
            if (lvSongList.SelectedItems.Count > 0)
            {
                newIndex = lvSongList.SelectedItems[0].Index;
            }
            switch (playMode)
            {
                case PlayMode.ListLoop:
                case PlayMode.SingleLoop:
                    this.PlayNewIndex(newIndex);
                    this.UpdateCurrentSongToUI(newIndex);
                    break;
                case PlayMode.Shuffle:
                    this.BuildRandomList(lvSongList.Items.Count);
                    jumpSongIndex = newIndex;
                    this.PlayNewIndex(randomList[newIndex]);
                    this.UpdateCurrentSongToUI(randomList[newIndex]);
                    break;
            }
        }
        private void BtnLoop_Click(object? sender, EventArgs e)
        {
            switch (playMode)
            {
                case PlayMode.ListLoop:
                    this.playMode = PlayMode.Shuffle;
                    this.btnLoop.ImageNormal = Properties.Resources.random;
                    break;
                case PlayMode.Shuffle:
                    this.playMode = PlayMode.SingleLoop;
                    this.btnLoop.ImageNormal = Properties.Resources.repeat;
                    break;
                case PlayMode.SingleLoop:
                    this.playMode = PlayMode.ListLoop;
                    this.btnLoop.ImageNormal = Properties.Resources.sequence;
                    break;
            }
        }

        private void BtnMinium_Click(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void BtnClose_Click(object? sender, EventArgs e)
        {
            if (StandAloneMode)
            {
                this.Close();
            }
            else
            {
                this.Hide();
            }
        }
        #endregion

        #region 播放Trackbar
        private void TimerPlay_Tick(object? sender, EventArgs e)
        {
            //设置当前播放时间
            int ms = player!.GetPosition(); // 这个取值又是准确的
            lblCurrTime.Text = String.Format("{0:D2}:{1:D2}", ms / 60000, (ms / 1000) % 60);

            //设置滑动条值，采用妥协方式
            double corrected = ms;
            corrected = corrected * tkbProgress.Maximum / currentSongTotalMS;
            if ((int)corrected <= tkbProgress.Maximum)
                tkbProgress.Value = (int)corrected;

        }
        private void TkbProgress_MouseDown(object? sender, EventArgs e)
        {
            timerPlay.Stop();
        }
        private void TkbProgress_MouseUp(object? sender, EventArgs e)
        {
            if (player == null) return;
            switch (player?.GetState())
            {
                case MciPlayer.PlaybackState.Playing:
                    player?.Seek(tkbProgress.Value);
                    player?.Play();
                    timerPlay.Start();
                    break;
                case MciPlayer.PlaybackState.Stopped:
                case MciPlayer.PlaybackState.Paused:
                    player?.Seek(tkbProgress.Value);
                    this.btnPlayPause.Down = false;
                    break;
            }
        }
        #endregion

        #region 音量控制按钮和Trackbar
        private int preVolume = 500;
        private void BtnVolume_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("btnVolume_Click");
            if (btnVolume.Down)
            {
                preVolume = tkbVol.Value;
                tkbVol.Value = tkbVol.Minimum;
            }
            else
            {
                tkbVol.Value = preVolume;
            }
            player?.Volume(tkbVol.Value);
        }
        private void BtnVolume_MouseEnter(object? sender, EventArgs e)
        {
            this.tkbVol.Visible = true;
            this.tkbVol.BringToFront();
        }
        private void BtnVolume_MouseLeave(object? sender, EventArgs e)
        {
            Rectangle r = this.tkbVol.Bounds;
            r.Height += 30; // 两个控件之间有点距离
            if (!r.Contains(this.PointToClient(Control.MousePosition)))
                this.tkbVol.Visible = false;
        }
        private void TkbVol_Scroll(object? sender, EventArgs e)
        {
            if (tkbVol.Value == tkbVol.Minimum)  // 拖到最低改变状态
            {
                btnVolume.Down = true;
            }
            else if (btnVolume.Down) // 静音状态移动则解除静音
            {
                btnVolume.Down = false;
            }
            player?.Volume(tkbVol.Value);
        }
        private void TkbVol_MouseHover(object? sender, EventArgs e)
        {
            new ToolTip().SetToolTip(this, $"音量:{tkbVol.Value}");
        }
        #endregion

        #region 窗体Notify、Load、Shown、Closed事件
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MciEventCode.MM_MCINOTIFY:
                    if (m.WParam.ToInt32() == MciEventCode.MCI_NOTIFY_SUCCESSFUL)
                    {
                        this.EndOfPlayCallback();
                    }
                    Console.WriteLine("MM_MCINOTIFY " + m.WParam + " " + m.LParam);
                    break;
            }
            base.DefWndProc(ref m);
        }

        /*
         * 设置各种初始状态
         */
        private void MusicPlayerForm_Load(object? sender, EventArgs e)
        {
            //重置播放器状态信息
            ReloadStatus();
            //读取播放器列表历史记录
            localSongsList = ReadHistorySongsList(localSongsFilePath);
            favoriteSongsList = ReadHistorySongsList(favoriteSongsFilePath);
            // 显示本地文件列表
            AddSongsToListView(localSongsList);
        }

        /*
         * 设置任务栏缩略图的属性与绑定事件 
         */
        private void MusicPlayerForm_Shown(object? sender, EventArgs e)
        {
            ////暂停按钮
            //ttbbtnPlayPause = new ThumbnailToolbarButton(Properties.Resources.播放icon, "播放");
            //ttbbtnPlayPause.Enabled = true;
            //ttbbtnPlayPause.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(btnPlay_Click);

            ////上一首按钮
            //ttbbtnPre = new ThumbnailToolbarButton(Properties.Resources.上一首icon, "上一首");
            //ttbbtnPre.Enabled = true;
            //ttbbtnPre.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(btnBack_Click);

            ////下一首按钮
            //ttbbtnNext = new ThumbnailToolbarButton(Properties.Resources.下一首icon, "下一首");
            //ttbbtnNext.Enabled = true;
            //ttbbtnNext.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(BtnNext_Click);
            //TaskbarManager.Instance.ThumbnailToolbars.AddButtons(this.Handle, ttbbtnPre, ttbbtnPlayPause, ttbbtnNext);

            //裁剪显示略缩图
            //坐标值为多个父容器相对的位置坐标累加所得
            //Point p = new Point(4, 558);
            //TaskbarManager.Instance.TabbedThumbnail.SetThumbnailClip(this.Handle, new Rectangle(p, pbSmallAlbum.Size));
        }

        /*
         * 窗体关闭事件 
         */
        private void MusicPlayerForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            Application.Exit();
            this.Dispose();
        }
        #endregion
    }
}
