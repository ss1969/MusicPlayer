namespace MusicPlayer
{
    partial class MusicPlayerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblPlaylist = new System.Windows.Forms.Label();
            this.lblFavourite = new System.Windows.Forms.Label();
            this.tkbProgress = new System.Windows.Forms.TrackBar();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblOpen = new System.Windows.Forms.Label();
            this.lvSongList = new System.Windows.Forms.ListView();
            this.colNum = new System.Windows.Forms.ColumnHeader();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colSinger = new System.Windows.Forms.ColumnHeader();
            this.colAlbum = new System.Windows.Forms.ColumnHeader();
            this.colDuration = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.odlgFile = new System.Windows.Forms.OpenFileDialog();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.cmsSongListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveSongFromList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFilePath = new System.Windows.Forms.ToolStripMenuItem();
            this.tkbVol = new System.Windows.Forms.TrackBar();
            this.lblCurrTime = new System.Windows.Forms.Label();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.controlBarPanel = new System.Windows.Forms.Panel();
            this.btnLoop = new KControlsLib.ImageButton();
            this.btnVolume = new KControlsLib.ImageButton();
            this.btnNext = new KControlsLib.ImageButton();
            this.btnPlayPause = new KControlsLib.ImageButton();
            this.btnPrev = new KControlsLib.ImageButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.paTitle = new System.Windows.Forms.Panel();
            this.btnMinium = new KControlsLib.ImageButton();
            this.btnClose = new KControlsLib.ImageButton();
            this.txtSearchSongName = new System.Windows.Forms.TextBox();
            this.lblClear = new System.Windows.Forms.Label();
            this.pbAlbum = new System.Windows.Forms.PictureBox();
            this.lblMusicName = new System.Windows.Forms.Label();
            this.lblArtistName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblInvade = new System.Windows.Forms.Label();
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tkbProgress)).BeginInit();
            this.cmsSongListMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkbVol)).BeginInit();
            this.controlBarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPlayPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPrev)).BeginInit();
            this.paTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinium)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlbum)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPlaylist
            // 
            this.lblPlaylist.AutoSize = true;
            this.lblPlaylist.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPlaylist.ForeColor = System.Drawing.Color.White;
            this.lblPlaylist.Location = new System.Drawing.Point(2, 5);
            this.lblPlaylist.Name = "lblPlaylist";
            this.lblPlaylist.Size = new System.Drawing.Size(74, 21);
            this.lblPlaylist.TabIndex = 2;
            this.lblPlaylist.Text = "本地文件";
            this.lblPlaylist.Click += new System.EventHandler(this.lblPlaylist_Click);
            // 
            // lblFavourite
            // 
            this.lblFavourite.AutoSize = true;
            this.lblFavourite.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFavourite.ForeColor = System.Drawing.Color.Gray;
            this.lblFavourite.Location = new System.Drawing.Point(92, 5);
            this.lblFavourite.Name = "lblFavourite";
            this.lblFavourite.Size = new System.Drawing.Size(74, 21);
            this.lblFavourite.TabIndex = 2;
            this.lblFavourite.Text = "收藏列表";
            this.lblFavourite.Click += new System.EventHandler(this.lblFavourite_Click);
            // 
            // tkbProgress
            // 
            this.tkbProgress.AutoSize = false;
            this.tkbProgress.Location = new System.Drawing.Point(168, 12);
            this.tkbProgress.Maximum = 100;
            this.tkbProgress.Name = "tkbProgress";
            this.tkbProgress.Size = new System.Drawing.Size(282, 27);
            this.tkbProgress.TabIndex = 3;
            this.tkbProgress.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(162, 534);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(83, 17);
            this.lblTime.TabIndex = 4;
            this.lblTime.Text = "00:00 / 00:00";
            // 
            // lblOpen
            // 
            this.lblOpen.AutoSize = true;
            this.lblOpen.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblOpen.ForeColor = System.Drawing.Color.White;
            this.lblOpen.Location = new System.Drawing.Point(182, 5);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Size = new System.Drawing.Size(32, 21);
            this.lblOpen.TabIndex = 2;
            this.lblOpen.Text = "➕";
            this.lblOpen.Click += new System.EventHandler(this.lblOpen_Click);
            // 
            // lvSongList
            // 
            this.lvSongList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(24)))), ((int)(((byte)(28)))));
            this.lvSongList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvSongList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNum,
            this.colName,
            this.colSinger,
            this.colAlbum,
            this.colDuration,
            this.colSize});
            this.lvSongList.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lvSongList.ForeColor = System.Drawing.Color.White;
            this.lvSongList.FullRowSelect = true;
            this.lvSongList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSongList.Location = new System.Drawing.Point(2, 32);
            this.lvSongList.MultiSelect = false;
            this.lvSongList.Name = "lvSongList";
            this.lvSongList.OwnerDraw = true;
            this.lvSongList.Size = new System.Drawing.Size(892, 543);
            this.lvSongList.TabIndex = 5;
            this.lvSongList.UseCompatibleStateImageBehavior = false;
            this.lvSongList.View = System.Windows.Forms.View.Details;
            // 
            // colNum
            // 
            this.colNum.Text = "";
            this.colNum.Width = 62;
            // 
            // colName
            // 
            this.colName.Text = "音乐标题";
            this.colName.Width = 310;
            // 
            // colSinger
            // 
            this.colSinger.Text = "歌手";
            this.colSinger.Width = 200;
            // 
            // colAlbum
            // 
            this.colAlbum.Text = "专辑";
            this.colAlbum.Width = 180;
            // 
            // colDuration
            // 
            this.colDuration.Text = "时长";
            this.colDuration.Width = 70;
            // 
            // colSize
            // 
            this.colSize.Text = "大小";
            this.colSize.Width = 70;
            // 
            // odlgFile
            // 
            this.odlgFile.FileName = "openFileDialog1";
            // 
            // cmsSongListMenu
            // 
            this.cmsSongListMenu.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmsSongListMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsSongListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFavorite,
            this.tsmiRemoveSongFromList,
            this.tsmiOpenFilePath});
            this.cmsSongListMenu.Name = "contextMenuStrip1";
            this.cmsSongListMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.cmsSongListMenu.Size = new System.Drawing.Size(188, 82);
            // 
            // tsmiFavorite
            // 
            this.tsmiFavorite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(50)))));
            this.tsmiFavorite.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tsmiFavorite.ForeColor = System.Drawing.Color.White;
            this.tsmiFavorite.Image = global::MusicPlayer.Properties.Resources.favorite;
            this.tsmiFavorite.Name = "tsmiFavorite";
            this.tsmiFavorite.Size = new System.Drawing.Size(187, 26);
            this.tsmiFavorite.Text = "收藏到歌单(&I)";
            this.tsmiFavorite.Click += new System.EventHandler(this.tsmiFavorite_Click);
            // 
            // tsmiRemoveSongFromList
            // 
            this.tsmiRemoveSongFromList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(50)))));
            this.tsmiRemoveSongFromList.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tsmiRemoveSongFromList.ForeColor = System.Drawing.Color.White;
            this.tsmiRemoveSongFromList.Image = global::MusicPlayer.Properties.Resources.delete;
            this.tsmiRemoveSongFromList.Name = "tsmiRemoveSongFromList";
            this.tsmiRemoveSongFromList.Size = new System.Drawing.Size(187, 26);
            this.tsmiRemoveSongFromList.Text = "从列表中删除(&D)";
            this.tsmiRemoveSongFromList.Click += new System.EventHandler(this.tsmiRemoveSongFromList_Click);
            // 
            // tsmiOpenFilePath
            // 
            this.tsmiOpenFilePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(50)))));
            this.tsmiOpenFilePath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tsmiOpenFilePath.ForeColor = System.Drawing.Color.White;
            this.tsmiOpenFilePath.Image = global::MusicPlayer.Properties.Resources.open;
            this.tsmiOpenFilePath.Name = "tsmiOpenFilePath";
            this.tsmiOpenFilePath.Size = new System.Drawing.Size(187, 26);
            this.tsmiOpenFilePath.Text = "打开文件位置(&F)";
            this.tsmiOpenFilePath.Click += new System.EventHandler(this.tsmiOpenFilePath_Click);
            // 
            // tkbVol
            // 
            this.tkbVol.AutoSize = false;
            this.tkbVol.Location = new System.Drawing.Point(759, 555);
            this.tkbVol.Margin = new System.Windows.Forms.Padding(4);
            this.tkbVol.Maximum = 1000;
            this.tkbVol.Name = "tkbVol";
            this.tkbVol.Size = new System.Drawing.Size(105, 30);
            this.tkbVol.TabIndex = 7;
            this.tkbVol.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tkbVol.Value = 500;
            this.tkbVol.Visible = false;
            // 
            // lblCurrTime
            // 
            this.lblCurrTime.AutoSize = true;
            this.lblCurrTime.ForeColor = System.Drawing.Color.White;
            this.lblCurrTime.Location = new System.Drawing.Point(172, 40);
            this.lblCurrTime.Name = "lblCurrTime";
            this.lblCurrTime.Size = new System.Drawing.Size(39, 17);
            this.lblCurrTime.TabIndex = 8;
            this.lblCurrTime.Text = "00:00";
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.ForeColor = System.Drawing.Color.White;
            this.lblEndTime.Location = new System.Drawing.Point(411, 40);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(39, 17);
            this.lblEndTime.TabIndex = 8;
            this.lblEndTime.Text = "00:00";
            // 
            // controlBarPanel
            // 
            this.controlBarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(35)))), ((int)(((byte)(39)))));
            this.controlBarPanel.Controls.Add(this.btnLoop);
            this.controlBarPanel.Controls.Add(this.btnVolume);
            this.controlBarPanel.Controls.Add(this.btnNext);
            this.controlBarPanel.Controls.Add(this.btnPlayPause);
            this.controlBarPanel.Controls.Add(this.btnPrev);
            this.controlBarPanel.Controls.Add(this.lblEndTime);
            this.controlBarPanel.Controls.Add(this.lblCurrTime);
            this.controlBarPanel.Controls.Add(this.tkbProgress);
            this.controlBarPanel.Location = new System.Drawing.Point(332, 577);
            this.controlBarPanel.Name = "controlBarPanel";
            this.controlBarPanel.Size = new System.Drawing.Size(562, 66);
            this.controlBarPanel.TabIndex = 9;
            // 
            // btnLoop
            // 
            this.btnLoop.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnLoop.Down = false;
            this.btnLoop.HoverText = "";
            this.btnLoop.HoverTextToggle = "";
            this.btnLoop.ImageDisabled = null;
            this.btnLoop.ImageDown = null;
            this.btnLoop.ImageDownHover = null;
            this.btnLoop.ImageNormal = null;
            this.btnLoop.ImageNormalHover = null;
            this.btnLoop.Location = new System.Drawing.Point(513, 17);
            this.btnLoop.Margin = new System.Windows.Forms.Padding(0);
            this.btnLoop.Name = "btnLoop";
            this.btnLoop.Size = new System.Drawing.Size(35, 35);
            this.btnLoop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnLoop.TabIndex = 9;
            this.btnLoop.TabStop = false;
            this.btnLoop.ToggleMode = false;
            // 
            // btnVolume
            // 
            this.btnVolume.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnVolume.Down = false;
            this.btnVolume.HoverText = "";
            this.btnVolume.HoverTextToggle = "";
            this.btnVolume.ImageDisabled = null;
            this.btnVolume.ImageDown = null;
            this.btnVolume.ImageDownHover = null;
            this.btnVolume.ImageNormal = null;
            this.btnVolume.ImageNormalHover = null;
            this.btnVolume.Location = new System.Drawing.Point(464, 17);
            this.btnVolume.Margin = new System.Windows.Forms.Padding(0);
            this.btnVolume.Name = "btnVolume";
            this.btnVolume.Size = new System.Drawing.Size(35, 35);
            this.btnVolume.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnVolume.TabIndex = 9;
            this.btnVolume.TabStop = false;
            this.btnVolume.ToggleMode = false;
            // 
            // btnNext
            // 
            this.btnNext.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnNext.Down = false;
            this.btnNext.HoverText = "";
            this.btnNext.HoverTextToggle = "";
            this.btnNext.ImageDisabled = null;
            this.btnNext.ImageDown = null;
            this.btnNext.ImageDownHover = null;
            this.btnNext.ImageNormal = null;
            this.btnNext.ImageNormalHover = null;
            this.btnNext.Location = new System.Drawing.Point(116, 12);
            this.btnNext.Margin = new System.Windows.Forms.Padding(0);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(45, 45);
            this.btnNext.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNext.TabIndex = 9;
            this.btnNext.TabStop = false;
            this.btnNext.ToggleMode = false;
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnPlayPause.Down = false;
            this.btnPlayPause.HoverText = "";
            this.btnPlayPause.HoverTextToggle = "";
            this.btnPlayPause.ImageDisabled = null;
            this.btnPlayPause.ImageDown = null;
            this.btnPlayPause.ImageDownHover = null;
            this.btnPlayPause.ImageNormal = null;
            this.btnPlayPause.ImageNormalHover = null;
            this.btnPlayPause.Location = new System.Drawing.Point(63, 12);
            this.btnPlayPause.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(45, 45);
            this.btnPlayPause.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPlayPause.TabIndex = 9;
            this.btnPlayPause.TabStop = false;
            this.btnPlayPause.ToggleMode = false;
            // 
            // btnPrev
            // 
            this.btnPrev.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnPrev.Down = false;
            this.btnPrev.HoverText = "";
            this.btnPrev.HoverTextToggle = "";
            this.btnPrev.ImageDisabled = null;
            this.btnPrev.ImageDown = null;
            this.btnPrev.ImageDownHover = null;
            this.btnPrev.ImageNormal = null;
            this.btnPrev.ImageNormalHover = null;
            this.btnPrev.Location = new System.Drawing.Point(10, 12);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(0);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(45, 45);
            this.btnPrev.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPrev.TabIndex = 9;
            this.btnPrev.TabStop = false;
            this.btnPrev.ToggleMode = false;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblTitle.Location = new System.Drawing.Point(382, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(101, 21);
            this.lblTitle.TabIndex = 11;
            this.lblTitle.Text = "MusicPlayer";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // paTitle
            // 
            this.paTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(35)))), ((int)(((byte)(39)))));
            this.paTitle.Controls.Add(this.btnMinium);
            this.paTitle.Controls.Add(this.btnClose);
            this.paTitle.Controls.Add(this.txtSearchSongName);
            this.paTitle.Controls.Add(this.lblTitle);
            this.paTitle.Controls.Add(this.lblClear);
            this.paTitle.Controls.Add(this.lblOpen);
            this.paTitle.Controls.Add(this.lblPlaylist);
            this.paTitle.Controls.Add(this.lblFavourite);
            this.paTitle.ForeColor = System.Drawing.Color.Gray;
            this.paTitle.Location = new System.Drawing.Point(2, 1);
            this.paTitle.Name = "paTitle";
            this.paTitle.Size = new System.Drawing.Size(892, 32);
            this.paTitle.TabIndex = 12;
            // 
            // btnMinium
            // 
            this.btnMinium.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnMinium.Down = false;
            this.btnMinium.HoverText = "";
            this.btnMinium.HoverTextToggle = "";
            this.btnMinium.ImageDisabled = null;
            this.btnMinium.ImageDown = null;
            this.btnMinium.ImageDownHover = null;
            this.btnMinium.ImageNormal = null;
            this.btnMinium.ImageNormalHover = null;
            this.btnMinium.Location = new System.Drawing.Point(829, 5);
            this.btnMinium.Margin = new System.Windows.Forms.Padding(0);
            this.btnMinium.Name = "btnMinium";
            this.btnMinium.Size = new System.Drawing.Size(23, 23);
            this.btnMinium.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnMinium.TabIndex = 9;
            this.btnMinium.TabStop = false;
            this.btnMinium.ToggleMode = false;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnClose.Down = false;
            this.btnClose.HoverText = "";
            this.btnClose.HoverTextToggle = "";
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageDown = null;
            this.btnClose.ImageDownHover = null;
            this.btnClose.ImageNormal = null;
            this.btnClose.ImageNormalHover = null;
            this.btnClose.Location = new System.Drawing.Point(864, 5);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(23, 23);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnClose.TabIndex = 9;
            this.btnClose.TabStop = false;
            this.btnClose.ToggleMode = false;
            // 
            // txtSearchSongName
            // 
            this.txtSearchSongName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(24)))));
            this.txtSearchSongName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearchSongName.ForeColor = System.Drawing.Color.Gray;
            this.txtSearchSongName.Location = new System.Drawing.Point(611, 5);
            this.txtSearchSongName.Name = "txtSearchSongName";
            this.txtSearchSongName.Size = new System.Drawing.Size(198, 23);
            this.txtSearchSongName.TabIndex = 12;
            this.txtSearchSongName.Text = "输入要搜索的歌曲名";
            // 
            // lblClear
            // 
            this.lblClear.AutoSize = true;
            this.lblClear.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblClear.ForeColor = System.Drawing.Color.White;
            this.lblClear.Location = new System.Drawing.Point(220, 5);
            this.lblClear.Name = "lblClear";
            this.lblClear.Size = new System.Drawing.Size(32, 21);
            this.lblClear.TabIndex = 2;
            this.lblClear.Text = "➖";
            this.lblClear.Click += new System.EventHandler(this.lblClear_Click);
            // 
            // pbAlbum
            // 
            this.pbAlbum.Image = global::MusicPlayer.Properties.Resources.defaultSmallAlbum;
            this.pbAlbum.Location = new System.Drawing.Point(10, 0);
            this.pbAlbum.Name = "pbAlbum";
            this.pbAlbum.Size = new System.Drawing.Size(66, 66);
            this.pbAlbum.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbAlbum.TabIndex = 10;
            this.pbAlbum.TabStop = false;
            // 
            // lblMusicName
            // 
            this.lblMusicName.ForeColor = System.Drawing.Color.White;
            this.lblMusicName.Location = new System.Drawing.Point(82, 9);
            this.lblMusicName.Name = "lblMusicName";
            this.lblMusicName.Size = new System.Drawing.Size(242, 21);
            this.lblMusicName.TabIndex = 11;
            this.lblMusicName.Text = "Unknown Song";
            // 
            // lblArtistName
            // 
            this.lblArtistName.ForeColor = System.Drawing.Color.White;
            this.lblArtistName.Location = new System.Drawing.Point(82, 37);
            this.lblArtistName.Name = "lblArtistName";
            this.lblArtistName.Size = new System.Drawing.Size(242, 21);
            this.lblArtistName.TabIndex = 11;
            this.lblArtistName.Text = "Unknown Artist";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(35)))), ((int)(((byte)(39)))));
            this.panel1.Controls.Add(this.lblArtistName);
            this.panel1.Controls.Add(this.pbAlbum);
            this.panel1.Controls.Add(this.lblMusicName);
            this.panel1.Location = new System.Drawing.Point(2, 577);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 66);
            this.panel1.TabIndex = 13;
            // 
            // lblInvade
            // 
            this.lblInvade.AutoSize = true;
            this.lblInvade.Font = new System.Drawing.Font("Microsoft YaHei UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInvade.ForeColor = System.Drawing.Color.SandyBrown;
            this.lblInvade.Location = new System.Drawing.Point(298, 263);
            this.lblInvade.Name = "lblInvade";
            this.lblInvade.Size = new System.Drawing.Size(279, 48);
            this.lblInvade.TabIndex = 14;
            this.lblInvade.Text = "外部控制模式中";
            this.lblInvade.Visible = false;
            // 
            // MusicPlayerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(896, 644);
            this.Controls.Add(this.lblInvade);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.paTitle);
            this.Controls.Add(this.controlBarPanel);
            this.Controls.Add(this.lvSongList);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.tkbVol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MusicPlayerForm";
            this.Text = "音乐播放器";
            ((System.ComponentModel.ISupportInitialize)(this.tkbProgress)).EndInit();
            this.cmsSongListMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tkbVol)).EndInit();
            this.controlBarPanel.ResumeLayout(false);
            this.controlBarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPlayPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPrev)).EndInit();
            this.paTitle.ResumeLayout(false);
            this.paTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinium)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlbum)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblPlaylist;
        private Label lblFavourite;
        private TrackBar tkbProgress;
        private Label lblTime;
        private Label lblOpen;
        private ListView lvSongList;
        private OpenFileDialog odlgFile;
        private ToolTip tip;
        private ContextMenuStrip cmsSongListMenu;
        private TrackBar tkbVol;
        private Label lblCurrTime;
        private Label lblEndTime;
        private Panel controlBarPanel;
        private ColumnHeader colNum;
        private ColumnHeader colName;
        private ColumnHeader colSinger;
        private ColumnHeader colAlbum;
        private ColumnHeader colDuration;
        private ColumnHeader colSize;
        private Label lblTitle;
        private Panel paTitle;
        private TextBox txtSearchSongName;
        private ToolStripMenuItem tsmiFavorite;
        private ToolStripMenuItem tsmiRemoveSongFromList;
        private ToolStripMenuItem tsmiOpenFilePath;
        private KControlsLib.ImageButton btnLoop;
        private KControlsLib.ImageButton btnVolume;
        private KControlsLib.ImageButton btnNext;
        private KControlsLib.ImageButton btnPlayPause;
        private KControlsLib.ImageButton btnPrev;
        private KControlsLib.ImageButton btnMinium;
        private KControlsLib.ImageButton btnClose;
        private PictureBox pbAlbum;
        private Label lblMusicName;
        private Label lblArtistName;
        private Panel panel1;
        private Label lblClear;
        private Label lblInvade;
        private System.Windows.Forms.Timer timerPlay;
    }
}