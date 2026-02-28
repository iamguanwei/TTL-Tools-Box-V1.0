namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class VoiceGenerationPanel
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pan_语音生成 = new System.Windows.Forms.Panel();
            this.bt_语音生成_打开角色声音预览目录 = new System.Windows.Forms.Button();
            this.pan_语音生成_工作状态 = new System.Windows.Forms.Panel();
            this.lab_语音生成_任务提交信息 = new System.Windows.Forms.Label();
            this.lab_语音生成_选中行信息 = new System.Windows.Forms.Label();
            this.bt_语音生成_设置临时目录 = new System.Windows.Forms.Button();
            this.bt_语音生成_打开临时目录 = new System.Windows.Forms.Button();
            this.bt_语音生成_错后 = new System.Windows.Forms.Button();
            this.bt_语音生成_提前 = new System.Windows.Forms.Button();
            this.cb_语音生成_自动开始任务 = new System.Windows.Forms.CheckBox();
            this.dgv_语音生成_任务清单 = new System.Windows.Forms.DataGridView();
            this.cms_语音生成_任务控制 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.启动SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.停止TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重新启动SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.上移UToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下移DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.预览声音MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开临时文件夹EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件夹FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改存储文件夹MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.复制CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.清空所有任务AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pan_语音生成.SuspendLayout();
            this.pan_语音生成_工作状态.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_语音生成_任务清单)).BeginInit();
            this.cms_语音生成_任务控制.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_语音生成
            // 
            this.pan_语音生成.Controls.Add(this.bt_语音生成_打开角色声音预览目录);
            this.pan_语音生成.Controls.Add(this.pan_语音生成_工作状态);
            this.pan_语音生成.Controls.Add(this.bt_语音生成_设置临时目录);
            this.pan_语音生成.Controls.Add(this.bt_语音生成_打开临时目录);
            this.pan_语音生成.Controls.Add(this.bt_语音生成_错后);
            this.pan_语音生成.Controls.Add(this.bt_语音生成_提前);
            this.pan_语音生成.Controls.Add(this.cb_语音生成_自动开始任务);
            this.pan_语音生成.Controls.Add(this.dgv_语音生成_任务清单);
            this.pan_语音生成.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_语音生成.Location = new System.Drawing.Point(0, 0);
            this.pan_语音生成.Name = "pan_语音生成";
            this.pan_语音生成.Size = new System.Drawing.Size(1301, 652);
            this.pan_语音生成.TabIndex = 0;
            // 
            // bt_语音生成_打开角色声音预览目录
            // 
            this.bt_语音生成_打开角色声音预览目录.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成_打开角色声音预览目录.Location = new System.Drawing.Point(1150, 8);
            this.bt_语音生成_打开角色声音预览目录.Name = "bt_语音生成_打开角色声音预览目录";
            this.bt_语音生成_打开角色声音预览目录.Size = new System.Drawing.Size(146, 23);
            this.bt_语音生成_打开角色声音预览目录.TabIndex = 21;
            this.bt_语音生成_打开角色声音预览目录.Text = "打开角色声音预览目录";
            this.bt_语音生成_打开角色声音预览目录.UseVisualStyleBackColor = true;
            this.bt_语音生成_打开角色声音预览目录.Click += new System.EventHandler(this.bt_语音生成_打开角色声音预览目录_Click);
            // 
            // pan_语音生成_工作状态
            // 
            this.pan_语音生成_工作状态.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_语音生成_工作状态.Controls.Add(this.lab_语音生成_任务提交信息);
            this.pan_语音生成_工作状态.Controls.Add(this.lab_语音生成_选中行信息);
            this.pan_语音生成_工作状态.Location = new System.Drawing.Point(5, 628);
            this.pan_语音生成_工作状态.Name = "pan_语音生成_工作状态";
            this.pan_语音生成_工作状态.Size = new System.Drawing.Size(1291, 19);
            this.pan_语音生成_工作状态.TabIndex = 20;
            // 
            // lab_语音生成_任务提交信息
            // 
            this.lab_语音生成_任务提交信息.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_语音生成_任务提交信息.Location = new System.Drawing.Point(3, 3);
            this.lab_语音生成_任务提交信息.Name = "lab_语音生成_任务提交信息";
            this.lab_语音生成_任务提交信息.Size = new System.Drawing.Size(900, 14);
            this.lab_语音生成_任务提交信息.TabIndex = 2;
            this.lab_语音生成_任务提交信息.Text = "没有任务提交";
            this.lab_语音生成_任务提交信息.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lab_语音生成_选中行信息
            // 
            this.lab_语音生成_选中行信息.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_语音生成_选中行信息.Location = new System.Drawing.Point(988, 3);
            this.lab_语音生成_选中行信息.Name = "lab_语音生成_选中行信息";
            this.lab_语音生成_选中行信息.Size = new System.Drawing.Size(300, 14);
            this.lab_语音生成_选中行信息.TabIndex = 1;
            this.lab_语音生成_选中行信息.Text = "共有 {0} 行，选中 {1} 行";
            this.lab_语音生成_选中行信息.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bt_语音生成_设置临时目录
            // 
            this.bt_语音生成_设置临时目录.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成_设置临时目录.Location = new System.Drawing.Point(942, 7);
            this.bt_语音生成_设置临时目录.Name = "bt_语音生成_设置临时目录";
            this.bt_语音生成_设置临时目录.Size = new System.Drawing.Size(98, 23);
            this.bt_语音生成_设置临时目录.TabIndex = 19;
            this.bt_语音生成_设置临时目录.Text = "更改临时目录";
            this.bt_语音生成_设置临时目录.UseVisualStyleBackColor = true;
            this.bt_语音生成_设置临时目录.Click += new System.EventHandler(this.bt_语音生成_设置临时目录_Click);
            // 
            // bt_语音生成_打开临时目录
            // 
            this.bt_语音生成_打开临时目录.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_语音生成_打开临时目录.Location = new System.Drawing.Point(1046, 8);
            this.bt_语音生成_打开临时目录.Name = "bt_语音生成_打开临时目录";
            this.bt_语音生成_打开临时目录.Size = new System.Drawing.Size(98, 23);
            this.bt_语音生成_打开临时目录.TabIndex = 18;
            this.bt_语音生成_打开临时目录.Text = "打开临时目录";
            this.bt_语音生成_打开临时目录.UseVisualStyleBackColor = true;
            this.bt_语音生成_打开临时目录.Click += new System.EventHandler(this.bt_语音生成_打开临时目录_Click);
            // 
            // bt_语音生成_错后
            // 
            this.bt_语音生成_错后.Location = new System.Drawing.Point(143, 7);
            this.bt_语音生成_错后.Name = "bt_语音生成_错后";
            this.bt_语音生成_错后.Size = new System.Drawing.Size(32, 23);
            this.bt_语音生成_错后.TabIndex = 2;
            this.bt_语音生成_错后.Text = "↓";
            this.bt_语音生成_错后.UseVisualStyleBackColor = true;
            this.bt_语音生成_错后.Click += new System.EventHandler(this.bt_语音生成_错后_Click);
            // 
            // bt_语音生成_提前
            // 
            this.bt_语音生成_提前.Location = new System.Drawing.Point(105, 7);
            this.bt_语音生成_提前.Name = "bt_语音生成_提前";
            this.bt_语音生成_提前.Size = new System.Drawing.Size(32, 23);
            this.bt_语音生成_提前.TabIndex = 2;
            this.bt_语音生成_提前.Text = "↑";
            this.bt_语音生成_提前.UseVisualStyleBackColor = true;
            this.bt_语音生成_提前.Click += new System.EventHandler(this.bt_语音生成_提前_Click);
            // 
            // cb_语音生成_自动开始任务
            // 
            this.cb_语音生成_自动开始任务.AutoSize = true;
            this.cb_语音生成_自动开始任务.Checked = true;
            this.cb_语音生成_自动开始任务.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_语音生成_自动开始任务.Location = new System.Drawing.Point(3, 12);
            this.cb_语音生成_自动开始任务.Name = "cb_语音生成_自动开始任务";
            this.cb_语音生成_自动开始任务.Size = new System.Drawing.Size(96, 16);
            this.cb_语音生成_自动开始任务.TabIndex = 1;
            this.cb_语音生成_自动开始任务.Text = "自动开始任务";
            this.cb_语音生成_自动开始任务.UseVisualStyleBackColor = true;
            this.cb_语音生成_自动开始任务.CheckedChanged += new System.EventHandler(this.cb_语音生成_自动开始任务_CheckedChanged);
            // 
            // dgv_语音生成_任务清单
            // 
            this.dgv_语音生成_任务清单.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_语音生成_任务清单.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_语音生成_任务清单.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_语音生成_任务清单.ContextMenuStrip = this.cms_语音生成_任务控制;
            this.dgv_语音生成_任务清单.Location = new System.Drawing.Point(3, 36);
            this.dgv_语音生成_任务清单.Name = "dgv_语音生成_任务清单";
            this.dgv_语音生成_任务清单.ReadOnly = true;
            this.dgv_语音生成_任务清单.RowHeadersVisible = false;
            this.dgv_语音生成_任务清单.RowTemplate.Height = 23;
            this.dgv_语音生成_任务清单.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_语音生成_任务清单.Size = new System.Drawing.Size(1293, 586);
            this.dgv_语音生成_任务清单.TabIndex = 0;
            this.dgv_语音生成_任务清单.SelectionChanged += new System.EventHandler(this.dgv_语音生成_任务清单_SelectionChanged);
            this.dgv_语音生成_任务清单.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgv_语音生成_任务清单_MouseDown);
            // 
            // cms_语音生成_任务控制
            // 
            this.cms_语音生成_任务控制.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.启动SToolStripMenuItem,
            this.停止TToolStripMenuItem,
            this.重新启动SToolStripMenuItem,
            this.toolStripSeparator1,
            this.上移UToolStripMenuItem,
            this.下移DToolStripMenuItem,
            this.toolStripSeparator2,
            this.预览声音MToolStripMenuItem,
            this.打开临时文件夹EToolStripMenuItem,
            this.打开文件夹FToolStripMenuItem,
            this.修改存储文件夹MToolStripMenuItem,
            this.toolStripMenuItem4,
            this.复制CToolStripMenuItem,
            this.删除RToolStripMenuItem,
            this.toolStripMenuItem3,
            this.清空所有任务AToolStripMenuItem});
            this.cms_语音生成_任务控制.Name = "cms_TTL生成队列";
            this.cms_语音生成_任务控制.Size = new System.Drawing.Size(190, 292);
            this.cms_语音生成_任务控制.Opening += new System.ComponentModel.CancelEventHandler(this.cms_语音生成_任务控制_Opening);
            // 
            // 启动SToolStripMenuItem
            // 
            this.启动SToolStripMenuItem.Name = "启动SToolStripMenuItem";
            this.启动SToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.启动SToolStripMenuItem.Text = "启动(&S)";
            this.启动SToolStripMenuItem.Click += new System.EventHandler(this.启动SToolStripMenuItem_Click);
            // 
            // 停止TToolStripMenuItem
            // 
            this.停止TToolStripMenuItem.Name = "停止TToolStripMenuItem";
            this.停止TToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.停止TToolStripMenuItem.Text = "停止(&T)";
            this.停止TToolStripMenuItem.Click += new System.EventHandler(this.暂停TToolStripMenuItem_Click);
            // 
            // 重新启动SToolStripMenuItem
            // 
            this.重新启动SToolStripMenuItem.Name = "重新启动SToolStripMenuItem";
            this.重新启动SToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.重新启动SToolStripMenuItem.Text = "重新启动(&S)";
            this.重新启动SToolStripMenuItem.Click += new System.EventHandler(this.重新启动SToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // 上移UToolStripMenuItem
            // 
            this.上移UToolStripMenuItem.Name = "上移UToolStripMenuItem";
            this.上移UToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.上移UToolStripMenuItem.Text = "上移(&U)";
            this.上移UToolStripMenuItem.Click += new System.EventHandler(this.上移UToolStripMenuItem_Click);
            // 
            // 下移DToolStripMenuItem
            // 
            this.下移DToolStripMenuItem.Name = "下移DToolStripMenuItem";
            this.下移DToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.下移DToolStripMenuItem.Text = "下移(&D)";
            this.下移DToolStripMenuItem.Click += new System.EventHandler(this.下移DToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // 预览声音MToolStripMenuItem
            // 
            this.预览声音MToolStripMenuItem.Name = "预览声音MToolStripMenuItem";
            this.预览声音MToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.预览声音MToolStripMenuItem.Text = "预览声音(&M)";
            this.预览声音MToolStripMenuItem.Click += new System.EventHandler(this.预览声音MToolStripMenuItem_Click);
            // 
            // 打开临时文件夹EToolStripMenuItem
            // 
            this.打开临时文件夹EToolStripMenuItem.Name = "打开临时文件夹EToolStripMenuItem";
            this.打开临时文件夹EToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.打开临时文件夹EToolStripMenuItem.Text = "打开临时文件夹(&E)";
            this.打开临时文件夹EToolStripMenuItem.Click += new System.EventHandler(this.打开临时文件夹EToolStripMenuItem_Click);
            // 
            // 打开文件夹FToolStripMenuItem
            // 
            this.打开文件夹FToolStripMenuItem.Name = "打开文件夹FToolStripMenuItem";
            this.打开文件夹FToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.打开文件夹FToolStripMenuItem.Text = "打开保存文件夹(&F)";
            this.打开文件夹FToolStripMenuItem.Click += new System.EventHandler(this.打开文件夹FToolStripMenuItem_Click);
            // 
            // 修改存储文件夹MToolStripMenuItem
            // 
            this.修改存储文件夹MToolStripMenuItem.Name = "修改存储文件夹MToolStripMenuItem";
            this.修改存储文件夹MToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.修改存储文件夹MToolStripMenuItem.Text = "修改保存文件夹(&M)...";
            this.修改存储文件夹MToolStripMenuItem.Click += new System.EventHandler(this.修改存储文件夹MToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(186, 6);
            // 
            // 复制CToolStripMenuItem
            // 
            this.复制CToolStripMenuItem.Name = "复制CToolStripMenuItem";
            this.复制CToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.复制CToolStripMenuItem.Text = "复制(&C)";
            this.复制CToolStripMenuItem.Click += new System.EventHandler(this.复制CToolStripMenuItem_Click);
            // 
            // 删除RToolStripMenuItem
            // 
            this.删除RToolStripMenuItem.Name = "删除RToolStripMenuItem";
            this.删除RToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.删除RToolStripMenuItem.Text = "删除(&R)";
            this.删除RToolStripMenuItem.Click += new System.EventHandler(this.删除RToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(186, 6);
            // 
            // 清空所有任务AToolStripMenuItem
            // 
            this.清空所有任务AToolStripMenuItem.Name = "清空所有任务AToolStripMenuItem";
            this.清空所有任务AToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.清空所有任务AToolStripMenuItem.Text = "清空所有任务(&A)...";
            this.清空所有任务AToolStripMenuItem.Click += new System.EventHandler(this.清空所有任务AToolStripMenuItem_Click);
            // 
            // VoiceGenerationPanel
            // 
            this.Controls.Add(this.pan_语音生成);
            this.Name = "VoiceGenerationPanel";
            this.Size = new System.Drawing.Size(1301, 652);
            this.pan_语音生成.ResumeLayout(false);
            this.pan_语音生成.PerformLayout();
            this.pan_语音生成_工作状态.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_语音生成_任务清单)).EndInit();
            this.cms_语音生成_任务控制.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// 语音生成主面板。
        /// </summary>
        internal System.Windows.Forms.Panel pan_语音生成;

        /// <summary>
        /// 打开角色声音预览目录按钮。
        /// </summary>
        internal System.Windows.Forms.Button bt_语音生成_打开角色声音预览目录;

        /// <summary>
        /// 语音生成工作状态面板。
        /// </summary>
        internal System.Windows.Forms.Panel pan_语音生成_工作状态;

        /// <summary>
        /// 语音生成任务提交信息标签。
        /// </summary>
        internal System.Windows.Forms.Label lab_语音生成_任务提交信息;

        /// <summary>
        /// 语音生成选中行信息标签。
        /// </summary>
        internal System.Windows.Forms.Label lab_语音生成_选中行信息;

        /// <summary>
        /// 设置临时目录按钮。
        /// </summary>
        internal System.Windows.Forms.Button bt_语音生成_设置临时目录;

        /// <summary>
        /// 打开临时目录按钮。
        /// </summary>
        internal System.Windows.Forms.Button bt_语音生成_打开临时目录;

        /// <summary>
        /// 错后按钮。
        /// </summary>
        internal System.Windows.Forms.Button bt_语音生成_错后;

        /// <summary>
        /// 提前按钮。
        /// </summary>
        internal System.Windows.Forms.Button bt_语音生成_提前;

        /// <summary>
        /// 自动开始任务复选框。
        /// </summary>
        internal System.Windows.Forms.CheckBox cb_语音生成_自动开始任务;

        /// <summary>
        /// 语音生成任务清单表格。
        /// </summary>
        internal System.Windows.Forms.DataGridView dgv_语音生成_任务清单;

        /// <summary>
        /// 语音生成任务控制右键菜单。
        /// </summary>
        internal System.Windows.Forms.ContextMenuStrip cms_语音生成_任务控制;

        /// <summary>
        /// 启动菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 启动SToolStripMenuItem;

        /// <summary>
        /// 停止菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 停止TToolStripMenuItem;

        /// <summary>
        /// 重新启动菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 重新启动SToolStripMenuItem;

        /// <summary>
        /// 分隔线1。
        /// </summary>
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

        /// <summary>
        /// 上移菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 上移UToolStripMenuItem;

        /// <summary>
        /// 下移菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 下移DToolStripMenuItem;

        /// <summary>
        /// 分隔线2。
        /// </summary>
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

        /// <summary>
        /// 预览声音菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 预览声音MToolStripMenuItem;

        /// <summary>
        /// 打开临时文件夹菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 打开临时文件夹EToolStripMenuItem;

        /// <summary>
        /// 打开文件夹菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 打开文件夹FToolStripMenuItem;

        /// <summary>
        /// 修改存储文件夹菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 修改存储文件夹MToolStripMenuItem;

        /// <summary>
        /// 分隔线4。
        /// </summary>
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;

        /// <summary>
        /// 复制菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 复制CToolStripMenuItem;

        /// <summary>
        /// 删除菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 删除RToolStripMenuItem;

        /// <summary>
        /// 分隔线3。
        /// </summary>
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;

        /// <summary>
        /// 清空所有任务菜单项。
        /// </summary>
        internal System.Windows.Forms.ToolStripMenuItem 清空所有任务AToolStripMenuItem;
    }
}
