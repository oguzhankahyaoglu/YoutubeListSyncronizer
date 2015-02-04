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
            this.SuspendLayout();
            // 
            // txtPlaylist
            // 
            this.txtPlaylist.Location = new System.Drawing.Point(126, 12);
            this.txtPlaylist.Name = "txtPlaylist";
            this.txtPlaylist.Size = new System.Drawing.Size(488, 20);
            this.txtPlaylist.TabIndex = 0;
            this.txtPlaylist.Text = "https://www.youtube.com/playlist?list=FL0O6m-jrVllHCo94bJ-a9bA";
            // 
            // btnFetchPlaylist
            // 
            this.btnFetchPlaylist.Location = new System.Drawing.Point(620, 10);
            this.btnFetchPlaylist.Name = "btnFetchPlaylist";
            this.btnFetchPlaylist.Size = new System.Drawing.Size(96, 23);
            this.btnFetchPlaylist.TabIndex = 1;
            this.btnFetchPlaylist.Text = "Fetch Videos";
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
            this.folderBrowser.RootFolder = System.Environment.SpecialFolder.MyVideos;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Youtube Playlist Link:";
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(560, 537);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(156, 23);
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
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(13, 39);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(703, 494);
            this.listView.TabIndex = 5;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
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
            this.columnHeader3.Width = 386;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 101;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 569);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(703, 22);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // cbmMaxRes
            // 
            this.cbmMaxRes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbmMaxRes.FormattingEnabled = true;
            this.cbmMaxRes.Location = new System.Drawing.Point(454, 539);
            this.cbmMaxRes.Name = "cbmMaxRes";
            this.cbmMaxRes.Size = new System.Drawing.Size(100, 21);
            this.cbmMaxRes.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(335, 544);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Max Video Resolution:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 603);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbmMaxRes);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.btnFetchPlaylist);
            this.Controls.Add(this.txtPlaylist);
            this.MaximumSize = new System.Drawing.Size(744, 641);
            this.MinimumSize = new System.Drawing.Size(744, 641);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Youtube List Syncronizer";
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
    }
}

