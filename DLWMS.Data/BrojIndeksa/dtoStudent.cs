using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLWMS.Data.IB210000
{
    public class dtoStudent
    {
        DLWMSDbContext db=Konekcija.db;
        public Student Student { get; set; }
        public string Indeks => Student.BrojIndeksa;
        public string ImePrezime => Student.Ime + " "+Student.Prezime;
        public DateTime DatumRodjenja => Student.DatumRodjenja;
        public string Prosjek => IzracunajProsjek();

        private string IzracunajProsjek()
        {
            DLWMSDbContext db = Konekcija.db;
            return db.StudentiPredmeti.Include(x => x.Student)
                .Where(x => x.Student.Id == Student.Id).Average(x => x.Ocjena).ToString("0.00");
        }       
    }
}
