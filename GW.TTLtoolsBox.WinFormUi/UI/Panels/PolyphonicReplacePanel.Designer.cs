namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class PolyphonicReplacePanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.sc_主分割 = new System.Windows.Forms.SplitContainer();
            this.gb_替换工作面板 = new System.Windows.Forms.GroupBox();
            this.pan_实施工作面板 = new System.Windows.Forms.Panel();
            this.lab_替换工作信息 = new System.Windows.Forms.Label();
            this.bt_取消应用替换结果 = new System.Windows.Forms.Button();
            this.bt_应用替换结果 = new System.Windows.Forms.Button();
            this.bt_下一组替换目标 = new System.Windows.Forms.Button();
            this.bt_下一条替换目标 = new System.Windows.Forms.Button();
            this.bt_上一条替换目标 = new System.Windows.Forms.Button();
            this.pan_待选词 = new System.Windows.Forms.Panel();
            this.rb_待选词模板 = new System.Windows.Forms.RadioButton();
            this.lab_下文 = new System.Windows.Forms.Label();
            this.lab_替换目标 = new System.Windows.Forms.Label();
            this.lab_上文 = new System.Windows.Forms.Label();
            this.lab_选择多音字方案 = new System.Windows.Forms.Label();
            this.bt_开始替换最终文本中的多音字 = new System.Windows.Forms.Button();
            this.cb_选择多音字方案 = new System.Windows.Forms.ComboBox();
            this.sc_副面板主分割 = new System.Windows.Forms.SplitContainer();
            this.gb_替换方案 = new System.Windows.Forms.GroupBox();
            this.dgv_多音字方案 = new System.Windows.Forms.DataGridView();
            this.bt_还原多音字方案 = new System.Windows.Forms.Button();
            this.bt_保存多音字方案 = new System.Windows.Forms.Button();
            this.gb_最终文本 = new System.Windows.Forms.GroupBox();
            this.bt_清理最终文本 = new System.Windows.Forms.Button();
            this.bt_发送最终文本到下一个步骤 = new System.Windows.Forms.Button();
            this.bt_复制最终文本 = new System.Windows.Forms.Button();
            this.tb_最终文本 = new System.Windows.Forms.TextBox();
            this.cms_多音字替换方案 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.自动生成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.sc_主分割)).BeginInit();
            this.sc_主分割.Panel1.SuspendLayout();
            this.sc_主分割.Panel2.SuspendLayout();
            this.sc_主分割.SuspendLayout();
            this.gb_替换工作面板.SuspendLayout();
            this.pan_实施工作面板.SuspendLayout();
            this.pan_待选词.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sc_副面板主分割)).BeginInit();
            this.sc_副面板主分割.Panel1.SuspendLayout();
            this.sc_副面板主分割.Panel2.SuspendLayout();
            this.sc_副面板主分割.SuspendLayout();
            this.gb_替换方案.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_多音字方案)).BeginInit();
            this.gb_最终文本.SuspendLayout();
            this.cms_多音字替换方案.SuspendLayout();
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
            this.sc_主分割.Panel1.Controls.Add(this.gb_替换工作面板);
            // 
            // sc_主分割.Panel2
            // 
            this.sc_主分割.Panel2.Controls.Add(this.sc_副面板主分割);
            this.sc_主分割.Size = new System.Drawing.Size(1301, 652);
            this.sc_主分割.SplitterDistance = 590;
            this.sc_主分割.TabIndex = 0;
            // 
            // gb_替换工作面板
            // 
            this.gb_替换工作面板.Controls.Add(this.pan_实施工作面板);
            this.gb_替换工作面板.Controls.Add(this.lab_选择多音字方案);
            this.gb_替换工作面板.Controls.Add(this.bt_开始替换最终文本中的多音字);
            this.gb_替换工作面板.Controls.Add(this.cb_选择多音字方案);
            this.gb_替换工作面板.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_替换工作面板.Location = new System.Drawing.Point(0, 0);
            this.gb_替换工作面板.Name = "gb_替换工作面板";
            this.gb_替换工作面板.Size = new System.Drawing.Size(590, 652);
            this.gb_替换工作面板.TabIndex = 2;
            this.gb_替换工作面板.TabStop = false;
            this.gb_替换工作面板.Text = "工作面板";
            // 
            // pan_实施工作面板
            // 
            this.pan_实施工作面板.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_实施工作面板.Controls.Add(this.lab_替换工作信息);
            this.pan_实施工作面板.Controls.Add(this.bt_取消应用替换结果);
            this.pan_实施工作面板.Controls.Add(this.bt_应用替换结果);
            this.pan_实施工作面板.Controls.Add(this.bt_下一组替换目标);
            this.pan_实施工作面板.Controls.Add(this.bt_下一条替换目标);
            this.pan_实施工作面板.Controls.Add(this.bt_上一条替换目标);
            this.pan_实施工作面板.Controls.Add(this.pan_待选词);
            this.pan_实施工作面板.Controls.Add(this.lab_下文);
            this.pan_实施工作面板.Controls.Add(this.lab_替换目标);
            this.pan_实施工作面板.Controls.Add(this.lab_上文);
            this.pan_实施工作面板.Location = new System.Drawing.Point(3, 106);
            this.pan_实施工作面板.Name = "pan_实施工作面板";
            this.pan_实施工作面板.Size = new System.Drawing.Size(581, 540);
            this.pan_实施工作面板.TabIndex = 14;
            // 
            // lab_替换工作信息
            // 
            this.lab_替换工作信息.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_替换工作信息.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_替换工作信息.ForeColor = System.Drawing.Color.DarkGreen;
            this.lab_替换工作信息.Location = new System.Drawing.Point(5, 165);
            this.lab_替换工作信息.Name = "lab_替换工作信息";
            this.lab_替换工作信息.Size = new System.Drawing.Size(573, 19);
            this.lab_替换工作信息.TabIndex = 12;
            this.lab_替换工作信息.Text = "共有 {0} 个多音词需要替换，正在替换第 {1} 个，剩余 {2} 个。本组共有 {3} 个，剩余 {4} 个。";
            this.lab_替换工作信息.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bt_取消应用替换结果
            // 
            this.bt_取消应用替换结果.Location = new System.Drawing.Point(74, 416);
            this.bt_取消应用替换结果.Name = "bt_取消应用替换结果";
            this.bt_取消应用替换结果.Size = new System.Drawing.Size(85, 28);
            this.bt_取消应用替换结果.TabIndex = 11;
            this.bt_取消应用替换结果.Text = "取消";
            this.bt_取消应用替换结果.UseVisualStyleBackColor = true;
            this.bt_取消应用替换结果.Click += new System.EventHandler(this.bt_取消应用替换结果_Click);
            // 
            // bt_应用替换结果
            // 
            this.bt_应用替换结果.Location = new System.Drawing.Point(74, 356);
            this.bt_应用替换结果.Name = "bt_应用替换结果";
            this.bt_应用替换结果.Size = new System.Drawing.Size(85, 51);
            this.bt_应用替换结果.TabIndex = 11;
            this.bt_应用替换结果.Text = "应用";
            this.bt_应用替换结果.UseVisualStyleBackColor = true;
            this.bt_应用替换结果.Click += new System.EventHandler(this.bt_应用替换结果_Click);
            // 
            // bt_下一组替换目标
            // 
            this.bt_下一组替换目标.Location = new System.Drawing.Point(74, 288);
            this.bt_下一组替换目标.Name = "bt_下一组替换目标";
            this.bt_下一组替换目标.Size = new System.Drawing.Size(85, 28);
            this.bt_下一组替换目标.TabIndex = 11;
            this.bt_下一组替换目标.Text = "下一组";
            this.bt_下一组替换目标.UseVisualStyleBackColor = true;
            this.bt_下一组替换目标.Click += new System.EventHandler(this.bt_下一组替换目标_Click);
            // 
            // bt_下一条替换目标
            // 
            this.bt_下一条替换目标.Location = new System.Drawing.Point(74, 230);
            this.bt_下一条替换目标.Name = "bt_下一条替换目标";
            this.bt_下一条替换目标.Size = new System.Drawing.Size(85, 28);
            this.bt_下一条替换目标.TabIndex = 11;
            this.bt_下一条替换目标.Text = "下一条";
            this.bt_下一条替换目标.UseVisualStyleBackColor = true;
            this.bt_下一条替换目标.Click += new System.EventHandler(this.bt_下一条替换目标_Click);
            // 
            // bt_上一条替换目标
            // 
            this.bt_上一条替换目标.Location = new System.Drawing.Point(74, 187);
            this.bt_上一条替换目标.Name = "bt_上一条替换目标";
            this.bt_上一条替换目标.Size = new System.Drawing.Size(85, 28);
            this.bt_上一条替换目标.TabIndex = 11;
            this.bt_上一条替换目标.Text = "上一条";
            this.bt_上一条替换目标.UseVisualStyleBackColor = true;
            this.bt_上一条替换目标.Click += new System.EventHandler(this.bt_上一条替换目标_Click);
            // 
            // pan_待选词
            // 
            this.pan_待选词.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_待选词.Controls.Add(this.rb_待选词模板);
            this.pan_待选词.Location = new System.Drawing.Point(194, 187);
            this.pan_待选词.Name = "pan_待选词";
            this.pan_待选词.Size = new System.Drawing.Size(384, 257);
            this.pan_待选词.TabIndex = 1;
            // 
            // rb_待选词模板
            // 
            this.rb_待选词模板.AutoSize = true;
            this.rb_待选词模板.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_待选词模板.ForeColor = System.Drawing.Color.BlueViolet;
            this.rb_待选词模板.Location = new System.Drawing.Point(3, 3);
            this.rb_待选词模板.Name = "rb_待选词模板";
            this.rb_待选词模板.Size = new System.Drawing.Size(70, 31);
            this.rb_待选词模板.TabIndex = 0;
            this.rb_待选词模板.TabStop = true;
            this.rb_待选词模板.Text = "模板";
            this.rb_待选词模板.UseVisualStyleBackColor = true;
            this.rb_待选词模板.Visible = false;
            // 
            // lab_下文
            // 
            this.lab_下文.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_下文.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_下文.Location = new System.Drawing.Point(3, 110);
            this.lab_下文.Name = "lab_下文";
            this.lab_下文.Size = new System.Drawing.Size(575, 30);
            this.lab_下文.TabIndex = 0;
            this.lab_下文.Text = "这里显示下文...";
            this.lab_下文.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lab_替换目标
            // 
            this.lab_替换目标.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_替换目标.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_替换目标.ForeColor = System.Drawing.Color.Red;
            this.lab_替换目标.Location = new System.Drawing.Point(3, 54);
            this.lab_替换目标.Name = "lab_替换目标";
            this.lab_替换目标.Size = new System.Drawing.Size(575, 56);
            this.lab_替换目标.TabIndex = 0;
            this.lab_替换目标.Text = "替换目标";
            this.lab_替换目标.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lab_上文
            // 
            this.lab_上文.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_上文.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_上文.Location = new System.Drawing.Point(3, 24);
            this.lab_上文.Name = "lab_上文";
            this.lab_上文.Size = new System.Drawing.Size(575, 30);
            this.lab_上文.TabIndex = 0;
            this.lab_上文.Text = "...这里显示上文";
            this.lab_上文.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lab_选择多音字方案
            // 
            this.lab_选择多音字方案.AutoSize = true;
            this.lab_选择多音字方案.Location = new System.Drawing.Point(6, 33);
            this.lab_选择多音字方案.Name = "lab_选择多音字方案";
            this.lab_选择多音字方案.Size = new System.Drawing.Size(65, 12);
            this.lab_选择多音字方案.TabIndex = 13;
            this.lab_选择多音字方案.Text = "选择方案：";
            // 
            // bt_开始替换最终文本中的多音字
            // 
            this.bt_开始替换最终文本中的多音字.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_开始替换最终文本中的多音字.Location = new System.Drawing.Point(480, 64);
            this.bt_开始替换最终文本中的多音字.Name = "bt_开始替换最终文本中的多音字";
            this.bt_开始替换最终文本中的多音字.Size = new System.Drawing.Size(104, 28);
            this.bt_开始替换最终文本中的多音字.TabIndex = 11;
            this.bt_开始替换最终文本中的多音字.Text = "开始替换";
            this.bt_开始替换最终文本中的多音字.UseVisualStyleBackColor = true;
            this.bt_开始替换最终文本中的多音字.Click += new System.EventHandler(this.bt_替换最终文本中的多音字_Click);
            // 
            // cb_选择多音字方案
            // 
            this.cb_选择多音字方案.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_选择多音字方案.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_选择多音字方案.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_选择多音字方案.FormattingEnabled = true;
            this.cb_选择多音字方案.Location = new System.Drawing.Point(77, 29);
            this.cb_选择多音字方案.Name = "cb_选择多音字方案";
            this.cb_选择多音字方案.Size = new System.Drawing.Size(507, 20);
            this.cb_选择多音字方案.TabIndex = 12;
            // 
            // sc_副面板主分割
            // 
            this.sc_副面板主分割.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_副面板主分割.Location = new System.Drawing.Point(0, 0);
            this.sc_副面板主分割.Name = "sc_副面板主分割";
            this.sc_副面板主分割.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sc_副面板主分割.Panel1
            // 
            this.sc_副面板主分割.Panel1.Controls.Add(this.gb_替换方案);
            // 
            // sc_副面板主分割.Panel2
            // 
            this.sc_副面板主分割.Panel2.Controls.Add(this.gb_最终文本);
            this.sc_副面板主分割.Size = new System.Drawing.Size(707, 652);
            this.sc_副面板主分割.SplitterDistance = 392;
            this.sc_副面板主分割.TabIndex = 1;
            // 
            // gb_替换方案
            // 
            this.gb_替换方案.Controls.Add(this.dgv_多音字方案);
            this.gb_替换方案.Controls.Add(this.bt_还原多音字方案);
            this.gb_替换方案.Controls.Add(this.bt_保存多音字方案);
            this.gb_替换方案.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_替换方案.Location = new System.Drawing.Point(0, 0);
            this.gb_替换方案.Name = "gb_替换方案";
            this.gb_替换方案.Size = new System.Drawing.Size(707, 392);
            this.gb_替换方案.TabIndex = 1;
            this.gb_替换方案.TabStop = false;
            this.gb_替换方案.Text = "多音字替换方案";
            // 
            // dgv_多音字方案
            // 
            this.dgv_多音字方案.AllowUserToOrderColumns = true;
            this.dgv_多音字方案.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_多音字方案.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_多音字方案.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_多音字方案.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_多音字方案.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_多音字方案.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_多音字方案.Location = new System.Drawing.Point(8, 20);
            this.dgv_多音字方案.Name = "dgv_多音字方案";
            this.dgv_多音字方案.RowTemplate.Height = 23;
            this.dgv_多音字方案.Size = new System.Drawing.Size(693, 332);
            this.dgv_多音字方案.TabIndex = 0;
            this.dgv_多音字方案.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_多音字方案_CellEndEdit);
            this.dgv_多音字方案.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_多音字方案_RowsAdded);
            this.dgv_多音字方案.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgv_多音字方案_UserDeletedRow);
            this.dgv_多音字方案.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgv_多音字方案_MouseDown);
            // 
            // bt_还原多音字方案
            // 
            this.bt_还原多音字方案.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_还原多音字方案.Location = new System.Drawing.Point(118, 358);
            this.bt_还原多音字方案.Name = "bt_还原多音字方案";
            this.bt_还原多音字方案.Size = new System.Drawing.Size(104, 28);
            this.bt_还原多音字方案.TabIndex = 6;
            this.bt_还原多音字方案.Text = "还原方案";
            this.bt_还原多音字方案.UseVisualStyleBackColor = true;
            this.bt_还原多音字方案.Click += new System.EventHandler(this.bt_还原多音字方案_Click);
            // 
            // bt_保存多音字方案
            // 
            this.bt_保存多音字方案.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_保存多音字方案.Location = new System.Drawing.Point(8, 358);
            this.bt_保存多音字方案.Name = "bt_保存多音字方案";
            this.bt_保存多音字方案.Size = new System.Drawing.Size(104, 28);
            this.bt_保存多音字方案.TabIndex = 5;
            this.bt_保存多音字方案.Text = "保存方案";
            this.bt_保存多音字方案.UseVisualStyleBackColor = true;
            this.bt_保存多音字方案.Click += new System.EventHandler(this.bt_保存多音字方案_Click);
            // 
            // gb_最终文本
            // 
            this.gb_最终文本.Controls.Add(this.bt_清理最终文本);
            this.gb_最终文本.Controls.Add(this.bt_发送最终文本到下一个步骤);
            this.gb_最终文本.Controls.Add(this.bt_复制最终文本);
            this.gb_最终文本.Controls.Add(this.tb_最终文本);
            this.gb_最终文本.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_最终文本.Location = new System.Drawing.Point(0, 0);
            this.gb_最终文本.Name = "gb_最终文本";
            this.gb_最终文本.Size = new System.Drawing.Size(707, 256);
            this.gb_最终文本.TabIndex = 0;
            this.gb_最终文本.TabStop = false;
            this.gb_最终文本.Text = "最终文本";
            // 
            // bt_清理最终文本
            // 
            this.bt_清理最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_清理最终文本.Location = new System.Drawing.Point(597, 19);
            this.bt_清理最终文本.Name = "bt_清理最终文本";
            this.bt_清理最终文本.Size = new System.Drawing.Size(104, 28);
            this.bt_清理最终文本.TabIndex = 10;
            this.bt_清理最终文本.Text = "清理文本";
            this.bt_清理最终文本.UseVisualStyleBackColor = true;
            this.bt_清理最终文本.Click += new System.EventHandler(this.bt_清理最终文本_Click);
            // 
            // bt_发送最终文本到下一个步骤
            // 
            this.bt_发送最终文本到下一个步骤.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_发送最终文本到下一个步骤.Location = new System.Drawing.Point(597, 222);
            this.bt_发送最终文本到下一个步骤.Name = "bt_发送最终文本到下一个步骤";
            this.bt_发送最终文本到下一个步骤.Size = new System.Drawing.Size(104, 28);
            this.bt_发送最终文本到下一个步骤.TabIndex = 9;
            this.bt_发送最终文本到下一个步骤.Text = "发送到下一步";
            this.bt_发送最终文本到下一个步骤.UseVisualStyleBackColor = true;
            this.bt_发送最终文本到下一个步骤.Click += new System.EventHandler(this.bt_发送最终文本到下一个步骤_Click);
            // 
            // bt_复制最终文本
            // 
            this.bt_复制最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_复制最终文本.Location = new System.Drawing.Point(597, 188);
            this.bt_复制最终文本.Name = "bt_复制最终文本";
            this.bt_复制最终文本.Size = new System.Drawing.Size(104, 28);
            this.bt_复制最终文本.TabIndex = 8;
            this.bt_复制最终文本.Text = "复制文本";
            this.bt_复制最终文本.UseVisualStyleBackColor = true;
            this.bt_复制最终文本.Click += new System.EventHandler(this.bt_复制最终文本_Click);
            // 
            // tb_最终文本
            // 
            this.tb_最终文本.AcceptsReturn = true;
            this.tb_最终文本.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_最终文本.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_最终文本.Location = new System.Drawing.Point(6, 20);
            this.tb_最终文本.Multiline = true;
            this.tb_最终文本.Name = "tb_最终文本";
            this.tb_最终文本.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_最终文本.Size = new System.Drawing.Size(585, 230);
            this.tb_最终文本.TabIndex = 1;
            this.tb_最终文本.TextChanged += new System.EventHandler(this.tb_最终文本_TextChanged);
            // 
            // cms_多音字替换方案
            // 
            this.cms_多音字替换方案.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.自动生成ToolStripMenuItem});
            this.cms_多音字替换方案.Name = "cms_多音字替换_多音字替换方案";
            this.cms_多音字替换方案.Size = new System.Drawing.Size(177, 26);
            // 
            // 自动生成ToolStripMenuItem
            // 
            this.自动生成ToolStripMenuItem.Name = "自动生成ToolStripMenuItem";
            this.自动生成ToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.自动生成ToolStripMenuItem.Text = "自动生成（替换原有内容）";
            this.自动生成ToolStripMenuItem.Click += new System.EventHandler(this.自动生成ToolStripMenuItem_Click);
            // 
            // PolyphonicReplacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sc_主分割);
            this.Name = "PolyphonicReplacePanel";
            this.Size = new System.Drawing.Size(1301, 652);
            this.sc_主分割.Panel1.ResumeLayout(false);
            this.sc_主分割.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc_主分割)).EndInit();
            this.sc_主分割.ResumeLayout(false);
            this.gb_替换工作面板.ResumeLayout(false);
            this.gb_替换工作面板.PerformLayout();
            this.pan_实施工作面板.ResumeLayout(false);
            this.pan_待选词.ResumeLayout(false);
            this.pan_待选词.PerformLayout();
            this.sc_副面板主分割.Panel1.ResumeLayout(false);
            this.sc_副面板主分割.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sc_副面板主分割)).EndInit();
            this.sc_副面板主分割.ResumeLayout(false);
            this.gb_替换方案.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_多音字方案)).EndInit();
            this.gb_最终文本.ResumeLayout(false);
            this.gb_最终文本.PerformLayout();
            this.cms_多音字替换方案.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer sc_主分割;
        private System.Windows.Forms.GroupBox gb_替换工作面板;
        private System.Windows.Forms.Panel pan_实施工作面板;
        private System.Windows.Forms.Label lab_替换工作信息;
        private System.Windows.Forms.Button bt_取消应用替换结果;
        private System.Windows.Forms.Button bt_应用替换结果;
        private System.Windows.Forms.Button bt_下一组替换目标;
        private System.Windows.Forms.Button bt_下一条替换目标;
        private System.Windows.Forms.Button bt_上一条替换目标;
        private System.Windows.Forms.Panel pan_待选词;
        private System.Windows.Forms.RadioButton rb_待选词模板;
        private System.Windows.Forms.Label lab_下文;
        private System.Windows.Forms.Label lab_替换目标;
        private System.Windows.Forms.Label lab_上文;
        private System.Windows.Forms.Label lab_选择多音字方案;
        private System.Windows.Forms.Button bt_开始替换最终文本中的多音字;
        private System.Windows.Forms.ComboBox cb_选择多音字方案;
        private System.Windows.Forms.SplitContainer sc_副面板主分割;
        private System.Windows.Forms.GroupBox gb_替换方案;
        private System.Windows.Forms.DataGridView dgv_多音字方案;
        private System.Windows.Forms.Button bt_还原多音字方案;
        private System.Windows.Forms.Button bt_保存多音字方案;
        private System.Windows.Forms.GroupBox gb_最终文本;
        private System.Windows.Forms.Button bt_清理最终文本;
        private System.Windows.Forms.Button bt_发送最终文本到下一个步骤;
        private System.Windows.Forms.Button bt_复制最终文本;
        private System.Windows.Forms.TextBox tb_最终文本;
        private System.Windows.Forms.ContextMenuStrip cms_多音字替换方案;
        private System.Windows.Forms.ToolStripMenuItem 自动生成ToolStripMenuItem;
    }
}
