using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoDoDuongKinh
{
    public partial class FrmProperty : Form
    {
        public FrmProperty()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "config.ini";
                string fullPath = System.IO.Path.GetFullPath(filePath);
                txtDelayData_1.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayData_1", fullPath);
                txtDelayData_2.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayData_2", fullPath);
                txtDelayBeforeResult1.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult1", fullPath);
                txtDelayBeforeResult2.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult2", fullPath);
                txtGroup1_Y0.Text = clsReadDataINI.ReadValue("Length", "Group1_Y0 ", fullPath);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString(), "Không load được file cấu hình");
            }
        
        }

        private void FrmProperty_Load(object sender, EventArgs e)
        {
            try
            {
                string filePath = "config.ini";
                string fullPath = System.IO.Path.GetFullPath(filePath);
                txtDelayData_1.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayData_1", fullPath);
                txtDelayData_2.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayData_2", fullPath);
                txtDelayBeforeResult1.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult1", fullPath);
                txtDelayBeforeResult2.Text = clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult2", fullPath);
                txtGroup1_Y0.Text = clsReadDataINI.ReadValue("Length", "Group1_Y0 ", fullPath);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString(), "Không load được file cấu hình");
            }
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "config.ini";
                string fullPath = System.IO.Path.GetFullPath(filePath);
                clsReadDataINI.WriteValue("TimerDelay", "DelayData_1", txtDelayData_1.Text, fullPath);

                clsReadDataINI.WriteValue("TimerDelay", "DelayData_2", txtDelayData_2.Text, fullPath);
                clsReadDataINI.WriteValue("TimerDelay", "DelayBeforeResult1", txtDelayBeforeResult1.Text, fullPath);
                clsReadDataINI.WriteValue("TimerDelay", "DelayBeforeResult2", txtDelayBeforeResult2.Text, fullPath);
                clsReadDataINI.WriteValue("Length", "Group1_Y0 ", txtGroup1_Y0.Text, fullPath);


                MessageBox.Show("Đã cập nhập file cấu hình config.ini");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString(),"Không lưu được dữ liệu !!!");
            }
            
        }
 
    }
}
