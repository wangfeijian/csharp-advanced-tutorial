namespace PerformClickDemo
{
    partial class FormMain
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
            this.btnShowHello = new System.Windows.Forms.Button();
            this.btnShowError = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowHello
            // 
            this.btnShowHello.Location = new System.Drawing.Point(23, 24);
            this.btnShowHello.Name = "btnShowHello";
            this.btnShowHello.Size = new System.Drawing.Size(75, 23);
            this.btnShowHello.TabIndex = 0;
            this.btnShowHello.Text = "ShowHello";
            this.btnShowHello.UseVisualStyleBackColor = true;
            this.btnShowHello.Click += new System.EventHandler(this.btnShowHello_Click);
            // 
            // btnShowError
            // 
            this.btnShowError.Location = new System.Drawing.Point(170, 24);
            this.btnShowError.Name = "btnShowError";
            this.btnShowError.Size = new System.Drawing.Size(75, 23);
            this.btnShowError.TabIndex = 1;
            this.btnShowError.Text = "ShowError";
            this.btnShowError.UseVisualStyleBackColor = true;
            this.btnShowError.Click += new System.EventHandler(this.btnShowError_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 84);
            this.Controls.Add(this.btnShowError);
            this.Controls.Add(this.btnShowHello);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowHello;
        private System.Windows.Forms.Button btnShowError;
    }
}

