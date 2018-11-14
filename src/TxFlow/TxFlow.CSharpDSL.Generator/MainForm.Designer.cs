namespace TxFlow.CSharpDSL.Generator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.activityFilePathTextBox = new MetroFramework.Controls.MetroTextBox();
            this.activityToolboxNameTextBox = new MetroFramework.Controls.MetroTextBox();
            this.resultingProjectDirTextBox = new MetroFramework.Controls.MetroTextBox();
            this.chooseActivityDLLButton = new MetroFramework.Controls.MetroButton();
            this.chooseResultingProjectDirButton = new MetroFramework.Controls.MetroButton();
            this.generateButton = new MetroFramework.Controls.MetroButton();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(23, 178);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(634, 148);
            this.checkedListBox1.TabIndex = 7;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.Location = new System.Drawing.Point(23, 109);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(255, 19);
            this.metroLabel1.TabIndex = 10;
            this.metroLabel1.Text = ".NET Assembly containing activity-types:";
            // 
            // metroLabel2
            // 
            this.metroLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.Location = new System.Drawing.Point(23, 359);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(142, 19);
            this.metroLabel2.TabIndex = 11;
            this.metroLabel2.Text = "ActivityToolbox name:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel3.Location = new System.Drawing.Point(23, 416);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(115, 19);
            this.metroLabel3.TabIndex = 12;
            this.metroLabel3.Text = "Output directory:";
            // 
            // activityFilePathTextBox
            // 
            this.activityFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.activityFilePathTextBox.CustomButton.Image = null;
            this.activityFilePathTextBox.CustomButton.Location = new System.Drawing.Point(557, 1);
            this.activityFilePathTextBox.CustomButton.Name = "";
            this.activityFilePathTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.activityFilePathTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.activityFilePathTextBox.CustomButton.TabIndex = 1;
            this.activityFilePathTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.activityFilePathTextBox.CustomButton.UseSelectable = true;
            this.activityFilePathTextBox.CustomButton.Visible = false;
            this.activityFilePathTextBox.Lines = new string[0];
            this.activityFilePathTextBox.Location = new System.Drawing.Point(23, 131);
            this.activityFilePathTextBox.MaxLength = 32767;
            this.activityFilePathTextBox.Name = "activityFilePathTextBox";
            this.activityFilePathTextBox.PasswordChar = '\0';
            this.activityFilePathTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.activityFilePathTextBox.SelectedText = "";
            this.activityFilePathTextBox.SelectionLength = 0;
            this.activityFilePathTextBox.SelectionStart = 0;
            this.activityFilePathTextBox.ShortcutsEnabled = true;
            this.activityFilePathTextBox.Size = new System.Drawing.Size(579, 23);
            this.activityFilePathTextBox.TabIndex = 13;
            this.activityFilePathTextBox.UseSelectable = true;
            this.activityFilePathTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.activityFilePathTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.activityFilePathTextBox.TextChanged += new System.EventHandler(this.activityFilePathTextBox_TextChanged);
            // 
            // activityToolboxNameTextBox
            // 
            this.activityToolboxNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.activityToolboxNameTextBox.CustomButton.Image = null;
            this.activityToolboxNameTextBox.CustomButton.Location = new System.Drawing.Point(612, 1);
            this.activityToolboxNameTextBox.CustomButton.Name = "";
            this.activityToolboxNameTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.activityToolboxNameTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.activityToolboxNameTextBox.CustomButton.TabIndex = 1;
            this.activityToolboxNameTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.activityToolboxNameTextBox.CustomButton.UseSelectable = true;
            this.activityToolboxNameTextBox.CustomButton.Visible = false;
            this.activityToolboxNameTextBox.Lines = new string[0];
            this.activityToolboxNameTextBox.Location = new System.Drawing.Point(23, 381);
            this.activityToolboxNameTextBox.MaxLength = 32767;
            this.activityToolboxNameTextBox.Name = "activityToolboxNameTextBox";
            this.activityToolboxNameTextBox.PasswordChar = '\0';
            this.activityToolboxNameTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.activityToolboxNameTextBox.SelectedText = "";
            this.activityToolboxNameTextBox.SelectionLength = 0;
            this.activityToolboxNameTextBox.SelectionStart = 0;
            this.activityToolboxNameTextBox.ShortcutsEnabled = true;
            this.activityToolboxNameTextBox.Size = new System.Drawing.Size(634, 23);
            this.activityToolboxNameTextBox.TabIndex = 14;
            this.activityToolboxNameTextBox.UseSelectable = true;
            this.activityToolboxNameTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.activityToolboxNameTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // resultingProjectDirTextBox
            // 
            this.resultingProjectDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.resultingProjectDirTextBox.CustomButton.Image = null;
            this.resultingProjectDirTextBox.CustomButton.Location = new System.Drawing.Point(557, 1);
            this.resultingProjectDirTextBox.CustomButton.Name = "";
            this.resultingProjectDirTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.resultingProjectDirTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.resultingProjectDirTextBox.CustomButton.TabIndex = 1;
            this.resultingProjectDirTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.resultingProjectDirTextBox.CustomButton.UseSelectable = true;
            this.resultingProjectDirTextBox.CustomButton.Visible = false;
            this.resultingProjectDirTextBox.Lines = new string[0];
            this.resultingProjectDirTextBox.Location = new System.Drawing.Point(23, 438);
            this.resultingProjectDirTextBox.MaxLength = 32767;
            this.resultingProjectDirTextBox.Name = "resultingProjectDirTextBox";
            this.resultingProjectDirTextBox.PasswordChar = '\0';
            this.resultingProjectDirTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.resultingProjectDirTextBox.SelectedText = "";
            this.resultingProjectDirTextBox.SelectionLength = 0;
            this.resultingProjectDirTextBox.SelectionStart = 0;
            this.resultingProjectDirTextBox.ShortcutsEnabled = true;
            this.resultingProjectDirTextBox.Size = new System.Drawing.Size(579, 23);
            this.resultingProjectDirTextBox.TabIndex = 15;
            this.resultingProjectDirTextBox.UseSelectable = true;
            this.resultingProjectDirTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.resultingProjectDirTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // chooseActivityDLLButton
            // 
            this.chooseActivityDLLButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseActivityDLLButton.Location = new System.Drawing.Point(601, 131);
            this.chooseActivityDLLButton.Name = "chooseActivityDLLButton";
            this.chooseActivityDLLButton.Size = new System.Drawing.Size(56, 23);
            this.chooseActivityDLLButton.TabIndex = 16;
            this.chooseActivityDLLButton.Text = "Choose";
            this.chooseActivityDLLButton.UseSelectable = true;
            this.chooseActivityDLLButton.Click += new System.EventHandler(this.chooseActivityDLLButton_Click);
            // 
            // chooseResultingProjectDirButton
            // 
            this.chooseResultingProjectDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseResultingProjectDirButton.Location = new System.Drawing.Point(601, 438);
            this.chooseResultingProjectDirButton.Name = "chooseResultingProjectDirButton";
            this.chooseResultingProjectDirButton.Size = new System.Drawing.Size(56, 23);
            this.chooseResultingProjectDirButton.TabIndex = 17;
            this.chooseResultingProjectDirButton.Text = "Choose";
            this.chooseResultingProjectDirButton.UseSelectable = true;
            this.chooseResultingProjectDirButton.Click += new System.EventHandler(this.chooseResultingProjectDirButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generateButton.FontSize = MetroFramework.MetroButtonSize.Medium;
            this.generateButton.Location = new System.Drawing.Point(23, 487);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(634, 23);
            this.generateButton.TabIndex = 18;
            this.generateButton.Text = "Generate TxFlow Artefacts";
            this.generateButton.UseSelectable = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(23, 157);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(180, 19);
            this.metroLabel4.TabIndex = 19;
            this.metroLabel4.Text = "Choose loaded activity-types:";
            // 
            // metroLabel5
            // 
            this.metroLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel5.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel5.Location = new System.Drawing.Point(23, 334);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(73, 25);
            this.metroLabel5.TabIndex = 20;
            this.metroLabel5.Text = "Output";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel6.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel6.Location = new System.Drawing.Point(23, 84);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(58, 25);
            this.metroLabel6.TabIndex = 21;
            this.metroLabel6.Text = "Input";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackImage = global::TxFlow.CSharpDSL.Generator.Properties.Resources.TxFlowIcon1;
            this.BackImagePadding = new System.Windows.Forms.Padding(35, 10, 0, 0);
            this.BackMaxSize = 60;
            this.ClientSize = new System.Drawing.Size(680, 533);
            this.Controls.Add(this.metroLabel6);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.chooseResultingProjectDirButton);
            this.Controls.Add(this.chooseActivityDLLButton);
            this.Controls.Add(this.resultingProjectDirTextBox);
            this.Controls.Add(this.activityToolboxNameTextBox);
            this.Controls.Add(this.activityFilePathTextBox);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.checkedListBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "         Artefacts Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroTextBox activityFilePathTextBox;
        private MetroFramework.Controls.MetroTextBox activityToolboxNameTextBox;
        private MetroFramework.Controls.MetroTextBox resultingProjectDirTextBox;
        private MetroFramework.Controls.MetroButton chooseActivityDLLButton;
        private MetroFramework.Controls.MetroButton chooseResultingProjectDirButton;
        private MetroFramework.Controls.MetroButton generateButton;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel6;
    }
}

