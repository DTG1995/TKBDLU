using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKBDLU
{
    public partial class frmAddClass : Form
    {
        private int countClass = 0;
        public frmAddClass(int countclass)
        {
            this.countClass = countclass;
            InitializeComponent();
        }

        public string ClassName { get; set; }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ClassName = txtClass.Text;countClass++;this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmAddClass_Load(object sender, EventArgs e)
        {
            txtClass.Focus();
        }

        private void frmAddClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (countClass == 0)
            {
                MessageBox.Show(
                    "Hiện không có lớp để xem, bạn muốn thoát không?\nChọn Yes để thoát?\nChọn No để thử lại",
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                e.Cancel = true;
            }
        }
    }
}
