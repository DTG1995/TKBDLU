using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace TKBDLU
{
    public partial class frmContact : DevExpress.XtraEditors.XtraForm
    {
        public frmContact()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bool error = false;
            try
            {
                var testEmail = new MailAddress(txtEmail.Text);
            }
            catch
            {
                txtEmail.BackColor = Color.OrangeRed;
                error = true;
            }
            if (txtpwd.Text == "")
            {
                txtpwd.BackColor = Color.OrangeRed;
                error = true;
            }
            if (txtSub.Text == "")
            {
                txtSub.BackColor = Color.OrangeRed;
                error = true;
            }
            if (txtContent.Text == "")
            {
                txtContent.BackColor = Color.OrangeRed;
                error = true;
            }
            if (error)
            {
                MessageBox.Show("Lỗi! một số ô bạn nhập chưa đúng hoặc không có dữ liệu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;}
            var fromAddress = new MailAddress("fromemail@gmail.com","DTG TKBDLU");//email gửi
            var toAddress = new MailAddress("tiengioiit@gmail.com");
            string fromPassword = "password";//mk email gửi
            string subject = txtSub.Text;
            string body = "From: " + txtEmail.Text + "\nTên: " + txtpwd.Text + "\nNội dung: " + txtContent.Text;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                    MessageBox.Show("Cảm ơn bạn đã liên hệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch(SmtpException ex)
                {
                    MessageBox.Show("Có lỗi xảy ra trong quá trình gửi mail!\nXin bạn thử lại!\n" +
                                    "Lỗi: "+ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {
            txtEmail.BackColor = Color.White;
        }

        private void txtpwd_EditValueChanged(object sender, EventArgs e)
        {
            txtpwd.BackColor = Color.White;
        }

        private void txtSub_EditValueChanged(object sender, EventArgs e)
        {
            txtSub.BackColor = Color.White;
        }

        private void txtContent_EditValueChanged(object sender, EventArgs e)
        {
            txtContent.BackColor = Color.White;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();}
    }
}
