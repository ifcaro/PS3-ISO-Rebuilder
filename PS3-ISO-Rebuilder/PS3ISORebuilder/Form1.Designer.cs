using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PS3ISORebuilder
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
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFolder_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenISO_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.OpenIRD_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BuildItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outtypeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.BuildISO_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.BuildODE_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.ExtractISO_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompressISO_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Label_info_ird = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label_Info = new System.Windows.Forms.Label();
            this.infotitle = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.StatusLabel1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.Label_IsoCheck = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.Summary_Label = new System.Windows.Forms.Label();
            this.Panel7 = new System.Windows.Forms.Panel();
            this.Panel6 = new System.Windows.Forms.Panel();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Panel5 = new System.Windows.Forms.Panel();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel4 = new System.Windows.Forms.Panel();
            this.ListView1 = new PS3ISORebuilder.ListView_nf();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MenuStrip1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenMenuItem,
            this.BuildItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(7, 4, 0, 4);
            this.MenuStrip1.Size = new System.Drawing.Size(774, 27);
            this.MenuStrip1.TabIndex = 3;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenFolder_MenuItem,
            this.OpenISO_MenuItem,
            this.ToolStripSeparator1,
            this.OpenIRD_MenuItem});
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.Size = new System.Drawing.Size(48, 19);
            this.OpenMenuItem.Text = "Open";
            // 
            // OpenFolder_MenuItem
            // 
            this.OpenFolder_MenuItem.Name = "OpenFolder_MenuItem";
            this.OpenFolder_MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End)));
            this.OpenFolder_MenuItem.Size = new System.Drawing.Size(171, 22);
            this.OpenFolder_MenuItem.Text = "JB Folder";
            this.OpenFolder_MenuItem.Click += new System.EventHandler(this.OpenFolder_MenuItem_Click);
            // 
            // OpenISO_MenuItem
            // 
            this.OpenISO_MenuItem.Name = "OpenISO_MenuItem";
            this.OpenISO_MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.OpenISO_MenuItem.Size = new System.Drawing.Size(171, 22);
            this.OpenISO_MenuItem.Text = "ISO";
            this.OpenISO_MenuItem.Click += new System.EventHandler(this.OpenISO_MenuItem_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // OpenIRD_MenuItem
            // 
            this.OpenIRD_MenuItem.Name = "OpenIRD_MenuItem";
            this.OpenIRD_MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this.OpenIRD_MenuItem.Size = new System.Drawing.Size(171, 22);
            this.OpenIRD_MenuItem.Text = "IRD";
            this.OpenIRD_MenuItem.Click += new System.EventHandler(this.OpenIRD_MenuItem_Click);
            // 
            // BuildItem
            // 
            this.BuildItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outtypeComboBox,
            this.BuildISO_MenuItem,
            this.ToolStripMenuItem3,
            this.BuildODE_MenuItem,
            this.ToolStripMenuItem2,
            this.ExtractISO_MenuItem,
            this.CompressISO_MenuItem});
            this.BuildItem.Name = "BuildItem";
            this.BuildItem.Size = new System.Drawing.Size(92, 19);
            this.BuildItem.Text = "Build / Extract";
            // 
            // outtypeComboBox
            // 
            this.outtypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outtypeComboBox.Enabled = false;
            this.outtypeComboBox.Items.AddRange(new object[] {
            "Plain Header",
            "3K3Y Header",
            "COBRA Header"});
            this.outtypeComboBox.Name = "outtypeComboBox";
            this.outtypeComboBox.Size = new System.Drawing.Size(200, 23);
            this.outtypeComboBox.SelectedIndexChanged += new System.EventHandler(this.outtypeComboBox_Click);
            // 
            // BuildISO_MenuItem
            // 
            this.BuildISO_MenuItem.Enabled = false;
            this.BuildISO_MenuItem.Name = "BuildISO_MenuItem";
            this.BuildISO_MenuItem.Size = new System.Drawing.Size(260, 22);
            this.BuildISO_MenuItem.Text = "Build ISO";
            this.BuildISO_MenuItem.Click += new System.EventHandler(this.BuildISOItem_Click);
            // 
            // ToolStripMenuItem3
            // 
            this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
            this.ToolStripMenuItem3.Size = new System.Drawing.Size(257, 6);
            // 
            // BuildODE_MenuItem
            // 
            this.BuildODE_MenuItem.Enabled = false;
            this.BuildODE_MenuItem.Name = "BuildODE_MenuItem";
            this.BuildODE_MenuItem.Size = new System.Drawing.Size(260, 22);
            this.BuildODE_MenuItem.Text = "Build ISO without IRD (GenPS3iso)";
            this.BuildODE_MenuItem.Click += new System.EventHandler(this.BuildODE_MenuItem_Click);
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(257, 6);
            // 
            // ExtractISO_MenuItem
            // 
            this.ExtractISO_MenuItem.Enabled = false;
            this.ExtractISO_MenuItem.Name = "ExtractISO_MenuItem";
            this.ExtractISO_MenuItem.ShowShortcutKeys = false;
            this.ExtractISO_MenuItem.Size = new System.Drawing.Size(260, 22);
            this.ExtractISO_MenuItem.Text = "Extract ISO";
            this.ExtractISO_MenuItem.Click += new System.EventHandler(this.ExtractISOToolStripMenuItem_Click);
            // 
            // CompressISO_MenuItem
            // 
            this.CompressISO_MenuItem.Enabled = false;
            this.CompressISO_MenuItem.Name = "CompressISO_MenuItem";
            this.CompressISO_MenuItem.Size = new System.Drawing.Size(260, 22);
            this.CompressISO_MenuItem.Text = "Compress ISO -> CSO";
            this.CompressISO_MenuItem.Click += new System.EventHandler(this.CompressISOToolStripMenuItem_Click);
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.Label_info_ird);
            this.Panel1.Controls.Add(this.Label6);
            this.Panel1.Controls.Add(this.Label_Info);
            this.Panel1.Controls.Add(this.infotitle);
            this.Panel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Panel1.Location = new System.Drawing.Point(-1, 30);
            this.Panel1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(786, 58);
            this.Panel1.TabIndex = 6;
            // 
            // Label_info_ird
            // 
            this.Label_info_ird.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_info_ird.AutoEllipsis = true;
            this.Label_info_ird.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Label_info_ird.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Label_info_ird.Location = new System.Drawing.Point(103, 32);
            this.Label_info_ird.Name = "Label_info_ird";
            this.Label_info_ird.Size = new System.Drawing.Size(668, 15);
            this.Label_info_ird.TabIndex = 3;
            this.Label_info_ird.Click += new System.EventHandler(this.Label_info_ird_Click);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(4, 32);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(93, 15);
            this.Label6.TabIndex = 2;
            this.Label6.Text = "Game Info IRD:";
            // 
            // Label_Info
            // 
            this.Label_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_Info.AutoEllipsis = true;
            this.Label_Info.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Label_Info.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Label_Info.Location = new System.Drawing.Point(103, 9);
            this.Label_Info.Name = "Label_Info";
            this.Label_Info.Size = new System.Drawing.Size(668, 15);
            this.Label_Info.TabIndex = 1;
            this.Label_Info.Click += new System.EventHandler(this.Label_Info_Click);
            // 
            // infotitle
            // 
            this.infotitle.AutoSize = true;
            this.infotitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infotitle.Location = new System.Drawing.Point(4, 9);
            this.infotitle.Name = "infotitle";
            this.infotitle.Size = new System.Drawing.Size(69, 15);
            this.infotitle.TabIndex = 0;
            this.infotitle.Text = "Game Info:";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Enabled = false;
            this.Cancel_Button.Location = new System.Drawing.Point(9, 5);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(31, 26);
            this.Cancel_Button.TabIndex = 9;
            this.Cancel_Button.Text = "X";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressBar1.Location = new System.Drawing.Point(46, 10);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(146, 16);
            this.ProgressBar1.TabIndex = 10;
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLabel1.AutoEllipsis = true;
            this.StatusLabel1.Location = new System.Drawing.Point(200, 10);
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(478, 20);
            this.StatusLabel1.TabIndex = 11;
            // 
            // Panel2
            // 
            this.Panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel2.Controls.Add(this.Label_IsoCheck);
            this.Panel2.Controls.Add(this.Label5);
            this.Panel2.Controls.Add(this.Cancel_Button);
            this.Panel2.Controls.Add(this.StatusLabel1);
            this.Panel2.Controls.Add(this.ProgressBar1);
            this.Panel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Panel2.Location = new System.Drawing.Point(-3, 277);
            this.Panel2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(786, 39);
            this.Panel2.TabIndex = 7;
            // 
            // Label_IsoCheck
            // 
            this.Label_IsoCheck.AllowDrop = true;
            this.Label_IsoCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_IsoCheck.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label_IsoCheck.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label_IsoCheck.Location = new System.Drawing.Point(684, -1);
            this.Label_IsoCheck.Name = "Label_IsoCheck";
            this.Label_IsoCheck.Size = new System.Drawing.Size(49, 39);
            this.Label_IsoCheck.TabIndex = 13;
            this.Label_IsoCheck.Text = "ISO Check";
            this.Label_IsoCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label_IsoCheck.DragDrop += new System.Windows.Forms.DragEventHandler(this.Label_IsoCheck_DragDrop);
            this.Label_IsoCheck.DragEnter += new System.Windows.Forms.DragEventHandler(this.Label_IsoCheck_DragEnter);
            // 
            // Label5
            // 
            this.Label5.AllowDrop = true;
            this.Label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label5.Location = new System.Drawing.Point(732, -1);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(49, 39);
            this.Label5.TabIndex = 12;
            this.Label5.Text = "MD5";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label5.DragDrop += new System.Windows.Forms.DragEventHandler(this.Label5_DragDrop);
            this.Label5.DragEnter += new System.Windows.Forms.DragEventHandler(this.Label5_DragEnter);
            // 
            // Panel3
            // 
            this.Panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel3.BackColor = System.Drawing.SystemColors.Info;
            this.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel3.Controls.Add(this.Summary_Label);
            this.Panel3.Controls.Add(this.Panel7);
            this.Panel3.Controls.Add(this.Panel6);
            this.Panel3.Controls.Add(this.Label4);
            this.Panel3.Controls.Add(this.Label3);
            this.Panel3.Controls.Add(this.Label2);
            this.Panel3.Controls.Add(this.Panel5);
            this.Panel3.Controls.Add(this.Label1);
            this.Panel3.Controls.Add(this.Panel4);
            this.Panel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Panel3.Location = new System.Drawing.Point(-6, 252);
            this.Panel3.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(786, 26);
            this.Panel3.TabIndex = 13;
            // 
            // Summary_Label
            // 
            this.Summary_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Summary_Label.Location = new System.Drawing.Point(13, 5);
            this.Summary_Label.Name = "Summary_Label";
            this.Summary_Label.Size = new System.Drawing.Size(468, 15);
            this.Summary_Label.TabIndex = 14;
            // 
            // Panel7
            // 
            this.Panel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel7.BackColor = System.Drawing.Color.Coral;
            this.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel7.Location = new System.Drawing.Point(708, 4);
            this.Panel7.Name = "Panel7";
            this.Panel7.Size = new System.Drawing.Size(16, 16);
            this.Panel7.TabIndex = 7;
            // 
            // Panel6
            // 
            this.Panel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel6.BackColor = System.Drawing.Color.Gold;
            this.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel6.Location = new System.Drawing.Point(642, 4);
            this.Panel6.Name = "Panel6";
            this.Panel6.Size = new System.Drawing.Size(16, 16);
            this.Panel6.TabIndex = 6;
            // 
            // Label4
            // 
            this.Label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(726, 5);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(48, 15);
            this.Label4.TabIndex = 5;
            this.Label4.Text = "missing";
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(660, 5);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(42, 15);
            this.Label3.TabIndex = 4;
            this.Label3.Text = "invalid";
            // 
            // Label2
            // 
            this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(601, 5);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(32, 15);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "valid";
            // 
            // Panel5
            // 
            this.Panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel5.BackColor = System.Drawing.Color.LightGreen;
            this.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel5.Location = new System.Drawing.Point(583, 4);
            this.Panel5.Name = "Panel5";
            this.Panel5.Size = new System.Drawing.Size(16, 16);
            this.Panel5.TabIndex = 2;
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(503, 5);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(72, 15);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "not required";
            // 
            // Panel4
            // 
            this.Panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel4.BackColor = System.Drawing.SystemColors.Window;
            this.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel4.Location = new System.Drawing.Point(485, 4);
            this.Panel4.Name = "Panel4";
            this.Panel4.Size = new System.Drawing.Size(16, 16);
            this.Panel4.TabIndex = 0;
            // 
            // ListView1
            // 
            this.ListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2,
            this.ColumnHeader3,
            this.ColumnHeader4});
            this.ListView1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListView1.FullRowSelect = true;
            this.ListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListView1.Location = new System.Drawing.Point(0, 88);
            this.ListView1.Name = "ListView1";
            this.ListView1.Size = new System.Drawing.Size(774, 164);
            this.ListView1.TabIndex = 7;
            this.ListView1.UseCompatibleStateImageBehavior = false;
            this.ListView1.View = System.Windows.Forms.View.Details;
            this.ListView1.SelectedIndexChanged += new System.EventHandler(this.ListView1_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Tag = "4";
            this.ColumnHeader1.Text = "File";
            this.ColumnHeader1.Width = 335;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Tag = "3";
            this.ColumnHeader2.Text = "MD5";
            this.ColumnHeader2.Width = 249;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Tag = "2";
            this.ColumnHeader3.Text = "Sector";
            this.ColumnHeader3.Width = 79;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Tag = "2";
            this.ColumnHeader4.Text = "Length";
            this.ColumnHeader4.Width = 86;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(774, 314);
            this.Controls.Add(this.Panel3);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.ListView1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.MenuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.MinimumSize = new System.Drawing.Size(790, 260);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "PS3-ISO-Rebuilder";
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.Panel2.ResumeLayout(false);
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip MenuStrip1;
        private ToolStripMenuItem OpenMenuItem;
        private ToolStripMenuItem OpenISO_MenuItem;
        private ToolStripMenuItem OpenFolder_MenuItem;
        private Panel Panel1;
        private ListView_nf ListView1;
        private ColumnHeader ColumnHeader1;
        private ColumnHeader ColumnHeader2;
        private ColumnHeader ColumnHeader3;
        private ColumnHeader ColumnHeader4;
        private ToolStripMenuItem BuildItem;
        private Label Label_Info;
        private Label infotitle;
        private Button Cancel_Button;
        private ProgressBar ProgressBar1;
        private Label StatusLabel1;
        private Panel Panel2;
        private Panel Panel3;
        private Panel Panel7;
        private Panel Panel6;
        private Label Label4;
        private Label Label3;
        private Label Label2;
        private Panel Panel5;
        private Label Label1;
        private Panel Panel4;
        private ToolStripMenuItem BuildODE_MenuItem;
        private ToolStripMenuItem BuildISO_MenuItem;
        private Label Label_info_ird;
        private Label Label6;
        private Label Summary_Label;
        private Label Label5;
        private ToolStripSeparator ToolStripMenuItem2;
        private ToolStripMenuItem ExtractISO_MenuItem;
        private ToolStripComboBox outtypeComboBox;
        private ToolStripSeparator ToolStripMenuItem3;
        private ToolStripSeparator ToolStripSeparator1;
        private ToolStripMenuItem OpenIRD_MenuItem;
        private Label Label_IsoCheck;
        private ToolStripMenuItem CompressISO_MenuItem;
    }
}