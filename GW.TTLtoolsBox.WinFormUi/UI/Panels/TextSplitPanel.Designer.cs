namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class TextSplitPanel
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
            this.sc_主分割 = new System.Windows.Forms.SplitContainer();
            this.gb_最终文本 = new System.Windows.Forms.GroupBox();
            this.bt_发送最终文本到下一个步骤 = new System.Windows.Forms.Button();
            this.bt_复制最终文本 = new System.Windows.Forms.Button();
            this.bt_在最终文本中标示 = new System.Windows.Forms.Button();
            this.lab_最终文本字数 = new System.Windows.Forms.Label();
            this.tb_最终文本 = new System.Windows.Forms.TextBox();
            this.gb_原始文本 = new System.Windows.Forms.GroupBox();
            this.lab_原始文本字数 = new System.Windows.Forms.Label();
            this.bt_插入段落分隔符 = new System.Windows.Forms.Button();
            this.tb_原始文本 = new System.Windows.Forms.TextBox();
            this.gb_拆分参数 = new System.Windows.Forms.GroupBox();
            this.rb_拆分参数_按对话拆分 = new System.Windows.Forms.RadioButton();
            this.rb_拆分方式_按句子拆分 = new System.Windows.Forms.RadioButton();
            this.bt_开始拆分 = new System.Windows.Forms.Button();
            this.lab_拆分长度说明 = new System.Windows.Forms.Label();
            this.bt_清理所有文本 = new System.Windows.Forms.Button();
            this.lab_拆分长度 = new System.Windows.Forms.Label();
            this.nud_拆分长度 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.sc_主分割)).BeginInit();
            this.sc_主分割.Panel1.SuspendLayout();
            this.sc_主分割.Panel2.SuspendLayout();
            this.sc_主分割.SuspendLayout();
            this.gb_最终文本.SuspendLayout();
            this.gb_原始文本.SuspendLayout();
            this.gb_拆分参数.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_拆分长度)).BeginInit();
            this.SuspendLayout();
            // 
            // sc_主分割
            // 
            this.sc_主分割.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_主分割.Location = new System.Drawing.Point(0, 0);
            this.sc_主分割.Name = "sc_主分割";
            // 
            // sc_主分割.Panel1
            // 
            this.sc_主分割.Panel1.Controls.Add(this.gb_最终文本);
            this.sc_主分割.Panel1.Controls.Add(this.gb_原始文本);
            // 
            // sc_主分割.Panel2
            // 
            this.sc_主分割.Panel2.Controls.Add(this.gb_拆分参数);
            this.sc_主分割.Size = new System.Drawing.Size(1301, 652);
            this.sc_主分割.SplitterDistance = 810;
            this.sc_主分割.TabIndex = 0;
            // 
            // gb_最终文本
            // 
            this.gb_最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_最终文本.Controls.Add(this.bt_发送最终文本到下一个步骤);
            this.gb_最终文本.Controls.Add(this.bt_复制最终文本);
            this.gb_最终文本.Controls.Add(this.bt_在最终文本中标示);
            this.gb_最终文本.Controls.Add(this.lab_最终文本字数);
            this.gb_最终文本.Controls.Add(this.tb_最终文本);
            this.gb_最终文本.Location = new System.Drawing.Point(3, 371);
            this.gb_最终文本.Name = "gb_最终文本";
            this.gb_最终文本.Size = new System.Drawing.Size(804, 278);
            this.gb_最终文本.TabIndex = 3;
            this.gb_最终文本.TabStop = false;
            this.gb_最终文本.Text = "最终文本";
            // 
            // bt_发送最终文本到下一个步骤
            // 
            this.bt_发送最终文本到下一个步骤.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_发送最终文本到下一个步骤.Location = new System.Drawing.Point(694, 244);
            this.bt_发送最终文本到下一个步骤.Name = "bt_发送最终文本到下一个步骤";
            this.bt_发送最终文本到下一个步骤.Size = new System.Drawing.Size(104, 28);
            this.bt_发送最终文本到下一个步骤.TabIndex = 7;
            this.bt_发送最终文本到下一个步骤.Text = "发送到下一步";
            this.bt_发送最终文本到下一个步骤.UseVisualStyleBackColor = true;
            this.bt_发送最终文本到下一个步骤.Click += new System.EventHandler(this.bt_发送最终文本到下一个步骤_Click);
            // 
            // bt_复制最终文本
            // 
            this.bt_复制最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_复制最终文本.Location = new System.Drawing.Point(584, 244);
            this.bt_复制最终文本.Name = "bt_复制最终文本";
            this.bt_复制最终文本.Size = new System.Drawing.Size(104, 28);
            this.bt_复制最终文本.TabIndex = 6;
            this.bt_复制最终文本.Text = "复制文本";
            this.bt_复制最终文本.UseVisualStyleBackColor = true;
            this.bt_复制最终文本.Click += new System.EventHandler(this.bt_复制最终文本_Click);
            // 
            // bt_在最终文本中标示
            // 
            this.bt_在最终文本中标示.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_在最终文本中标示.Location = new System.Drawing.Point(699, 23);
            this.bt_在最终文本中标示.Name = "bt_在最终文本中标示";
            this.bt_在最终文本中标示.Size = new System.Drawing.Size(99, 23);
            this.bt_在最终文本中标示.TabIndex = 4;
            this.bt_在最终文本中标示.Text = "标示";
            this.bt_在最终文本中标示.UseVisualStyleBackColor = true;
            this.bt_在最终文本中标示.Click += new System.EventHandler(this.bt_在最终文本中标示_Click);
            // 
            // lab_最终文本字数
            // 
            this.lab_最终文本字数.AutoSize = true;
            this.lab_最终文本字数.Location = new System.Drawing.Point(6, 28);
            this.lab_最终文本字数.Name = "lab_最终文本字数";
            this.lab_最终文本字数.Size = new System.Drawing.Size(41, 12);
            this.lab_最终文本字数.TabIndex = 1;
            this.lab_最终文本字数.Text = "字数：";
            // 
            // tb_最终文本
            // 
            this.tb_最终文本.AcceptsReturn = true;
            this.tb_最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_最终文本.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_最终文本.Location = new System.Drawing.Point(6, 50);
            this.tb_最终文本.Multiline = true;
            this.tb_最终文本.Name = "tb_最终文本";
            this.tb_最终文本.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_最终文本.Size = new System.Drawing.Size(792, 188);
            this.tb_最终文本.TabIndex = 0;
            this.tb_最终文本.TextChanged += new System.EventHandler(this.tb_最终文本_TextChanged);
            // 
            // gb_原始文本
            // 
            this.gb_原始文本.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_原始文本.Controls.Add(this.lab_原始文本字数);
            this.gb_原始文本.Controls.Add(this.bt_插入段落分隔符);
            this.gb_原始文本.Controls.Add(this.tb_原始文本);
            this.gb_原始文本.Location = new System.Drawing.Point(3, 3);
            this.gb_原始文本.Name = "gb_原始文本";
            this.gb_原始文本.Size = new System.Drawing.Size(804, 362);
            this.gb_原始文本.TabIndex = 1;
            this.gb_原始文本.TabStop = false;
            this.gb_原始文本.Text = "原始文本";
            // 
            // lab_原始文本字数
            // 
            this.lab_原始文本字数.AutoSize = true;
            this.lab_原始文本字数.Location = new System.Drawing.Point(6, 28);
            this.lab_原始文本字数.Name = "lab_原始文本字数";
            this.lab_原始文本字数.Size = new System.Drawing.Size(41, 12);
            this.lab_原始文本字数.TabIndex = 1;
            this.lab_原始文本字数.Text = "字数：";
            // 
            // bt_插入段落分隔符
            // 
            this.bt_插入段落分隔符.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_插入段落分隔符.Location = new System.Drawing.Point(694, 16);
            this.bt_插入段落分隔符.Name = "bt_插入段落分隔符";
            this.bt_插入段落分隔符.Size = new System.Drawing.Size(104, 28);
            this.bt_插入段落分隔符.TabIndex = 6;
            this.bt_插入段落分隔符.Text = "插入段落分隔符";
            this.bt_插入段落分隔符.UseVisualStyleBackColor = true;
            this.bt_插入段落分隔符.Click += new System.EventHandler(this.bt_插入段落分隔符_Click);
            // 
            // tb_原始文本
            // 
            this.tb_原始文本.AcceptsReturn = true;
            this.tb_原始文本.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_原始文本.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_原始文本.Location = new System.Drawing.Point(6, 50);
            this.tb_原始文本.Multiline = true;
            this.tb_原始文本.Name = "tb_原始文本";
            this.tb_原始文本.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_原始文本.Size = new System.Drawing.Size(792, 306);
            this.tb_原始文本.TabIndex = 0;
            this.tb_原始文本.TextChanged += new System.EventHandler(this.tb_原始文本_TextChanged);
            // 
            // gb_拆分参数
            // 
            this.gb_拆分参数.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_拆分参数.Controls.Add(this.rb_拆分参数_按对话拆分);
            this.gb_拆分参数.Controls.Add(this.rb_拆分方式_按句子拆分);
            this.gb_拆分参数.Controls.Add(this.bt_开始拆分);
            this.gb_拆分参数.Controls.Add(this.lab_拆分长度说明);
            this.gb_拆分参数.Controls.Add(this.bt_清理所有文本);
            this.gb_拆分参数.Controls.Add(this.lab_拆分长度);
            this.gb_拆分参数.Controls.Add(this.nud_拆分长度);
            this.gb_拆分参数.Location = new System.Drawing.Point(3, 3);
            this.gb_拆分参数.Name = "gb_拆分参数";
            this.gb_拆分参数.Size = new System.Drawing.Size(481, 646);
            this.gb_拆分参数.TabIndex = 2;
            this.gb_拆分参数.TabStop = false;
            this.gb_拆分参数.Text = "拆分参数";
            // 
            // rb_拆分参数_按对话拆分
            // 
            this.rb_拆分参数_按对话拆分.AutoSize = true;
            this.rb_拆分参数_按对话拆分.Location = new System.Drawing.Point(6, 94);
            this.rb_拆分参数_按对话拆分.Name = "rb_拆分参数_按对话拆分";
            this.rb_拆分参数_按对话拆分.Size = new System.Drawing.Size(83, 16);
            this.rb_拆分参数_按对话拆分.TabIndex = 7;
            this.rb_拆分参数_按对话拆分.Text = "按对话拆分";
            this.rb_拆分参数_按对话拆分.UseVisualStyleBackColor = true;
            // 
            // rb_拆分方式_按句子拆分
            // 
            this.rb_拆分方式_按句子拆分.AutoSize = true;
            this.rb_拆分方式_按句子拆分.Checked = true;
            this.rb_拆分方式_按句子拆分.Location = new System.Drawing.Point(6, 27);
            this.rb_拆分方式_按句子拆分.Name = "rb_拆分方式_按句子拆分";
            this.rb_拆分方式_按句子拆分.Size = new System.Drawing.Size(83, 16);
            this.rb_拆分方式_按句子拆分.TabIndex = 7;
            this.rb_拆分方式_按句子拆分.TabStop = true;
            this.rb_拆分方式_按句子拆分.Text = "按句子拆分";
            this.rb_拆分方式_按句子拆分.UseVisualStyleBackColor = true;
            // 
            // bt_开始拆分
            // 
            this.bt_开始拆分.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_开始拆分.Location = new System.Drawing.Point(6, 612);
            this.bt_开始拆分.Name = "bt_开始拆分";
            this.bt_开始拆分.Size = new System.Drawing.Size(104, 28);
            this.bt_开始拆分.TabIndex = 6;
            this.bt_开始拆分.Text = "开始拆分";
            this.bt_开始拆分.UseVisualStyleBackColor = true;
            this.bt_开始拆分.Click += new System.EventHandler(this.bt_开始拆分_Click);
            // 
            // lab_拆分长度说明
            // 
            this.lab_拆分长度说明.AutoSize = true;
            this.lab_拆分长度说明.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lab_拆分长度说明.Location = new System.Drawing.Point(171, 51);
            this.lab_拆分长度说明.Name = "lab_拆分长度说明";
            this.lab_拆分长度说明.Size = new System.Drawing.Size(311, 12);
            this.lab_拆分长度说明.TabIndex = 5;
            this.lab_拆分长度说明.Text = "0表示按整句切分，有些引擎可以借此略微控制播放速度。";
            // 
            // bt_清理所有文本
            // 
            this.bt_清理所有文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_清理所有文本.Location = new System.Drawing.Point(371, 612);
            this.bt_清理所有文本.Name = "bt_清理所有文本";
            this.bt_清理所有文本.Size = new System.Drawing.Size(104, 28);
            this.bt_清理所有文本.TabIndex = 5;
            this.bt_清理所有文本.Text = "清理所有文本";
            this.bt_清理所有文本.UseVisualStyleBackColor = true;
            this.bt_清理所有文本.Click += new System.EventHandler(this.bt_清理所有文本_Click);
            // 
            // lab_拆分长度
            // 
            this.lab_拆分长度.AutoSize = true;
            this.lab_拆分长度.Location = new System.Drawing.Point(22, 51);
            this.lab_拆分长度.Name = "lab_拆分长度";
            this.lab_拆分长度.Size = new System.Drawing.Size(53, 12);
            this.lab_拆分长度.TabIndex = 4;
            this.lab_拆分长度.Text = "拆分长度";
            // 
            // nud_拆分长度
            // 
            this.nud_拆分长度.AutoSize = true;
            this.nud_拆分长度.Location = new System.Drawing.Point(81, 46);
            this.nud_拆分长度.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_拆分长度.Name = "nud_拆分长度";
            this.nud_拆分长度.Size = new System.Drawing.Size(84, 21);
            this.nud_拆分长度.TabIndex = 3;
            // 
            // TextSplitPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sc_主分割);
            this.Name = "TextSplitPanel";
            this.Size = new System.Drawing.Size(1301, 652);
            this.sc_主分割.Panel1.ResumeLayout(false);
            this.sc_主分割.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc_主分割)).EndInit();
            this.sc_主分割.ResumeLayout(false);
            this.gb_最终文本.ResumeLayout(false);
            this.gb_最终文本.PerformLayout();
            this.gb_原始文本.ResumeLayout(false);
            this.gb_原始文本.PerformLayout();
            this.gb_拆分参数.ResumeLayout(false);
            this.gb_拆分参数.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_拆分长度)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer sc_主分割;
        private System.Windows.Forms.GroupBox gb_最终文本;
        private System.Windows.Forms.Button bt_发送最终文本到下一个步骤;
        private System.Windows.Forms.Button bt_复制最终文本;
        private System.Windows.Forms.Button bt_在最终文本中标示;
        private System.Windows.Forms.Label lab_最终文本字数;
        private System.Windows.Forms.TextBox tb_最终文本;
        private System.Windows.Forms.GroupBox gb_原始文本;
        private System.Windows.Forms.Label lab_原始文本字数;
        private System.Windows.Forms.Button bt_插入段落分隔符;
        private System.Windows.Forms.TextBox tb_原始文本;
        private System.Windows.Forms.GroupBox gb_拆分参数;
        private System.Windows.Forms.RadioButton rb_拆分参数_按对话拆分;
        private System.Windows.Forms.RadioButton rb_拆分方式_按句子拆分;
        private System.Windows.Forms.Button bt_开始拆分;
        private System.Windows.Forms.Label lab_拆分长度说明;
        private System.Windows.Forms.Button bt_清理所有文本;
        private System.Windows.Forms.Label lab_拆分长度;
        private System.Windows.Forms.NumericUpDown nud_拆分长度;
    }
}
