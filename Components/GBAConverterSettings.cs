using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class GBAConverterSettings : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _display2Rows;
        private bool _dropDecimals;
        private TimeAccuracy _accuracy;
        private bool _overrideTextColor;
        private bool _overrideTimeColor;
        private Color _textColor;
        private Color _timeColor;

        public bool Display2Rows
        {
            get => _display2Rows;
            set
            {
                if (_display2Rows != value)
                {
                    _display2Rows = value;
                    OnPropertyChanged(nameof(Display2Rows));
                }
            }
        }

        public LayoutMode Mode { get; set; }

        // Time formatting options
        public bool DropDecimals
        {
            get => _dropDecimals;
            set
            {
                if (_dropDecimals != value)
                {
                    _dropDecimals = value;
                    OnPropertyChanged(nameof(DropDecimals));
                }
            }
        }

        public TimeAccuracy Accuracy
        {
            get => _accuracy;
            set
            {
                if (_accuracy != value)
                {
                    _accuracy = value;
                    OnPropertyChanged(nameof(Accuracy));
                }
            }
        }

        // Color options
        public bool OverrideTextColor
        {
            get => _overrideTextColor;
            set
            {
                if (_overrideTextColor != value)
                {
                    _overrideTextColor = value;
                    OnPropertyChanged(nameof(OverrideTextColor));
                }
            }
        }

        public bool OverrideTimeColor
        {
            get => _overrideTimeColor;
            set
            {
                if (_overrideTimeColor != value)
                {
                    _overrideTimeColor = value;
                    OnPropertyChanged(nameof(OverrideTimeColor));
                }
            }
        }
        // GBA <> NSO switcher
        public bool ConvertGBAToNSO { get; set; }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }

        public Color TimeColor
        {
            get => _timeColor;
            set
            {
                if (_timeColor != value)
                {
                    _timeColor = value;
                    OnPropertyChanged(nameof(TimeColor));
                }
            }
        }

        public enum TimeAccuracy
        {
            Seconds,
            Tenths,
            Hundredths,
            Milliseconds
        }

        public GBAConverterSettings()
        {
            InitializeComponent();

            // Set defaults
            Display2Rows = false;
            DropDecimals = true;
            Accuracy = TimeAccuracy.Hundredths;
            OverrideTextColor = false;
            OverrideTimeColor = false;
            TextColor = Color.FromArgb(255, 255, 255);
            TimeColor = Color.FromArgb(255, 255, 255);
            ConvertGBAToNSO = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GBAConverterSettings_Load(object sender, EventArgs e)
        {
            if (Mode == LayoutMode.Horizontal)
            {
                chkTwoRows.Enabled = false;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.Checked = true;
            }
            else
            {
                chkTwoRows.Enabled = true;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            // Set up data bindings
            chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimeColor.DataBindings.Add("Checked", this, "OverrideTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkConvertDirection.DataBindings.Add("Checked", this, "ConvertGBAToNSO", false, DataSourceUpdateMode.OnPropertyChanged); 

            // Set initial radio button states
            UpdateAccuracyRadioButtons();

            // Set initial button colors
            btnTextColor.BackColor = TextColor;
            btnTimeColor.BackColor = TimeColor;
            btnTextColor.Enabled = OverrideTextColor;
            btnTimeColor.Enabled = OverrideTimeColor;

            UpdateDescriptionLabel();
        }

        private void UpdateAccuracyRadioButtons()
        {
            rdoSeconds.Checked = Accuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = Accuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = Accuracy == TimeAccuracy.Hundredths;
            rdoMilliseconds.Checked = Accuracy == TimeAccuracy.Milliseconds;
        }

        private void UpdateAccuracy()
        {
            if (rdoSeconds.Checked)
                Accuracy = TimeAccuracy.Seconds;
            else if (rdoTenths.Checked)
                Accuracy = TimeAccuracy.Tenths;
            else if (rdoHundredths.Checked)
                Accuracy = TimeAccuracy.Hundredths;
            else if (rdoMilliseconds.Checked)
                Accuracy = TimeAccuracy.Milliseconds;
        }

        private void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSeconds.Checked)
                UpdateAccuracy();
        }

        private void rdoTenths_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTenths.Checked)
                UpdateAccuracy();
        }

        private void rdoHundredths_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoHundredths.Checked)
                UpdateAccuracy();
        }

        private void rdoMilliseconds_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoMilliseconds.Checked)
                UpdateAccuracy();
        }

        private void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            btnTextColor.Enabled = chkOverrideTextColor.Checked;
        }

        private void chkOverrideTimeColor_CheckedChanged(object sender, EventArgs e)
        {
            btnTimeColor.Enabled = chkOverrideTimeColor.Checked;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorDialog();

            if (button == btnTextColor)
            {
                picker.Color = TextColor;
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    TextColor = picker.Color;
                    btnTextColor.BackColor = TextColor;
                }
            }
            else if (button == btnTimeColor)
            {
                picker.Color = TimeColor;
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    TimeColor = picker.Color;
                    btnTimeColor.BackColor = TimeColor;
                }
            }
        }

        private void UpdateDescriptionLabel()
        {
            label1.Text = ConvertGBAToNSO
                ? "Converts 59.7275Hz GBA time to 60Hz NSO time"
                : "Converts 60Hz NSO time to 59.7275Hz GBA time";
        }

        private void chkConvertDirection_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDescriptionLabel();
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
                   SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
                   SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
                   SettingsHelper.CreateSetting(document, parent, "Accuracy", Accuracy) ^
                   SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
                   SettingsHelper.CreateSetting(document, parent, "OverrideTimeColor", OverrideTimeColor) ^
                   SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
                   SettingsHelper.CreateSetting(document, parent, "TimeColor", TimeColor) ^
                   SettingsHelper.CreateSetting(document, parent, "ConvertGBAToNSO", ConvertGBAToNSO);
            ;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
            DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"], true);
            Accuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["Accuracy"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"], false);
            OverrideTimeColor = SettingsHelper.ParseBool(element["OverrideTimeColor"], false);
            TextColor = SettingsHelper.ParseColor(element["TextColor"], Color.FromArgb(255, 255, 255));
            TimeColor = SettingsHelper.ParseColor(element["TimeColor"], Color.FromArgb(255, 255, 255));
            ConvertGBAToNSO = SettingsHelper.ParseBool(element["ConvertGBAToNSO"], false);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkTwoRows = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnTextColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chkOverrideTextColor = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.chkDropDecimals = new System.Windows.Forms.CheckBox();
            this.rdoSeconds = new System.Windows.Forms.RadioButton();
            this.rdoTenths = new System.Windows.Forms.RadioButton();
            this.rdoHundredths = new System.Windows.Forms.RadioButton();
            this.rdoMilliseconds = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnTimeColor = new System.Windows.Forms.Button();
            this.chkOverrideTimeColor = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkConvertDirection = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkTwoRows, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkConvertDirection, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 380);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Converts 60Hz Switch time to 59.7275Hz GBA time";
            // 
            // chkTwoRows
            // 
            this.chkTwoRows.AutoSize = true;
            this.chkTwoRows.Location = new System.Drawing.Point(3, 33);
            this.chkTwoRows.Name = "chkTwoRows";
            this.chkTwoRows.Size = new System.Drawing.Size(99, 17);
            this.chkTwoRows.TabIndex = 1;
            this.chkTwoRows.Text = "Display 2 Rows";
            this.chkTwoRows.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 84);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text Color";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnTextColor, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkOverrideTextColor, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(450, 65);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnTextColor
            // 
            this.btnTextColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTextColor.Location = new System.Drawing.Point(156, 32);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(25, 25);
            this.btnTextColor.TabIndex = 1;
            this.btnTextColor.UseVisualStyleBackColor = false;
            this.btnTextColor.Click += new System.EventHandler(this.ColorButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Color:";
            // 
            // chkOverrideTextColor
            // 
            this.chkOverrideTextColor.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.chkOverrideTextColor, 3);
            this.chkOverrideTextColor.Location = new System.Drawing.Point(3, 3);
            this.chkOverrideTextColor.Name = "chkOverrideTextColor";
            this.chkOverrideTextColor.Size = new System.Drawing.Size(142, 17);
            this.chkOverrideTextColor.TabIndex = 0;
            this.chkOverrideTextColor.Text = "Override Layout Settings";
            this.chkOverrideTextColor.UseVisualStyleBackColor = true;
            this.chkOverrideTextColor.CheckedChanged += new System.EventHandler(this.chkOverrideTextColor_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 183);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 194);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Time";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(450, 175);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.tableLayoutPanel5);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 93);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(444, 84);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Accuracy";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 4;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel5.Controls.Add(this.chkDropDecimals, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.rdoSeconds, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.rdoTenths, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.rdoHundredths, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.rdoMilliseconds, 3, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(438, 65);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // chkDropDecimals
            // 
            this.chkDropDecimals.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.chkDropDecimals, 3);
            this.chkDropDecimals.Location = new System.Drawing.Point(3, 3);
            this.chkDropDecimals.Name = "chkDropDecimals";
            this.chkDropDecimals.Size = new System.Drawing.Size(226, 17);
            this.chkDropDecimals.TabIndex = 0;
            this.chkDropDecimals.Text = "Drop Decimals When More Than 1 Minute";
            this.chkDropDecimals.UseVisualStyleBackColor = true;
            // 
            // rdoSeconds
            // 
            this.rdoSeconds.AutoSize = true;
            this.rdoSeconds.Location = new System.Drawing.Point(3, 35);
            this.rdoSeconds.Name = "rdoSeconds";
            this.rdoSeconds.Size = new System.Drawing.Size(67, 17);
            this.rdoSeconds.TabIndex = 1;
            this.rdoSeconds.TabStop = true;
            this.rdoSeconds.Text = "Seconds";
            this.rdoSeconds.UseVisualStyleBackColor = true;
            this.rdoSeconds.CheckedChanged += new System.EventHandler(this.rdoSeconds_CheckedChanged);
            // 
            // rdoTenths
            // 
            this.rdoTenths.AutoSize = true;
            this.rdoTenths.Location = new System.Drawing.Point(112, 35);
            this.rdoTenths.Name = "rdoTenths";
            this.rdoTenths.Size = new System.Drawing.Size(58, 17);
            this.rdoTenths.TabIndex = 2;
            this.rdoTenths.TabStop = true;
            this.rdoTenths.Text = "Tenths";
            this.rdoTenths.UseVisualStyleBackColor = true;
            this.rdoTenths.CheckedChanged += new System.EventHandler(this.rdoTenths_CheckedChanged);
            // 
            // rdoHundredths
            // 
            this.rdoHundredths.AutoSize = true;
            this.rdoHundredths.Location = new System.Drawing.Point(221, 35);
            this.rdoHundredths.Name = "rdoHundredths";
            this.rdoHundredths.Size = new System.Drawing.Size(80, 17);
            this.rdoHundredths.TabIndex = 3;
            this.rdoHundredths.TabStop = true;
            this.rdoHundredths.Text = "Hundredths";
            this.rdoHundredths.UseVisualStyleBackColor = true;
            this.rdoHundredths.CheckedChanged += new System.EventHandler(this.rdoHundredths_CheckedChanged);
            // 
            // rdoMilliseconds
            // 
            this.rdoMilliseconds.AutoSize = true;
            this.rdoMilliseconds.Location = new System.Drawing.Point(330, 35);
            this.rdoMilliseconds.Name = "rdoMilliseconds";
            this.rdoMilliseconds.Size = new System.Drawing.Size(82, 17);
            this.rdoMilliseconds.TabIndex = 4;
            this.rdoMilliseconds.TabStop = true;
            this.rdoMilliseconds.Text = "Milliseconds";
            this.rdoMilliseconds.UseVisualStyleBackColor = true;
            this.rdoMilliseconds.CheckedChanged += new System.EventHandler(this.rdoMilliseconds_CheckedChanged);
            // 
            // groupBox3
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.tableLayoutPanel4);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 84);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Color";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.btnTimeColor, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.chkOverrideTimeColor, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(438, 65);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // btnTimeColor
            // 
            this.btnTimeColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTimeColor.Location = new System.Drawing.Point(156, 28);
            this.btnTimeColor.Name = "btnTimeColor";
            this.btnTimeColor.Size = new System.Drawing.Size(25, 25);
            this.btnTimeColor.TabIndex = 1;
            this.btnTimeColor.UseVisualStyleBackColor = false;
            this.btnTimeColor.Click += new System.EventHandler(this.ColorButtonClick);
            // 
            // chkOverrideTimeColor
            // 
            this.chkOverrideTimeColor.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.chkOverrideTimeColor, 3);
            this.chkOverrideTimeColor.Location = new System.Drawing.Point(3, 3);
            this.chkOverrideTimeColor.Name = "chkOverrideTimeColor";
            this.chkOverrideTimeColor.Size = new System.Drawing.Size(142, 17);
            this.chkOverrideTimeColor.TabIndex = 0;
            this.chkOverrideTimeColor.Text = "Override Layout Settings";
            this.chkOverrideTimeColor.UseVisualStyleBackColor = true;
            this.chkOverrideTimeColor.CheckedChanged += new System.EventHandler(this.chkOverrideTimeColor_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Color:";
            // 
            // chkConvertDirection
            // 
            this.chkConvertDirection.AutoSize = true;
            this.chkConvertDirection.Location = new System.Drawing.Point(3, 63);
            this.chkConvertDirection.Name = "chkConvertDirection";
            this.chkConvertDirection.Size = new System.Drawing.Size(288, 17);
            this.chkConvertDirection.TabIndex = 1;
            this.chkConvertDirection.Text = "Convert GBA time to NSO time (instead of NSO to GBA)";
            this.chkConvertDirection.UseVisualStyleBackColor = true;
            this.chkConvertDirection.CheckedChanged += new System.EventHandler(this.chkConvertDirection_CheckedChanged);
            // 
            // GBAConverterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GBAConverterSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 394);
            this.Load += new System.EventHandler(this.GBAConverterSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkTwoRows;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnTextColor;
        private System.Windows.Forms.CheckBox chkOverrideTextColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnTimeColor;
        private System.Windows.Forms.CheckBox chkOverrideTimeColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.CheckBox chkDropDecimals;
        private System.Windows.Forms.RadioButton rdoSeconds;
        private System.Windows.Forms.RadioButton rdoTenths;
        private System.Windows.Forms.RadioButton rdoHundredths;
        private System.Windows.Forms.RadioButton rdoMilliseconds;
        private System.Windows.Forms.CheckBox chkConvertDirection;
    }
}

// Separate file for LayoutMode enum - create this in a separate .cs file or add to existing shared file
public enum LayoutMode
{
    Vertical,
    Horizontal
}

// Separate file for SettingsHelper - create this in a separate .cs file or add to existing shared file  
public static class SettingsHelper
{
    public static int CreateSetting(XmlDocument document, XmlElement parent, string name, object value)
    {
        if (document != null && parent != null)
        {
            var element = document.CreateElement(name);
            element.InnerText = value?.ToString() ?? "";
            parent.AppendChild(element);
        }
        return value?.GetHashCode() ?? 0;
    }

    public static bool ParseBool(XmlNode node, bool defaultValue = false)
    {
        if (node == null) return defaultValue;
        return bool.TryParse(node.InnerText, out bool result) ? result : defaultValue;
    }

    public static T ParseEnum<T>(XmlNode node) where T : struct
    {
        if (node == null) return default(T);
        return Enum.TryParse<T>(node.InnerText, out T result) ? result : default(T);
    }

    public static Color ParseColor(XmlNode node, Color defaultValue)
    {
        if (node == null) return defaultValue;
        try
        {
            var colorString = node.InnerText;
            if (colorString.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(colorString);
            }
            else
            {
                return Color.FromArgb(int.Parse(colorString));
            }
        }
        catch
        {
            return defaultValue;
        }
    }
}
