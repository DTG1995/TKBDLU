using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport;
using System.Web.Script.Serialization;
using SKYPE4COMLib;
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
            loadTerm();
            loadCBBType();
            loadCBBYear();
            loadWeek();
            loadClass();
            loadTKB();
            loadFirst = false;
        }

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
            cbbYear.Properties.ValueMember = "Year";cbbYear.ItemIndex = 1;
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

        private void loadTerm()
        {
            List<TermTKB> arr = new List<TermTKB>();
            arr.Add(new TermTKB { ID = "HK01", Display = "Học Kỳ 1" });
            arr.Add(new TermTKB { ID = "HK02", Display = "Học Kỳ 2" });
            arr.Add(new TermTKB { ID = "HK03", Display = "Học Kỳ 3" });
            cbbTerm.Properties.DataSource = arr;
            cbbTerm.Properties.DisplayMember = "Display";
            cbbTerm.Properties.ValueMember = "ID";
            int month = DateTime.Now.Month;
            if (month <= 6)
                cbbTerm.ItemIndex = 1;
            else if (month >= 9)
                cbbWeek.ItemIndex = 0;
            else
                cbbTerm.ItemIndex = 2;        }
        private void loadWeek()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("http://qlgd.dlu.edu.vn/Public/GetWeek/"+cbbYear.EditValue+"$"+cbbTerm.EditValue);
                JavaScriptSerializer js = new JavaScriptSerializer();
                WeekTKB[] weeks = js.Deserialize<WeekTKB[]>(json);
                cbbWeek.Properties.DataSource = weeks;
                cbbWeek.Properties.DisplayMember = "DisPlayWeek";
                cbbWeek.Properties.ValueMember = "Week";
                string weekCurrent = weeks[0].WeekOfYear2.Split('$')[0];
                cbbWeek.EditValue = (int.Parse(weekCurrent)+1).ToString();
                cbbWeek.Properties.ForceInitialize();
            }
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
            StreamWriter sw;
            string txt = "";
            if (File.Exists("listClassTKB"))
            {
                File.WriteAllText(@"listClassTKB.txt", String.Empty);
            }else{
                sw= File.CreateText(@"listClassTKB.txt");
                sw.Close();
            }
            for (int i = 0; i < listClass.Count; i++)
            {
                txt += listClass[i].ID + ">" + listClass[i].DisplayClass + ">" + (listClass[i].Active ? "1" : "0") +
                       "\n";
            }
            File.WriteAllText(@"listClassTKB.txt", txt);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            frmAddClass frm = new frmAddClass(listClass.Count);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                listClass.Add(new ClassTKB { ID = frm.ClassName, DisplayClass = frm.ClassName, Active = false });
                indexClassActive = listClass.Count - 1;
            }
            frm.Dispose();
            writeFileClass();
            cbbClass.Properties.DataSource = listClass;
        }

        private void cbbClass_EditValueChanged(object sender, EventArgs e)
        {
            if (!loadFirst)
                loadTKB();
        }

        private void loadTKB()
        {
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                var html = webClient.DownloadString("http://qlgd.dlu.edu.vn/public/DrawingClassStudentSchedules_Mau2?YearStudy="+cbbYear.EditValue+"&TermID="+cbbTerm.EditValue+"&Week="+cbbWeek.EditValue+"&ClassStudentID="+cbbClass.EditValue).Replace("30px","80px");
                wbTKB.DocumentText ="<style>th {font-weight: bold;text-align: -internal-center;}</style>"+ html;
            }}

        private bool loadFirst = true;
        private void cbbTerm_EditValueChanged(object sender, EventArgs e)
        {
            if (!loadFirst)
            {
                loadTKB();
                loadWeek();}
        }

        private void cbbWeek_EditValueChanged(object sender, EventArgs e)
        {
            if (!loadFirst)
                loadTKB();
        }

        private void cbbYear_EditValueChanged(object sender, EventArgs e)
        {
            if (!loadFirst)
            {
                loadTKB();
                loadWeek();
            }
        }
        //fb
        private void pictureEdit2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://facebook.com/DTG1995");
        }

        
        //skype
        private void pictureEdit3_Click(object sender, EventArgs e)
        {
            Thread tr = new Thread(CallSkype);
            if(tr.ThreadState == ThreadState.Stopped)
                tr.Start();
            
        }

        private void CallSkype(){
            try
            {
                Skype skype;
                skype = new SKYPE4COMLib.Skype();
                string SkypeID = "tiengioiit";
                Call call = skype.PlaceCall(SkypeID);
            }
            catch
            {
                MessageBox.Show("Không thể mở Skype, hoặc không được cho phép!, nếu thử lại không được\n" +
                                "Hãy tắt ứng dụng, chạy lại ứng dụng và xin phép Skype", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //gmail
        private void pictureEdit4_Click(object sender, EventArgs e)
        {
            frmContact frm = new frmContact();
            frm.ShowDialog();
        }

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listClass.Count; i++)
            {
                if (listClass[i].ID == cbbClass.EditValue)
                    listClass[i].Active = true;
                else listClass[i].Active = false;
            }
            writeFileClass();
            MessageBox.Show("Sét mặc định thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
