namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class RoleMappingPanel
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
            this.dgv_角色映射 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色映射)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_角色映射
            // 
            this.dgv_角色映射.AllowUserToAddRows = true;
            this.dgv_角色映射.AllowUserToDeleteRows = true;
            this.dgv_角色映射.AllowUserToResizeRows = false;
            this.dgv_角色映射.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_角色映射.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_角色映射.Location = new System.Drawing.Point(0, 0);
            this.dgv_角色映射.Name = "dgv_角色映射";
            this.dgv_角色映射.RowHeadersWidth = 25;
            this.dgv_角色映射.RowTemplate.Height = 23;
            this.dgv_角色映射.Size = new System.Drawing.Size(600, 400);
            this.dgv_角色映射.TabIndex = 0;
            // 
            // RoleMappingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv_角色映射);
            this.Name = "RoleMappingPanel";
            this.Size = new System.Drawing.Size(600, 400);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色映射)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_角色映射;
    }
}
