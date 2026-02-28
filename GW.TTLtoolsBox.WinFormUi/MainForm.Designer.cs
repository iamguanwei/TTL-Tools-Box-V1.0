namespace GW.TTLtoolsBox.WinFormUi
{
    partial class MainForm
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ms_主菜单 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.保存SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.另存为AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.退出EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pan_主框架 = new System.Windows.Forms.Panel();
            this.tc_工具箱 = new System.Windows.Forms.TabControl();
            this.tp_TTL方案 = new System.Windows.Forms.TabPage();
            this.tp_角色映射 = new System.Windows.Forms.TabPage();
            this.pan_角色映射 = new System.Windows.Forms.Panel();
            this.tp_文本拆分 = new System.Windows.Forms.TabPage();
            this.tp_多音字替换 = new System.Windows.Forms.TabPage();
            this.tp_角色和情绪指定 = new System.Windows.Forms.TabPage();
            this.pan_角色和情绪指定 = new System.Windows.Forms.Panel();
            this.tp_语音生成预处理 = new System.Windows.Forms.TabPage();
            this.pan_语音生成预处理 = new System.Windows.Forms.Panel();
            this.tp_语音生成 = new System.Windows.Forms.TabPage();
            this.pan_语音生成 = new System.Windows.Forms.Panel();
            this.ss_主状态栏 = new System.Windows.Forms.StatusStrip();
            this.tssl_TTL连接状态显示 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ms_主菜单.SuspendLayout();
            this.pan_主框架.SuspendLayout();
            this.tc_工具箱.SuspendLayout();
            this.tp_TTL方案.SuspendLayout();
            this.tp_角色映射.SuspendLayout();
            this.pan_角色映射.SuspendLayout();
            this.tp_文本拆分.SuspendLayout();
            this.tp_多音字替换.SuspendLayout();
            this.tp_角色和情绪指定.SuspendLayout();
            this.pan_角色和情绪指定.SuspendLayout();
            this.tp_语音生成预处理.SuspendLayout();
            this.pan_语音生成预处理.SuspendLayout();
            this.tp_语音生成.SuspendLayout();
            this.pan_语音生成.SuspendLayout();
            this.ss_主状态栏.SuspendLayout();
            this.SuspendLayout();
            // 
            // ms_主菜单
            // 
            this.ms_主菜单.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem});
            this.ms_主菜单.Location = new System.Drawing.Point(0, 0);
            this.ms_主菜单.Name = "ms_主菜单";
            this.ms_主菜单.Size = new System.Drawing.Size(1315, 25);
            this.ms_主菜单.TabIndex = 0;
            this.ms_主菜单.Text = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripMenuItem,
            this.打开OToolStripMenuItem,
            this.toolStripMenuItem2,
            this.保存SToolStripMenuItem,
            this.另存为AToolStripMenuItem,
            this.toolStripMenuItem1,
            this.退出EToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // 新建NToolStripMenuItem
            // 
            this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
            this.新建NToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.新建NToolStripMenuItem.Text = "新建(&N)...";
            this.新建NToolStripMenuItem.Click += new System.EventHandler(this.新建NToolStripMenuItem_Click);
            // 
            // 打开OToolStripMenuItem
            // 
            this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            this.打开OToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.打开OToolStripMenuItem.Text = "打开(&O)...";
            this.打开OToolStripMenuItem.Click += new System.EventHandler(this.打开OToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 6);
            // 
            // 保存SToolStripMenuItem
            // 
            this.保存SToolStripMenuItem.Name = "保存SToolStripMenuItem";
            this.保存SToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.保存SToolStripMenuItem.Text = "保存(&S)";
            this.保存SToolStripMenuItem.Click += new System.EventHandler(this.保存SToolStripMenuItem_Click);
            // 
            // 另存为AToolStripMenuItem
            // 
            this.另存为AToolStripMenuItem.Name = "另存为AToolStripMenuItem";
            this.另存为AToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.另存为AToolStripMenuItem.Text = "另存为(&A)...";
            this.另存为AToolStripMenuItem.Click += new System.EventHandler(this.另存为AToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
            // 
            // 退出EToolStripMenuItem
            // 
            this.退出EToolStripMenuItem.Name = "退出EToolStripMenuItem";
            this.退出EToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.退出EToolStripMenuItem.Text = "退出(&E)";
            this.退出EToolStripMenuItem.Click += new System.EventHandler(this.退出EToolStripMenuItem_Click);
            // 
            // pan_主框架
            // 
            this.pan_主框架.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_主框架.Controls.Add(this.tc_工具箱);
            this.pan_主框架.Location = new System.Drawing.Point(0, 25);
            this.pan_主框架.Name = "pan_主框架";
            this.pan_主框架.Size = new System.Drawing.Size(1315, 684);
            this.pan_主框架.TabIndex = 1;
            // 
            // tc_工具箱
            // 
            this.tc_工具箱.Controls.Add(this.tp_TTL方案);
            this.tc_工具箱.Controls.Add(this.tp_角色映射);
            this.tc_工具箱.Controls.Add(this.tp_文本拆分);
            this.tc_工具箱.Controls.Add(this.tp_多音字替换);
            this.tc_工具箱.Controls.Add(this.tp_角色和情绪指定);
            this.tc_工具箱.Controls.Add(this.tp_语音生成预处理);
            this.tc_工具箱.Controls.Add(this.tp_语音生成);
            this.tc_工具箱.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_工具箱.Location = new System.Drawing.Point(0, 0);
            this.tc_工具箱.Name = "tc_工具箱";
            this.tc_工具箱.SelectedIndex = 0;
            this.tc_工具箱.Size = new System.Drawing.Size(1315, 684);
            this.tc_工具箱.TabIndex = 1;
            this.tc_工具箱.SelectedIndexChanged += new System.EventHandler(this.tc_工具箱_SelectedIndexChanged);
            // 
            // tp_TTL方案
            // 
            this.tp_TTL方案.Location = new System.Drawing.Point(4, 22);
            this.tp_TTL方案.Name = "tp_TTL方案";
            this.tp_TTL方案.Padding = new System.Windows.Forms.Padding(3);
            this.tp_TTL方案.Size = new System.Drawing.Size(1307, 658);
            this.tp_TTL方案.TabIndex = 4;
            this.tp_TTL方案.Text = "TTL方案";
            this.tp_TTL方案.UseVisualStyleBackColor = true;
            // 
            // tp_角色映射
            // 
            this.tp_角色映射.Controls.Add(this.pan_角色映射);
            this.tp_角色映射.Location = new System.Drawing.Point(4, 22);
            this.tp_角色映射.Name = "tp_角色映射";
            this.tp_角色映射.Padding = new System.Windows.Forms.Padding(3);
            this.tp_角色映射.Size = new System.Drawing.Size(1307, 658);
            this.tp_角色映射.TabIndex = 6;
            this.tp_角色映射.Text = "角色映射";
            this.tp_角色映射.UseVisualStyleBackColor = true;
            // 
            // pan_角色映射
            // 
            this.pan_角色映射.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_角色映射.Location = new System.Drawing.Point(3, 3);
            this.pan_角色映射.Name = "pan_角色映射";
            this.pan_角色映射.Size = new System.Drawing.Size(1301, 652);
            this.pan_角色映射.TabIndex = 0;
            // 
            // tp_文本拆分
            // 
            this.tp_文本拆分.Location = new System.Drawing.Point(4, 22);
            this.tp_文本拆分.Name = "tp_文本拆分";
            this.tp_文本拆分.Padding = new System.Windows.Forms.Padding(3);
            this.tp_文本拆分.Size = new System.Drawing.Size(1307, 658);
            this.tp_文本拆分.TabIndex = 0;
            this.tp_文本拆分.Text = "文本拆分";
            this.tp_文本拆分.UseVisualStyleBackColor = true;
            // 
            // tp_多音字替换
            // 
            this.tp_多音字替换.Location = new System.Drawing.Point(4, 22);
            this.tp_多音字替换.Name = "tp_多音字替换";
            this.tp_多音字替换.Padding = new System.Windows.Forms.Padding(3);
            this.tp_多音字替换.Size = new System.Drawing.Size(1307, 658);
            this.tp_多音字替换.TabIndex = 3;
            this.tp_多音字替换.Text = "多音字替换";
            this.tp_多音字替换.UseVisualStyleBackColor = true;
            // 
            // tp_角色和情绪指定
            // 
            this.tp_角色和情绪指定.Controls.Add(this.pan_角色和情绪指定);
            this.tp_角色和情绪指定.Location = new System.Drawing.Point(4, 22);
            this.tp_角色和情绪指定.Name = "tp_角色和情绪指定";
            this.tp_角色和情绪指定.Padding = new System.Windows.Forms.Padding(3);
            this.tp_角色和情绪指定.Size = new System.Drawing.Size(1307, 658);
            this.tp_角色和情绪指定.TabIndex = 1;
            this.tp_角色和情绪指定.Text = "角色和情绪指定";
            this.tp_角色和情绪指定.UseVisualStyleBackColor = true;
            // 
            // pan_角色和情绪指定
            // 
            this.pan_角色和情绪指定.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_角色和情绪指定.Location = new System.Drawing.Point(3, 3);
            this.pan_角色和情绪指定.Name = "pan_角色和情绪指定";
            this.pan_角色和情绪指定.Size = new System.Drawing.Size(1301, 652);
            this.pan_角色和情绪指定.TabIndex = 0;
            // 
            // tp_语音生成预处理
            // 
            this.tp_语音生成预处理.Controls.Add(this.pan_语音生成预处理);
            this.tp_语音生成预处理.Location = new System.Drawing.Point(4, 22);
            this.tp_语音生成预处理.Name = "tp_语音生成预处理";
            this.tp_语音生成预处理.Padding = new System.Windows.Forms.Padding(3);
            this.tp_语音生成预处理.Size = new System.Drawing.Size(1307, 658);
            this.tp_语音生成预处理.TabIndex = 5;
            this.tp_语音生成预处理.Text = "语音生成预处理";
            this.tp_语音生成预处理.UseVisualStyleBackColor = true;
            // 
            // pan_语音生成预处理
            // 
            this.pan_语音生成预处理.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_语音生成预处理.Location = new System.Drawing.Point(3, 3);
            this.pan_语音生成预处理.Name = "pan_语音生成预处理";
            this.pan_语音生成预处理.Size = new System.Drawing.Size(1301, 652);
            this.pan_语音生成预处理.TabIndex = 0;
            // 
            // tp_语音生成
            // 
            this.tp_语音生成.Controls.Add(this.pan_语音生成);
            this.tp_语音生成.Location = new System.Drawing.Point(4, 22);
            this.tp_语音生成.Name = "tp_语音生成";
            this.tp_语音生成.Padding = new System.Windows.Forms.Padding(3);
            this.tp_语音生成.Size = new System.Drawing.Size(1307, 658);
            this.tp_语音生成.TabIndex = 2;
            this.tp_语音生成.Text = "语音生成";
            this.tp_语音生成.UseVisualStyleBackColor = true;
            // 
            // pan_语音生成
            // 
            this.pan_语音生成.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_语音生成.Location = new System.Drawing.Point(3, 3);
            this.pan_语音生成.Name = "pan_语音生成";
            this.pan_语音生成.Size = new System.Drawing.Size(1301, 652);
            this.pan_语音生成.TabIndex = 0;
            // 
            // ss_主状态栏
            // 
            this.ss_主状态栏.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_TTL连接状态显示});
            this.ss_主状态栏.Location = new System.Drawing.Point(0, 712);
            this.ss_主状态栏.Name = "ss_主状态栏";
            this.ss_主状态栏.Size = new System.Drawing.Size(1315, 22);
            this.ss_主状态栏.TabIndex = 2;
            this.ss_主状态栏.Text = "statusStrip1";
            // 
            // tssl_TTL连接状态显示
            // 
            this.tssl_TTL连接状态显示.Name = "tssl_TTL连接状态显示";
            this.tssl_TTL连接状态显示.Size = new System.Drawing.Size(76, 17);
            this.tssl_TTL连接状态显示.Text = "TTL连接状态";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1315, 734);
            this.Controls.Add(this.ss_主状态栏);
            this.Controls.Add(this.pan_主框架);
            this.Controls.Add(this.ms_主菜单);
            this.MainMenuStrip = this.ms_主菜单;
            this.Name = "MainForm";
            this.Text = "TTL 工具箱";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ms_主菜单.ResumeLayout(false);
            this.ms_主菜单.PerformLayout();
            this.pan_主框架.ResumeLayout(false);
            this.tc_工具箱.ResumeLayout(false);
            this.tp_TTL方案.ResumeLayout(false);
            this.tp_角色映射.ResumeLayout(false);
            this.pan_角色映射.ResumeLayout(false);
            this.tp_文本拆分.ResumeLayout(false);
            this.tp_多音字替换.ResumeLayout(false);
            this.tp_角色和情绪指定.ResumeLayout(false);
            this.pan_角色和情绪指定.ResumeLayout(false);
            this.tp_语音生成预处理.ResumeLayout(false);
            this.pan_语音生成预处理.ResumeLayout(false);
            this.tp_语音生成.ResumeLayout(false);
            this.pan_语音生成.ResumeLayout(false);
            this.ss_主状态栏.ResumeLayout(false);
            this.ss_主状态栏.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip ms_主菜单;
        internal System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        internal System.Windows.Forms.Panel pan_主框架;
        internal System.Windows.Forms.TabControl tc_工具箱;
        internal System.Windows.Forms.TabPage tp_文本拆分;
        internal System.Windows.Forms.TabPage tp_多音字替换;
        internal System.Windows.Forms.TabPage tp_角色和情绪指定;
        internal System.Windows.Forms.TabPage tp_语音生成预处理;
        internal System.Windows.Forms.TabPage tp_语音生成;
        internal System.Windows.Forms.TabPage tp_TTL方案;
        internal System.Windows.Forms.Panel pan_角色和情绪指定;
        internal System.Windows.Forms.ToolStripMenuItem 保存SToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem 另存为AToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem 新建NToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        internal System.Windows.Forms.ToolStripMenuItem 退出EToolStripMenuItem;
        internal System.Windows.Forms.Panel pan_语音生成预处理;
        internal System.Windows.Forms.TabPage tp_角色映射;
        internal System.Windows.Forms.ToolStripMenuItem 打开OToolStripMenuItem;
        internal System.Windows.Forms.Panel pan_角色映射;
        internal System.Windows.Forms.Panel pan_语音生成;
        internal System.Windows.Forms.StatusStrip ss_主状态栏;
        internal System.Windows.Forms.ToolStripStatusLabel tssl_TTL连接状态显示;
    }
}
