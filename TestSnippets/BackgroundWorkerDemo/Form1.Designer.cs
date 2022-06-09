namespace BackgroundWorkerDemo
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._startButton = new System.Windows.Forms.Button();
            this._stopButton = new System.Windows.Forms.Button();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(12, 45);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(776, 85);
            this._progressBar.TabIndex = 0;
            // 
            // _startButton
            // 
            this._startButton.Font = new System.Drawing.Font("华文仿宋", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._startButton.Location = new System.Drawing.Point(12, 136);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(361, 74);
            this._startButton.TabIndex = 1;
            this._startButton.Text = "开始";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // _stopButton
            // 
            this._stopButton.Enabled = false;
            this._stopButton.Font = new System.Drawing.Font("华文仿宋", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._stopButton.Location = new System.Drawing.Point(427, 136);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(361, 74);
            this._stopButton.TabIndex = 1;
            this._stopButton.Text = "停止";
            this._stopButton.UseVisualStyleBackColor = true;
            this._stopButton.Click += new System.EventHandler(this._stopButton_Click);
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Font = new System.Drawing.Font("楷体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._label.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this._label.Location = new System.Drawing.Point(13, 13);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(200, 29);
            this._label.TabIndex = 2;
            this._label.Text = "当前进度：0%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 217);
            this.Controls.Add(this._label);
            this.Controls.Add(this._stopButton);
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "测试BackgroundWorker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.Button _stopButton;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
        private System.Windows.Forms.Label _label;
    }
}

