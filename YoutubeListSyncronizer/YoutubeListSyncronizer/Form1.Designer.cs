namespace YoutubeListSyncronizer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtPlaylist = new System.Windows.Forms.TextBox();
            this.btnFetchPlaylist = new System.Windows.Forms.Button();
            this.lblPercent = new System.Windows.Forms.Label();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cbmMaxRes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPlaylist
            // 
            this.txtPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPlaylist.Location = new System.Drawing.Point(167, 3);
            this.txtPlaylist.Multiline = true;
            this.txtPlaylist.Name = "txtPlaylist";
            this.txtPlaylist.Size = new System.Drawing.Size(487, 19);
            this.txtPlaylist.TabIndex = 0;
            this.txtPlaylist.Text = "https://www.youtube.com/playlist?list=FL0O6m-jrVllHCo94bJ-a9bA";
            // 
            // btnFetchPlaylist
            // 
            this.btnFetchPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFetchPlaylist.Location = new System.Drawing.Point(660, 3);
            this.btnFetchPlaylist.Name = "btnFetchPlaylist";
            this.btnFetchPlaylist.Size = new System.Drawing.Size(180, 19);
            this.btnFetchPlaylist.TabIndex = 1;
            this.btnFetchPlaylist.Text = "Fetch Videos ";
            this.btnFetchPlaylist.UseVisualStyleBackColor = true;
            this.btnFetchPlaylist.Click += new System.EventHandler(this.btnFetchPlaylist_Click);
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(12, 39);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(0, 13);
            this.lblPercent.TabIndex = 2;
            // 
            // folderBrowser
            // 
            this.folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(86, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Youtube Link:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDownload
            // 
            this.btnDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(660, 459);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(180, 19);
            this.btnDownload.TabIndex = 4;
            this.btnDownload.Text = "Start Downloading";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.tableLayoutPanel1.SetColumnSpan(this.listView, 3);
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(3, 28);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(837, 425);
            this.listView.TabIndex = 5;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listView_MouseMove);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 38;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "VideoID";
            this.columnHeader2.Width = 159;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Video Title";
            this.columnHeader3.Width = 415;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 215;
            // 
            // progressBar
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar, 3);
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(3, 484);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(837, 21);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // cbmMaxRes
            // 
            this.cbmMaxRes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbmMaxRes.FormattingEnabled = true;
            this.cbmMaxRes.Location = new System.Drawing.Point(307, 0);
            this.cbmMaxRes.Margin = new System.Windows.Forms.Padding(0);
            this.cbmMaxRes.Name = "cbmMaxRes";
            this.cbmMaxRes.Size = new System.Drawing.Size(100, 21);
            this.cbmMaxRes.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Max Video Resolution:";
            // 
            // numericUpDown
            // 
            this.numericUpDown.Location = new System.Drawing.Point(129, 0);
            this.numericUpDown.Margin = new System.Windows.Forms.Padding(0);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(59, 20);
            this.numericUpDown.TabIndex = 9;
            this.numericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Max Parallel Downloads:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.96195F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.03806F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPlaylist, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.listView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFetchPlaylist, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.progressBar, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnDownload, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(843, 508);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.cbmMaxRes);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(167, 459);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(487, 19);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 508);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblPercent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Youtube Sycronizer";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPlaylist;
        private System.Windows.Forms.Button btnFetchPlaylist;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox cbmMaxRes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}

