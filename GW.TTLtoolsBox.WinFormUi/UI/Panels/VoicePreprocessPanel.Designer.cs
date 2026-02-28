namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class VoicePreprocessPanel
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_语音生成预处理_最终文本 = new System.Windows.Forms.TextBox();
            this.lab_语音生成与处理_最终文本 = new System.Windows.Forms.Label();
            this.bt_语音生成预处理_清理文本 = new System.Windows.Forms.Button();
            this.bt_语音生成预处理_插入段落分隔符 = new System.Windows.Forms.Button();
            this.pan_语音生成预处理_参数 = new System.Windows.Forms.Panel();
            this.nud_语音生成预处理_空白时长 = new System.Windows.Forms.NumericUpDown();
            this.bt_语音生成预处理_发送到语音生成 = new System.Windows.Forms.Button();
            this.nud_语音生成预处理_语速设置 = new System.Windows.Forms.NumericUpDown();
            this.cb_语音生成预处理_默认角色设置 = new System.Windows.Forms.ComboBox();
            this.lab_语音生成预处理_语速设置 = new System.Windows.Forms.Label();
            this.lab_语音生成预处理_空白时长 = new System.Windows.Forms.Label();
            this.lab_语音生成预处理_默认角色设置 = new System.Windows.Forms.Label();
            this.pan_语音生成预处理_参数.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_语音生成预处理_空白时长)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_语音生成预处理_语速设置)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_语音生成预处理_最终文本
            // 
            this.tb_语音生成预处理_最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_语音生成预处理_最终文本.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_语音生成预处理_最终文本.Location = new System.Drawing.Point(12, 36);
            this.tb_语音生成预处理_最终文本.Multiline = true;
            this.tb_语音生成预处理_最终文本.Name = "tb_语音生成预处理_最终文本";
            this.tb_语音生成预处理_最终文本.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_语音生成预处理_最终文本.Size = new System.Drawing.Size(1260, 524);
            this.tb_语音生成预处理_最终文本.TabIndex = 1;
            this.tb_语音生成预处理_最终文本.TextChanged += new System.EventHandler(this.tb_语音生成预处理_最终文本_TextChanged);
            // 
            // lab_语音生成与处理_最终文本
            // 
            this.lab_语音生成与处理_最终文本.AutoSize = true;
            this.lab_语音生成与处理_最终文本.Location = new System.Drawing.Point(10, 13);
            this.lab_语音生成与处理_最终文本.Name = "lab_语音生成与处理_最终文本";
            this.lab_语音生成与处理_最终文本.Size = new System.Drawing.Size(215, 12);
            this.lab_语音生成与处理_最终文本.TabIndex = 0;
            this.lab_语音生成与处理_最终文本.Text = "最终文本：将分为 {0} 个语音生成任务";
            // 
            // bt_语音生成预处理_清理文本
            // 
            this.bt_语音生成预处理_清理文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成预处理_清理文本.Location = new System.Drawing.Point(1168, 5);
            this.bt_语音生成预处理_清理文本.Name = "bt_语音生成预处理_清理文本";
            this.bt_语音生成预处理_清理文本.Size = new System.Drawing.Size(104, 28);
            this.bt_语音生成预处理_清理文本.TabIndex = 12;
            this.bt_语音生成预处理_清理文本.Text = "清理文本";
            this.bt_语音生成预处理_清理文本.UseVisualStyleBackColor = true;
            this.bt_语音生成预处理_清理文本.Click += new System.EventHandler(this.bt_语音生成预处理_清理文本_Click);
            // 
            // bt_语音生成预处理_插入段落分隔符
            // 
            this.bt_语音生成预处理_插入段落分隔符.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成预处理_插入段落分隔符.Location = new System.Drawing.Point(1058, 5);
            this.bt_语音生成预处理_插入段落分隔符.Name = "bt_语音生成预处理_插入段落分隔符";
            this.bt_语音生成预处理_插入段落分隔符.Size = new System.Drawing.Size(104, 28);
            this.bt_语音生成预处理_插入段落分隔符.TabIndex = 13;
            this.bt_语音生成预处理_插入段落分隔符.Text = "插入段落分隔符";
            this.bt_语音生成预处理_插入段落分隔符.UseVisualStyleBackColor = true;
            this.bt_语音生成预处理_插入段落分隔符.Click += new System.EventHandler(this.bt_语音生成预处理_插入段落分隔符_Click);
            // 
            // pan_语音生成预处理_参数
            // 
            this.pan_语音生成预处理_参数.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_语音生成预处理_参数.Controls.Add(this.nud_语音生成预处理_空白时长);
            this.pan_语音生成预处理_参数.Controls.Add(this.bt_语音生成预处理_发送到语音生成);
            this.pan_语音生成预处理_参数.Controls.Add(this.nud_语音生成预处理_语速设置);
            this.pan_语音生成预处理_参数.Controls.Add(this.cb_语音生成预处理_默认角色设置);
            this.pan_语音生成预处理_参数.Controls.Add(this.lab_语音生成预处理_语速设置);
            this.pan_语音生成预处理_参数.Controls.Add(this.lab_语音生成预处理_空白时长);
            this.pan_语音生成预处理_参数.Controls.Add(this.lab_语音生成预处理_默认角色设置);
            this.pan_语音生成预处理_参数.Location = new System.Drawing.Point(0, 566);
            this.pan_语音生成预处理_参数.Name = "pan_语音生成预处理_参数";
            this.pan_语音生成预处理_参数.Size = new System.Drawing.Size(1280, 80);
            this.pan_语音生成预处理_参数.TabIndex = 14;
            // 
            // nud_语音生成预处理_空白时长
            // 
            this.nud_语音生成预处理_空白时长.DecimalPlaces = 1;
            this.nud_语音生成预处理_空白时长.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_语音生成预处理_空白时长.Location = new System.Drawing.Point(265, 47);
            this.nud_语音生成预处理_空白时长.Name = "nud_语音生成预处理_空白时长";
            this.nud_语音生成预处理_空白时长.Size = new System.Drawing.Size(57, 21);
            this.nud_语音生成预处理_空白时长.TabIndex = 15;
            this.nud_语音生成预处理_空白时长.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_语音生成预处理_空白时长.ValueChanged += new System.EventHandler(this.nud_语音生成预处理_空白时长_ValueChanged);
            // 
            // bt_语音生成预处理_发送到语音生成
            // 
            this.bt_语音生成预处理_发送到语音生成.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成预处理_发送到语音生成.Location = new System.Drawing.Point(1126, 9);
            this.bt_语音生成预处理_发送到语音生成.Name = "bt_语音生成预处理_发送到语音生成";
            this.bt_语音生成预处理_发送到语音生成.Size = new System.Drawing.Size(146, 60);
            this.bt_语音生成预处理_发送到语音生成.TabIndex = 12;
            this.bt_语音生成预处理_发送到语音生成.Text = "发送到语音生成 >>>";
            this.bt_语音生成预处理_发送到语音生成.UseVisualStyleBackColor = true;
            this.bt_语音生成预处理_发送到语音生成.Click += new System.EventHandler(this.bt_语音生成预处理_发送到语音生成_Click);
            // 
            // nud_语音生成预处理_语速设置
            // 
            this.nud_语音生成预处理_语速设置.Location = new System.Drawing.Point(84, 48);
            this.nud_语音生成预处理_语速设置.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_语音生成预处理_语速设置.Name = "nud_语音生成预处理_语速设置";
            this.nud_语音生成预处理_语速设置.Size = new System.Drawing.Size(54, 21);
            this.nud_语音生成预处理_语速设置.TabIndex = 2;
            this.nud_语音生成预处理_语速设置.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nud_语音生成预处理_语速设置.ValueChanged += new System.EventHandler(this.nud_语音生成预处理_语速设置_ValueChanged);
            // 
            // cb_语音生成预处理_默认角色设置
            // 
            this.cb_语音生成预处理_默认角色设置.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_语音生成预处理_默认角色设置.FormattingEnabled = true;
            this.cb_语音生成预处理_默认角色设置.Location = new System.Drawing.Point(84, 9);
            this.cb_语音生成预处理_默认角色设置.Name = "cb_语音生成预处理_默认角色设置";
            this.cb_语音生成预处理_默认角色设置.Size = new System.Drawing.Size(244, 20);
            this.cb_语音生成预处理_默认角色设置.TabIndex = 1;
            // 
            // lab_语音生成预处理_语速设置
            // 
            this.lab_语音生成预处理_语速设置.AutoSize = true;
            this.lab_语音生成预处理_语速设置.Location = new System.Drawing.Point(12, 52);
            this.lab_语音生成预处理_语速设置.Name = "lab_语音生成预处理_语速设置";
            this.lab_语音生成预处理_语速设置.Size = new System.Drawing.Size(65, 12);
            this.lab_语音生成预处理_语速设置.TabIndex = 3;
            this.lab_语音生成预处理_语速设置.Text = "语速设置：";
            // 
            // lab_语音生成预处理_空白时长
            // 
            this.lab_语音生成预处理_空白时长.AutoSize = true;
            this.lab_语音生成预处理_空白时长.Location = new System.Drawing.Point(165, 52);
            this.lab_语音生成预处理_空白时长.Name = "lab_语音生成预处理_空白时长";
            this.lab_语音生成预处理_空白时长.Size = new System.Drawing.Size(113, 12);
            this.lab_语音生成预处理_空白时长.TabIndex = 4;
            this.lab_语音生成预处理_空白时长.Text = "段间空白时长(秒)：";
            // 
            // lab_语音生成预处理_默认角色设置
            // 
            this.lab_语音生成预处理_默认角色设置.AutoSize = true;
            this.lab_语音生成预处理_默认角色设置.Location = new System.Drawing.Point(12, 13);
            this.lab_语音生成预处理_默认角色设置.Name = "lab_语音生成预处理_默认角色设置";
            this.lab_语音生成预处理_默认角色设置.Size = new System.Drawing.Size(65, 12);
            this.lab_语音生成预处理_默认角色设置.TabIndex = 0;
            this.lab_语音生成预处理_默认角色设置.Text = "默认角色：";
            // 
            // VoicePreprocessPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pan_语音生成预处理_参数);
            this.Controls.Add(this.bt_语音生成预处理_插入段落分隔符);
            this.Controls.Add(this.bt_语音生成预处理_清理文本);
            this.Controls.Add(this.tb_语音生成预处理_最终文本);
            this.Controls.Add(this.lab_语音生成与处理_最终文本);
            this.Name = "VoicePreprocessPanel";
            this.Size = new System.Drawing.Size(1280, 646);
            this.pan_语音生成预处理_参数.ResumeLayout(false);
            this.pan_语音生成预处理_参数.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_语音生成预处理_空白时长)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_语音生成预处理_语速设置)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_语音生成预处理_最终文本;
        private System.Windows.Forms.Label lab_语音生成与处理_最终文本;
        private System.Windows.Forms.Button bt_语音生成预处理_清理文本;
        private System.Windows.Forms.Button bt_语音生成预处理_插入段落分隔符;
        private System.Windows.Forms.Panel pan_语音生成预处理_参数;
        private System.Windows.Forms.NumericUpDown nud_语音生成预处理_空白时长;
        private System.Windows.Forms.NumericUpDown nud_语音生成预处理_语速设置;
        private System.Windows.Forms.ComboBox cb_语音生成预处理_默认角色设置;
        private System.Windows.Forms.Button bt_语音生成预处理_发送到语音生成;
        private System.Windows.Forms.Label lab_语音生成预处理_语速设置;
        private System.Windows.Forms.Label lab_语音生成预处理_空白时长;
        private System.Windows.Forms.Label lab_语音生成预处理_默认角色设置;
    }
}
