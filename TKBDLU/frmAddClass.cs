using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            if (checkClassExist())
            {
                MessageBox.Show("Lớp đã có!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClass.Focus();
            }
            else
            {
                ClassName = txtClass.Text;
                countClass++;
                this.DialogResult = DialogResult.OK;
                this.Close();    
            }
        }

        private void frmAddClass_Load(object sender, EventArgs e)
        {
            txtClass.Focus();
            readFileClass();
        }

        private void frmAddClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (countClass == 0)
            {
               if(MessageBox.Show(
                    "Hiện không có lớp để xem, bạn muốn thoát không?\nChọn Yes để thoát?\nChọn No để thử lại",
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
                e.Cancel = true;
            }
        }
        private List<ClassTKB> listClass = new List<ClassTKB>(); 
        private void readFileClass()
        {
            try
            {
                string[] lines = File.ReadAllLines(@"listClassTKB.txt");
                if (lines != null)
                    for (int i = 0; i < lines.Count(); i++)
                    {
                        string[] arr = lines[i].Split('>');
                        ClassTKB classItem = new ClassTKB
                        {
                            ID = arr[0],
                            DisplayClass = arr[1],
                            Active = (arr.Count() == 2 ? false : (arr[2] == "1" ? true : false))
                        };
                        listClass.Add(classItem);
                    }
            }
            catch { }
        }

        private bool checkClassExist()
        {
            for (int i = 0; i < listClass.Count; i++)
            {
                if (listClass[i].ID == txtClass.Text)
                    return true;
            }
            return false;
        }
    }
}
