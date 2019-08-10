using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using PS3ISORebuilder.IRDFile;
using PS3ISORebuilder.ISO9660;
using PS3ISORebuilder.My;
using PS3ISORebuilder.My.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace PS3ISORebuilder
{
    public partial class Form1 : Form
    {
        private class sectorkey
        {
            public long sector;

            public string name;

            public sectorkey(long sec, string fname)
            {
                sector = sec;
                name = fname;
            }
        }

        private enum outtype : byte
        {
            plain,
            k3,
            ode
        }

        public delegate void SetText(Label x, string i);

        public delegate void AddlistItems(ListViewItem[] i);

        public delegate void RemovelistItems();

        public delegate void invoke_Button(Button item, bool @bool);

        public delegate void invoke_toolstripitem(ToolStripMenuItem item, bool @bool);

        public delegate void invoke_toolstripComboBox(ToolStripComboBox item, bool @bool);

        public delegate void SetProgressPercentCallback(ProgressBar o, uint percent);

        public delegate void dropallow(Label item, bool @bool);

        private const int Bufferzize = 65536;

        private SFOReader _SFO;

        private IRD _IRD;

        private PS3ISORebuilder.ISO9660.ISO9660 _ISO;

        private string _isofile;

        private bool _isosplitt;

        private bool _splittoutput;

        private string root_dir;

        private long root_size;

        private string game_id;

        private string game_title;

        private string ird_id;

        private string ird_title;

        private string gameinfo;

        private string filestatus;

        private string outfile;

        private string outpath;

        private RegistryKey Settings;

        private string lastjbfolder;

        private string lastisofolder;

        private string lastirdfolder;

        private Dictionary<string, ListViewItem> isolist;

        private List<string> dragfiles;

        private Dictionary<sectorkey, ListViewItem> irdlist;

        private List<string> irddirlist;

        private Dictionary<string, ListViewItem> matchItems;

        private Dictionary<string, ListViewItem> wrongItems;

        private Dictionary<string, ListViewItem> missItems;

        private Dictionary<string, ListViewItem> backuplist;

        private outtype @out;

        private BackgroundWorker readisoThread;

        private BackgroundWorker buildIsoThread;

        private BackgroundWorker extractIsoThread;

        private BackgroundWorker compressIsoThread;

        private BackgroundWorker md5Thread;

        private BackgroundWorker checkISOThread;

        private BackgroundWorker buildOdeTread;

        private Process buildOdeProcess;

        private BackgroundWorker readDirThread;

        private BackgroundWorker readIrdThread;

        private BackgroundWorker compare;

        public Form1()
        {
            base.Load += Form1_Load;
            base.FormClosing += Form1_FormClosing;
            _isofile = "";
            _isosplitt = false;
            _splittoutput = false;
            root_dir = "";
            root_size = 0L;
            game_id = "";
            game_title = "";
            ird_id = "";
            ird_title = "";
            gameinfo = "Game ID: {0}    Game Name: {1}    Game Version: {2}    App Version: {3}    Update: {4}";
            filestatus = "{0}/{1} ({2})";
            outfile = "";
            outpath = "";
            lastjbfolder = "";
            lastisofolder = "";
            lastirdfolder = "";
            isolist = new Dictionary<string, ListViewItem>();
            dragfiles = new List<string>();
            irdlist = new Dictionary<sectorkey, ListViewItem>();
            irddirlist = new List<string>();
            matchItems = new Dictionary<string, ListViewItem>();
            wrongItems = new Dictionary<string, ListViewItem>();
            missItems = new Dictionary<string, ListViewItem>();
            backuplist = new Dictionary<string, ListViewItem>();
            readisoThread = new BackgroundWorker();
            buildIsoThread = new BackgroundWorker();
            extractIsoThread = new BackgroundWorker();
            compressIsoThread = new BackgroundWorker();
            md5Thread = new BackgroundWorker();
            checkISOThread = new BackgroundWorker();
            buildOdeTread = new BackgroundWorker();
            buildOdeProcess = new Process();
            readDirThread = new BackgroundWorker();
            readIrdThread = new BackgroundWorker();
            compare = new BackgroundWorker();
            @out = outtype.plain;
            InitializeComponent();
        }


        private void drop_allow(Label item, bool @bool)
        {
            if (item.InvokeRequired)
            {
                dropallow method = drop_allow;
                item.Invoke(method, item, @bool);
            }
            else
            {
                item.AllowDrop = @bool;
            }
        }

        private void setbutton(Button item, bool @bool)
        {
            if (item.InvokeRequired)
            {
                invoke_Button method = setbutton;
                item.Invoke(method, item, @bool);
            }
            else
            {
                item.Enabled = @bool;
            }
        }

        private void settoolstripComboBox(ToolStripComboBox item, bool @bool)
        {
            if (item.GetCurrentParent().InvokeRequired)
            {
                invoke_toolstripComboBox method = settoolstripComboBox;
                item.GetCurrentParent().Invoke(method, item, @bool);
            }
            else
            {
                item.Enabled = @bool;
            }
        }

        private void settoolstripitem(ToolStripMenuItem item, bool @bool)
        {
            if (item.GetCurrentParent().InvokeRequired)
            {
                invoke_toolstripitem method = settoolstripitem;
                item.GetCurrentParent().Invoke(method, item, @bool);
            }
            else
            {
                item.Enabled = @bool;
            }
        }

        private void Textset(Label obj, string j)
        {
            if (obj.InvokeRequired)
            {
                SetText method = Textset;
                obj.Invoke(method, obj, j);
            }
            else
            {
                obj.Text = j;
            }
        }

        private void listItemsAdd(ListViewItem[] i)
        {
            if (ListView1.InvokeRequired)
            {
                AddlistItems method = listItemsAdd;
                ListView1.Invoke(method, new object[1]
                {
                    i
                });
            }
            else
            {
                ListView1.Items.AddRange(i);
            }
        }

        private void listItemsRemove()
        {
            if (ListView1.InvokeRequired)
            {
                RemovelistItems method = listItemsRemove;
                ListView1.Invoke(method);
            }
            else
            {
                ListView1.Items.Clear();
            }
        }

        public void SetProgressPercent(ProgressBar o, uint percent)
        {
            if (o.InvokeRequired)
            {
                SetProgressPercentCallback method = SetProgressPercent;
                Invoke(method, o, percent);
                return;
            }
            o.Maximum = 100;
            if ((long)percent >= 0L && (long)percent <= 100L)
            {
                o.Value = checked((int)percent);
            }
            else
            {
                o.Value = 0;
            }
        }

        private void OpenFolder_MenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select your PS3 (JB) Gamefolder";
            folderBrowserDialog.ShowNewFolderButton = false;
            if (Operators.CompareString(root_dir, "", TextCompare: false) != 0)
            {
                folderBrowserDialog.SelectedPath = root_dir;
            }
            else
            {
                folderBrowserDialog.SelectedPath = lastjbfolder;
            }
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                lastjbfolder = new DirectoryInfo(folderBrowserDialog.SelectedPath).Parent.FullName;
                Settings.SetValue("lastjbfolder", lastjbfolder);
                _ISO = null;
                root_dir = "";
                _isofile = "";
                disable_controls();
                Textset(Summary_Label, "");
                listItemsRemove();
                if (File.Exists(folderBrowserDialog.SelectedPath + "\\PS3_GAME\\PARAM.SFO"))
                {
                    FileStream fileStream = File.OpenRead(folderBrowserDialog.SelectedPath + "\\PS3_GAME\\PARAM.SFO");
                    _SFO = new SFOReader(fileStream);
                    fileStream.Close();
                    if (_SFO.Entries.ContainsKey("TITLE_ID"))
                    {
                        string text = "00.00";
                        string text2 = "00.00";
                        string text3 = "";
                        if (_SFO.Entries.ContainsKey("TITLE_ID"))
                        {
                            game_id = _SFO.Entries["TITLE_ID"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        if (_SFO.Entries.ContainsKey("TITLE"))
                        {
                            game_title = _SFO.Entries["TITLE"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        if (_SFO.Entries.ContainsKey("VERSION"))
                        {
                            text = _SFO.Entries["VERSION"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        if (_SFO.Entries.ContainsKey("APP_VER"))
                        {
                            text2 = _SFO.Entries["APP_VER"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        if (_SFO.Entries.ContainsKey("PS3_SYSTEM_VER"))
                        {
                            text3 = fixupdate(_SFO.Entries["PS3_SYSTEM_VER"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", ""));
                        }
                        Textset(Label_Info, string.Format(gameinfo, game_id, game_title, text, text2, text3));
                        Textset(StatusLabel1, "reading Folder");
                        root_dir = folderBrowserDialog.SelectedPath;
                        disable_controls();
                        readDirThread = new BackgroundWorker();
                        readDirThread.DoWork += new DoWorkEventHandler(this.readDirThread_dowork);
                        readDirThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.readDirThread_Completed);
                        readDirThread.WorkerSupportsCancellation = true;
                        readDirThread.RunWorkerAsync();
                    }
                }
                else
                {
                    enable_controls();
                    game_id = "";
                    game_title = "";
                    _isofile = "";
                    root_dir = "";
                    Textset(Label_Info, "");
                    Textset(Summary_Label, "");
                    MessageBox.Show("is not a PS3 (JB) Gamefolder");
                }
            }
            else
            {
                root_dir = "";
            }
        }

        public void readDirThread_dowork(object sender, DoWorkEventArgs e)
        {
            isolist = new Dictionary<string, ListViewItem>();
            if (!Directory.Exists(root_dir + "\\PS3_UPDATE"))
            {
                try
                {
                    Directory.CreateDirectory(root_dir + "\\PS3_UPDATE");
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    ProjectData.ClearProjectError();
                }
            }
            if (!File.Exists(root_dir + "\\PS3_UPDATE\\PS3UPDAT.PUP"))
            {
                DialogResult dialogResult = MessageBox.Show("PS3UPDAT.PUP does not exist!\n\nDo you want to open the browser to download the matching PS3UPDAT.PUP?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    string text = Conversions.ToString(getupdateURL(fixupdate(Conversions.ToString(_SFO.Entries["PS3_SYSTEM_VER"].Data))));
                    if (Operators.CompareString(text, "", TextCompare: false) != 0)
                    {
                        Process.Start(text);
                        DialogResult dialogResult2 = MessageBox.Show("Click OK to proceed if you have downloaded and \nextracted the matching PS3UPDAT.PUP to this JB RiP Folder/PS3_UPDATE", Text, MessageBoxButtons.OK);
                    }
                }
            }
            if (File.Exists(root_dir + "\\PS3_UPDATE\\PS3UPDAT.PUP") && new FileInfo(root_dir + "\\PS3_UPDATE\\PS3UPDAT.PUP").Length < 268435456)
            {
                Textset(StatusLabel1, "resizing PS3UPDAT.PUP");
                FileStream fileStream = File.OpenWrite(root_dir + "\\PS3_UPDATE\\PS3UPDAT.PUP");
                fileStream.Seek(268435455L, SeekOrigin.Begin);
                fileStream.WriteByte(0);
                fileStream.Flush();
                fileStream.Close();
                Textset(StatusLabel1, "resizing finished");
            }
            string[] files = Directory.GetFiles(root_dir, "*.*", SearchOption.AllDirectories);
            root_size = 0L;
            long num = 0L;
            string[] array = files;
            int num2 = 0;
            checked
            {
                while (true)
                {
                    if (num2 < array.Length)
                    {
                        string text2 = array[num2];
                        num++;
                        FileInfo fileInfo = new FileInfo(text2);
                        string text3 = Strings.Replace(fileInfo.FullName, root_dir, "");
                        try
                        {
                            fileInfo.Attributes = FileAttributes.Normal;
                        }
                        catch (Exception projectError2)
                        {
                            ProjectData.SetProjectError(projectError2);
                            ProjectData.ClearProjectError();
                        }
                        if (readDirThread.CancellationPending)
                        {
                            break;
                        }
                        try
                        {
                            Textset(StatusLabel1, string.Format(filestatus, num, files.Count(), text2.Replace(root_dir, "")));
                            using (FileStream fileStream2 = fileInfo.OpenRead())
                            {
                                ListViewItem value = new ListViewItem(new string[4]
                                {
                                    text2.Replace(root_dir, ""),
                                    getMD5(fileStream2),
                                    "",
                                    Conversions.ToString(fileStream2.Length)
                                });
                                isolist.Add(text3.ToUpper(), value);
                                root_size += fileStream2.Length;
                            }
                        }
                        catch (Exception projectError3)
                        {
                            ProjectData.SetProjectError(projectError3);
                            ProjectData.ClearProjectError();
                        }
                        num2++;
                        continue;
                    }
                    return;
                }
                e.Cancel = true;
            }
        }

        private void readDirThread_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled | (e.Error != null))
            {
                isolist.Clear();
                ListView1.Items.Clear();
                Textset(StatusLabel1, "");
                Textset(Label_Info, "");
                game_id = "";
                game_title = "";
                root_dir = "";
                root_size = 0L;
            }
            else
            {
                Textset(StatusLabel1, "read finished");
                if (irdlist.Count > 0)
                {
                    compare = new BackgroundWorker();
                    compare.DoWork += new DoWorkEventHandler(this.compare_worker);
                    compare.RunWorkerAsync();
                }
                else
                {
                    listItemsAdd(isolist.Values.ToArray());
                }
            }
            enable_controls();
        }

        private void readirdThread_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            runcompare();
        }

        private void OpenISO_MenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ISO files (*.cso;*.iso;*.iso.0)|*.cso;*.iso;*.iso.0";
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory = lastisofolder;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            root_dir = "";
            lastisofolder = new FileInfo(openFileDialog.FileName).DirectoryName;
            Settings.SetValue("lastisofolder", lastisofolder);
            listItemsRemove();
            Textset(StatusLabel1, "reading ISO");
            Textset(Summary_Label, "");
            _isofile = openFileDialog.FileName;
            if (_isofile.EndsWith(".iso") | _isofile.EndsWith(".cso"))
            {
                _ISO = new PS3ISORebuilder.ISO9660.ISO9660(_isofile);
                _isosplitt = false;
            }
            else
            {
                _ISO = new PS3ISORebuilder.ISO9660.ISO9660(getsplitfile(_isofile));
                _isosplitt = true;
            }
            if (_ISO.fileexist("\\PS3_GAME\\PARAM.SFO"))
            {
                _SFO = new SFOReader(_ISO.findfile("\\PS3_GAME\\PARAM.SFO"));
                if (_SFO.Entries.ContainsKey("TITLE_ID"))
                {
                    string text = "00.00";
                    string text2 = "00.00";
                    string text3 = "";
                    if (_SFO.Entries.ContainsKey("TITLE_ID"))
                    {
                        game_id = _SFO.Entries["TITLE_ID"].Data.ToString().Replace("\r", "").Replace("\n", "")
                            .Replace("\r\n", "");
                    }
                    if (_SFO.Entries.ContainsKey("TITLE"))
                    {
                        game_title = _SFO.Entries["TITLE"].Data.ToString().Replace("\r", "").Replace("\n", "")
                            .Replace("\r\n", "");
                    }
                    if (_SFO.Entries.ContainsKey("VERSION"))
                    {
                        text = _SFO.Entries["VERSION"].Data.ToString().Replace("\r", "").Replace("\n", "")
                            .Replace("\r\n", "");
                    }
                    if (_SFO.Entries.ContainsKey("APP_VER"))
                    {
                        text2 = _SFO.Entries["APP_VER"].Data.ToString().Replace("\r", "").Replace("\n", "")
                            .Replace("\r\n", "");
                    }
                    if (_SFO.Entries.ContainsKey("PS3_SYSTEM_VER"))
                    {
                        text3 = fixupdate(_SFO.Entries["PS3_SYSTEM_VER"].Data.ToString().Replace("\r", "").Replace("\n", "")
                            .Replace("\r\n", ""));
                    }
                    Textset(Label_Info, string.Format(gameinfo, game_id, game_title, text, text2, text3));
                    listItemsRemove();
                    disable_controls();
                    readisoThread = new BackgroundWorker();
                    readisoThread.DoWork += new DoWorkEventHandler(this.readisoThread_dowork);
                    readisoThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.readisoThread_completed);
                    readisoThread.WorkerSupportsCancellation = true;
                    readisoThread.WorkerReportsProgress = false;
                    readisoThread.RunWorkerAsync();
                }
            }
            else
            {
                enable_controls();
                game_id = "";
                game_title = "";
                _isofile = "";
                Textset(Label_Info, "");
                MessageBox.Show("not a valid ps3 iso");
                _ISO.close();
            }
        }

        public void readisoThread_dowork(object sender, DoWorkEventArgs e)
        {
            isolist = new Dictionary<string, ListViewItem>();
            int num = 0;
            PS3ISORebuilder.ISO9660.DirectoryRecord[] array = _ISO.filelist.Values.ToArray();
            int num2 = 0;
            checked
            {
                while (true)
                {
                    if (num2 < array.Length)
                    {
                        PS3ISORebuilder.ISO9660.DirectoryRecord directoryRecord = array[num2];
                        if (readisoThread.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        num++;
                        string text = directoryRecord.fullname.Replace("\\ ", "\\#");
                        Textset(StatusLabel1, Conversions.ToString(num) + "/" + Conversions.ToString(_ISO.filelist.Count) + " (" + text + ")");
                        ListViewItem listViewItem = new ListViewItem(text);
                        listViewItem.SubItems.AddRange(new string[3]
                        {
                            getMD5(directoryRecord),
                            Conversion.Hex(directoryRecord.firstDataSector),
                            Conversions.ToString(directoryRecord.Length)
                        });
                        isolist.Add(text.ToUpper(), listViewItem);
                        if (readisoThread.CancellationPending)
                        {
                            break;
                        }
                        num2++;
                        continue;
                    }
                    return;
                }
                e.Cancel = true;
            }
        }

        public void readisoThread_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled | (e.Error != null))
            {
                game_id = "";
                game_title = "";
                _isofile = "";
                Textset(Label_Info, "");
                Textset(StatusLabel1, "");
                listItemsRemove();
                isolist.Clear();
                SetProgressPercent(ProgressBar1, 0u);
            }
            else
            {
                Textset(StatusLabel1, "read finished");
                SetProgressPercent(ProgressBar1, 0u);
                if (irdlist.Count > 0)
                {
                    compare = new BackgroundWorker();
                    compare.DoWork += new DoWorkEventHandler(this.compare_worker);
                    compare.RunWorkerAsync();
                }
                else
                {
                    listItemsAdd(isolist.Values.ToArray());
                }
            }
            enable_controls();
            _ISO.close();
        }

        private void OpenIRD_MenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ird files (*.ird)|*.ird";
            openFileDialog.InitialDirectory = lastirdfolder;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lastirdfolder = new FileInfo(openFileDialog.FileName).DirectoryName;
                Settings.SetValue("lastirdfolder", lastirdfolder);
                disable_controls();
                _IRD = new IRD(openFileDialog.FileName);
                if ((_IRD != null) & (_IRD.version >= 6))
                {
                    ird_id = _IRD.GAMEID.ToString().Replace("\r", "").Replace("\n", "")
                        .Replace("\r\n", "");
                    ird_title = _IRD.GAMENAME.ToString().Replace("\r", "").Replace("\n", "")
                        .Replace("\r\n", "");
                    Textset(Label_info_ird, string.Format(gameinfo, ird_id, ird_title, _IRD.GameVersion.Replace("\r", "").Replace("\n", "").Replace("\r\n", ""), _IRD.AppVersion.Replace("\r", "").Replace("\n", "").Replace("\r\n", ""), _IRD.UpdateVersion.Replace("\r", "").Replace("\n", "").Replace("\r\n", "")));
                    Textset(Summary_Label, "");
                    listItemsRemove();
                    readIrdThread = new BackgroundWorker();
                    readIrdThread.DoWork += new DoWorkEventHandler(this.readirdThread_work);
                    readIrdThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.readirdThread_Completed);
                    readIrdThread.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Your IRD file is incorrect");
                    enable_controls();
                }
            }
        }

        public void readirdThread_work(object sender, DoWorkEventArgs e)
        {
            setbutton(Cancel_Button, @bool: false);
            int num = 0;
            irdlist = new Dictionary<sectorkey, ListViewItem>();
            irddirlist = new List<string>();
            Textset(StatusLabel1, "IRD Load ...");
            num = 0;
            checked
            {
                foreach (KeyValuePair<string, PS3ISORebuilder.IRDFile.DirectoryRecord> item in _IRD.isoheader.dirlist)
                {
                    if (readIrdThread.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    num++;
                    PS3ISORebuilder.IRDFile.DirectoryRecord value = item.Value;
                    Textset(StatusLabel1, string.Format(filestatus, num, _IRD.isoheader.dirlist.Count, value.entrypath));
                    SetProgressPercent(ProgressBar1, (uint)Math.Round(unchecked((double)checked(num * 100) / (double)_IRD.isoheader.dirlist.Count)));
                    irddirlist.Add(value.entrypath);
                }
                num = 0;
                using (Dictionary<string, PS3ISORebuilder.IRDFile.DirectoryRecord>.Enumerator enumerator2 = _IRD.isoheader.filelist.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator2.MoveNext())
                        {
                            return;
                        }
                        KeyValuePair<string, PS3ISORebuilder.IRDFile.DirectoryRecord> current2 = enumerator2.Current;
                        if (readIrdThread.CancellationPending)
                        {
                            break;
                        }
                        num++;
                        PS3ISORebuilder.IRDFile.DirectoryRecord value2 = current2.Value;
                        string text = value2.entrypath.Replace("\\ ", "\\#");
                        Textset(StatusLabel1, string.Format(filestatus, num, _IRD.isoheader.filelist.Count, text));
                        SetProgressPercent(ProgressBar1, (uint)Math.Round(unchecked((double)checked(num * 100) / (double)_IRD.isoheader.filelist.Count)));
                        ListViewItem listViewItem = new ListViewItem(text);
                        listViewItem.SubItems.AddRange(new string[3]
                        {
                            value2.md5String,
                            Conversion.Hex(value2.firstDataSector),
                            Conversions.ToString(value2.Length)
                        });
                        irdlist.Add(new sectorkey(value2.firstDataSector, text.ToUpper()), listViewItem);
                    }
                    e.Cancel = true;
                }
            }
        }

        private void runcompare()
        {
            disable_controls();
            compare = new BackgroundWorker();
            compare.DoWork += new DoWorkEventHandler(this.compare_worker);
            compare.RunWorkerAsync();
        }

        private void compare_worker(object sender, DoWorkEventArgs e)
        {
            checked
            {
                if (isolist.Count > 0)
                {
                    Textset(StatusLabel1, "comparing");
                    SetProgressPercent(ProgressBar1, 0u);
                    matchItems = new Dictionary<string, ListViewItem>();
                    wrongItems = new Dictionary<string, ListViewItem>();
                    missItems = new Dictionary<string, ListViewItem>();
                    backuplist = new Dictionary<string, ListViewItem>();
                    backuplist = new Dictionary<string, ListViewItem>(isolist);
                    if ((Operators.CompareString(root_dir, "", TextCompare: false) != 0) & (Operators.CompareString(game_id, ird_id, TextCompare: false) == 0))
                    {
                        foreach (string item in irddirlist)
                        {
                            if (!Directory.Exists(root_dir + item))
                            {
                                try
                                {
                                    Directory.CreateDirectory(root_dir + item);
                                }
                                catch (Exception projectError)
                                {
                                    ProjectData.SetProjectError(projectError);
                                    ProjectData.ClearProjectError();
                                }
                            }
                        }
                    }
                    foreach (KeyValuePair<sectorkey, ListViewItem> item2 in from v in irdlist
                                                                            orderby v.Key.sector
                                                                            select v)
                    {
                        string text = item2.Key.name.ToUpper();
                        if (isolist.ContainsKey(text) | isolist.ContainsKey(text.Remove(text.Length - 1)))
                        {
                            ListViewItem listViewItem = null;
                            if (isolist.ContainsKey(text))
                            {
                                listViewItem = isolist[text];
                                isolist.Remove(text);
                            }
                            else if (isolist.ContainsKey(text.Remove(text.Length - 1)))
                            {
                                listViewItem = isolist[text.Remove(text.Length - 1)];
                                isolist.Remove(text.Remove(text.Length - 1));
                            }
                            if (Operators.CompareString(listViewItem.SubItems[1].Text, item2.Value.SubItems[1].Text, TextCompare: false) == 0)
                            {
                                item2.Value.BackColor = Color.LightGreen;
                                matchItems.Add(text, item2.Value);
                            }
                            else if ((Operators.CompareString(text, "\\PS3_GAME\\LICDIR\\LIC.DAT", TextCompare: false) == 0) & (Operators.CompareString(game_id, ird_id, TextCompare: false) == 0) & (Operators.CompareString(root_dir, "", TextCompare: false) != 0))
                            {
                                byte[] lIC = GetLIC(game_id);
                                if (Operators.CompareString(getMD5(lIC), item2.Value.SubItems[1].Text, TextCompare: false) == 0)
                                {
                                    File.WriteAllBytes(root_dir + text, lIC);
                                    item2.Value.BackColor = Color.LightGreen;
                                    matchItems.Add(text, item2.Value);
                                }
                                else
                                {
                                    item2.Value.BackColor = Color.Gold;
                                    wrongItems.Add(text, item2.Value);
                                }
                            }
                            else
                            {
                                item2.Value.BackColor = Color.Gold;
                                wrongItems.Add(text, item2.Value);
                            }
                        }
                        else if ((Operators.CompareString(item2.Value.SubItems[3].Text, "0", TextCompare: false) == 0) & (Operators.CompareString(game_id, ird_id, TextCompare: false) == 0) & (Operators.CompareString(root_dir, "", TextCompare: false) != 0))
                        {
                            try
                            {
                                FileStream fileStream = File.Open(root_dir + item2.Value.Text, FileMode.Create);
                                fileStream.Close();
                                item2.Value.BackColor = Color.LightGreen;
                                matchItems.Add(text, item2.Value);
                            }
                            catch (Exception projectError2)
                            {
                                ProjectData.SetProjectError(projectError2);
                                item2.Value.BackColor = Color.Coral;
                                missItems.Add(text, item2.Value);
                                ProjectData.ClearProjectError();
                            }
                        }
                        else if ((Operators.CompareString(text, "\\PS3_GAME\\LICDIR\\LIC.DAT", TextCompare: false) == 0) & (Operators.CompareString(game_id, ird_id, TextCompare: false) == 0) & (Operators.CompareString(root_dir, "", TextCompare: false) != 0))
                        {
                            byte[] lIC2 = GetLIC(game_id);
                            if (Operators.CompareString(getMD5(lIC2), item2.Value.SubItems[1].Text, TextCompare: false) == 0)
                            {
                                try
                                {
                                    File.WriteAllBytes(root_dir + text, lIC2);
                                    item2.Value.BackColor = Color.LightGreen;
                                    matchItems.Add(text, item2.Value);
                                }
                                catch (Exception projectError3)
                                {
                                    ProjectData.SetProjectError(projectError3);
                                    item2.Value.BackColor = Color.Coral;
                                    missItems.Add(text, item2.Value);
                                    ProjectData.ClearProjectError();
                                }
                            }
                            else
                            {
                                item2.Value.BackColor = Color.Coral;
                                missItems.Add(text, item2.Value);
                            }
                        }
                        else
                        {
                            item2.Value.BackColor = Color.Coral;
                            missItems.Add(text, item2.Value);
                        }
                    }
                    listItemsRemove();
                    listItemsAdd(isolist.Values.ToArray());
                    listItemsAdd(missItems.Values.ToArray());
                    listItemsAdd(wrongItems.Values.ToArray());
                    listItemsAdd(matchItems.Values.ToArray());
                    Textset(Summary_Label, Conversions.ToString(matchItems.Count) + " valid | " + Conversions.ToString(wrongItems.Count) + " invalid | " + Conversions.ToString(missItems.Count) + " missing | " + Conversions.ToString(isolist.Count) + " not required");
                    Textset(StatusLabel1, "Compare finished");
                    isolist.Clear();
                    isolist = new Dictionary<string, ListViewItem>(backuplist);
                }
                else
                {
                    listItemsRemove();
                    listItemsAdd(irdlist.Values.ToArray());
                    Textset(StatusLabel1, "IRD loaded");
                }
                enable_controls();
            }
        }

        private void BuildISOItem_Click(object sender, EventArgs e)
        {
            if (!((irdlist.Count != 0) & (_IRD != null)))
            {
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "iso files (*.iso)|*.iso";
            saveFileDialog.FileName = game_id + "-[" + cleanfilename(game_title) + "].iso";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (Operators.CompareString(_isofile, saveFileDialog.FileName, TextCompare: false) != 0)
            {
                if (decimal.Compare(new decimal(new DriveInfo(saveFileDialog.FileName).AvailableFreeSpace), new decimal(_IRD.disksize)) > 0)
                {
                    disable_controls();
                    outfile = saveFileDialog.FileName;
                    if ((_ISO != null) & (Operators.CompareString(_isofile, "", TextCompare: false) != 0) & !_isosplitt)
                    {
                        _ISO = new PS3ISORebuilder.ISO9660.ISO9660(_isofile);
                    }
                    else if ((_ISO != null) & (Operators.CompareString(_isofile, "", TextCompare: false) != 0) & _isosplitt)
                    {
                        _ISO = new PS3ISORebuilder.ISO9660.ISO9660(getsplitfile(_isofile));
                    }
                    buildIsoThread = new BackgroundWorker();
                    buildIsoThread.DoWork += new DoWorkEventHandler(this.buildiso);
                    buildIsoThread.ProgressChanged += new ProgressChangedEventHandler(this.buildIsoThread_Progress);
                    buildIsoThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.buildisoThreadfinished);
                    buildIsoThread.WorkerSupportsCancellation = true;
                    buildIsoThread.WorkerReportsProgress = true;
                    buildIsoThread.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("not enough free space on target drive");
                }
            }
            else
            {
                MessageBox.Show("choose another file, this is the source file");
            }
        }

        public void buildiso(object sender, DoWorkEventArgs e)
        {
            int num = 0;
            checked
            {
                try
                {
                    FileStream fileStream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
                    _IRD.header.Position = 0L;
                    _IRD.header.CopyTo(fileStream);
                    byte[] array = _IRD.Data2;
                    try
                    {
                        array = D2_Random(_IRD.Data2);
                    }
                    catch (Exception projectError)
                    {
                        ProjectData.SetProjectError(projectError);
                        ProjectData.ClearProjectError();
                    }
                    if (@out == outtype.k3)
                    {
                        fileStream.Position = 3952L;
                        byte[] bytes = Encoding.ASCII.GetBytes("Decrypted 3K BLD");
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Write(_IRD.Data1, 0, _IRD.Data1.Length);
                        fileStream.Write(array, 0, array.Length);
                        fileStream.Write(_IRD.PIC, 0, _IRD.PIC.Length);
                    }
                    else if (@out == outtype.ode)
                    {
                        fileStream.Position = 3952L;
                        byte[] bytes2 = Encoding.ASCII.GetBytes("Decr. COBRA IRD ");
                        fileStream.Write(bytes2, 0, bytes2.Length);
                        fileStream.Write(_IRD.Data1, 0, _IRD.Data1.Length);
                        fileStream.Write(array, 0, array.Length);
                        fileStream.Write(_IRD.PIC, 0, _IRD.PIC.Length);
                    }
                    foreach (KeyValuePair<sectorkey, ListViewItem> item in from v in irdlist
                                                                           orderby v.Key.sector
                                                                           select v)
                    {
                        Stream stream = null;
                        string text = item.Value.Text;
                        num++;
                        if (Operators.CompareString(root_dir, "", TextCompare: false) != 0)
                        {
                            if (File.Exists(root_dir + text))
                            {
                                stream = File.OpenRead(root_dir + text);
                            }
                            else if (File.Exists(root_dir + text.Remove(text.Length - 1)))
                            {
                                stream = File.OpenRead(root_dir + text.Remove(text.Length - 1));
                            }
                        }
                        else if (_ISO != null)
                        {
                            if (_ISO.fileexist(text))
                            {
                                stream = _ISO.findfile(text);
                            }
                            else if (_ISO.fileexist(text.Remove(text.Length - 1)))
                            {
                                stream = _ISO.findfile(text.Remove(text.Length - 1));
                            }
                        }
                        if (stream != null)
                        {
                            stream.Position = 0L;
                            fileStream.Position = item.Key.sector * _IRD.isoheader.Blocksize;
                            Textset(StatusLabel1, string.Format(filestatus, num, _IRD.isoheader.filelist.Count, text));
                            long length = stream.Length;
                            long num2 = 0L;
                            using (HashAlgorithm hashAlgorithm = MD5.Create())
                            {
                                byte[] array2;
                                int num3;
                                do
                                {
                                    array2 = new byte[65537];
                                    num3 = stream.Read(array2, 0, array2.Length);
                                    num2 += num3;
                                    hashAlgorithm.TransformBlock(array2, 0, num3, null, 0);
                                    fileStream.Write(array2, 0, num3);
                                    if (num2 > 0 && length > 0)
                                    {
                                        buildIsoThread.ReportProgress((int)Math.Round(unchecked((double)num2 / (double)length * 100.0)));
                                    }
                                    if (buildIsoThread.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        fileStream.Close();
                                        if (File.Exists(outfile))
                                        {
                                            File.Delete(outfile);
                                        }
                                        return;
                                    }
                                }
                                while (num3 != 0);
                                hashAlgorithm.TransformFinalBlock(array2, 0, 0);
                                if (Operators.CompareString(BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "").ToUpper(), BitConverter.ToString(_IRD.FileHashes[item.Key.sector]).Replace("-", "").ToUpper(), TextCompare: false) != 0)
                                {
                                    MessageBox.Show("write fail on file " + text);
                                    e.Cancel = true;
                                    fileStream.Close();
                                    if (File.Exists(outfile))
                                    {
                                        File.Delete(outfile);
                                    }
                                    return;
                                }
                            }
                            stream.Close();
                        }
                        else
                        {
                            MessageBox.Show("File not Found " + text);
                        }
                    }
                    fileStream.Position = fileStream.Length;
                    _IRD.footer.Position = 0L;
                    _IRD.footer.CopyTo(fileStream);
                    fileStream.Close();
                    if ((_ISO != null) & (Operators.CompareString(_isofile, "", TextCompare: false) != 0))
                    {
                        _ISO.close();
                    }
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    ProjectData.ClearProjectError();
                }
            }
        }

        private void buildisoThreadfinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled | (e.Error != null))
            {
                Textset(StatusLabel1, "");
            }
            else
            {
                Textset(StatusLabel1, "Build finished");
            }
            SetProgressPercent(ProgressBar1, 0u);
            enable_controls();
        }

        private void buildIsoThread_Progress(object sender, ProgressChangedEventArgs e)
        {
            SetProgressPercent(ProgressBar1, checked((uint)e.ProgressPercentage));
        }

        private void BuildODE_MenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllBytes(Path.GetTempPath() + "\\genps3iso.exe", Resources.genps3iso);
            }
            catch (Exception projectError)
            {
                ProjectData.SetProjectError(projectError);
                ProjectData.ClearProjectError();
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ird files (*.iso)|*.iso";
            saveFileDialog.FileName = game_id + "-[" + cleanfilename(game_title) + "].iso";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            outfile = saveFileDialog.FileName;
            if (new DriveInfo(saveFileDialog.FileName).AvailableFreeSpace > root_size)
            {
                if (DialogResult.Yes == MessageBox.Show("split output", "", MessageBoxButtons.YesNo))
                {
                    _splittoutput = true;
                }
                else
                {
                    _splittoutput = false;
                }
                disable_controls();
                setbutton(Cancel_Button, @bool: false);
                Textset(StatusLabel1, "Build start!");
                buildOdeTread = new BackgroundWorker();
                buildOdeTread.DoWork += new DoWorkEventHandler(this.buildOdeTread_DoWork);
                buildOdeTread.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("not enough free space on target drive");
            }
        }

        private void buildOdeTread_DoWork(object sender, DoWorkEventArgs e)
        {
            if (File.Exists(Path.GetTempPath() + "\\genps3iso.exe") & (Operators.CompareString(root_dir, "", TextCompare: false) != 0))
            {
                try
                {
                    buildOdeProcess = new Process();
                    buildOdeProcess.Exited += new System.EventHandler(this.buildOdeProcess_Exited);
                    buildOdeProcess.EnableRaisingEvents = true;
                    string arguments = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("/c genps3iso.exe ", Interaction.IIf(_splittoutput, " -s ", "")), " \""), root_dir), "\" \""), outfile), ""));
                    ProcessStartInfo startInfo = buildOdeProcess.StartInfo;
                    startInfo.Arguments = arguments;
                    startInfo.FileName = "cmd";
                    startInfo.WorkingDirectory = Path.GetTempPath();
                    startInfo.UseShellExecute = true;
                    startInfo.RedirectStandardOutput = false;
                    startInfo.RedirectStandardError = false;
                    startInfo = null;
                    buildOdeProcess.Start();
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    ProjectData.ClearProjectError();
                }
            }
        }

        private void buildOdeProcess_Exited(object sender, EventArgs e)
        {
            Textset(StatusLabel1, "Build finished");
            enable_controls();
        }

        private void Label5_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
                try
                {
                    string[] array2 = array;
                    foreach (string text in array2)
                    {
                        if (File.Exists(text))
                        {
                            dragfiles.Add(text);
                        }
                    }
                    disable_controls();
                    md5Thread = new BackgroundWorker();
                    md5Thread.DoWork += new DoWorkEventHandler(this.filesmd5);
                    md5Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.filemd5_WorkerCompleted);
                    md5Thread.WorkerSupportsCancellation = true;
                    md5Thread.RunWorkerAsync();
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    ProjectData.ClearProjectError();
                }
            }
        }

        private void filesmd5(object sender, DoWorkEventArgs e)
        {
            int num = 0;
            foreach (string dragfile in dragfiles)
            {
                num = checked(num + 1);
                FileStream fileStream = File.OpenRead(dragfile);
                Textset(StatusLabel1, string.Format(filestatus, num, dragfiles.Count, dragfile));
                MessageBox.Show(getMD5(fileStream), dragfile, MessageBoxButtons.OK);
                fileStream.Close();
            }
            dragfiles.Clear();
        }

        private void filemd5_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            enable_controls();
            Textset(StatusLabel1, "");
            SetProgressPercent(ProgressBar1, 0u);
        }

        private void Label5_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Label_IsoCheck_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
                try
                {
                    string[] array2 = array;
                    foreach (string text in array2)
                    {
                        if (File.Exists(text))
                        {
                            FileInfo fileInfo = new FileInfo(text);
                            if ((Operators.CompareString(fileInfo.Extension.ToLower(), ".iso", TextCompare: false) == 0) | (Operators.CompareString(fileInfo.Extension.ToLower(), ".cso", TextCompare: false) == 0))
                            {
                                dragfiles.Add(text);
                            }
                        }
                    }
                    disable_controls();
                    checkISOThread = new BackgroundWorker();
                    checkISOThread.DoWork += new DoWorkEventHandler(this.checkISO);
                    checkISOThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.checkISOThread_RunWorkerCompleted);
                    checkISOThread.RunWorkerAsync();
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    ProjectData.ClearProjectError();
                }
            }
        }

        private void checkISO(object sender, DoWorkEventArgs e)
        {
            int num = 0;
            checked
            {
                foreach (string dragfile in dragfiles)
                {
                    num++;
                    Textset(StatusLabel1, string.Format(filestatus, num, dragfiles.Count, dragfile));
                    PS3ISORebuilder.ISO9660.ISO9660 iSO = new PS3ISORebuilder.ISO9660.ISO9660(dragfile);
                    if (iSO.fileexist("\\PS3_GAME\\PARAM.SFO"))
                    {
                        SFOReader sFOReader = new SFOReader(iSO.findfile("\\PS3_GAME\\PARAM.SFO"));
                        string text = "";
                        string text2 = "";
                        if (sFOReader.Entries.ContainsKey("TITLE_ID"))
                        {
                            text = sFOReader.Entries["TITLE_ID"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        if (sFOReader.Entries.ContainsKey("TITLE"))
                        {
                            text2 = sFOReader.Entries["TITLE"].Data.ToString().Replace("\r", "").Replace("\n", "")
                                .Replace("\r\n", "");
                        }
                        long num2 = (long)iSO.Disksize;
                        long num3 = 0L;
                        foreach (PS3ISORebuilder.ISO9660.DirectoryRecord value in iSO.filelist.Values)
                        {
                            num3 += value.Length;
                        }
                        bool flag = false;
                        if (iSO.fileexist("\\PS3_GAME\\LICDIR\\LIC.DAT"))
                        {
                            BinaryReader binaryReader = new BinaryReader(iSO.findfile("\\PS3_GAME\\LICDIR\\LIC.DAT"));
                            if (Operators.CompareString(Encoding.ASCII.GetString(binaryReader.ReadBytes(8), 0, 8), "PS3LICDA", TextCompare: false) == 0)
                            {
                                flag = true;
                            }
                        }
                        else if (iSO.fileexist("\\PS3_GAME\\USRDIR\\EBOOT.BIN"))
                        {
                            BinaryReader binaryReader2 = new BinaryReader(iSO.findfile("\\PS3_GAME\\USRDIR\\EBOOT.BIN"));
                            if (Operators.CompareString(Encoding.ASCII.GetString(binaryReader2.ReadBytes(3), 0, 3), "SCE", TextCompare: false) == 0)
                            {
                                flag = true;
                            }
                        }
                        uint num4 = 0u;
                        try
                        {
                            num4 = (uint)(unchecked((long)Swap(BitConverter.ToUInt32(iSO.readsector(0uL), 0))) * 2L - 1);
                        }
                        catch (Exception ex)
                        {
                            ProjectData.SetProjectError(ex);
                            Exception ex2 = ex;
                            ProjectData.ClearProjectError();
                        }
                        byte[] array = iSO.readsector(1uL);
                        string left = Encoding.ASCII.GetString(array, 0, 12).Trim();
                        bool flag2 = false;
                        if (Operators.CompareString(left, "PlayStation3", TextCompare: false) == 0)
                        {
                            flag2 = true;
                        }
                        string text3 = (array[1904] <= 0) ? "Unknow" : Encoding.ASCII.GetString(array, 1904, 16).Trim();
                        string text4 = BitConverter.ToString(array, 1920, 16).Replace("-", "").ToLower();
                        string text5 = BitConverter.ToString(array, 1930, 16).Replace("-", "").ToLower();
                        MessageBox_RichText messageBox_RichText = new MessageBox_RichText();
                        messageBox_RichText.RichTextBox1.Rtf = unchecked("{\\rtf1 {\\b GameID:}\\tab " + text + " \\par{\\b Title:}\\tab\\tab " + text2 + " \\par{\\b Decrypted:}\\tab " + flag.ToString() + " \\par{\\b PS3DiskIdent:}\\tab " + flag2.ToString() + " \\par{\\b Disksize:}\\tab " + Conversions.ToString(Math.Round((double)num2 / 1024.0 / 1024.0, 0)) + " MB \\par{\\b Datasize:}\\tab " + Conversions.ToString(Math.Round((double)num3 / 1024.0 / 1024.0, 0))) + " MB \\par{\\b Directorys:}\\tab " + Conversions.ToString(iSO.dirlist.Count) + " \\par{\\b Files:}\\tab\\tab " + Conversions.ToString(iSO.filelist.Count) + " \\par \\par{\\b Generator:}\\tab " + text3 + " \\par{\\b Data1:}\\tab\\tab " + text4 + " \\par{\\b Data2:}\\tab\\tab " + text5 + " \\par}";
                        iSO.close();
                        messageBox_RichText.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No PS3 Diskimage!");
                    }
                }
                dragfiles.Clear();
            }
        }

        private void checkISOThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            enable_controls();
            Textset(StatusLabel1, "");
            SetProgressPercent(ProgressBar1, 0u);
        }

        private void Label_IsoCheck_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ExtractISOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((isolist.Count > 0) & File.Exists(_isofile))
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    outpath = Path.Combine(folderBrowserDialog.SelectedPath, game_id + "-[" + cleanfilename(game_title) + "]");
                    Directory.CreateDirectory(outpath);
                    extractIsoThread = new BackgroundWorker();
                    extractIsoThread.DoWork += new DoWorkEventHandler(this.extractIso);
                    extractIsoThread.ProgressChanged += new ProgressChangedEventHandler(this.extractiso_ProgressChanged);
                    extractIsoThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.extractIso_RunWorkerCompleted);
                    extractIsoThread.WorkerSupportsCancellation = true;
                    extractIsoThread.WorkerReportsProgress = true;
                    extractIsoThread.RunWorkerAsync();
                    disable_controls();
                }
            }
            else
            {
                outpath = "";
                enable_controls();
            }
        }

        public void extractIso(object sender, DoWorkEventArgs e)
        {
            checked
            {
                if ((isolist.Count > 0) & File.Exists(_isofile) & Directory.Exists(outpath))
                {
                    if ((_ISO != null) & (Operators.CompareString(_isofile, "", TextCompare: false) != 0) & !_isosplitt)
                    {
                        _ISO = new PS3ISORebuilder.ISO9660.ISO9660(_isofile);
                    }
                    else if ((_ISO != null) & (Operators.CompareString(_isofile, "", TextCompare: false) != 0) & _isosplitt)
                    {
                        _ISO = new PS3ISORebuilder.ISO9660.ISO9660(getsplitfile(_isofile));
                    }
                    foreach (PS3ISORebuilder.ISO9660.DirectoryRecord value in _ISO.dirlist.Values)
                    {
                        try
                        {
                            string path = Path.Combine(outpath + value.fullname);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                        }
                        catch (Exception projectError)
                        {
                            ProjectData.SetProjectError(projectError);
                            ProjectData.ClearProjectError();
                        }
                    }
                    int num = 0;
                    foreach (PS3ISORebuilder.ISO9660.DirectoryRecord value2 in _ISO.filelist.Values)
                    {
                        num++;
                        Textset(StatusLabel1, string.Format(filestatus, num, _ISO.filelist.Count, value2.fullname));
                        FileStream fileStream = new FileStream(Path.Combine(outpath + value2.fullname), FileMode.Create, FileAccess.ReadWrite);
                        long length = value2.Length;
                        long num2 = 0L;
                        int num3;
                        do
                        {
                            byte[] array = new byte[65537];
                            num3 = value2.Read(array, 0, array.Length);
                            num2 += num3;
                            fileStream.Write(array, 0, num3);
                            if (num2 > 0 && length > 0)
                            {
                                extractIsoThread.ReportProgress((int)Math.Round(unchecked((double)num2 / (double)length * 100.0)));
                            }
                            if (extractIsoThread.CancellationPending)
                            {
                                e.Cancel = true;
                                fileStream.Close();
                                return;
                            }
                        }
                        while (num3 != 0);
                        fileStream.Close();
                    }
                }
            }
        }

        public void extractiso_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgressPercent(ProgressBar1, checked((uint)e.ProgressPercentage));
        }

        public void extractIso_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled | (e.Error != null))
            {
                Textset(StatusLabel1, "");
            }
            else
            {
                Textset(StatusLabel1, "Extract finished");
            }
            SetProgressPercent(ProgressBar1, 0u);
            enable_controls();
        }

        private void CompressISOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(_isofile))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "iso files (*.cso)|*.cso";
                saveFileDialog.FileName = game_id + "-[" + cleanfilename(game_title) + "].cso";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outfile = saveFileDialog.FileName;
                    compressIsoThread = new BackgroundWorker();
                    compressIsoThread.DoWork += new DoWorkEventHandler(this.compressIso);
                    compressIsoThread.ProgressChanged += new ProgressChangedEventHandler(this.compressiso_ProgressChanged);
                    compressIsoThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.compressIso_RunWorkerCompleted);
                    compressIsoThread.WorkerSupportsCancellation = true;
                    compressIsoThread.WorkerReportsProgress = true;
                    compressIsoThread.RunWorkerAsync();
                    disable_controls();
                }
            }
            else
            {
                outfile = "";
                enable_controls();
            }
        }

        public void compressIso(object sender, DoWorkEventArgs e)
        {
            PS3ISORebuilder.ISO9660.ISO9660 iSO = new PS3ISORebuilder.ISO9660.ISO9660(_isofile);
            ulong num = iSO.VolumeDescriptor.VolumeSpaceSize;
            FileStream fileStream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            byte[] array = new byte[16]
            {
                67,
                80,
                83,
                51,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            };
            IntToBytes(array, num, 4L);
            IntToBytes(array, iSO.Blocksize, 12L);
            fileStream.Write(array, 0, array.Length);
            checked
            {
                byte[] array2 = new byte[Convert.ToInt32(decimal.Subtract(decimal.Multiply(decimal.Add(new decimal(num), decimal.One), new decimal(8L)), decimal.One)) + 1];
                binaryWriter.Write(array2, 0, array2.Length);
                long num2 = 0L;
                ulong num3 = num;
                for (ulong num4 = unchecked((ulong)num2); num4 <= num3; num4++)
                {
                    Textset(StatusLabel1, string.Format(filestatus, num4.ToString(), num.ToString(), outfile));
                    if (compressIsoThread.CancellationPending)
                    {
                        e.Cancel = true;
                        fileStream.Close();
                        File.Delete(outfile);
                        return;
                    }
                    ulong val = (ulong)fileStream.Position;
                    if (num4 == num)
                    {
                        IntToBytes(array2, val, Convert.ToInt64(decimal.Multiply(new decimal(num4), new decimal(8L))));
                    }
                    else
                    {
                        byte[] array3 = iSO.readsector(num4);
                        MemoryStream memoryStream = new MemoryStream();
                        DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: true);
                        deflateStream.Write(array3, 0, array3.Length);
                        deflateStream.Close();
                        deflateStream.Dispose();
                        byte[] array4 = new byte[(int)(memoryStream.Length - 1) + 1];
                        memoryStream.Seek(0L, SeekOrigin.Begin);
                        memoryStream.Read(array4, 0, array4.Length);
                        memoryStream.Close();
                        if (array4.Length >= array3.Length)
                        {
                            IntToBytes(array2, val, Convert.ToUInt32(decimal.Multiply(new decimal(num4), new decimal(8L))));
                            fileStream.Write(array3, 0, array3.Length);
                        }
                        else
                        {
                            IntToBytes(array2, val, Convert.ToUInt32(decimal.Multiply(new decimal(num4), new decimal(8L))));
                            fileStream.Write(array4, 0, array4.Length);
                        }
                    }
                    compressIsoThread.ReportProgress(Convert.ToInt32(decimal.Divide(decimal.Multiply(new decimal(100L), new decimal(num4)), new decimal(num))));
                }
                fileStream.Seek(array.Length, SeekOrigin.Begin);
                fileStream.Write(array2, 0, array2.Length);
                fileStream.Close();
                fileStream = null;
            }
        }

        public void compressiso_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgressPercent(ProgressBar1, checked((uint)e.ProgressPercentage));
        }

        public void compressIso_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled | (e.Error != null))
            {
                Textset(StatusLabel1, "");
            }
            else
            {
                Textset(StatusLabel1, "Compress finished");
            }
            SetProgressPercent(ProgressBar1, 0u);
            enable_controls();
        }

        private byte[] IntToBytes(byte[] bytes, uint val, long offset)
        {
            byte[] bytes2 = BitConverter.GetBytes(val);
            Array.Copy(bytes2, 0L, bytes, offset, bytes2.Length);
            return bytes;
        }

        private byte[] IntToBytes(byte[] bytes, ulong val, long offset)
        {
            byte[] bytes2 = BitConverter.GetBytes(val);
            Array.Copy(bytes2, 0L, bytes, offset, bytes2.Length);
            return bytes;
        }

        private Stream getsplitfile(string fname)
        {
            string[] array = fname.Split('.');
            MultiStream multiStream = new MultiStream();
            multiStream.Add(File.OpenRead(fname));
            bool flag = true;
            while (flag)
            {
                string[] array2 = array;
                string[] array3 = array2;
                int num = checked(array.Length - 1);
                array3[num] = Conversions.ToString(Conversions.ToDouble(array2[num]) + 1.0);
                if (File.Exists(Strings.Join(array, ".")))
                {
                    multiStream.Add(File.OpenRead(Strings.Join(array, ".")));
                }
                else
                {
                    flag = false;
                }
            }
            return multiStream;
        }

        public string fixupdate(string x)
        {
            string text = x.TrimStart('0');
            return text.Substring(0, checked(text.Length - 2));
        }

        public string cleanfilename(string tex)
        {
            StringBuilder stringBuilder = new StringBuilder(tex);
            stringBuilder.Replace("®", "(R)").Replace("™", "(TM)").Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("ß", "ss")
                .Replace("Ä", "Ae")
                .Replace("Ö", "Oe")
                .Replace("Ü", "Ue")
                .Replace("'", "_");
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string text = string.Join("", stringBuilder.ToString().Split(invalidFileNameChars, StringSplitOptions.RemoveEmptyEntries));
            return text.Trim();
        }

        private void Label_Info_Click(object sender, EventArgs e)
        {
            if (Operators.CompareString(game_id, "", TextCompare: false) != 0)
            {
                Process.Start("http://jonnysp.bplaced.net/?search=" + game_id);
            }
        }

        private void Label_info_ird_Click(object sender, EventArgs e)
        {
            if (Operators.CompareString(ird_id, "", TextCompare: false) != 0)
            {
                Process.Start("http://jonnysp.bplaced.net/?search=" + ird_id);
            }
        }

        private void outtypeComboBox_Click(object sender, EventArgs e)
        {
            if (Operators.CompareString(outtypeComboBox.Text, "3K3Y Header", TextCompare: false) == 0)
            {
                @out = outtype.k3;
            }
            else if (Operators.CompareString(outtypeComboBox.Text, "COBRA Header", TextCompare: false) == 0)
            {
                @out = outtype.ode;
            }
            else
            {
                @out = outtype.plain;
            }
            Settings.SetValue("outtype", @out.GetHashCode());
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "PS3-ISO-Rebuilder - " + MyProject.Application.Info.Version.ToString();
            Settings = Registry.CurrentUser.CreateSubKey("Software\\PS3-ISO-Rebuilder");
            Settings = Registry.CurrentUser.OpenSubKey("Software\\PS3-ISO-Rebuilder", writable: true);
            try
            {
                lastjbfolder = Settings.GetValue("lastjbfolder", "").ToString();
            }
            catch (Exception projectError)
            {
                ProjectData.SetProjectError(projectError);
                lastjbfolder = "";
                ProjectData.ClearProjectError();
            }
            try
            {
                lastisofolder = Settings.GetValue("lastisofolder", "").ToString();
            }
            catch (Exception projectError2)
            {
                ProjectData.SetProjectError(projectError2);
                lastisofolder = "";
                ProjectData.ClearProjectError();
            }
            try
            {
                lastirdfolder = Settings.GetValue("lastirdfolder", "").ToString();
            }
            catch (Exception projectError3)
            {
                ProjectData.SetProjectError(projectError3);
                lastirdfolder = "";
                ProjectData.ClearProjectError();
            }
            try
            {
                @out = (outtype)checked((byte)Conversions.ToInteger(Settings.GetValue("outtype", 1).ToString()));
            }
            catch (Exception projectError4)
            {
                ProjectData.SetProjectError(projectError4);
                @out = outtype.k3;
                ProjectData.ClearProjectError();
            }
            if (@out == outtype.plain)
            {
                outtypeComboBox.Text = "Plain Header";
            }
            else if (@out == outtype.k3)
            {
                outtypeComboBox.Text = "3K3Y Header";
            }
            else
            {
                outtypeComboBox.Text = "COBRA Header";
            }
            enable_controls();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (readisoThread.IsBusy | readDirThread.IsBusy | readIrdThread.IsBusy | buildIsoThread.IsBusy | md5Thread.IsBusy | compare.IsBusy | extractIsoThread.IsBusy | compressIsoThread.IsBusy)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            if (readisoThread.IsBusy)
            {
                readisoThread.CancelAsync();
            }
            if (readDirThread.IsBusy)
            {
                readDirThread.CancelAsync();
            }
            if (buildIsoThread.IsBusy)
            {
                buildIsoThread.CancelAsync();
            }
            if (md5Thread.IsBusy)
            {
                md5Thread.CancelAsync();
            }
            if (extractIsoThread.IsBusy)
            {
                extractIsoThread.CancelAsync();
            }
            if (compressIsoThread.IsBusy)
            {
                compressIsoThread.CancelAsync();
            }
        }

        public byte[] GetLIC(string _gameid)
        {
            byte[] array = new byte[65536];
            MemoryStream memoryStream = new MemoryStream(array);
            memoryStream.Write(new byte[32]
            {
                80,
                83,
                51,
                76,
                73,
                67,
                68,
                65,
                0,
                0,
                0,
                1,
                128,
                0,
                0,
                0,
                0,
                0,
                9,
                0,
                0,
                0,
                8,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1
            }, 0, 32);
            memoryStream.Seek(2048L, SeekOrigin.Begin);
            memoryStream.Write(new byte[1]
            {
                1
            }, 0, 1);
            byte[] bytes = Encoding.ASCII.GetBytes(_gameid);
            memoryStream.Write(bytes, 0, bytes.Length);
            CRC32 cRC = new CRC32();
            CRC32 cRC2 = cRC;
            Stream stream = memoryStream;
            int crc = cRC2.GetCrc32(ref stream, 2304L);
            memoryStream = (MemoryStream)stream;
            int value = crc;
            byte[] bytes2 = BitConverter.GetBytes(value);
            Array.Reverse(bytes2);
            memoryStream.Seek(32L, SeekOrigin.Begin);
            memoryStream.Write(bytes2, 0, bytes2.Length);
            memoryStream.Flush();
            return array;
        }

        public string getMD5(byte[] buff)
        {
            MemoryStream memoryStream = new MemoryStream(buff);
            long length = memoryStream.Length;
            long num = 0L;
            using (HashAlgorithm hashAlgorithm = MD5.Create())
            {
                byte[] array;
                int num2;
                do
                {
                    array = new byte[131073];
                    num2 = memoryStream.Read(array, 0, array.Length);
                    num = checked(num + num2);
                    hashAlgorithm.TransformBlock(array, 0, num2, null, 0);
                }
                while (num2 != 0);
                hashAlgorithm.TransformFinalBlock(array, 0, 0);
                memoryStream.Close();
                return BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "").ToUpper();
            }
        }

        public string getMD5(Stream filestream)
        {
            filestream.Position = 0L;
            long length = filestream.Length;
            long num = 0L;
            checked
            {
                using (HashAlgorithm hashAlgorithm = MD5.Create())
                {
                    byte[] array;
                    int num2;
                    do
                    {
                        array = new byte[131073];
                        num2 = filestream.Read(array, 0, array.Length);
                        num += num2;
                        hashAlgorithm.TransformBlock(array, 0, num2, null, 0);
                        if (num > 0 && length > 0)
                        {
                            SetProgressPercent(ProgressBar1, (uint)Math.Round(unchecked((double)num / (double)length * 100.0)));
                        }
                        if (readDirThread.CancellationPending | md5Thread.CancellationPending | readisoThread.CancellationPending)
                        {
                            return "";
                        }
                    }
                    while (num2 != 0);
                    hashAlgorithm.TransformFinalBlock(array, 0, 0);
                    return BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "").ToUpper();
                }
            }
        }

        public void disable_controls()
        {
            SetProgressPercent(ProgressBar1, 0u);
            drop_allow(Label5, @bool: false);
            drop_allow(Label_IsoCheck, @bool: false);
            settoolstripComboBox(outtypeComboBox, @bool: false);
            settoolstripitem(BuildISO_MenuItem, @bool: false);
            settoolstripitem(BuildODE_MenuItem, @bool: false);
            settoolstripitem(OpenIRD_MenuItem, @bool: false);
            settoolstripitem(OpenISO_MenuItem, @bool: false);
            settoolstripitem(OpenFolder_MenuItem, @bool: false);
            settoolstripitem(ExtractISO_MenuItem, @bool: false);
            settoolstripitem(CompressISO_MenuItem, @bool: false);
            setbutton(Cancel_Button, @bool: true);
        }

        public void enable_controls()
        {
            SetProgressPercent(ProgressBar1, 0u);
            drop_allow(Label5, @bool: true);
            drop_allow(Label_IsoCheck, @bool: true);
            setbutton(Cancel_Button, @bool: false);
            settoolstripitem(OpenISO_MenuItem, @bool: true);
            settoolstripitem(OpenFolder_MenuItem, @bool: true);
            settoolstripitem(OpenIRD_MenuItem, @bool: true);
            if ((missItems.Count == 0) & (wrongItems.Count == 0) & (irdlist.Count > 0) & (isolist.Count > 0))
            {
                settoolstripitem(BuildISO_MenuItem, @bool: true);
                settoolstripComboBox(outtypeComboBox, @bool: true);
            }
            else
            {
                settoolstripitem(BuildISO_MenuItem, @bool: false);
                settoolstripComboBox(outtypeComboBox, @bool: false);
            }
            if ((isolist.Count > 0) & (_ISO != null))
            {
                settoolstripitem(ExtractISO_MenuItem, @bool: true);
                settoolstripitem(CompressISO_MenuItem, @bool: true);
            }
            else
            {
                settoolstripitem(ExtractISO_MenuItem, @bool: false);
                settoolstripitem(CompressISO_MenuItem, @bool: false);
            }
            if ((Operators.CompareString(root_dir, "", TextCompare: false) != 0) & (Operators.CompareString(game_id, "", TextCompare: false) != 0) & (isolist.Count > 0))
            {
                settoolstripitem(BuildODE_MenuItem, @bool: true);
            }
            else
            {
                settoolstripitem(BuildODE_MenuItem, @bool: false);
            }
        }

        public object getupdateURL(string version = "")
        {
            if (Operators.CompareString(version, string.Empty, TextCompare: false) == 0)
            {
                return "";
            }
            try
            {
                WebRequest webRequest = WebRequest.Create("http://www.ps3updat.ru.nu/update.php?version=" + version);
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = webRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                string result2 = "";
                ProjectData.ClearProjectError();
                return result2;
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = ListView1.SelectedItems;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-------------------------------------------------------");
            if (Operators.CompareString(game_id, "", TextCompare: false) != 0)
            {
                stringBuilder.Append(game_id);
            }
            else
            {
                stringBuilder.Append(ird_id);
            }
            if (Operators.CompareString(game_title, "", TextCompare: false) != 0)
            {
                stringBuilder.Append(" - " + game_title);
            }
            else
            {
                stringBuilder.Append(" - " + ird_title);
            }
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("-------------------------------------------------------");
            if (selectedItems.Count > 0)
            {
                IEnumerator enumerator = default(IEnumerator);
                try
                {
                    enumerator = selectedItems.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ListViewItem listViewItem = (ListViewItem)enumerator.Current;
                        stringBuilder.AppendLine(listViewItem.Text + " | " + listViewItem.SubItems[1].Text + " | " + listViewItem.SubItems[3].Text);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                try
                {
                    Clipboard.SetText(stringBuilder.ToString(), TextDataFormat.Text);
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    ProjectData.ClearProjectError();
                }
            }
        }

        public byte[] D2_Random(byte[] data2)
        {
            byte[] key = new byte[16]
            {
                124,
                221,
                14,
                2,
                7,
                110,
                254,
                69,
                153,
                177,
                184,
                44,
                53,
                153,
                25,
                179
            };
            byte[] iV = new byte[16]
            {
                34,
                38,
                146,
                141,
                68,
                3,
                47,
                67,
                106,
                253,
                38,
                126,
                116,
                139,
                35,
                147
            };
            byte[] array = DecryptFromBytes(data2, key, iV);
            if (BitConverter.ToInt32(array, 12) == 0)
            {
                return data2;
            }
            int value = new Random().Next(1, 2097151);
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.Reverse();
            Array.Copy(bytes, 0, array, 12, bytes.Length);
            return EncryptToBytes(array, key, iV);
        }

        public static byte[] EncryptToBytes(byte[] data, byte[] Key, byte[] IV)
        {
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = Key;
                rijndaelManaged.IV = IV;
                rijndaelManaged.Padding = PaddingMode.None;
                rijndaelManaged.Mode = CipherMode.CBC;
                ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] DecryptFromBytes(byte[] data, byte[] Key, byte[] IV)
        {
            byte[] array = new byte[checked(data.Length - 1 + 1)];
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = Key;
                rijndaelManaged.IV = IV;
                rijndaelManaged.Padding = PaddingMode.None;
                rijndaelManaged.Mode = CipherMode.CBC;
                ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(array, 0, array.Length);
                        return array;
                    }
                }
            }
        }

        public uint Swap(uint value)
        {
            checked
            {
                return (uint)unchecked(((uint)((int)value & -16777216) >> 24) | (((long)value & 16711680L) >> 8) | (((long)value & 65280L) << 8) | (((long)value & 255L) << 24));
            }
        }
    }
}
