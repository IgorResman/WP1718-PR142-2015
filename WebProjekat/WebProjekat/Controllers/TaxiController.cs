using System;
using System.Collections.Generic;
using System.IO;
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

            Korisnik k = Session["korisnik"] as Korisnik;

            if (k == null)
            {
                k = new Korisnik();
                Session["korisnik"] = k;
                return View();
            }
            else
            {
                foreach (Musterija m in Registrovani.Musterije)
                {
                    if (m.KorisnickoIme == k.KorisnickoIme && m.Lozinka != k.Lozinka)
                    {
                        return View("LozinkaGreska");
                    }
                    else if (m.KorisnickoIme == k.KorisnickoIme && m.Lozinka == k.Lozinka)
                    {
                        m.Ulogovan = true;
                        
                        return View("musterijaWelcome", m);
                    }
                }

                foreach (Vozac v in Registrovani.Vozaci)
                {
                    if (v.KorisnickoIme == k.KorisnickoIme && v.Lozinka != k.Lozinka)
                    {
                        return View("LozinkaGreska");

                    }
                    else if (v.KorisnickoIme == k.KorisnickoIme && v.Lozinka == k.Lozinka)
                    {
                        v.Ulogovan = true;
                        
                        return View("vozacWelcome", v);
                    }
                }

                foreach (Dispecer d in Registrovani.Dispeceri)
                {
                    if (d.KorisnickoIme == k.KorisnickoIme && d.Lozinka != k.Lozinka)
                    {
                        return View("LozinkaGreska");

                    }
                    else if (d.KorisnickoIme == k.KorisnickoIme && d.Lozinka == k.Lozinka)
                    {
                        d.Ulogovan = true;
                        
                        return View("dispecerWelcome", d);
                    }
                }


                return View("Greska");
            }
        }

        public ActionResult DodeljivanjeVozaca(string date, string musterija, string disp)
        {
            Dispecer di = new Dispecer();
            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == disp)
                {
                    di = d;
                }
            }
            Voznja voznja = new Voznja();
            foreach (Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.ToString() == date && v.Musterija.KorisnickoIme == musterija)
                {
                    v.Dispecer = di;
                    voznja = v;
                }
            }

            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                if (d == di)
                {
                    d.Voznja.Add(voznja);
                    foreach (Korisnik k in Registrovani.SviZajedno)
                    {
                        if (d.KorisnickoIme == k.KorisnickoIme)
                        {
                            k.Voznja = d.Voznja;
                            break;
                        }
                    }
                }
            }

            return View(voznja);
        }


        #region Dodavanja
        public ActionResult DodajVozaca()
        {
            return View();
        }

        public ActionResult Dodeli(string vozac, string datum, string musterija)
        {
            Vozac ret = new Vozac();
            foreach (Vozac v in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == vozac)
                {
                    foreach (Voznja voznja in Voznje.SveVoznje)
                    {
                        if (voznja.DatumIVremePorudzbine.ToString() == datum && musterija == voznja.Musterija.KorisnickoIme)
                        {
                            v.Zauzet = true;
                            voznja.Vozac = v;
                            v.Voznja.Add(voznja);
                            ret = v;
                            break;
                        }
                    }
                }
            }

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == ret.KorisnickoIme)
                {
                    k.Voznja = ret.Voznja;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("UspesnoDodatVozac", ret);
        }

        public ActionResult DodavanjeVozaca(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj, string godiste, string reg, string taxiBroj, string tip)
        {
            int god = int.Parse(godiste);
            int br = int.Parse(taxiBroj);
            TipVozila ti = TipVozila.Kombi;

            switch (tip)
            {
                case "kombi":
                    ti = TipVozila.Kombi;
                    break;
                case "putnicki":
                    ti = TipVozila.Putnicko;
                    break;
            }
            Pol p = Pol.Muski;
            switch (pol)
            {
                case "musko":
                    p = Pol.Muski;
                    break;
                case "zensko":
                    p = Pol.Zenski;
                    break;
            }
            long jmb = long.Parse(jmbg);

            Vozac v = new Vozac(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Vozac);

            Automobil a = new Automobil(v, god, reg, br, ti);

            v.Automobil = a;
            foreach (Vozac voz in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == voz.KorisnickoIme)
                {
                    return View("VozacPostoji");
                }
            }

            Registrovani.Vozaci.Add(v);

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == v.KorisnickoIme)
                {
                    return View("KorisnickoPostoji");
                }
            }

            Registrovani.SviZajedno.Add(v);

            Sacuvaj(Registrovani.SviZajedno);

            Dispecer d = Session["korisnik"] as Dispecer;

            return View("Uspeh", d);
        }

        #endregion

        [HttpPost]
        public ActionResult RegistrujSe(string ime ,string prezime, string pol,string jmbg,string korisnicko,string lozinka,string mail,string broj)
        {
            Pol p = Pol.Muski;
            switch(pol)
            {
                case "musko":
                    p = Pol.Muski;
                    break;
                case "zensko":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            Korisnik m = new Musterija(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Musterija);


            foreach(Korisnik k in Registrovani.Musterije)
            {
                if(k.KorisnickoIme == m.KorisnickoIme)
                {
                    return View("KorisnickoPostoji");
                }
            }
            Registrovani.SviZajedno.Add(m);
            Registrovani.Musterije.Add(m as Musterija);

            Sacuvaj(Registrovani.SviZajedno);

            return View("Index");
            
        }

        public ActionResult VratiProfilDisp()
        {
            Dispecer d = Session["korisnik"] as Dispecer;

            return View("dispecerWelcome", d);
        }

        public ActionResult VozacPrihvata(string musterija, string datum, string vozac)
        {
            Voznja ret = new Voznja();

            foreach (Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.ToString() == datum && v.Musterija.KorisnickoIme == musterija)
                {
                    v.Status = StatusVoznje.Prihvacena;
                    ret = v;
                    break;
                }
            }

            Vozac voza = new Vozac();
            foreach (Vozac voz in Registrovani.Vozaci)
            {
                if (voz.KorisnickoIme == vozac)
                {
                    voz.Voznja.Add(ret);
                    voza = voz;
                    break;
                }
            }


            ret.Vozac = voza;
            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == voza.KorisnickoIme)
                {
                    k.Voznja = voza.Voznja;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("vozacWelcome", voza);
        }

        #region Izmene
        public ActionResult Izmeni()
        {
            Dispecer d = Session["korisnik"] as Dispecer;
            return View(d);
        }

        public ActionResult IzmeniVozac()
        {
            Vozac v = Session["korisnik"] as Vozac;
            return View(v);
        }

        public ActionResult IzmeniMusterija()
        {
            Musterija m = Session["korisnik"] as Musterija;

            return View(m);
        }
        #endregion

        #region Registracija i pocetne strane
        public ActionResult dispecerWelcome()
        {
            return View();
        }


        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Uloguj(string korisnickoIme, string lozinka)
        {
            foreach (Musterija m in Registrovani.Musterije)
            {
                if (m.KorisnickoIme == korisnickoIme && m.Lozinka != lozinka)
                {
                    return View("LozinkaGreska");
                }
                else if (m.KorisnickoIme == korisnickoIme && m.Lozinka == lozinka)
                {
                    m.Ulogovan = true;
                    Session["korisnik"] = m;
                    return View("musterijaWelcome", m);
                }
            }

            foreach (Vozac v in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == korisnickoIme && v.Lozinka != lozinka)
                {
                    return View("LozinkaGreska");

                }
                else if (v.KorisnickoIme == korisnickoIme && v.Lozinka == lozinka)
                {
                    v.Ulogovan = true;
                    Session["korisnik"] = v;
                    return View("vozacWelcome", v);
                }
            }

            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == korisnickoIme && d.Lozinka != lozinka)
                {
                    return View("LozinkaGreska");

                }
                else if (d.KorisnickoIme == korisnickoIme && d.Lozinka == lozinka)
                {
                    d.Ulogovan = true;
                    Session["korisnik"] = d;
                    return View("dispecerWelcome", d);
                }
            }
            return View("Greska");
        }

        #endregion



        #region Cuvanje u xml
        private void Sacuvaj(List<Korisnik> musterije)
        {
            SacuvajLjude(musterije);
            SacuvajVoznje(Voznje.SveVoznje);
        }

        private void SacuvajVoznje(List<Voznja> sveVoznje)
        {
            string filename = @"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\voznje.xml";
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;

                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartElement("Voznje");

                foreach (Voznja v in sveVoznje)
                {
                    
                    if (v.Odrediste == null)
                    {
                        Adresa a = new Adresa("Nepoznata", 0, "Nepoznato", 0);
                        Lokacija l = new Lokacija(0, 0, a);
                        v.Odrediste = l;
                    }
                    if (v.Vozac == null)
                    {
                        Vozac voz = new Vozac("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Vozac);
                        v.Vozac = voz;
                    }
                    if (v.Dispecer == null)
                    {
                        Dispecer d = new Dispecer("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Dispecer);
                        v.Dispecer = d;
                    }

                    if (v.Komentar == null)
                    {
                        Komentar k = new Komentar("bez opisa", DateTime.MinValue, v.Vozac, v, Ocena.neocenjen);
                        v.Komentar = k;
                    }
                    if(v.Musterija == null)
                    {
                        Musterija m = new Musterija("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Musterija);
                        v.Musterija = m;
                    }

                    string odrediste = v.Odrediste.Adresa.Mesto + "_" + v.Odrediste.Adresa.Broj.ToString() + "," + v.Odrediste.Adresa.Mesto + "_" + v.Odrediste.Adresa.BrojMesta.ToString();
                    string pocetna = v.PocetnaLokacija.Adresa.Naziv + "_" + v.PocetnaLokacija.Adresa.Broj.ToString() + "," + v.PocetnaLokacija.Adresa.Mesto + "_" + v.PocetnaLokacija.Adresa.BrojMesta.ToString();

                    writer.WriteStartElement("Voznja");
                    writer.WriteElementString("DatumPorudzbine", v.DatumIVremePorudzbine.ToString());
                    writer.WriteElementString("PocetnaLokacija", pocetna);
                    writer.WriteElementString("KrajnjaLokacija", odrediste);
                    writer.WriteElementString("TipVozila", v.Tip.ToString());
                    writer.WriteElementString("MusterijaIme", v.Musterija.Ime);
                    writer.WriteElementString("MusterijaPrezime", v.Musterija.Prezime);
                    writer.WriteElementString("VozacIme", v.Vozac.Ime);
                    writer.WriteElementString("VozacPrezime", v.Vozac.Prezime);
                    writer.WriteElementString("DispecerIme", v.Dispecer.Ime);
                    writer.WriteElementString("DispececrPrezime", v.Dispecer.Prezime);
                    writer.WriteElementString("Status", v.Status.ToString());
                    writer.WriteElementString("KomentarOpis", v.Komentar.Opis);
                    writer.WriteElementString("KomentarDatum", v.Komentar.Datum.ToString());
                    writer.WriteElementString("KomentarOcena", v.Komentar.Ocena.ToString());
                    writer.WriteElementString("Iznos", v.Iznos.ToString());
                    writer.WriteEndElement();
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        private void SacuvajLjude(List<Korisnik> korisnici)
        {
            string filename = @"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\korisnici.xml";
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;

                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartElement("Ulogovani");
                foreach (Korisnik m in korisnici)
                {
                    if (m.Uloga == Uloga.Vozac)
                    {
                        foreach (Vozac vo in Registrovani.Vozaci)
                        {
                            if (vo.KorisnickoIme == m.KorisnickoIme)
                            {
                                writer.WriteStartElement("Korisnik");
                                writer.WriteElementString("Ime", vo.Ime);
                                writer.WriteElementString("Prezime", vo.Prezime);
                                writer.WriteElementString("Jmbg", vo.Jmbg.ToString());
                                writer.WriteElementString("KorisnickoIme", vo.KorisnickoIme);
                                writer.WriteElementString("Lozinka", vo.Lozinka);
                                writer.WriteElementString("Pol", vo.Pol.ToString());
                                writer.WriteElementString("E-Mail", vo.Mail);
                                writer.WriteElementString("BrojTelefona", vo.BrTelefona);
                                writer.WriteElementString("Uloga", vo.Uloga.ToString());
                                writer.WriteStartElement("Automobil");
                                writer.WriteElementString("Godiste", vo.Automobil.Godiste.ToString());
                                writer.WriteElementString("Registracija", vo.Automobil.Registracija.ToString());
                                writer.WriteElementString("TaxiBroj", vo.Automobil.BrojVozila.ToString());
                                writer.WriteElementString("TipVozila", vo.Automobil.Tip.ToString());
                                writer.WriteEndElement();
                                writer.WriteStartElement("Adresa");
                                writer.WriteElementString("NazivUlice", vo.Lokacija.Adresa.Naziv);
                                writer.WriteElementString("BrojUlice", vo.Lokacija.Adresa.Broj.ToString());
                                writer.WriteElementString("Grad", vo.Lokacija.Adresa.Mesto);
                                writer.WriteElementString("PostanskiBroj", vo.Lokacija.Adresa.BrojMesta.ToString());
                                writer.WriteElementString("X", vo.Lokacija.X.ToString());
                                writer.WriteElementString("Y", vo.Lokacija.Y.ToString());
                                writer.WriteEndElement();
                                writer.WriteEndElement();
                            }
                        }
                    }
                    else
                    {
                        writer.WriteStartElement("Korisnik");
                        writer.WriteElementString("Ime", m.Ime);
                        writer.WriteElementString("Prezime", m.Prezime);
                        writer.WriteElementString("Jmbg", m.Jmbg.ToString());
                        writer.WriteElementString("KorisnickoIme", m.KorisnickoIme);
                        writer.WriteElementString("Lozinka", m.Lozinka);
                        writer.WriteElementString("Pol", m.Pol.ToString());
                        writer.WriteElementString("E-Mail", m.Mail);
                        writer.WriteElementString("BrojTelefona", m.BrTelefona);
                        writer.WriteElementString("Uloga", m.Uloga.ToString());
                        writer.WriteEndElement();
                    }
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

        }
        #endregion



        #region Izmene podataka

        public ActionResult IzmeniPodatkeDispecer(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj)
        {
            Dispecer ret = new Dispecer();

            Pol p = Pol.Muski;
            switch (pol)
            {
                case "Muski":
                    p = Pol.Muski;
                    break;
                case "Zenski":
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

            Korisnik korisnik = new Korisnik();

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == ret.KorisnickoIme)
                {
                    korisnik = ret;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);
            return View("dispecerWelcome",ret);
        }

        public ActionResult IzmeniPodatkeVozac(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj)
        {
            Vozac ret = new Vozac();
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

            foreach (Vozac d in Registrovani.Vozaci)
            {
                if (d.KorisnickoIme == korisnicko)
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
            Korisnik korisnik = new Korisnik();

            foreach(Korisnik k in Registrovani.SviZajedno)
            {
                if(k.KorisnickoIme == ret.KorisnickoIme)
                {
                    korisnik = ret;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("vozacWelcome", ret);
        }

        public ActionResult IzmeniPodatkeMusterija(string ime, string prezime, string pol, string jmbg, string korisnicko, string lozinka, string mail, string broj)
        {
            Musterija ret = new Musterija();
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

            foreach (Musterija d in Registrovani.Musterije)
            {
                if (d.KorisnickoIme == korisnicko)
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
            Korisnik korisnik = new Korisnik();

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == ret.KorisnickoIme)
                {
                    korisnik = ret;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("musterijaWelcome",ret);
        }

        #endregion

        #region Odjave

        public ActionResult OdjaviDispecer()
        {
            Session.Abandon();
            Dispecer d = new Dispecer();
            Session["korisnik"] = d;
            return View("Index");
        }

        public ActionResult OdjaviVozac()
        {
            Session.Abandon();
            Vozac d = new Vozac();
            Session["korisnik"] = d;
            return View("Index");
        }

        public ActionResult OdjaviMusterija()
        {
            Session.Abandon();
            Musterija d = new Musterija();
            Session["korisnik"] = d;
            return View("Index");
        }
        #endregion

        #region Vozac menja lokacije

        public ActionResult PromeniLokaciju()
        {
            Vozac v = Session["korisnik"] as Vozac;
            return View(v);
        }

        public ActionResult PromeniLokVozac(string x,string y,string ulica,string broj,string grad,string pozivni)
        {
            int xx = int.Parse(x);
            int yy = int.Parse(y);
            int br = int.Parse(broj);
            int poz = int.Parse(pozivni);

            Adresa a = new Adresa(ulica, br, grad, poz);

            Lokacija l = new Lokacija(xx, yy, a);

            Vozac v = Session["korisnik"] as Vozac;

            v.Lokacija = l;

            foreach(Korisnik kor in Registrovani.SviZajedno)
            {
                if(kor.KorisnickoIme == v.KorisnickoIme)
                {
                    kor.KorisnickoIme = v.KorisnickoIme;
                    kor.Lozinka = v.Lozinka;
                    kor.Ime = v.Ime;
                    kor.Prezime = v.Prezime;
                    kor.Mail = v.Mail;
                    kor.BrTelefona = v.BrTelefona;
                    kor.Jmbg = v.Jmbg;
                    kor.Pol = v.Pol;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("vozacWelcome",v);
        }
        #endregion

        public ActionResult ZatraziVoznjuMusterija()
        {
            return View();
        }

        

        public ActionResult VratiProfilMusterija()
        {
            Musterija m = Session["korisnik"] as Musterija;

            return View("musterijaWelcome", m);
        }

        #region Postavljanje komentara(vracanje na stranicu Musterija)
        public ActionResult VratiProfilMusterijaKomentar(string komentar,string datum)
        {
            Musterija m = Session["korisnik"] as Musterija;

            string[] dat = datum.Split(' ', '-', ':');

            int day = int.Parse(dat[0]);
            int month = 0;
            switch (dat[1])
            {
                case "Jan":
                    month = 1;
                    break;
                case "Feb":
                    month = 2;
                    break;
                case "Mar":
                    month = 3;
                    break;
                case "Apr":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "Jun":
                    month = 6;
                    break;
                case "Jul":
                    month = 7;
                    break;
                case "Aug":
                    month = 8;
                    break;
                case "Sep":
                    month = 9;
                    break;
                case "Oct":
                    month = 10;
                    break;
                case "Nov":
                    month = 11;
                    break;
                case "Dec":
                    month = 12;
                    break;
            }

            int year = int.Parse(dat[2]);
            year = year + 2000;
            int hour = int.Parse(dat[3]);
            int minute = int.Parse(dat[4]);
            int second = int.Parse(dat[5]);

            DateTime d = new DateTime(year, month, day, hour, minute, second);
            Korisnik kor = m;
            

            foreach(Voznja v in m.Voznja)
            {
                if (v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year)
                {
                    foreach (Voznja voz in Voznje.SveVoznje)
                    {
                        if (voz == v)
                        {
                            Komentar k = new Komentar(komentar, DateTime.Now, kor, v, Ocena.neocenjen);
                            v.Komentar = k;
                            break;
                        }
                    }
                    Komentar kom = new Komentar(komentar, DateTime.Now, kor, v, Ocena.neocenjen);
                    v.Komentar = kom;
                }
            }
            return View("musterijaWelcome", m);
        }
        #endregion


        #region izmene voznje musterije
        public ActionResult IzmeniVoznjuMusterija(string korisnik,string datum)
        {
            string[] dat = datum.Split(' ', '-', ':');

            int day = int.Parse(dat[0]);
            int month = 0;
            switch (dat[1])
            {
                case "Jan":
                    month = 1;
                    break;
                case "Feb":
                    month = 2;
                    break;
                case "Mar":
                    month = 3;
                    break;
                case "Apr":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "Jun":
                    month = 6;
                    break;
                case "Jul":
                    month = 7;
                    break;
                case "Aug":
                    month = 8;
                    break;
                case "Sep":
                    month = 9;
                    break;
                case "Oct":
                    month = 10;
                    break;
                case "Nov":
                    month = 11;
                    break;
                case "Dec":
                    month = 12;
                    break;
            }

            int year = int.Parse(dat[2]);
            year = year + 2000;
            int hour = int.Parse(dat[3]);
            int minute = int.Parse(dat[4]);
            int second = int.Parse(dat[5]);

            DateTime d = new DateTime(year, month, day, hour, minute, second);

            Voznja vo = new Voznja();

            foreach (Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year && v.DatumIVremePorudzbine.Hour == d.Hour && v.DatumIVremePorudzbine.Minute == d.Minute && v.DatumIVremePorudzbine.Second == d.Second && v.Musterija.KorisnickoIme == korisnik)
                {
                    vo = v;
                    break;
                }
            }
            return View("IzmenaVoznjeMusterija",vo);
        }

        public ActionResult IzmenilaMusterija(string korisnicko,string datum,string ulica,string broj,string mesto,string postanski,string vozilo)
        {
            string[] dat = datum.Split(' ', '-', ':');

            int day = int.Parse(dat[0]);
            int month = 0;
            switch (dat[1])
            {
                case "Jan":
                    month = 1;
                    break;
                case "Feb":
                    month = 2;
                    break;
                case "Mar":
                    month = 3;
                    break;
                case "Apr":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "Jun":
                    month = 6;
                    break;
                case "Jul":
                    month = 7;
                    break;
                case "Aug":
                    month = 8;
                    break;
                case "Sep":
                    month = 9;
                    break;
                case "Oct":
                    month = 10;
                    break;
                case "Nov":
                    month = 11;
                    break;
                case "Dec":
                    month = 12;
                    break;
            }

            int year = int.Parse(dat[2]);
            year = year + 2000;
            int hour = int.Parse(dat[3]);
            int minute = int.Parse(dat[4]);
            int second = int.Parse(dat[5]);

            DateTime d = new DateTime(year, month, day, hour, minute, second);
            int br = int.Parse(broj);
            int post = int.Parse(postanski);
            Voznja v = new Voznja();
            Adresa a = new Adresa(ulica, br, mesto, post);
            Lokacija l = new Lokacija(1, 1, a);
            v.DatumIVremePorudzbine = d;
            v.PocetnaLokacija = l;

            switch (vozilo)
            {
                case "putnicko":
                    v.Tip = TipVozila.Putnicko;
                    break;
                case "kombi":
                    v.Tip = TipVozila.Kombi;
                    break;
            }

            foreach(Voznja vo in Voznje.SveVoznje)
            {
                if(vo.DatumIVremePorudzbine.Day == d.Day && vo.DatumIVremePorudzbine.Month == d.Month && vo.DatumIVremePorudzbine.Year == d.Year && vo.DatumIVremePorudzbine.Hour == d.Hour && vo.DatumIVremePorudzbine.Minute == d.Minute && vo.DatumIVremePorudzbine.Second == d.Second && vo.Musterija.KorisnickoIme == korisnicko)
                {
                    vo.PocetnaLokacija = v.PocetnaLokacija;
                    vo.Tip = v.Tip;
                    v = vo;
                    break;
                }
            }

            foreach(Voznja vo in v.Musterija.Voznja)
            {
                if(vo.DatumIVremePorudzbine.Day == d.Day && vo.DatumIVremePorudzbine.Month == d.Month && vo.DatumIVremePorudzbine.Year == d.Year && vo.DatumIVremePorudzbine.Hour == d.Hour && vo.DatumIVremePorudzbine.Minute == d.Minute && vo.DatumIVremePorudzbine.Second == d.Second)
                {
                    vo.PocetnaLokacija = v.PocetnaLokacija;
                    vo.Tip = v.Tip;
                    v = vo;
                }
            }

            return View("musterijaWelcome",v.Musterija);
        }
        #endregion

        public ActionResult OtazujeMusterija(string korisnik, string datum)
        {
            string[] dat = datum.Split(' ', '-', ':');

            int day = int.Parse(dat[0]);
            int month = 0;
            switch(dat[1])
            {
                case "Jan":
                    month = 1;
                    break;
                case "Feb":
                    month = 2;
                    break;
                case "Mar":
                    month = 3;
                    break;
                case "Apr":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "Jun":
                    month = 6;
                    break;
                case "Jul":
                    month = 7;
                    break;
                case "Aug":
                    month = 8;
                    break;
                case "Sep":
                    month = 9;
                    break;
                case "Oct":
                    month = 10;
                    break;
                case "Nov":
                    month = 11;
                    break;
                case "Dec":
                    month = 12;
                    break;
            }

            int year = int.Parse(dat[2]);
            year = year + 2000;
            int hour = int.Parse(dat[3]);
            int minute = int.Parse(dat[4]);
            int second = int.Parse(dat[5]);

            DateTime d = new DateTime(year, month, day, hour, minute, second);

            Voznja vo = new Voznja();

            foreach(Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year && v.DatumIVremePorudzbine.Hour == d.Hour && v.DatumIVremePorudzbine.Minute == d.Minute && v.DatumIVremePorudzbine.Second == d.Second && v.Musterija.KorisnickoIme == korisnik)
                {
                    v.Status = StatusVoznje.Otkazana;
                    vo = v;
                }
            }
            Musterija musterija = new Musterija();
            foreach(Musterija m in Registrovani.Musterije)
            {
                if (m.KorisnickoIme == korisnik)
                {
                    foreach(Voznja v in m.Voznja)
                    {
                        if(v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year && v.DatumIVremePorudzbine.Hour == d.Hour && v.DatumIVremePorudzbine.Minute == d.Minute && v.DatumIVremePorudzbine.Second == d.Second)
                        {
                            v.Status = StatusVoznje.Otkazana;
                            musterija = m;
                            break;
                        }
                    }

                }
            }
            Sacuvaj(Registrovani.SviZajedno);
            return View("UspesnoOtkazanaVoznja",vo);
        }


        #region Zakazivanje voznja
        public ActionResult DispecerZakazujeVoznju()
        {
            Dispecer d = Session["korisnik"] as Dispecer;
            return View(d);
        }

        public ActionResult ZatrazilaMusterija(string ulica, string broj, string mesto, string postanski, string vozilo)
        {
            int br = int.Parse(broj);
            int post = int.Parse(postanski);

            Musterija m = Session["korisnik"] as Musterija;

            Voznja v = new Voznja();
            Adresa a = new Adresa(ulica, br, mesto, post);
            Lokacija l = new Lokacija(1, 1, a);
            v.DatumIVremePorudzbine = DateTime.Now;
            v.PocetnaLokacija = l;

            switch (vozilo)
            {
                case "putnicko":
                    v.Tip = TipVozila.Putnicko;
                    break;
                case "kombi":
                    v.Tip = TipVozila.Kombi;
                    break;
            }

            v.Musterija = m;
            v.Status = StatusVoznje.Kreirana;
            m.Voznja.Add(v);

            Voznje.SveVoznje.Add(v);

            foreach (Musterija mu in Registrovani.Musterije)
            {
                if (mu.KorisnickoIme == m.KorisnickoIme)
                {
                    mu.Voznja = m.Voznja;
                    break;
                }
            }

            foreach(Korisnik k in Registrovani.SviZajedno)
            {
                if(k.KorisnickoIme == m.KorisnickoIme)
                {
                    k.Voznja = m.Voznja;
                }
            }
            Sacuvaj(Registrovani.SviZajedno);
            return View("UspesnoZatrazenaVoznja", v);
        }

        public ActionResult ZatrazioDispecer(string ulica, string broj, string mesto, string postanski, string vozilo, string dispecer, string korisnickoVozac)
        {
            int br = int.Parse(broj);
            int post = int.Parse(postanski);

            Voznja v = new Voznja();
            Adresa a = new Adresa(ulica, br, mesto, post);
            Lokacija l = new Lokacija(1, 1, a);
            v.DatumIVremePorudzbine = DateTime.Now;
            v.PocetnaLokacija = l;
            v.Status = StatusVoznje.Formirana;
            switch (vozilo)
            {
                case "putnicko":
                    v.Tip = TipVozila.Putnicko;
                    break;
                case "kombi":
                    v.Tip = TipVozila.Kombi;
                    break;
            }

            Vozac vozac = new Vozac();



            foreach (Vozac vo in Registrovani.Vozaci)
            {
                if (vo.KorisnickoIme == korisnickoVozac)
                {
                    vozac = vo;
                }
            }

            if (v.Tip != vozac.Automobil.Tip)
            {
                return View("GreskaAuto");
            }
            else
            {
                vozac.Zauzet = true;
            }

            v.Vozac = vozac;
            v.Status = StatusVoznje.Kreirana;
            Dispecer disp = new Dispecer();
            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == dispecer)
                {
                    disp = d;
                }
            }
            v.Dispecer = disp;

            disp.Voznja.Add(v);

            foreach (Vozac voza in Registrovani.Vozaci)
            {
                if (voza.KorisnickoIme == korisnickoVozac)
                {
                    voza.Voznja.Add(v);
                    vozac = voza;
                    break;
                }
            }

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == korisnickoVozac)
                {
                    k.Voznja = vozac.Voznja;
                }
            }

            Session["korisnik"] = disp;

            Voznje.SveVoznje.Add(v);

            Sacuvaj(Registrovani.SviZajedno);

            return View("UspesnoZatrazenaVoznja", v);
        }
        #endregion



        public ActionResult OstaviKomentar(string komentar,string ocena,string korisnicko,string datum)
        {
            Musterija must = new Musterija();
            Komentar k = new Komentar();
            k.Datum = DateTime.Now;
           switch(ocena)
            {
                case "Neocenjen":
                    k.Ocena = Ocena.neocenjen;
                    break;
                case "Veoma loša":
                    k.Ocena = Ocena.veomaLose;
                    break;
                case "Loša":
                    k.Ocena = Ocena.lose;
                    break;
                case "Dobra":
                    k.Ocena = Ocena.dobro;
                    break;
                case "Veoma Dobra":
                    k.Ocena = Ocena.veomaDobro;
                    break;
                case "Odlična":
                    k.Ocena = Ocena.odlicno;
                    break;
            }

            k.Opis = komentar.Trim();

           foreach(Voznja v in Voznje.SveVoznje)
            {
                if(v.DatumIVremePorudzbine.ToString() == datum && v.Musterija.KorisnickoIme == korisnicko)
                {
                    k.Voznja = v;
                    k.Korisnik = v.Musterija;
                    k.Voznja.Komentar = k;
                }
            }

           foreach(Musterija m in Registrovani.Musterije)
            {
                if(m.KorisnickoIme == korisnicko)
                {
                    k.Korisnik = m;
                    must = m;
                    
                }
            }


            foreach (Korisnik kor in Registrovani.SviZajedno)
            {
                if (kor.KorisnickoIme == korisnicko)
                {
                    kor.Voznja = k.Korisnik.Voznja;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("musterijaWelcome",must);
        }





        #region Izmene statusa voznji od strane vozaca(gotova voznja i sve ostalo) + dodavanje komentara vozaca

        public ActionResult UspesnaVoznjaVozac(string ulica, string broj, string grad, string posta, string di, string iznos, string datum, string musterija, string vozac)
        {
            Adresa a = new Adresa(ulica, int.Parse(broj), grad, int.Parse(posta));
            Lokacija l = new Lokacija(1, 1, a);
            Voznja voznja = new Voznja();
            Voznja dispecervoznja = new Voznja();
            Voznja musterijaVoznja = new Voznja();
            foreach (Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.ToString() == datum && v.Vozac.KorisnickoIme == vozac)
                {
                    v.Odrediste = l;
                    v.Iznos = int.Parse(iznos);
                    dispecervoznja = v;
                    musterijaVoznja = v;
                    voznja = v;

                    break;
                }
            }

            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                foreach (Voznja vo in d.Voznja)
                {
                    if (vo.DatumIVremePorudzbine == voznja.DatumIVremePorudzbine && vo.Dispecer.KorisnickoIme == vo.Dispecer.KorisnickoIme)
                    {
                        vo.Odrediste = dispecervoznja.Odrediste;
                        vo.Iznos = dispecervoznja.Iznos;
                    }
                }
            }


            Dispecer disp = new Dispecer();

            disp = voznja.Dispecer;

            Vozac vozaac = new Vozac();
            foreach (Vozac voza in Registrovani.Vozaci)
            {
                foreach (Voznja voz in voza.Voznja)
                {
                    if (voznja.DatumIVremePorudzbine == voz.DatumIVremePorudzbine && voznja.Vozac.KorisnickoIme == voz.Vozac.KorisnickoIme)
                    {
                        voz.Odrediste = voznja.Odrediste;
                        voz.Iznos = voznja.Iznos;
                        vozaac = voza;
                        break;
                    }
                }
            }

            Musterija must = new Musterija();
            foreach (Musterija m in Registrovani.Musterije)
            {
                foreach (Voznja voz in m.Voznja)
                {
                    if (voznja.DatumIVremePorudzbine == voz.DatumIVremePorudzbine && voznja.Vozac.KorisnickoIme == voz.Vozac.KorisnickoIme)
                    {
                        voz.Odrediste = voznja.Odrediste;
                        voz.Iznos = voznja.Iznos;
                        must = m;
                        break;
                    }
                }
            }

            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == must.KorisnickoIme)
                {
                    k.Voznja = must.Voznja;
                }
                else if (k.KorisnickoIme == vozaac.KorisnickoIme)
                {
                    k.Voznja = vozaac.Voznja;
                }
                else if (k.KorisnickoIme == disp.KorisnickoIme)
                {
                    k.Voznja = disp.Voznja;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("vozacWelcome", vozaac);
        }

        public ActionResult VozacMenjaStatus(string datum,string musterija)
        {
            Voznja ret = new Voznja();

            foreach(Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.ToString() == datum && v.Musterija.KorisnickoIme == musterija)
                {
                    ret = v;
                }
            }
            return View(ret);
        }

        public ActionResult IzmenaStatusaVozac(string datum,string musterija,string status)
        {
            Vozac ret = new Vozac();
            StatusVoznje s = StatusVoznje.Formirana;

            switch(status)
            {
                case "Formirana":
                    s = StatusVoznje.Formirana;
                    break;
                case "Kreirana":
                    s = StatusVoznje.Kreirana;
                    break;
                case "Neuspesna":
                    s = StatusVoznje.Neuspesna;
                    break;
                case "Obradjena":
                    s = StatusVoznje.Obradjena;
                    break;
                case "Otkazana":
                    s = StatusVoznje.Otkazana;
                    break;
                case "Prihvacena":
                    s = StatusVoznje.Prihvacena;
                    break;
                case "Uspesna":
                    s = StatusVoznje.Uspesna;
                    break;
            }

            Korisnik k = new Korisnik();
            Voznja voz = new Voznja();
            foreach (Voznja v in Voznje.SveVoznje)
            {
                if (v.DatumIVremePorudzbine.ToString() == datum && v.Musterija.KorisnickoIme == musterija)
                {
                    v.Status = s;

                    foreach(Vozac vo in Registrovani.Vozaci)
                    {
                        if(vo.KorisnickoIme == v.Vozac.KorisnickoIme)
                        {
                            foreach(Voznja voznja in vo.Voznja)
                            {
                                if(v.DatumIVremePorudzbine == voznja.DatumIVremePorudzbine && v.Musterija.KorisnickoIme == voznja.Musterija.KorisnickoIme)
                                {
                                    voznja.Status = s;
                                    k = vo;
                                    ret = vo;
                                    voz = voznja;
                                }
                            }
                        }
                    }
                }
            }

            foreach(Korisnik kor in Registrovani.SviZajedno)
            {
                if(kor.KorisnickoIme == k.KorisnickoIme)
                {
                    kor.Voznja = k.Voznja;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            if(s == StatusVoznje.Neuspesna)
            {
                return View("vozacOstavljaKomentar",voz);
            }
            else if(s == StatusVoznje.Uspesna)
            {
                return View("vozacUspesnaVoznja", voz);
            }

            return View("vozacWelcome",ret);
        }

        public ActionResult VozacOstavljaKomentar(string komentar, string datum, string korisnicko)
        {

            Vozac v = new Vozac();
            Dispecer d = new Dispecer();

            foreach (Vozac vozac in Registrovani.Vozaci)
            {
                if (vozac.KorisnickoIme == korisnicko)
                {
                    v = vozac;
                }
            }

            Voznja voznja = new Voznja();
            foreach (Voznja vo in Voznje.SveVoznje)
            {
                if (vo.DatumIVremePorudzbine.ToString() == datum && vo.Vozac.KorisnickoIme == v.KorisnickoIme)
                {
                    vo.Komentar.Datum = DateTime.Now;
                    vo.Komentar.Korisnik = v;
                    vo.Komentar.Ocena = Ocena.neocenjen;
                    vo.Komentar.Opis = komentar.Trim();
                    vo.Komentar.Voznja = vo;
                    voznja = vo;
                }
            }

            foreach (Voznja voz in v.Voznja)
            {
                if (voz.DatumIVremePorudzbine.ToString() == datum && voz.Vozac.KorisnickoIme == v.KorisnickoIme)
                {
                    voz.Komentar = voznja.Komentar;
                }
            }



            foreach (Korisnik k in Registrovani.SviZajedno)
            {
                if (k.KorisnickoIme == v.KorisnickoIme)
                {
                    foreach (Musterija m in Registrovani.Musterije)
                    {
                        if (voznja.Musterija.KorisnickoIme == m.KorisnickoIme)
                        {
                            foreach (Voznja voznj in m.Voznja)
                            {
                                if (voznj.DatumIVremePorudzbine == voznja.DatumIVremePorudzbine)
                                {
                                    voznj.Komentar = voznja.Komentar;
                                }
                            }
                            k.Voznja = v.Voznja;
                            break;
                        }
                    }
                }
            }

            d = voznja.Dispecer;

            foreach (Voznja vozn in d.Voznja)
            {
                if (vozn.DatumIVremePorudzbine == voznja.DatumIVremePorudzbine)
                {
                    vozn.Status = StatusVoznje.Neuspesna;
                    vozn.Komentar = voznja.Komentar;
                }
            }

            foreach (Dispecer disp in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == disp.KorisnickoIme)
                {
                    disp.Voznja = d.Voznja;
                    break;
                }
            }

            foreach (Korisnik kor in Registrovani.SviZajedno)
            {
                if (kor.KorisnickoIme == d.KorisnickoIme)
                {
                    kor.Voznja = d.Voznja;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);

            return View("vozacWelcome", v);
        }
        #endregion

        public ActionResult OstavljanjeKomentara(string korisnik,string datum)
        {
            Voznja v = new Voznja();

            foreach (Voznja voznja in Voznje.SveVoznje)
            {
                if (voznja.DatumIVremePorudzbine.ToString() == datum && voznja.Musterija.KorisnickoIme == korisnik)
                {
                    v = voznja;
                    break;
                }
            }
            return View("OstavljanjeKomentara", v);
        }




        public ActionResult UspesnoOtkazanaVoznja(string korisnik,string datum)
        {
            Voznja v = new Voznja();

            foreach(Voznja voznja in Voznje.SveVoznje)
            {
                if(voznja.DatumIVremePorudzbine.ToString() == datum && voznja.Musterija.KorisnickoIme == korisnik)
                {
                    v = voznja;
                    break;
                }
            }
            return View("UspesnoOtkazanaVoznja",v);
        }

        public ActionResult PrikaziKomentar(string datum,string korisnik,string vozac,string dispecer)
        {


            Voznja ret = new Voznja();
            foreach(Korisnik k in Registrovani.SviZajedno)
            {
                foreach(Voznja v in k.Voznja)
                {
                    if(v.DatumIVremePorudzbine.ToString() == datum && v.Musterija.KorisnickoIme == korisnik && vozac==v.Vozac.KorisnickoIme && dispecer == v.Dispecer.KorisnickoIme)
                    {
                        ret = v;
                        break;
                    }
                }
            }
            return View("PrikazKomentara",ret);
        }

        #region Filtriranje sortiranje i ostalo

        public ActionResult FiltriraMusterija(string filterStatus,string musterija)
        {
            Korisnik ret = new Korisnik();
            foreach(Korisnik must in Registrovani.SviZajedno)
            {
                if(must.KorisnickoIme == musterija)
                {
                    ret = must;
                }
            }
            ret.Filter = true;

            StatusVoznje status = StatusVoznje.Formirana;

            switch(filterStatus)
            {
                case "Formirana":
                    status = StatusVoznje.Formirana;
                    break;
                case "Kreirana":
                    status = StatusVoznje.Kreirana;
                    break;
                case "Neuspešna":
                    status = StatusVoznje.Neuspesna;
                    break;
                case "Uspešna":
                    status = StatusVoznje.Uspesna;
                    break;
                case "Prihvaćena":
                    status = StatusVoznje.Prihvacena;
                    break;
                case "Otkazana":
                    status = StatusVoznje.Otkazana;
                    break;
                case "Obrađena":
                    status = StatusVoznje.Obradjena;
                    break;
            }

            foreach(Voznja voz in ret.Voznja)
            {
                if(voz.Status == status)
                {
                    ret.Filtrirane.Add(voz);
                }
            }
            Dispecer d = new Dispecer();
            Musterija m = new Musterija();
            Vozac v = new Vozac();
            if(ret.Uloga == Uloga.Musterija)
            {
                foreach(Musterija mu in Registrovani.Musterije)
                {
                    if(mu.KorisnickoIme == ret.KorisnickoIme)
                    {
                        mu.Filtrirane = ret.Filtrirane;
                        mu.Sortirane = ret.Sortirane;
                        mu.Pretrazene = ret.Pretrazene;
                        m = mu;
                        return View("musterijaWelcome", m);
                    }
                }
            }
            else if(ret.Uloga == Uloga.Dispecer)
            {
                foreach(Dispecer di in Registrovani.Dispeceri)
                {
                    if(di.KorisnickoIme == ret.KorisnickoIme)
                    {
                        di.Filtrirane = ret.Filtrirane;
                        di.Sortirane = ret.Sortirane;
                        di.Pretrazene = ret.Pretrazene;
                        d = di;
                        return View("dispecerWelcome", d);
                    }
                }
            }
            else if(ret.Uloga == Uloga.Vozac)
            {
                foreach(Vozac vo in Registrovani.Vozaci)
                {
                    if(vo.KorisnickoIme == ret.KorisnickoIme)
                    {
                        vo.Filtrirane = ret.Filtrirane;
                        vo.Sortirane = ret.Sortirane;
                        vo.Pretrazene = ret.Pretrazene;
                        v = vo;
                        return View("vozacWelcome", vo);
                    }
                }
            }

            return View("musterijaWelcome",ret);
        }

        public ActionResult SoriraMusterija(string datum,string ocena,string musterija)
        {
            Korisnik ret = new Korisnik();
            foreach (Korisnik must in Registrovani.SviZajedno)
            {
                if (must.KorisnickoIme == musterija)
                {
                    ret = must;
                }
            }

            ret.Sortiranje = true;

            if(ret.Filter)
            {
                ret.Sortirane = ret.Filtrirane;
            }
            else if(ret.Pretrazivanje)
            {
                ret.Sortirane = ret.Pretrazene;
            }
            else
            {
                foreach (Voznja voz in ret.Voznja)
                {
                    ret.Sortirane.Add(voz);
                }
            }

            bool dat = false;
            bool oc = false;

            if(datum != null)
            {
                dat = true;
            }

            if(ocena != null)
            {
                oc = true;
            }

            if(dat)
            {
                ret.Sortirane = ret.Voznja.OrderBy(o => o.DatumIVremePorudzbine).ToList();
            }
            else if(oc)
            {
                ret.Sortirane = ret.Voznja.OrderBy(o => o.Komentar.Ocena).ToList();
            }

            Dispecer d = new Dispecer();
            Musterija m = new Musterija();
            Vozac v = new Vozac();
            if (ret.Uloga == Uloga.Musterija)
            {
                foreach (Musterija mu in Registrovani.Musterije)
                {
                    if (mu.KorisnickoIme == ret.KorisnickoIme)
                    {
                        mu.Filtrirane = ret.Filtrirane;
                        mu.Sortirane = ret.Sortirane;
                        mu.Pretrazene = ret.Pretrazene;
                        m = mu;
                        return View("musterijaWelcome", m);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Dispecer)
            {
                foreach (Dispecer di in Registrovani.Dispeceri)
                {
                    if (di.KorisnickoIme == ret.KorisnickoIme)
                    {
                        di.Filtrirane = ret.Filtrirane;
                        di.Sortirane = ret.Sortirane;
                        di.Pretrazene = ret.Pretrazene;
                        d = di;
                        return View("dispecerWelcome", d);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Vozac)
            {
                foreach (Vozac vo in Registrovani.Vozaci)
                {
                    if (vo.KorisnickoIme == ret.KorisnickoIme)
                    {
                        vo.Filtrirane = ret.Filtrirane;
                        vo.Sortirane = ret.Sortirane;
                        vo.Pretrazene = ret.Pretrazene;
                        v = vo;
                        return View("vozacWelcome", vo);
                    }
                }
            }

            return View("musterijaWelcome",ret);
        }

        public ActionResult PonistiFilter(string musterija)
        {
            Korisnik ret = new Korisnik();
            foreach (Korisnik must in Registrovani.SviZajedno)
            {
                if (must.KorisnickoIme == musterija)
                {
                    ret = must;
                }
            }

            ret.Filter = false;
            ret.Sortiranje = false;
            ret.Pretrazivanje = false;

            ret.Filtrirane = new List<Voznja>();
            ret.Sortirane = new List<Voznja>();
            ret.Pretrazene = new List<Voznja>();

            Dispecer d = new Dispecer();
            Musterija m = new Musterija();
            Vozac v = new Vozac();
            if (ret.Uloga == Uloga.Musterija)
            {
                foreach (Musterija mu in Registrovani.Musterije)
                {
                    if (mu.KorisnickoIme == ret.KorisnickoIme)
                    {
                        mu.Filtrirane = ret.Filtrirane;
                        mu.Sortirane = ret.Sortirane;
                        mu.Pretrazene = ret.Pretrazene;
                        m = mu;
                        return View("musterijaWelcome", m);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Dispecer)
            {
                foreach (Dispecer di in Registrovani.Dispeceri)
                {
                    if (di.KorisnickoIme == ret.KorisnickoIme)
                    {
                        di.Filtrirane = ret.Filtrirane;
                        di.Sortirane = ret.Sortirane;
                        di.Pretrazene = ret.Pretrazene;
                        di.TraziVozac = false;
                        di.TraziMusteriju = false;
                        di.NadjeneMusterije = new List<Voznja>();
                        di.NadjeniVozaci = new List<Voznja>();
                        d = di;
                        return View("dispecerWelcome", d);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Vozac)
            {
                foreach (Vozac vo in Registrovani.Vozaci)
                {
                    if (vo.KorisnickoIme == ret.KorisnickoIme)
                    {
                        vo.Filtrirane = ret.Filtrirane;
                        vo.Sortirane = ret.Sortirane;
                        vo.Pretrazene = ret.Pretrazene;
                        v = vo;
                        return View("vozacWelcome", vo);
                    }
                }
            }

            return View("musterijaWelcome",ret);
        }

        public ActionResult TraziMusterija(string datum,string ocena,string cena,string musterija)
        {
            if(datum == null || ocena == null || cena == null)
            {
                return View("GreskaTrazenje");
            }

            Ocena o = Ocena.neocenjen;

            switch(ocena)
            {
                case "Neocenjen":
                    o = Ocena.neocenjen;
                    break;
                case "Veoma loša":
                    o = Ocena.veomaLose;
                    break;
                case "Loša":
                    o = Ocena.lose;
                     break;
                case "Dobra":
                    o = Ocena.dobro;
                    break;
                case "Veoma dobra":
                    o = Ocena.veomaDobro;
                    break;
                case "Odlična":
                    o = Ocena.odlicno;
                    break;
            }

            List<Voznja> pomocna = new List<Voznja>();

            Korisnik ret = new Korisnik();
            foreach (Korisnik must in Registrovani.SviZajedno)
            {
                if (must.KorisnickoIme == musterija)
                {
                    ret = must;
                }
            }

            ret.Pretrazivanje = true;

            if (ret.Filter)
            {
                pomocna = ret.Filtrirane;
            }
            else if (ret.Sortiranje)
            {
                pomocna = ret.Sortirane;
            }
            else
            {
                foreach (Voznja voz in ret.Voznja)
                {
                    pomocna.Add(voz);
                }
            }

            foreach(Voznja voznja in pomocna)
            {
                if(voznja.DatumIVremePorudzbine.ToString() == datum && voznja.Komentar.Ocena == o && voznja.Iznos == int.Parse(cena))
                {
                    ret.Pretrazene.Add(voznja);
                }
            }

            Dispecer d = new Dispecer();
            Musterija m = new Musterija();
            Vozac v = new Vozac();
            if (ret.Uloga == Uloga.Musterija)
            {
                foreach (Musterija mu in Registrovani.Musterije)
                {
                    if (mu.KorisnickoIme == ret.KorisnickoIme)
                    {
                        mu.Filtrirane = ret.Filtrirane;
                        mu.Sortirane = ret.Sortirane;
                        mu.Pretrazene = ret.Pretrazene;
                        m = mu;
                        return View("musterijaWelcome", m);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Dispecer)
            {
                foreach (Dispecer di in Registrovani.Dispeceri)
                {
                    if (di.KorisnickoIme == ret.KorisnickoIme)
                    {
                        di.Filtrirane = ret.Filtrirane;
                        di.Sortirane = ret.Sortirane;
                        di.Pretrazene = ret.Pretrazene;
                        d = di;
                        return View("dispecerWelcome", d);
                    }
                }
            }
            else if (ret.Uloga == Uloga.Vozac)
            {
                foreach (Vozac vo in Registrovani.Vozaci)
                {
                    if (vo.KorisnickoIme == ret.KorisnickoIme)
                    {
                        vo.Filtrirane = ret.Filtrirane;
                        vo.Sortirane = ret.Sortirane;
                        vo.Pretrazene = ret.Pretrazene;
                        v = vo;
                        return View("vozacWelcome", vo);
                    }
                }
            }


            return View("musterijaWelcome",ret);
        }

        public ActionResult TraziVozaca(string vozacI, string vozacP, string korisnik)
        {
            bool popunjenaPomocna = false;
            List<Voznja> pomocna = new List<Voznja>();
            Dispecer ret = new Dispecer();
            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if(d.KorisnickoIme == korisnik)
                {
                    ret = d;
                }
            }

            ret.TraziVozac = true;

            if (ret.Filter)
            {
                pomocna = ret.Filtrirane;
                popunjenaPomocna = true;
            }
            else if (ret.Sortiranje)
            {
                pomocna = ret.Sortirane;
                popunjenaPomocna = true;
            }

            if (vozacI != "" && vozacP != "")
            {
                if(!popunjenaPomocna)
                {
                    foreach(Vozac vozac in Registrovani.Vozaci)
                    {
                        if(vozac.Ime == vozacI && vozacP == vozac.Prezime)
                        {
                            foreach(Voznja voz in vozac.Voznja)
                            {
                                pomocna.Add(voz);
                            }
                        }
                    }
                }
                foreach(Voznja v in pomocna)
                {
                    if(v.Vozac.Ime == vozacI && v.Vozac.Prezime == vozacP)
                    {
                        ret.NadjeniVozaci.Add(v);
                    }
                }
            }
            else if(vozacP == "" && vozacI !="")
            {
                if (!popunjenaPomocna)
                {
                    foreach (Vozac vozac in Registrovani.Vozaci)
                    {
                        if (vozac.Ime == vozacI)
                        {
                            foreach (Voznja voz in vozac.Voznja)
                            {
                                pomocna.Add(voz);
                            }
                        }
                    }
                }
                foreach (Voznja v in pomocna)
                {
                    if (v.Vozac.Ime == vozacI)
                    {
                        ret.NadjeniVozaci.Add(v);
                    }
                }
            }
            else if(vozacI == "" && vozacP !="" )
            {
                if (!popunjenaPomocna)
                {
                    foreach (Vozac vozac in Registrovani.Vozaci)
                    {
                        if (vozacP == vozac.Prezime)
                        {
                            foreach (Voznja voz in vozac.Voznja)
                            {
                                pomocna.Add(voz);
                            }
                        }
                    }
                }
                foreach (Voznja v in pomocna)
                {
                    if (v.Vozac.Prezime == vozacP)
                    {
                        ret.NadjeniVozaci.Add(v);
                    }
                }
            }

            return View("dispecerWelcome",ret);
        }

        public ActionResult TraziMusteriju(string musterijaI,string musterijaP,string korisnik)
        {
            bool popunjenaPomocna = false;
            List<Voznja> pomocna = new List<Voznja>();
            Dispecer ret = new Dispecer();
            foreach (Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == korisnik)
                {
                    ret = d;
                }
            }

            ret.TraziMusteriju = true;
            if (ret.Filter)
            {
                pomocna = ret.Filtrirane;
                popunjenaPomocna = true;
            }
            else if (ret.Sortiranje)
            {
                pomocna = ret.Sortirane;
                popunjenaPomocna = true;
            }
            else if(ret.TraziVozac)
            {
                pomocna = ret.NadjeniVozaci;
                popunjenaPomocna = true;
            }

            if (musterijaI != "" && musterijaP != "")
            {
                if(!popunjenaPomocna)
                {
                    foreach (Musterija m in Registrovani.Musterije)
                    {
                        if (m.Ime == musterijaI && m.Prezime == musterijaP)
                        {
                            foreach (Voznja v in m.Voznja)
                            {
                                pomocna.Add(v);
                            }
                        }
                    }
                }
                

                foreach (Voznja v in pomocna)
                {
                    if (v.Musterija.Ime == musterijaI && v.Musterija.Prezime == musterijaP)
                    {
                        ret.NadjeneMusterije.Add(v);
                    }
                }
            }
            else if (musterijaP == "" && musterijaI != "")
            {
                if(!popunjenaPomocna)
                {
                    foreach (Musterija m in Registrovani.Musterije)
                    {
                        if (m.Ime == musterijaI)
                        {
                            foreach (Voznja v in m.Voznja)
                            {
                                pomocna.Add(v);
                            }
                        }
                    }
                }
                

                foreach (Voznja v in pomocna)
                {
                    if (v.Musterija.Ime == musterijaI)
                    {
                        ret.NadjeneMusterije.Add(v);
                    }
                }
            }
            else if (musterijaI == "" && musterijaP != "")
            {
                if(!popunjenaPomocna)
                {
                    foreach (Musterija m in Registrovani.Musterije)
                    {
                        if (m.Prezime == musterijaP)
                        {
                            foreach (Voznja v in m.Voznja)
                            {
                                pomocna.Add(v);
                            }
                        }
                    }
                }
                foreach (Voznja v in pomocna)
                {
                    if (v.Musterija.Prezime == musterijaP)
                    {
                        ret.NadjeneMusterije.Add(v);
                    }
                }
            }

            return View("dispecerWelcome",ret);
        }
        #endregion
    }
}