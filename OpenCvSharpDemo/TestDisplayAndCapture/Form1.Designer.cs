using ImageDisplay;

namespace TestDisplayAndCapture
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
            this.components = new System.ComponentModel.Container();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.imageDisplay = new ImageDisplay.ImageDisplayWindow();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCloseCamera = new System.Windows.Forms.Button();
            this.btnOpenCamera = new System.Windows.Forms.Button();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxSnapModel = new System.Windows.Forms.GroupBox();
            this.rtnCon = new System.Windows.Forms.RadioButton();
            this.rtnSingle = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.timeGrab = new System.Windows.Forms.Timer(this.components);
            this.groupBoxSnap = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxSnapModel.SuspendLayout();
            this.groupBoxSnap.SuspendLayout();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(12, 12);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(442, 430);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.imageDisplay;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCloseCamera);
            this.groupBox1.Controls.Add(this.btnOpenCamera);
            this.groupBox1.Controls.Add(this.txtIpAddress);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(461, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 75);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机设置";
            // 
            // btnCloseCamera
            // 
            this.btnCloseCamera.Location = new System.Drawing.Point(116, 45);
            this.btnCloseCamera.Name = "btnCloseCamera";
            this.btnCloseCamera.Size = new System.Drawing.Size(75, 23);
            this.btnCloseCamera.TabIndex = 2;
            this.btnCloseCamera.Text = "关闭相机";
            this.btnCloseCamera.UseVisualStyleBackColor = true;
            this.btnCloseCamera.Click += new System.EventHandler(this.btnCloseCamera_Click);
            // 
            // btnOpenCamera
            // 
            this.btnOpenCamera.Location = new System.Drawing.Point(9, 45);
            this.btnOpenCamera.Name = "btnOpenCamera";
            this.btnOpenCamera.Size = new System.Drawing.Size(75, 23);
            this.btnOpenCamera.TabIndex = 2;
            this.btnOpenCamera.Text = "打开相机";
            this.btnOpenCamera.UseVisualStyleBackColor = true;
            this.btnOpenCamera.Click += new System.EventHandler(this.btnOpenCamera_Click);
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(66, 18);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(125, 21);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "172.17.123.100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址：";
            // 
            // groupBoxSnapModel
            // 
            this.groupBoxSnapModel.Controls.Add(this.rtnCon);
            this.groupBoxSnapModel.Controls.Add(this.rtnSingle);
            this.groupBoxSnapModel.Location = new System.Drawing.Point(460, 94);
            this.groupBoxSnapModel.Name = "groupBoxSnapModel";
            this.groupBoxSnapModel.Size = new System.Drawing.Size(200, 50);
            this.groupBoxSnapModel.TabIndex = 2;
            this.groupBoxSnapModel.TabStop = false;
            this.groupBoxSnapModel.Text = "采集模式";
            // 
            // rtnCon
            // 
            this.rtnCon.AutoSize = true;
            this.rtnCon.Location = new System.Drawing.Point(117, 20);
            this.rtnCon.Name = "rtnCon";
            this.rtnCon.Size = new System.Drawing.Size(47, 16);
            this.rtnCon.TabIndex = 0;
            this.rtnCon.Tag = "1";
            this.rtnCon.Text = "连续";
            this.rtnCon.UseVisualStyleBackColor = true;
            this.rtnCon.CheckedChanged += new System.EventHandler(this.rtnSingle_CheckedChanged);
            // 
            // rtnSingle
            // 
            this.rtnSingle.AutoSize = true;
            this.rtnSingle.Checked = true;
            this.rtnSingle.Location = new System.Drawing.Point(10, 21);
            this.rtnSingle.Name = "rtnSingle";
            this.rtnSingle.Size = new System.Drawing.Size(47, 16);
            this.rtnSingle.TabIndex = 0;
            this.rtnSingle.TabStop = true;
            this.rtnSingle.Tag = "0";
            this.rtnSingle.Text = "单帧";
            this.rtnSingle.UseVisualStyleBackColor = true;
            this.rtnSingle.CheckedChanged += new System.EventHandler(this.rtnSingle_CheckedChanged);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(6, 20);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(191, 68);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "开始采集";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStop.Location = new System.Drawing.Point(6, 94);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(191, 68);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "停止采集";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // timeGrab
            // 
            this.timeGrab.Interval = 10;
            this.timeGrab.Tick += new System.EventHandler(this.timeGrab_Tick);
            // 
            // groupBoxSnap
            // 
            this.groupBoxSnap.Controls.Add(this.btnStart);
            this.groupBoxSnap.Controls.Add(this.btnStop);
            this.groupBoxSnap.Location = new System.Drawing.Point(461, 150);
            this.groupBoxSnap.Name = "groupBoxSnap";
            this.groupBoxSnap.Size = new System.Drawing.Size(200, 170);
            this.groupBoxSnap.TabIndex = 4;
            this.groupBoxSnap.TabStop = false;
            this.groupBoxSnap.Text = "触发";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 454);
            this.Controls.Add(this.groupBoxSnap);
            this.Controls.Add(this.groupBoxSnapModel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.elementHost1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxSnapModel.ResumeLayout(false);
            this.groupBoxSnapModel.PerformLayout();
            this.groupBoxSnap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private ImageDisplay.ImageDisplayWindow imageDisplay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenCamera;
        private System.Windows.Forms.Button btnCloseCamera;
        private System.Windows.Forms.GroupBox groupBoxSnapModel;
        private System.Windows.Forms.RadioButton rtnCon;
        private System.Windows.Forms.RadioButton rtnSingle;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer timeGrab;
        private System.Windows.Forms.GroupBox groupBoxSnap;
    }
}

