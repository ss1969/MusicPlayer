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

        private SongsInfo currSelectedSong = new(null);       //�����Ҽ��˵�
        private MciPlayer? player = null;
        private MciPlayer? playerStore = null;
        private bool playerStoreValid = false;                // �Ƿ��Ѿ�������һ����Ч�� mci player

        private string localSongsFilePath = Application.StartupPath + "\\songListHistory.txt";
        private string favoriteSongsFilePath = Application.StartupPath + "\\favoriteSongsHistory.txt";
        private List<SongsInfo> searchList = new List<SongsInfo>();                 //������������
        private List<SongsInfo> localSongsList = new List<SongsInfo>();             //��������List
        private List<SongsInfo> favoriteSongsList = new List<SongsInfo>();          //�ղ�����List
        private int[] randomList = new int[1];                                      // �����������
        private int randomListIndex = 0;                                            // ��������
        private int jumpSongIndex;                                                  // �ֶ�ѡ����Ҫ����������������ĸ���
        private int currentSongTotalMS = 0;                                         // ��ǰ�����ܳ��Ⱥ��룬��MCIȡ��

        #region ���캯�����¼���
        public MusicPlayerForm()
        {
            InitializeComponent();

            this.Load += MusicPlayerForm_Load;
            this.Shown += MusicPlayerForm_Shown;
            this.FormClosed += this.MusicPlayerForm_FormClosed;

            this.btnPrev.ImageNormal = Properties.Resources.prev;
            this.btnPrev.ImageDown = Properties.Resources.prevh;
            this.btnPrev.HoverText = "��һ��";
            this.btnPrev.Click += BtnPrev_Click;

            this.btnPlayPause.ImageNormal = Properties.Resources.play;
            this.btnPlayPause.ImageNormalHover = Properties.Resources.playh;
            this.btnPlayPause.ImageDown = Properties.Resources.pause;
            this.btnPlayPause.ImageDownHover = Properties.Resources.pauseh;
            this.btnPlayPause.HoverText = "����";
            this.btnPlayPause.HoverTextToggle = "��ͣ";
            this.btnPlayPause.ToggleMode = true;
            this.btnPlayPause.Click += BtnPlayPause_Click;

            this.btnNext.ImageNormal = Properties.Resources.next;
            this.btnNext.ImageDown = Properties.Resources.nexth;
            this.btnNext.HoverText = "��һ��";
            this.btnNext.Click += BtnNext_Click;

            this.btnLoop.ImageNormal = Properties.Resources.sequence;
            this.btnLoop.HoverText = "ѭ����ʽ";
            this.btnLoop.Click += BtnLoop_Click;

            this.btnVolume.ImageNormal = Properties.Resources.volume;
            this.btnVolume.ImageDown = Properties.Resources.mute;
            this.btnVolume.HoverText = "����";
            this.btnVolume.HoverTextToggle = "�������";
            this.btnVolume.ToggleMode = true;
            this.btnVolume.Click += BtnVolume_Click;
            this.btnVolume.MouseEnter += BtnVolume_MouseEnter;
            this.btnVolume.MouseLeave += BtnVolume_MouseLeave;

            this.btnMinium.ImageNormal = Properties.Resources.min;
            this.btnMinium.ImageNormalHover = Properties.Resources.minh;
            this.btnMinium.HoverText = "��С��";
            this.btnMinium.Click += BtnMinium_Click;

            this.btnClose.ImageNormal = Properties.Resources.close;
            this.btnClose.ImageNormalHover = Properties.Resources.closeh;
            this.btnClose.HoverText = "�ر�";
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

            // ��ΪMCI��ĳЩmp3ȡ�ܳ��ȡ�ȡ��ǰλ�ö������⣬�������϶����ܹر�
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

        #region �ⲿ���ú���
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
                    // �رն�ʱ��
                    this.timerPlay.Stop();
                    // ��ֹ��ť����ʾBanner
                    this.tkbProgress.Enabled = false;
                    this.btnPrev.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnPlayPause.Enabled = false;
                    this.lvSongList.DoubleClick -= LvSongList_DoubleClick;
                    this.lblInvade.Visible = true;
                }
                else
                {
                    // ����ť������Banner
                    this.tkbProgress.Enabled = true;
                    this.btnPrev.Enabled = true;
                    this.btnNext.Enabled = true;
                    this.btnPlayPause.Enabled = true;
                    this.lvSongList.DoubleClick += LvSongList_DoubleClick;
                    this.lblInvade.Visible = false;
                    // ����֮ǰplayer��Ч���ſ�����ʱ��
                    if (playerStoreValid) this.timerPlay.Start();
                }
            }
        }
        public void InvadeMode(bool enable)
        {
            // �����ؼ�
            this.ControlsEnable(enable);
            // �ⲿ����ģʽ����
            if (enable)
            {
                // ����player
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
                // �˳�Invadeģʽ����ֹͣ��ǰplayer
                player?.Stop();
                player?.Close();
                // ���֮ǰ�洢player��Ч���ָ����������ָ�����
                if (playerStoreValid)
                {
                    this.player = this.playerStore;
                    this.player?.Resume();
                    playerStoreValid = false;
                }
            }
            // ���
            this.Invaded = enable;
        }
        #endregion

        #region �������϶�
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

        #region �����˵�
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
            this.odlgFile.Filter = "ý���ļ�|*.mp3;*.wav;*.wma;*.avi;*.mpg;*.asf;*.wmv";
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

            // ������ʾ�ײ�Scrollbar���Ժ��ٸģ�kiilii
            if (lvSongList.Items.Count > 21)
                lvSongList.Columns[0].Width = 62 - SystemInformation.VerticalScrollBarWidth;
            else
                lvSongList.Columns[0].Width = 62;
        }
        #endregion

        #region �����б��Ҽ��˵�(�ղ����֡�ɾ�����֡����ļ�λ��)
        /*
         * �˵������ղ�����
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
         * �˵�����ɾ������
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
         * �˵��������ļ�λ��
         */
        private void tsmiOpenFilePath_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"Explorer.exe", "/select,\"" + currSelectedSong.FilePath + "\"");
        }
        #endregion

        #region ��д�����б���ʷ��¼
        /*
         * ������ʷ��¼�������ļ�
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
         * ��ȡ��ʷ��¼�������ļ�
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

        #region �����б�
        /*
         * �����б��ػ�
         */
        private void LvSongList_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            int index = e.ColumnIndex;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(27, 27, 25)), e.Bounds);
            TextRenderer.DrawText(e.Graphics, lvSongList.Columns[index].Text, new Font("΢���ź�", 9, FontStyle.Regular), e.Bounds, Color.Gray, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

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
            // ���е�ɫ�Բ�ͬ
            if (e.ItemIndex % 2 == 0)
            {
                e.SubItem!.BackColor = Color.FromArgb(27, 29, 32);
                e.DrawBackground();
            }
            // ���ֱ����ɫ��������ɫ
            e.SubItem!.ForeColor = e.ColumnIndex == 1 ? Color.White : Color.Gray;
            // ѡ�л���ɫ
            if ((e.ItemState & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(46, 47, 51)))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }
            // ��������
            if (!string.IsNullOrEmpty(e.SubItem.Text))
            {
                this.DrawText(e, e.Graphics, e.Bounds, 2);
            }
        }
        private void DrawText(DrawListViewSubItemEventArgs e, Graphics g, Rectangle r, int paddingLeft)
        {
            TextFormatFlags flags = GetFormatFlags(e.Header!.TextAlign);

            r.X += 1 + paddingLeft;//�ػ�ͼ��ʱ���ı�����
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
            // �趨ListView
            lvSongList.Items[index].Focused = true;
            lvSongList.Items[index].EnsureVisible();
            lvSongList.Items[index].Selected = true;
            lvSongList.Refresh();
            // �趨ͷ�����ֵ�
            List<SongsInfo> lst = currentPage switch
            {
                CurrentPage.Local => localSongsList,
                CurrentPage.Favourite => favoriteSongsList,
                _ => throw new NotImplementedException(),
            };
            pbAlbum.Image = lst[index].SmallAblum;
            lblArtistName.Text = lst[index].Artist;
            lblMusicName.Text = lst[index].OriginName;

            // ����MCIȡ�ܳ��ȣ�VBR�Ļ�������
            var si = new SongsInfo(lvSongList.Items[index].SubItems[6].Text);
            Console.WriteLine("Duration {0} bit {1}", si.Duration, si.ByteRate);
            var parts = si.Duration.Split(':');
            lblEndTime.Text = si.Duration[3..];
            currentSongTotalMS = Convert.ToInt32(parts[0]) * 3600000 + Convert.ToInt32(parts[1]) * 60000 + Convert.ToInt32(parts[2]) * 1000;
            // ������������MCIȡ��ֵ
            tkbProgress.Maximum = player!.GetLength();
        }
        /*
         * �����б�˫���¼�
         */
        private void LvSongList_DoubleClick(object? sender, EventArgs e)
        {
            Console.WriteLine(lvSongList.SelectedItems[0].Index);

            int currIndex = lvSongList.SelectedItems[0].Index;

            string songFilePath = lvSongList.Items[currIndex].SubItems[6].Text;
            Console.WriteLine("LvSongList_DoubleClick " + songFilePath);
            if (player?.DevicePath == songFilePath)
            {
                //ѡ�и���Ϊ���ڲ��ŵĸ���
                this.PlayPauseSwitch();
            }
            else
            {
                //ѡ�е�Ϊ��������
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

        #region �б�����
        private void TxtSearchSongName_Enter(object? sender, EventArgs e)
        {
            if (txtSearchSongName.Text == "����Ҫ�����ĸ�����")
                txtSearchSongName.Text = "";
        }

        private void TxtSearchSongName_Leave(object? sender, EventArgs e)
        {
            if (txtSearchSongName.Text == "")
                txtSearchSongName.Text = "����Ҫ�����ĸ�����";
        }

        private void TxtSearchSongName_TextChanged(object? sender, EventArgs e)
        {
            //��ʼ��
            if (txtSearchSongName.Text == "����Ҫ�����ĸ�����")
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

                for (int i = 0; i < localSongsList.Count; i++)  // �����б��ڲ���
                {
                    Match m = r.Match(localSongsList[i].FileName);
                    if (m.Success)
                    {
                        resultDic.Add(localSongsList[i].FilePath, localSongsList[i]);
                    }
                }

                for (int i = 0; i < favoriteSongsList.Count; i++)   // �ղ��б��ڲ���
                {
                    Match m = r.Match(favoriteSongsList[i].FileName);
                    if (m.Success && !resultDic.ContainsKey(favoriteSongsList[i].FilePath))
                    {
                        resultDic.Add(favoriteSongsList[i].FilePath, favoriteSongsList[i]);
                    }
                }

                if (resultDic.Count > 0)    // ����ҵ�������
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

        #region ���ź���,MCIPlayer����

        /// <summary>
        /// �л�����״̬�����ȷʵ�л��ˣ�����true
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
         * ����ָ���ĸ���
         * index�������б��У����������
         */
        private void PlayNewIndex(int index)
        {
            Console.WriteLine("PlayNewIndex {0}:{1}", index, lvSongList.Items[index].SubItems[6].Text);
            string filePath = lvSongList.Items[index].SubItems[6].Text;
            // ֱ���ؿ�����
            player?.Close();
            player = new MciPlayer(filePath, this);
            player.Open();
            player.Volume(tkbVol.Value);
            player.Play();
            timerPlay.Start();

            // ��ť
            btnPlayPause.Down = true;
        }

        /*
         * ���ò�����״̬��Ϣ
         */
        private void ReloadStatus()
        {
            //����ר������ΪĬ��
            pbAlbum.Image = Properties.Resources.defaultSmallAlbum;
            lblMusicName.Text = "δ֪����";
            lblArtistName.Text = "δ֪������";
            lblCurrTime.Text = "00:00";
            lblEndTime.Text = "00:00";
            tkbVol.Value = tkbVol.Maximum / 2;
            tkbProgress.Value = 0;
            if (lvSongList.Items.Count > 0 && lvSongList.SelectedItems.Count == 0)
            {
                lvSongList.Items[0].Selected = true;//�趨ѡ��            
                lvSongList.Items[0].EnsureVisible();//��֤�ɼ�
                lvSongList.Items[0].Focused = true;
            }
        }

        private void EndOfPlayCallback()     //���Ÿ��������ص�
        {
            /* �б�û�ļ�ֱ���� */
            if (lvSongList.Items.Count == 0) return;

            /* ���ݲ����ļ�������index */
            int currIndex = 0;
            for (int i = 0; i < lvSongList.Items.Count; i++)
            {
                if (player!.DevicePath == lvSongList.Items[i].SubItems[6].Text)
                {
                    currIndex = i;
                    break;
                }
            }
            /* ���ݲ���ģʽѡ����һ�� */
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
                    if (randomList[randomListIndex] == jumpSongIndex)   //ƥ�䵽��Ҫ�����ĸ���
                    {
                        randomListIndex++;
                    }
                    if (randomListIndex == randomList.Length)
                    {
                        this.BuildRandomList(lvSongList.Items.Count);   //���������������
                        jumpSongIndex = -1;                             //�ڶ��ֿ�ʼ �������и��� ������
                    }
                    currIndex = randomList[randomListIndex++];
                    if (randomListIndex == randomList.Length)
                    {
                        this.BuildRandomList(lvSongList.Items.Count);   //���������������
                        jumpSongIndex = -1;                             //�ڶ��ֿ�ʼ �������и��� ������
                    }
                    break;
            }
            /* ��ʼ���� */
            this.PlayNewIndex(currIndex);
            this.UpdateCurrentSongToUI(currIndex);
        }
        private void BuildRandomList(int songListCount)
        {
            randomListIndex = 0;
            randomList = new int[songListCount];
            //��ʼ������
            for (int i = 0; i < songListCount; i++)
            {
                randomList[i] = i;
            }
            //�������
            for (int i = songListCount - 1; i >= 0; i--)
            {
                Random r = new Random(Guid.NewGuid().GetHashCode());
                int j = r.Next(0, songListCount - 1);
                (randomList[i], randomList[j]) = (randomList[j], randomList[i]);
            }
            //�������
            //for (int i = 0; i < songListCount; i++)
            //{
            //    Console.Write(randomList[i] + " ");
            //}
            //Console.WriteLine(" ");
        }
        #endregion

        #region ��������
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
            // ������ͣ���ָ�������ɹ����˳�
            if (this.PlayPauseSwitch()) return;
            // ��ǰû�в��ţ����¿�ʼ
            // ��ѡ�У���ѡ�еĿ�ʼ; ��ѡ�У����趨����ģʽ��
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

        #region ����Trackbar
        private void TimerPlay_Tick(object? sender, EventArgs e)
        {
            //���õ�ǰ����ʱ��
            int ms = player!.GetPosition(); // ���ȡֵ����׼ȷ��
            lblCurrTime.Text = String.Format("{0:D2}:{1:D2}", ms / 60000, (ms / 1000) % 60);

            //���û�����ֵ��������Э��ʽ
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

        #region �������ư�ť��Trackbar
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
            r.Height += 30; // �����ؼ�֮���е����
            if (!r.Contains(this.PointToClient(Control.MousePosition)))
                this.tkbVol.Visible = false;
        }
        private void TkbVol_Scroll(object? sender, EventArgs e)
        {
            if (tkbVol.Value == tkbVol.Minimum)  // �ϵ���͸ı�״̬
            {
                btnVolume.Down = true;
            }
            else if (btnVolume.Down) // ����״̬�ƶ���������
            {
                btnVolume.Down = false;
            }
            player?.Volume(tkbVol.Value);
        }
        private void TkbVol_MouseHover(object? sender, EventArgs e)
        {
            new ToolTip().SetToolTip(this, $"����:{tkbVol.Value}");
        }
        #endregion

        #region ����Notify��Load��Shown��Closed�¼�
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
         * ���ø��ֳ�ʼ״̬
         */
        private void MusicPlayerForm_Load(object? sender, EventArgs e)
        {
            //���ò�����״̬��Ϣ
            ReloadStatus();
            //��ȡ�������б���ʷ��¼
            localSongsList = ReadHistorySongsList(localSongsFilePath);
            favoriteSongsList = ReadHistorySongsList(favoriteSongsFilePath);
            // ��ʾ�����ļ��б�
            AddSongsToListView(localSongsList);
        }

        /*
         * ��������������ͼ����������¼� 
         */
        private void MusicPlayerForm_Shown(object? sender, EventArgs e)
        {
            ////��ͣ��ť
            //ttbbtnPlayPause = new ThumbnailToolbarButton(Properties.Resources.����icon, "����");
            //ttbbtnPlayPause.Enabled = true;
            //ttbbtnPlayPause.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(btnPlay_Click);

            ////��һ�װ�ť
            //ttbbtnPre = new ThumbnailToolbarButton(Properties.Resources.��һ��icon, "��һ��");
            //ttbbtnPre.Enabled = true;
            //ttbbtnPre.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(btnBack_Click);

            ////��һ�װ�ť
            //ttbbtnNext = new ThumbnailToolbarButton(Properties.Resources.��һ��icon, "��һ��");
            //ttbbtnNext.Enabled = true;
            //ttbbtnNext.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(BtnNext_Click);
            //TaskbarManager.Instance.ThumbnailToolbars.AddButtons(this.Handle, ttbbtnPre, ttbbtnPlayPause, ttbbtnNext);

            //�ü���ʾ����ͼ
            //����ֵΪ�����������Ե�λ�������ۼ�����
            //Point p = new Point(4, 558);
            //TaskbarManager.Instance.TabbedThumbnail.SetThumbnailClip(this.Handle, new Rectangle(p, pbSmallAlbum.Size));
        }

        /*
         * ����ر��¼� 
         */
        private void MusicPlayerForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            Application.Exit();
            this.Dispose();
        }
        #endregion
    }
}
