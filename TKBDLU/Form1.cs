using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport;

namespace TKBDLU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadCBBType();
            loadCBBYear();
            loadClass();}

        private void loadCBBType()
        {
            List<TypeTKB> arr = new List<TypeTKB>();
            arr.Add(new TypeTKB{Name ="Thời khóa biểu sinh viên", ID = 1});

            cbbType.Properties.DataSource = arr;
            cbbType.Properties.DisplayMember = "Name";
            cbbType.Properties.ValueMember = "ID";
            cbbType.ItemIndex = 0;
        }
        List<ClassTKB> listClass = new List<ClassTKB>(); 
        private void loadCBBYear()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            List<YearTKB> arr = new List<YearTKB>();
            if (month < 9)
            {
                arr.Add(new YearTKB
                {
                    Year = (year - 2).ToString() + "-" + (year - 1).ToString()
                });
                arr.Add(new YearTKB
                {
                    Year =(year - 1).ToString() + "-" + (year).ToString()
                });
                cbbYear.Properties.DataSource = arr;
            }
            else
            {
                arr.Add(new YearTKB
                {
                    Year = (year - 1).ToString() + "-" + (year ).ToString()
                });
                arr.Add(new YearTKB
                {
                    Year = (year).ToString() + "-" + (year+1).ToString()
                });
                cbbYear.Properties.DataSource = arr;
            }
            cbbYear.Properties.DisplayMember = "Year";
            cbbYear.Properties.ValueMember = "Year";
            cbbYear.ItemIndex = 0;
        }

        private int indexClassActive = 1;
        private void loadClass()
        {
            readFileClass();
            if (listClass.Count <= 0)
            {
                frmAddClass frm = new frmAddClass(listClass.Count);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    listClass.Add(new ClassTKB{ID = frm.ClassName, DisplayClass = frm.ClassName, Active = true});
                    indexClassActive++;
                }
                frm.Dispose();
                writeFileClass();
                cbbClass.Properties.DataSource = listClass;
            }
            else
            {
                cbbClass.Properties.DataSource = listClass;
            }
            cbbClass.Properties.DisplayMember = "DisplayClass";
            cbbClass.Properties.ValueMember = "ID";
            cbbClass.ItemIndex = indexClassActive;
        }

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
                        if (arr[2] == "1")
                            indexClassActive = i;
                        listClass.Add(classItem);
                    }
            }
            catch { }
        }
        private void writeFileClass()
        {
            string txt = "";
            using (StreamWriter sw = (File.Exists(@"listClassTKB.txt")) ? File.AppendText(@"listClassTKB.txt") : File.CreateText(@"listClassTKB.txt"))
            {
                for (int i = 0; i < listClass.Count; i++)
                {
                    txt += listClass[i].ID + ">" + listClass[i].DisplayClass + ">" + (listClass[i].Active ? "1" : "0") +
                           "\n";
                }
                sw.Write(txt);
            }
        }
    }
}
