﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Padding = new Padding(borderSize);
            this.BackColor = Color.FromArgb(35, 26, 166);
        }
        private int borderSize = 1;
        private Size formSize;
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized: //Maximized form (After)
                    this.Padding = new Padding(0, 0, 0, 0);
                    break;
                case FormWindowState.Normal: //Restored form (After)
                    if (this.Padding.Top != borderSize)
                        this.Padding = new Padding(borderSize);
                    break;
            }
        }
        private void Xoa()
        {
            txt_so1.Text = "";
            txt_so2.Text = "";
            lbl_KQ.Text = "0";
            txt_so1.Focus();
        }
        private void Btn_KQ_Click(object sender, EventArgs e)
        {
            /*if (txt_so1.Text =="" || txt_so2.Text =="")
            {
                MessageBox.Show("Bạn phải nhập đủ 2 số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }*/
            if (txt_so1.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 1", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }
            else if (txt_so2.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 2", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so2.Focus();
            }
            else
            {
                try
                {
                    if(lbl_KQ.Text =="0")
                    {
                        int so1 = int.Parse(txt_so1.Text);
                        int so2 = int.Parse(txt_so2.Text);
                        lbl_KQ.Text = (so1 + so2).ToString();
                    }
                    else
                    {
                        guna2CirclePictureBox1_Click(sender,e);
                    }
                }
                catch (Exception ex)
                {
                   MessageBox.Show("Đã có lỗi xảy ra","Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020;
            const int SC_RESTORE = 0xF120;
            const int WM_NCHITTEST = 0x0084;
            const int resizeAreaSize = 10;
            #region Form Resize
            const int HTCLIENT = 1;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)
                {
                    if ((int)m.Result == HTCLIENT)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= resizeAreaSize)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTBOTTOM;
                            else
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }
            if (m.Msg == WM_SYSCOMMAND)
            {
                int wParam = (m.WParam.ToInt32() & 0xFFF0);
                if (wParam == SC_MINIMIZE)
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            formSize = this.ClientSize;
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            AdjustForm();
        }


        private void picbox_minimize_Click(object sender, EventArgs e)
        {
            formSize = this.ClientSize;
            this.WindowState = FormWindowState.Minimized;
        }

        private void picbox_maximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                formSize = this.ClientSize;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = formSize;
            }
        }

        private void picbox_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel_title_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            Xoa();
        }

        private void txt_so1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txt_so2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_tru_Click(object sender, EventArgs e)
        {
            /*if (txt_so1.Text == "" || txt_so2.Text == "")
            {
                MessageBox.Show("Bạn phải nhập đủ 2 số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }*/
            if (txt_so1.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 1", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }
            else if (txt_so2.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 2", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so2.Focus();
            }
            else
            {
                try
                {
                    if (lbl_KQ.Text == "0")
                    {
                        int so1 = int.Parse(txt_so1.Text);
                        int so2 = int.Parse(txt_so2.Text);
                        lbl_KQ.Text = (so1 - so2).ToString();
                    }
                    else
                    {
                        guna2CirclePictureBox1_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã có lỗi xảy ra", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_nhan_Click(object sender, EventArgs e)
        {
            /*if (txt_so1.Text == "" || txt_so2.Text == "")
            {
                MessageBox.Show("Bạn phải nhập đủ 2 số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }*/
            if (txt_so1.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 1", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }
            else if (txt_so2.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 2", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so2.Focus();
            }
            else
            {
                try
                {
                    if (lbl_KQ.Text == "0")
                    {
                        int so1 = int.Parse(txt_so1.Text);
                        int so2 = int.Parse(txt_so2.Text);
                        lbl_KQ.Text = (so1 * so2).ToString();
                    }
                    else
                    {
                        guna2CirclePictureBox1_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã có lỗi xảy ra", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_chia_Click(object sender, EventArgs e)
        {
            /*if (txt_so1.Text == "" || txt_so2.Text == "")
            {
                MessageBox.Show("Bạn phải nhập đủ 2 số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }*/
            if (txt_so1.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 1", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so1.Focus();
            }
            else if (txt_so2.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số 2", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_so2.Focus();
            }
            else
            {
                try
                {
                    if (lbl_KQ.Text == "0" && txt_so2.Text != "0")
                    {
                        int so1 = int.Parse(txt_so1.Text);
                        int so2 = int.Parse(txt_so2.Text);
                        lbl_KQ.Text = (so1 / so2).ToString();
                    }
                    else
                    {
                        DialogResult dt = MessageBox.Show("Số thứ 2 phải khác 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (dt == DialogResult.OK)
                        {
                            guna2CirclePictureBox1_Click(sender, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã có lỗi xảy ra", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
