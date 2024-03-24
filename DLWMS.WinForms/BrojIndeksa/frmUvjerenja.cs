using DLWMS.Data;
using DLWMS.Data.IB210000;
using DLWMS.WinForms.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLWMS.WinForms.IB210000
{
    public partial class frmUvjerenja : Form
    {
        private Student student;
        DLWMSDbContext db = Konekcija.db;
        List<StudentUvjerenje> uvjerenja = new List<StudentUvjerenje>();
        public frmUvjerenja(Student std)
        {
            InitializeComponent();
            dgvUvjerenja.AutoGenerateColumns = false;
            student = std;
            UcitajUvjerenja();
            UcitajVrste();
        }

        private void UcitajVrste()
        {
            List<string> vrste = new List<string>();
            vrste.Add("Uvjerenje o statusu studenta");
            vrste.Add("Uvjerenje o položenim predmetima");
            vrste.Add("Uvjerenje o prosjeku studenta");
            cmbVrsta.DataSource = vrste;
        }

        private void UcitajUvjerenja()
        {
            uvjerenja.Clear();
            uvjerenja=db.StudentiUvjerenja.Include(x=>x.Student)
                .Where(x=>x.Student.Id==student.Id).ToList();

            dgvUvjerenja.DataSource = null;
            dgvUvjerenja.DataSource=uvjerenja;
            Text = $"Student {student.Ime} {student.Prezime} ima {uvjerenja.Count} uvjerenja";

            btnDodaj.Enabled = uvjerenja.Count()==0 ? false : true;
        }

        private void btnNovoUvjerenje_Click(object sender, EventArgs e)
        {
            var frm = new frmNovoUvjerenje(student);
            frm.ShowDialog();
            UcitajUvjerenja();
            
        }

        private async void btnDodaj_Click(object sender, EventArgs e)
        {
            int brojUvjerenja;
            try
            {
                brojUvjerenja = int.Parse(txtBrojUvjerenja.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Nije unesen broj u textBox broj zahtjeva!!!");
                return;
            }
            txtInfo.ScrollBars = ScrollBars.Vertical;
            var vrsta = cmbVrsta.SelectedItem.ToString();
            var svrha=txtSvrha.Text;
            var slika = db.StudentiUvjerenja.First().Uplatnica;
                 
            var thread=new Thread( ()=>DodajUvjerenja(brojUvjerenja, vrsta,svrha, slika));
            
            thread.Start();
        }
        private void DodajUvjerenja(int brojUvjerenja, string vrsta, string svrha, byte[] uplatnica)
        {            
            for(int i=0;i<brojUvjerenja;i++)
            {
                var novo = new StudentUvjerenje()
                {
                    Student = student,
                    Svrha = svrha,
                    Vrsta = vrsta,
                    Printano = false,
                    Uplatnica = uplatnica,
                    Datum = DateTime.Now
                };
                db.StudentiUvjerenja.Add(novo);
                db.SaveChanges();
                Action ac = () => {
                    txtInfo.Text+= $"Studentu {student.Ime} {student.Prezime} ({student.BrojIndeksa}) " +
                    $"dodato {vrsta} dana {novo.Datum.ToString()}" + Environment.NewLine; 
                    SetCursor();
                };
                BeginInvoke(ac);
                Thread.Sleep(300);
            }
            MessageBox.Show($"Studentu {student.Ime} {student.Prezime} ({student.BrojIndeksa}) " +
                $"uspjesno dodato {brojUvjerenja} uvjerenja");
            BeginInvoke(UcitajUvjerenja);

        }

        private void SetCursor()
        {
            txtInfo.SelectionStart = txtInfo.Text.Length;
            txtInfo.ScrollToCaret();
        }

        private void dgvUvjerenja_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==5)
            {
                var uvj = dgvUvjerenja.SelectedRows[0].DataBoundItem as StudentUvjerenje;

                var rd = MessageBox.Show("Da li želite obrisati odabrano uvjerenje?", "Info", MessageBoxButtons.YesNo);
                if(rd==DialogResult.Yes)
                {
                    db.StudentiUvjerenja.Remove(uvj);
                    db.SaveChanges();
                    UcitajUvjerenja();
                }
            }
            else if(e.ColumnIndex==6)
            {
                var uvj = dgvUvjerenja.SelectedRows[0].DataBoundItem as StudentUvjerenje;
                var frm = new frmIzvjestaj(uvj);
                frm.ShowDialog();

                uvj.Printano = true;

                db.Entry(uvj).State = EntityState.Modified;
                db.SaveChanges();

                UcitajUvjerenja();
            }
        }
    }
}
