namespace WindowsFormsApp1
{
    partial class main_Form
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
            this.user_listView = new System.Windows.Forms.ListView();
            this.test_button = new System.Windows.Forms.Button();
            this.add_button1 = new System.Windows.Forms.Button();
            this.delete_button = new System.Windows.Forms.Button();
            this.pid_listView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // user_listView
            // 
            this.user_listView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.user_listView.CheckBoxes = true;
            this.user_listView.GridLines = true;
            this.user_listView.Location = new System.Drawing.Point(13, 30);
            this.user_listView.Name = "user_listView";
            this.user_listView.Size = new System.Drawing.Size(646, 119);
            this.user_listView.TabIndex = 0;
            this.user_listView.UseCompatibleStateImageBehavior = false;
            // 
            // test_button
            // 
            this.test_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.test_button.Location = new System.Drawing.Point(689, 42);
            this.test_button.Name = "test_button";
            this.test_button.Size = new System.Drawing.Size(86, 28);
            this.test_button.TabIndex = 1;
            this.test_button.Text = "测试";
            this.test_button.UseVisualStyleBackColor = true;
            this.test_button.Click += new System.EventHandler(this.test_button_Click);
            // 
            // add_button1
            // 
            this.add_button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.add_button1.Location = new System.Drawing.Point(689, 93);
            this.add_button1.Name = "add_button1";
            this.add_button1.Size = new System.Drawing.Size(86, 28);
            this.add_button1.TabIndex = 2;
            this.add_button1.Text = "添加";
            this.add_button1.UseVisualStyleBackColor = true;
            this.add_button1.Click += new System.EventHandler(this.add_button1_Click);
            // 
            // delete_button
            // 
            this.delete_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.delete_button.Location = new System.Drawing.Point(689, 144);
            this.delete_button.Name = "delete_button";
            this.delete_button.Size = new System.Drawing.Size(86, 27);
            this.delete_button.TabIndex = 3;
            this.delete_button.Text = "删除";
            this.delete_button.UseVisualStyleBackColor = true;
            this.delete_button.Click += new System.EventHandler(this.delete_button_Click);
            // 
            // pid_listView
            // 
            this.pid_listView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pid_listView.Location = new System.Drawing.Point(13, 201);
            this.pid_listView.Name = "pid_listView";
            this.pid_listView.Size = new System.Drawing.Size(645, 101);
            this.pid_listView.TabIndex = 4;
            this.pid_listView.UseCompatibleStateImageBehavior = false;
            // 
            // main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 378);
            this.Controls.Add(this.pid_listView);
            this.Controls.Add(this.delete_button);
            this.Controls.Add(this.add_button1);
            this.Controls.Add(this.test_button);
            this.Controls.Add(this.user_listView);
            this.Name = "main_Form";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.main_Form_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView user_listView;
        private System.Windows.Forms.Button test_button;
        private System.Windows.Forms.Button add_button1;
        private System.Windows.Forms.Button delete_button;
        private System.Windows.Forms.ListView pid_listView;
    }
}

