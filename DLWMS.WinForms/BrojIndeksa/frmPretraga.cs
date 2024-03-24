using DLWMS.Data;
using DLWMS.Data.IB210000;
using DLWMS.WinForms.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLWMS.WinForms.IB210000
{
    public partial class frmPretraga : Form
    {
        private Student student;
        DLWMSDbContext db = Konekcija.db;
        List<dtoStudent>studenti= new List<dtoStudent>();
        public frmPretraga()
        {
            InitializeComponent();
            UcitajSpolove();
            dgvStudenti.AutoGenerateColumns = false;
        }

        private void UcitajSpolove()
        {
            cmbSpol.DisplayMember = "Naziv";
            cmbSpol.ValueMember = "Id";
            cmbSpol.DataSource = db.Spolovi.ToList();
            cmbSpol.SelectedIndex = -1;
        }

        private void UcitajPodatke()
        {
            var spol=cmbSpol.SelectedItem as Spol;
            var dtmOd = dtpOd.Value;
            var dtmDo=dtpDo.Value;
            var lista = db.Studenti.Include(x => x.Spol).Where(x => x.Spol.Id == spol.Id &&
            (x.DatumRodjenja >= dtmOd && x.DatumRodjenja <= dtmDo)).ToList();
            dgvStudenti.DataSource = null;
            if(lista.Count== 0 )
            {
                MessageBox.Show($"U bazi nema studenata koji su sljedećeg spola: " +
                    $"{spol.Naziv} i da su rođeni u periodu od {dtmOd.ToShortDateString()} " +
                    $"do {dtmDo.ToShortDateString()}");
            }
            else
            {
                studenti.Clear();
                foreach (var std in lista)
                    studenti.Add(new dtoStudent() { Student = std });
                dgvStudenti.DataSource = studenti;
            }               
        }
        private void dtpOd_ValueChanged(object sender, EventArgs e)
        {
            if (validniPodaci())
                UcitajPodatke();
        }

        private void dtpDo_ValueChanged(object sender, EventArgs e)
        {
            if (validniPodaci())
                UcitajPodatke();
        }

       
        private bool validniPodaci()
        {
            return Helpers.Validator.ValidirajKontrolu(cmbSpol, err, Kljucevi.ObaveznaVrijednost);
        }

        private void cmbSpol_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (validniPodaci())
                UcitajPodatke();
        }

        private void dgvStudenti_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==4)
            {
                var std = dgvStudenti.SelectedRows[0].DataBoundItem as dtoStudent;
                var frm = new frmUvjerenja(std.Student);
                frm.ShowDialog();
            }
            else
            {
                var std = dgvStudenti.SelectedRows[0].DataBoundItem as dtoStudent;
                var frm = new frmStudentSlika(std);
                frm.ShowDialog();
            }
        }
    }
}
