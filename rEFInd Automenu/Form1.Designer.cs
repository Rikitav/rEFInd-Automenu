namespace rEFInd_Automenu
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.InstallButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.ConfigButton = new System.Windows.Forms.Button();
            this.GroupBoxInstallMode = new System.Windows.Forms.GroupBox();
            this.RadioInstallDesktop = new System.Windows.Forms.RadioButton();
            this.RadioInstallFlash = new System.Windows.Forms.RadioButton();
            this.RadioInstallComputer = new System.Windows.Forms.RadioButton();
            this.GroupBoxInstallOptions = new System.Windows.Forms.GroupBox();
            this.ButtonSelectThemeConfig = new System.Windows.Forms.Button();
            this.TextBoxThemeConf = new System.Windows.Forms.TextBox();
            this.CheckThemePath = new System.Windows.Forms.CheckBox();
            this.CheckForceArch = new System.Windows.Forms.CheckBox();
            this.RadioArchX86 = new System.Windows.Forms.RadioButton();
            this.RadioArchARM64 = new System.Windows.Forms.RadioButton();
            this.RadioArch64 = new System.Windows.Forms.RadioButton();
            this.GroupBoxFlashOptions = new System.Windows.Forms.GroupBox();
            this.ComboBoxSelectDrive = new System.Windows.Forms.ComboBox();
            this.CheckFormat = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.ToolStripESP = new System.Windows.Forms.ToolStripLabel();
            this.справкаToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.OpenFileThemeConf = new System.Windows.Forms.OpenFileDialog();
            this.CheckDownload = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GroupBoxInstallMode.SuspendLayout();
            this.GroupBoxInstallOptions.SuspendLayout();
            this.GroupBoxFlashOptions.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::rEFInd_Automenu.Properties.Resources.rEFInd_Automenu;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(350, 100);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // InstallButton
            // 
            this.InstallButton.Enabled = false;
            this.InstallButton.Location = new System.Drawing.Point(12, 118);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(350, 25);
            this.InstallButton.TabIndex = 1;
            this.InstallButton.Text = "Install rEFInd";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Enabled = false;
            this.RemoveButton.Location = new System.Drawing.Point(12, 149);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(170, 25);
            this.RemoveButton.TabIndex = 2;
            this.RemoveButton.Text = "Remove rEFInd";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // ConfigButton
            // 
            this.ConfigButton.Enabled = false;
            this.ConfigButton.Location = new System.Drawing.Point(188, 149);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(174, 25);
            this.ConfigButton.TabIndex = 3;
            this.ConfigButton.Text = "Open rEFInd Config";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // GroupBoxInstallMode
            // 
            this.GroupBoxInstallMode.Controls.Add(this.RadioInstallDesktop);
            this.GroupBoxInstallMode.Controls.Add(this.RadioInstallFlash);
            this.GroupBoxInstallMode.Controls.Add(this.RadioInstallComputer);
            this.GroupBoxInstallMode.Location = new System.Drawing.Point(12, 180);
            this.GroupBoxInstallMode.Name = "GroupBoxInstallMode";
            this.GroupBoxInstallMode.Size = new System.Drawing.Size(350, 54);
            this.GroupBoxInstallMode.TabIndex = 4;
            this.GroupBoxInstallMode.TabStop = false;
            this.GroupBoxInstallMode.Text = "Select Install Mode : ";
            // 
            // RadioInstallDesktop
            // 
            this.RadioInstallDesktop.AutoSize = true;
            this.RadioInstallDesktop.Location = new System.Drawing.Point(186, 22);
            this.RadioInstallDesktop.Name = "RadioInstallDesktop";
            this.RadioInstallDesktop.Size = new System.Drawing.Size(68, 19);
            this.RadioInstallDesktop.TabIndex = 2;
            this.RadioInstallDesktop.TabStop = true;
            this.RadioInstallDesktop.Text = "Desktop";
            this.RadioInstallDesktop.UseVisualStyleBackColor = true;
            this.RadioInstallDesktop.CheckedChanged += new System.EventHandler(this.RadioInstallDesktop_CheckedChanged);
            // 
            // RadioInstallFlash
            // 
            this.RadioInstallFlash.AutoSize = true;
            this.RadioInstallFlash.Location = new System.Drawing.Point(98, 22);
            this.RadioInstallFlash.Name = "RadioInstallFlash";
            this.RadioInstallFlash.Size = new System.Drawing.Size(82, 19);
            this.RadioInstallFlash.TabIndex = 1;
            this.RadioInstallFlash.TabStop = true;
            this.RadioInstallFlash.Text = "Flash Drive";
            this.RadioInstallFlash.UseVisualStyleBackColor = true;
            this.RadioInstallFlash.CheckedChanged += new System.EventHandler(this.RadioInstallFlash_CheckedChanged);
            // 
            // RadioInstallComputer
            // 
            this.RadioInstallComputer.AutoSize = true;
            this.RadioInstallComputer.Location = new System.Drawing.Point(13, 22);
            this.RadioInstallComputer.Name = "RadioInstallComputer";
            this.RadioInstallComputer.Size = new System.Drawing.Size(79, 19);
            this.RadioInstallComputer.TabIndex = 0;
            this.RadioInstallComputer.TabStop = true;
            this.RadioInstallComputer.Text = "Computer";
            this.RadioInstallComputer.UseVisualStyleBackColor = true;
            this.RadioInstallComputer.CheckedChanged += new System.EventHandler(this.RadioInstallComputer_CheckedChanged);
            // 
            // GroupBoxInstallOptions
            // 
            this.GroupBoxInstallOptions.Controls.Add(this.CheckDownload);
            this.GroupBoxInstallOptions.Controls.Add(this.ButtonSelectThemeConfig);
            this.GroupBoxInstallOptions.Controls.Add(this.TextBoxThemeConf);
            this.GroupBoxInstallOptions.Controls.Add(this.CheckThemePath);
            this.GroupBoxInstallOptions.Controls.Add(this.CheckForceArch);
            this.GroupBoxInstallOptions.Controls.Add(this.RadioArchX86);
            this.GroupBoxInstallOptions.Controls.Add(this.RadioArchARM64);
            this.GroupBoxInstallOptions.Controls.Add(this.RadioArch64);
            this.GroupBoxInstallOptions.Enabled = false;
            this.GroupBoxInstallOptions.Location = new System.Drawing.Point(12, 240);
            this.GroupBoxInstallOptions.Name = "GroupBoxInstallOptions";
            this.GroupBoxInstallOptions.Size = new System.Drawing.Size(350, 156);
            this.GroupBoxInstallOptions.TabIndex = 0;
            this.GroupBoxInstallOptions.TabStop = false;
            this.GroupBoxInstallOptions.Text = "Configure Install Options : ";
            // 
            // ButtonSelectThemeConfig
            // 
            this.ButtonSelectThemeConfig.Enabled = false;
            this.ButtonSelectThemeConfig.Location = new System.Drawing.Point(316, 121);
            this.ButtonSelectThemeConfig.Name = "ButtonSelectThemeConfig";
            this.ButtonSelectThemeConfig.Size = new System.Drawing.Size(23, 23);
            this.ButtonSelectThemeConfig.TabIndex = 6;
            this.ButtonSelectThemeConfig.Text = ">";
            this.ButtonSelectThemeConfig.UseVisualStyleBackColor = true;
            this.ButtonSelectThemeConfig.Click += new System.EventHandler(this.ButtonSelectThemeConf_Click);
            // 
            // TextBoxThemeConf
            // 
            this.TextBoxThemeConf.Enabled = false;
            this.TextBoxThemeConf.Location = new System.Drawing.Point(13, 122);
            this.TextBoxThemeConf.Name = "TextBoxThemeConf";
            this.TextBoxThemeConf.ReadOnly = true;
            this.TextBoxThemeConf.Size = new System.Drawing.Size(298, 23);
            this.TextBoxThemeConf.TabIndex = 5;
            // 
            // CheckThemePath
            // 
            this.CheckThemePath.AutoSize = true;
            this.CheckThemePath.Location = new System.Drawing.Point(13, 97);
            this.CheckThemePath.Name = "CheckThemePath";
            this.CheckThemePath.Size = new System.Drawing.Size(257, 19);
            this.CheckThemePath.TabIndex = 4;
            this.CheckThemePath.Text = "Select Path to your Theme.conf (Optional) : ";
            this.CheckThemePath.UseVisualStyleBackColor = true;
            this.CheckThemePath.CheckedChanged += new System.EventHandler(this.CheckThemePath_CheckedChanged);
            // 
            // CheckForceArch
            // 
            this.CheckForceArch.AutoSize = true;
            this.CheckForceArch.Location = new System.Drawing.Point(13, 47);
            this.CheckForceArch.Name = "CheckForceArch";
            this.CheckForceArch.Size = new System.Drawing.Size(264, 19);
            this.CheckForceArch.TabIndex = 3;
            this.CheckForceArch.Text = "Force-Set Porcessor Architecture (Optional) : ";
            this.CheckForceArch.UseVisualStyleBackColor = true;
            this.CheckForceArch.CheckedChanged += new System.EventHandler(this.CheckForceArch_CheckedChanged);
            // 
            // RadioArchX86
            // 
            this.RadioArchX86.AutoSize = true;
            this.RadioArchX86.Enabled = false;
            this.RadioArchX86.Location = new System.Drawing.Point(185, 72);
            this.RadioArchX86.Name = "RadioArchX86";
            this.RadioArchX86.Size = new System.Drawing.Size(44, 19);
            this.RadioArchX86.TabIndex = 2;
            this.RadioArchX86.TabStop = true;
            this.RadioArchX86.Text = "X86";
            this.RadioArchX86.UseVisualStyleBackColor = true;
            // 
            // RadioArchARM64
            // 
            this.RadioArchARM64.AutoSize = true;
            this.RadioArchARM64.Enabled = false;
            this.RadioArchARM64.Location = new System.Drawing.Point(116, 72);
            this.RadioArchARM64.Name = "RadioArchARM64";
            this.RadioArchARM64.Size = new System.Drawing.Size(63, 19);
            this.RadioArchARM64.TabIndex = 1;
            this.RadioArchARM64.TabStop = true;
            this.RadioArchARM64.Text = "ARM64";
            this.RadioArchARM64.UseVisualStyleBackColor = true;
            // 
            // RadioArch64
            // 
            this.RadioArch64.AutoSize = true;
            this.RadioArch64.Enabled = false;
            this.RadioArch64.Location = new System.Drawing.Point(13, 72);
            this.RadioArch64.Name = "RadioArch64";
            this.RadioArch64.Size = new System.Drawing.Size(98, 19);
            this.RadioArch64.TabIndex = 0;
            this.RadioArch64.TabStop = true;
            this.RadioArch64.Text = "AMD64 \\ IA64";
            this.RadioArch64.UseVisualStyleBackColor = true;
            // 
            // GroupBoxFlashOptions
            // 
            this.GroupBoxFlashOptions.Controls.Add(this.ComboBoxSelectDrive);
            this.GroupBoxFlashOptions.Controls.Add(this.CheckFormat);
            this.GroupBoxFlashOptions.Controls.Add(this.label1);
            this.GroupBoxFlashOptions.Enabled = false;
            this.GroupBoxFlashOptions.Location = new System.Drawing.Point(13, 402);
            this.GroupBoxFlashOptions.Name = "GroupBoxFlashOptions";
            this.GroupBoxFlashOptions.Size = new System.Drawing.Size(350, 94);
            this.GroupBoxFlashOptions.TabIndex = 0;
            this.GroupBoxFlashOptions.TabStop = false;
            this.GroupBoxFlashOptions.Text = "Configure Flash-Install Options : ";
            // 
            // ComboBoxSelectDrive
            // 
            this.ComboBoxSelectDrive.FormattingEnabled = true;
            this.ComboBoxSelectDrive.Location = new System.Drawing.Point(13, 37);
            this.ComboBoxSelectDrive.Name = "ComboBoxSelectDrive";
            this.ComboBoxSelectDrive.Size = new System.Drawing.Size(327, 23);
            this.ComboBoxSelectDrive.TabIndex = 8;
            this.ComboBoxSelectDrive.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectDrive_SelectedIndexChanged);
            // 
            // CheckFormat
            // 
            this.CheckFormat.AutoSize = true;
            this.CheckFormat.Enabled = false;
            this.CheckFormat.Location = new System.Drawing.Point(13, 66);
            this.CheckFormat.Name = "CheckFormat";
            this.CheckFormat.Size = new System.Drawing.Size(141, 19);
            this.CheckFormat.TabIndex = 7;
            this.CheckFormat.Text = "Format Drive to FAT32";
            this.CheckFormat.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select Drive to Install : ";
            // 
            // ToolStrip
            // 
            this.ToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripESP,
            this.справкаToolStripButton,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.ToolStripLabel});
            this.ToolStrip.Location = new System.Drawing.Point(0, 510);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(375, 25);
            this.ToolStrip.TabIndex = 5;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // ToolStripESP
            // 
            this.ToolStripESP.Name = "ToolStripESP";
            this.ToolStripESP.Size = new System.Drawing.Size(63, 22);
            this.ToolStripESP.Text = "ESP : Label";
            // 
            // справкаToolStripButton
            // 
            this.справкаToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.справкаToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.справкаToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("справкаToolStripButton.Image")));
            this.справкаToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.справкаToolStripButton.Name = "справкаToolStripButton";
            this.справкаToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.справкаToolStripButton.Text = "С&правка";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Н&астройки";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolStripLabel
            // 
            this.ToolStripLabel.Name = "ToolStripLabel";
            this.ToolStripLabel.Size = new System.Drawing.Size(106, 22);
            this.ToolStripLabel.Text = "Select Install Mode";
            // 
            // OpenFileThemeConf
            // 
            this.OpenFileThemeConf.DefaultExt = "theme.conf";
            this.OpenFileThemeConf.FileName = "ThemeConf";
            this.OpenFileThemeConf.Filter = "rEFInd Theme Configuration File|*.conf";
            // 
            // CheckDownload
            // 
            this.CheckDownload.AutoSize = true;
            this.CheckDownload.Location = new System.Drawing.Point(13, 22);
            this.CheckDownload.Name = "CheckDownload";
            this.CheckDownload.Size = new System.Drawing.Size(202, 19);
            this.CheckDownload.TabIndex = 7;
            this.CheckDownload.Text = "Download latest version of rEFInd";
            this.CheckDownload.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 535);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.GroupBoxInstallOptions);
            this.Controls.Add(this.GroupBoxFlashOptions);
            this.Controls.Add(this.GroupBoxInstallMode);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.InstallButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GroupBoxInstallMode.ResumeLayout(false);
            this.GroupBoxInstallMode.PerformLayout();
            this.GroupBoxInstallOptions.ResumeLayout(false);
            this.GroupBoxInstallOptions.PerformLayout();
            this.GroupBoxFlashOptions.ResumeLayout(false);
            this.GroupBoxFlashOptions.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Button InstallButton;
        private Button RemoveButton;
        private Button ConfigButton;
        private RadioButton RadioInstallDesktop;
        private RadioButton RadioInstallFlash;
        private RadioButton RadioInstallComputer;
        public GroupBox GroupBoxInstallMode;
        public GroupBox GroupBoxInstallOptions;
        public GroupBox GroupBoxFlashOptions;
        private Button ButtonSelectThemeConfig;
        public TextBox TextBoxThemeConf;
        public CheckBox CheckThemePath;
        public CheckBox CheckForceArch;
        public RadioButton RadioArchX86;
        public RadioButton RadioArchARM64;
        public RadioButton RadioArch64;
        private ComboBox ComboBoxSelectDrive;
        public CheckBox CheckFormat;
        private Label label1;
        private ToolStrip ToolStrip;
        private ToolStripButton справкаToolStripButton;
        private ToolStripButton toolStripButton1;
        private OpenFileDialog OpenFileThemeConf;
        public ToolStripLabel ToolStripLabel;
        private ToolStripLabel ToolStripESP;
        private ToolStripSeparator toolStripSeparator1;
        private CheckBox CheckDownload;
    }
}