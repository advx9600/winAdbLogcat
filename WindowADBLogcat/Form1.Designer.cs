namespace WindowADBLogcat
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonSwitch = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.comboBoxDevices = new System.Windows.Forms.ComboBox();
            this.comboBoxDebugLevel = new System.Windows.Forms.ComboBox();
            this.txtLogcat = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(153, 12);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 1;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonSwitch
            // 
            this.buttonSwitch.Location = new System.Drawing.Point(244, 12);
            this.buttonSwitch.Name = "buttonSwitch";
            this.buttonSwitch.Size = new System.Drawing.Size(75, 23);
            this.buttonSwitch.TabIndex = 2;
            this.buttonSwitch.Text = "Start";
            this.buttonSwitch.UseVisualStyleBackColor = true;
            this.buttonSwitch.Click += new System.EventHandler(this.buttonSwitch_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(339, 12);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(450, 14);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(183, 21);
            this.textBoxFilter.TabIndex = 4;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // comboBoxDevices
            // 
            this.comboBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevices.FormattingEnabled = true;
            this.comboBoxDevices.Location = new System.Drawing.Point(12, 15);
            this.comboBoxDevices.Name = "comboBoxDevices";
            this.comboBoxDevices.Size = new System.Drawing.Size(121, 20);
            this.comboBoxDevices.TabIndex = 5;
            this.comboBoxDevices.SelectionChangeCommitted += new System.EventHandler(this.comboBoxDevices_SelectionChangeCommitted);
            // 
            // comboBoxDebugLevel
            // 
            this.comboBoxDebugLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDebugLevel.FormattingEnabled = true;
            this.comboBoxDebugLevel.Location = new System.Drawing.Point(662, 12);
            this.comboBoxDebugLevel.Name = "comboBoxDebugLevel";
            this.comboBoxDebugLevel.Size = new System.Drawing.Size(121, 20);
            this.comboBoxDebugLevel.TabIndex = 6;
            this.comboBoxDebugLevel.SelectedValueChanged += new System.EventHandler(this.comboBoxDebugLevel_SelectedValueChanged);
            // 
            // txtLogcat
            // 
            this.txtLogcat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogcat.Location = new System.Drawing.Point(7, 48);
            this.txtLogcat.Name = "txtLogcat";
            this.txtLogcat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLogcat.Size = new System.Drawing.Size(1032, 253);
            this.txtLogcat.TabIndex = 7;
            this.txtLogcat.Text = "";
            this.txtLogcat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLogcat_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 313);
            this.Controls.Add(this.txtLogcat);
            this.Controls.Add(this.comboBoxDebugLevel);
            this.Controls.Add(this.comboBoxDevices);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSwitch);
            this.Controls.Add(this.buttonReset);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonSwitch;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.ComboBox comboBoxDevices;
        private System.Windows.Forms.ComboBox comboBoxDebugLevel;
        private System.Windows.Forms.RichTextBox txtLogcat;
    }
}

