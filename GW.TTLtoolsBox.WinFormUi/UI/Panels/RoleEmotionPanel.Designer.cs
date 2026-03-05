namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    partial class RoleEmotionPanel
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
            this.dgv_角色和情绪指定 = new System.Windows.Forms.DataGridView();
            this.bt_发送所有段落到下一步 = new System.Windows.Forms.Button();
            this.bt_清理文本 = new System.Windows.Forms.Button();
            this.bt_复制文本 = new System.Windows.Forms.Button();
            this.bt_粘贴文本 = new System.Windows.Forms.Button();
            this.bt_上一段 = new System.Windows.Forms.Button();
            this.bt_下一段 = new System.Windows.Forms.Button();
            this.cb_语音段 = new System.Windows.Forms.ComboBox();
            this.bt_发送当前段落到下一步 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色和情绪指定)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_角色和情绪指定
            // 
            this.dgv_角色和情绪指定.AllowUserToAddRows = false;
            this.dgv_角色和情绪指定.AllowUserToDeleteRows = false;
            this.dgv_角色和情绪指定.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_角色和情绪指定.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_角色和情绪指定.Location = new System.Drawing.Point(5, 45);
            this.dgv_角色和情绪指定.Name = "dgv_角色和情绪指定";
            this.dgv_角色和情绪指定.RowTemplate.Height = 23;
            this.dgv_角色和情绪指定.Size = new System.Drawing.Size(1291, 568);
            this.dgv_角色和情绪指定.TabIndex = 0;
            // 
            // bt_发送所有段落到下一步
            // 
            this.bt_发送所有段落到下一步.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_发送所有段落到下一步.Location = new System.Drawing.Point(1132, 619);
            this.bt_发送所有段落到下一步.Name = "bt_发送所有段落到下一步";
            this.bt_发送所有段落到下一步.Size = new System.Drawing.Size(164, 28);
            this.bt_发送所有段落到下一步.TabIndex = 1;
            this.bt_发送所有段落到下一步.Text = "发送所有段落到下一步";
            this.bt_发送所有段落到下一步.UseVisualStyleBackColor = true;
            // 
            // bt_清理文本
            // 
            this.bt_清理文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_清理文本.Location = new System.Drawing.Point(5, 619);
            this.bt_清理文本.Name = "bt_清理文本";
            this.bt_清理文本.Size = new System.Drawing.Size(104, 28);
            this.bt_清理文本.TabIndex = 2;
            this.bt_清理文本.Text = "清理文本";
            this.bt_清理文本.UseVisualStyleBackColor = true;
            // 
            // bt_复制文本
            // 
            this.bt_复制文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_复制文本.Location = new System.Drawing.Point(804, 619);
            this.bt_复制文本.Name = "bt_复制文本";
            this.bt_复制文本.Size = new System.Drawing.Size(104, 28);
            this.bt_复制文本.TabIndex = 3;
            this.bt_复制文本.Text = "复制文本";
            this.bt_复制文本.UseVisualStyleBackColor = true;
            // 
            // bt_粘贴文本
            // 
            this.bt_粘贴文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_粘贴文本.Location = new System.Drawing.Point(694, 619);
            this.bt_粘贴文本.Name = "bt_粘贴文本";
            this.bt_粘贴文本.Size = new System.Drawing.Size(104, 28);
            this.bt_粘贴文本.TabIndex = 4;
            this.bt_粘贴文本.Text = "粘贴文本";
            this.bt_粘贴文本.UseVisualStyleBackColor = true;
            // 
            // bt_上一段
            // 
            this.bt_上一段.Location = new System.Drawing.Point(5, 16);
            this.bt_上一段.Name = "bt_上一段";
            this.bt_上一段.Size = new System.Drawing.Size(40, 23);
            this.bt_上一段.TabIndex = 5;
            this.bt_上一段.Text = "←";
            this.bt_上一段.UseVisualStyleBackColor = true;
            // 
            // bt_下一段
            // 
            this.bt_下一段.Location = new System.Drawing.Point(136, 16);
            this.bt_下一段.Name = "bt_下一段";
            this.bt_下一段.Size = new System.Drawing.Size(40, 23);
            this.bt_下一段.TabIndex = 6;
            this.bt_下一段.Text = "→";
            this.bt_下一段.UseVisualStyleBackColor = true;
            // 
            // cb_语音段
            // 
            this.cb_语音段.FormattingEnabled = true;
            this.cb_语音段.Location = new System.Drawing.Point(51, 18);
            this.cb_语音段.Name = "cb_语音段";
            this.cb_语音段.Size = new System.Drawing.Size(79, 20);
            this.cb_语音段.TabIndex = 7;
            // 
            // bt_发送当前段落到下一步
            // 
            this.bt_发送当前段落到下一步.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_发送当前段落到下一步.Location = new System.Drawing.Point(962, 619);
            this.bt_发送当前段落到下一步.Name = "bt_发送当前段落到下一步";
            this.bt_发送当前段落到下一步.Size = new System.Drawing.Size(164, 28);
            this.bt_发送当前段落到下一步.TabIndex = 1;
            this.bt_发送当前段落到下一步.Text = "发送当前段落到下一步";
            this.bt_发送当前段落到下一步.UseVisualStyleBackColor = true;
            // 
            // RoleEmotionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cb_语音段);
            this.Controls.Add(this.bt_下一段);
            this.Controls.Add(this.bt_上一段);
            this.Controls.Add(this.bt_粘贴文本);
            this.Controls.Add(this.bt_复制文本);
            this.Controls.Add(this.bt_清理文本);
            this.Controls.Add(this.bt_发送当前段落到下一步);
            this.Controls.Add(this.bt_发送所有段落到下一步);
            this.Controls.Add(this.dgv_角色和情绪指定);
            this.Name = "RoleEmotionPanel";
            this.Size = new System.Drawing.Size(1301, 652);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色和情绪指定)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// 角色和情绪指定表格。
        /// </summary>
        private System.Windows.Forms.DataGridView dgv_角色和情绪指定;

        /// <summary>
        /// 发送所有段落到下一步按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_发送所有段落到下一步;

        /// <summary>
        /// 清理文本按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_清理文本;

        /// <summary>
        /// 复制文本按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_复制文本;

        /// <summary>
        /// 粘贴文本按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_粘贴文本;

        /// <summary>
        /// 上一段导航按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_上一段;

        /// <summary>
        /// 下一段导航按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_下一段;

        /// <summary>
        /// 语音段选择下拉框。
        /// </summary>
        private System.Windows.Forms.ComboBox cb_语音段;

        /// <summary>
        /// 发送当前段落到下一步按钮。
        /// </summary>
        private System.Windows.Forms.Button bt_发送当前段落到下一步;
    }
}
