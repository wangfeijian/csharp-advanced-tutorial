using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageCapture;

namespace TestDisplayAndCapture
{
    public partial class Form1 : Form
    {
        private CaptureBase _capture;
        private bool _isSingle = true;
        private bool _isGrab;
        private WriteableBitmap _tempWriteableBitmap;
        public Form1()
        {
            InitializeComponent();
            imageDisplay.CameraColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(135, 206, 235));
            btnStop.Enabled = false;
            groupBoxSnapModel.Enabled = false;
            groupBoxSnap.Enabled = false;
        }

        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            try
            {
                _capture = new CaptureBasler(txtIpAddress.Text, false);
                groupBoxSnapModel.Enabled = true;
                groupBoxSnap.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnCloseCamera_Click(object sender, EventArgs e)
        {
            if (_capture == null)
            {
                MessageBox.Show("请先打开相机");
                return;
            }

            _capture.Close();
            groupBoxSnapModel.Enabled = false;
            groupBoxSnap.Enabled = false;
        }

        private void rtnSingle_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            _isSingle = Convert.ToInt16(btn.Tag) == 0;
            btnStop.Enabled = !_isSingle;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!_isSingle)
            {
                _capture.StopGrab();
                _isGrab = false;
                btnStart.Enabled = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_isSingle)
            {
                _capture.Snap();
                _tempWriteableBitmap = CaptureImageConverter.BitmapToWriteableBitmap(_capture.GetImage(true));
                imageDisplay.ShowImageBitmap = _tempWriteableBitmap;
            }
            else
            {
                _isGrab = true;
                btnStart.Enabled = false;
                timeGrab.Enabled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _capture.Close();
        }

        private void timeGrab_Tick(object sender, EventArgs e)
        {
            if (!_isGrab)
            {
                timeGrab.Enabled = false;
            }

            _capture.Grab();
            _tempWriteableBitmap = CaptureImageConverter.BitmapToWriteableBitmap(_capture.GetImage(true));
            imageDisplay.ShowImageBitmap = _tempWriteableBitmap;
        }
    }
}
