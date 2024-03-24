using DLWMS.Data.IB210000;
using Microsoft.Reporting.WinForms;
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
    public partial class frmIzvjestaj : Form
    {
        private StudentUvjerenje uvj;
        public frmIzvjestaj(StudentUvjerenje stduvj)
        {
            InitializeComponent();
            uvj = stduvj;
        }

        private void frmIzvjestajIB210000_Load(object sender, EventArgs e)
        {
            var rpc = new ReportParameterCollection();
            rpc.Add(new ReportParameter("rpImePrezime", uvj.Student.Ime +" "+uvj.Student.Prezime));
            rpc.Add(new ReportParameter("rpIndeks", uvj.Student.BrojIndeksa));
            rpc.Add(new ReportParameter("rpDatum", uvj.Datum.ToShortDateString()));
            rpc.Add(new ReportParameter("rpVrsta", uvj.Vrsta));
            rpc.Add(new ReportParameter("rpSvrha", uvj.Svrha));
            rpc.Add(new ReportParameter("rpDatumPrintanja", DateTime.Now.ToShortDateString()));

            reportViewer1.LocalReport.SetParameters(rpc);

            reportViewer1.RefreshReport();
        }
    }
}
