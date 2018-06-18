using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class TaxiController : Controller
    {
        // GET: Taxi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DodajVozaca()
        {
            return View();
        }

        public ActionResult DodavanjeVozaca(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj)
        {
            Pol p = Pol.Muski;
            switch (pol)
            {
                case "muski":
                    p = Pol.Muski;
                    break;
                case "zenski":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            Vozac v = new Vozac(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Vozac);

            Registrovani.Vozaci.Add(v);

            return View("Uspeh",v);
        }
        public ActionResult RegistrujSe(string ime ,string prezime, string pol,string jmbg,string korisnicko,string lozinka,string mail,string broj)
        {
            Pol p = Pol.Muski;
            switch(pol)
            {
                case "muski":
                    p = Pol.Muski;
                    break;
                case "zenski":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            Musterija m = new Musterija(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Musterija);

            Registrovani.Musterije.Add(m);

            Sacuvaj(Registrovani.Musterije);

            return View("Uspeh",m);
            
        }

        public ActionResult Izmeni()
        {
            return View();
        }

        private void Sacuvaj(List<Musterija> musterije)
        {
            string filename = @"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\proba.xml";
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;

                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartElement("Ulogovani");
                foreach (Musterija m in musterije)
                {
                    m.Jmbg.ToString();
                    writer.WriteStartElement("Korisnik");
                    writer.WriteElementString("Ime", m.Ime);
                    writer.WriteElementString("Prezime", m.Prezime);
                    writer.WriteElementString("Jmbg", m.Jmbg.ToString());
                    writer.WriteElementString("KorisnickoIme", m.KorisnickoIme);
                    writer.WriteElementString("Lozinka", m.Lozinka);
                    writer.WriteElementString("Pol", m.Pol.ToString());
                    writer.WriteElementString("E-Mail", m.Mail);
                    writer.WriteElementString("Uloga", m.Uloga.ToString());
                    writer.WriteStartElement("Voznje");
                    int i = 1;
                    foreach(Voznja v in m.Voznja)
                    {
                        writer.WriteStartElement("VoznjaBroj" + i.ToString());
                        writer.WriteElementString("DatumPorudzbine" , v.DatumIVremePorudzbine.ToString());
                        writer.WriteElementString("PocetnaLokacija", v.PocetnaLokacija.ToString());
                        writer.WriteElementString("KrajnjaLokacija", v.Odrediste.ToString());
                        writer.WriteElementString("TipVozila",v.Tip.ToString());
                        writer.WriteElementString("MusterijaIme", v.Musterija.Ime);
                        writer.WriteElementString("MusterijaPrezime", v.Musterija.Prezime);
                        writer.WriteElementString("VozacIme", v.Vozac.Ime);
                        writer.WriteElementString("VozacPrezime", v.Vozac.Prezime);
                        writer.WriteElementString("DispecerIme", v.Dispecer.Ime);
                        writer.WriteElementString("DispececrPrezime", v.Dispecer.Prezime);
                        writer.WriteElementString("Iznos", v.Iznos.ToString());
                        writer.WriteEndElement();
                        i++;
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.Flush();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public ActionResult Registracija()
        {
            return View();
        }

        public ActionResult Uloguj(string korisnickoIme,string lozinka)
        {
            foreach(Musterija m in Registrovani.Musterije)
            {
                if(m.KorisnickoIme == korisnickoIme && m.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", m.KorisnickoIme);
                }
                else if (m.KorisnickoIme == korisnickoIme && m.Lozinka == lozinka)
                {
                    m.Ulogovan = true;
                    return View("musterijaWelcome",m);
                }
            }

            foreach(Vozac v in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == korisnickoIme && v.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", v.KorisnickoIme);

                }
                else if (v.KorisnickoIme == korisnickoIme && v.Lozinka == lozinka)
                {
                    v.Ulogovan = true;
                    return View("vozacWelcome",v);
                }
            }

            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == korisnickoIme && d.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", d.KorisnickoIme);

                }
                else if (d.KorisnickoIme == korisnickoIme && d.Lozinka == lozinka)
                {
                    d.Ulogovan = true;
                    return View("dispecerWelcome", d);
                }
            }


            return View("Greska");
        }
        

        public ActionResult IzmeniPodatkeDispecer(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj)
        {
            Dispecer ret = new Dispecer();

            Pol p = Pol.Muski;
            switch (pol)
            {
                case "muski":
                    p = Pol.Muski;
                    break;
                case "zenski":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if(d.KorisnickoIme == korisnicko)
                {
                    d.Ime = ime;
                    d.Prezime = prezime;
                    d.Pol = p;
                    d.Jmbg = jmb;
                    d.Lozinka = lozinka;
                    d.BrTelefona = broj;
                    d.Mail = mail;
                    ret = d;
                    break;
                }
            }

            return View("dispecerWelcome",ret);
        }

        public ActionResult OdjaviDispecer()
        {
            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if(d.Ulogovan == true)
                {
                    d.Ulogovan = false;
                }
            }
            return View("Index");
        }
    }
}