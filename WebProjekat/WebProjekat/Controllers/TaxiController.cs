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
                case "musko":
                    p = Pol.Muski;
                    break;
                case "zensko":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            Vozac v = new Vozac(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Vozac);


            foreach(Vozac voz in Registrovani.Vozaci)
            {
                if(v.KorisnickoIme == voz.KorisnickoIme)
                {
                    return View("VozacPostoji");
                }
            }

            Registrovani.Vozaci.Add(v);

            foreach(Korisnik k in Registrovani.SviZajedno)
            {
                if(k.KorisnickoIme == v.KorisnickoIme)
                {
                    return View("KorisnickoPostoji");
                }
            }

            Registrovani.SviZajedno.Add(v);

            Sacuvaj(Registrovani.SviZajedno);

            Dispecer d = HttpContext.Application["Dispecer"] as Dispecer;

            return View("Uspeh",d);
        }
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
            Dispecer d =HttpContext.Application["Dispecer"] as Dispecer;

            return View("dispecerWelcome", d);
        }

        public ActionResult Izmeni()
        {
            Dispecer d = HttpContext.Application["Dispecer"] as Dispecer;
            return View(d);
        }

        public ActionResult IzmeniVozac()
        {
            Vozac v = HttpContext.Application["Vozac"] as Vozac;
            return View(v);
        }

        public ActionResult IzmeniMusterija()
        {
            Musterija m = HttpContext.Application["Musterija"] as Musterija;

            return View(m);
        }

        public ActionResult dispecerWelcome()
        {
            return View();
        }

        private void Sacuvaj(List<Korisnik> musterije)
        {
            string filename = @"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\baza.xml";
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;

                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartElement("Ulogovani");
                foreach (Korisnik m in musterije)
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
                                writer.WriteStartElement("Adresa");
                                writer.WriteElementString("NazivUlice", vo.Lokacija.Adresa.Naziv);
                                writer.WriteElementString("BrojUlice", vo.Lokacija.Adresa.Broj.ToString());
                                writer.WriteElementString("Grad", vo.Lokacija.Adresa.Mesto);
                                writer.WriteElementString("PostanskiBroj", vo.Lokacija.Adresa.BrojMesta.ToString());
                                writer.WriteElementString("X", vo.Lokacija.X.ToString());
                                writer.WriteElementString("Y", vo.Lokacija.Y.ToString());
                                writer.WriteEndElement();
                                writer.WriteStartElement("Voznje");
                                int i = 1;
                                foreach (Voznja v in m.Voznja)
                                {
                                    string odrediste = v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.Broj.ToString() + "," + v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.BrojMesta.ToString();
                                    string pocetna = v.PocetnaLokacija.Adresa.Naziv + " " + v.PocetnaLokacija.Adresa.Broj.ToString() + "," + v.PocetnaLokacija.Adresa.Mesto + " " + v.PocetnaLokacija.Adresa.BrojMesta.ToString();


                                    writer.WriteStartElement("VoznjaBroj" + i.ToString());
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
                                    writer.WriteElementString("Iznos", v.Iznos.ToString());
                                    writer.WriteEndElement();
                                    i++;
                                }
                                writer.WriteEndElement();
                                writer.WriteEndElement();

                            }
                        }
                    }
                    else
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
                        writer.WriteElementString("BrojTelefona", m.BrTelefona);
                        writer.WriteElementString("Uloga", m.Uloga.ToString());
                        writer.WriteStartElement("Voznje");

                        if (m.Uloga == Uloga.Musterija)
                        {
                            foreach (Musterija musterija in Registrovani.Musterije)
                            {
                                if (m.KorisnickoIme == musterija.KorisnickoIme)
                                {
                                    int i = 1;
                                    foreach (Voznja v in musterija.Voznja)
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

                                        string odrediste = v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.Broj.ToString() + "," + v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.BrojMesta.ToString();
                                        string pocetna = v.PocetnaLokacija.Adresa.Naziv + " " + v.PocetnaLokacija.Adresa.Broj.ToString() + "," + v.PocetnaLokacija.Adresa.Mesto + " " + v.PocetnaLokacija.Adresa.BrojMesta.ToString();

                                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        writer.WriteStartElement("VoznjaBroj" + i.ToString());
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
                                        writer.WriteElementString("Iznos", v.Iznos.ToString());
                                        writer.WriteEndElement();
                                        i++;
                                    }
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();

                                }
                            }
                        }
                        else if (m.Uloga == Uloga.Dispecer)
                        {
                            foreach (Dispecer musterija in Registrovani.Dispeceri)
                            {
                                if (m.KorisnickoIme == musterija.KorisnickoIme)
                                {
                                    int i = 1;
                                    foreach (Voznja v in musterija.Voznja)
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

                                        string odrediste = v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.Broj.ToString() + ","+ v.Odrediste.Adresa.Mesto + " " + v.Odrediste.Adresa.BrojMesta.ToString();
                                        string pocetna = v.PocetnaLokacija.Adresa.Naziv + " " + v.PocetnaLokacija.Adresa.Broj.ToString() + "," + v.PocetnaLokacija.Adresa.Mesto + " " + v.PocetnaLokacija.Adresa.BrojMesta.ToString();
                                        
                                        
                                        
                                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        writer.WriteStartElement("VoznjaBroj" + i.ToString());
                                        writer.WriteElementString("DatumPorudzbine", v.DatumIVremePorudzbine.ToString());
                                        writer.WriteElementString("PocetnaLokacija", pocetna );
                                        writer.WriteElementString("KrajnjaLokacija", odrediste);
                                        writer.WriteElementString("TipVozila", v.Tip.ToString());
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
                                    writer.WriteEndElement();

                                }
                            }
                        } 
                    }
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
                    return View("LozinkaGreska");
                }
                else if (m.KorisnickoIme == korisnickoIme && m.Lozinka == lozinka)
                {
                    m.Ulogovan = true;
                    HttpContext.Application["Musterija"] = m;
                    return View("musterijaWelcome",m);
                }
            }

            foreach(Vozac v in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == korisnickoIme && v.Lozinka != lozinka)
                {
                    return View("LozinkaGreska");

                }
                else if (v.KorisnickoIme == korisnickoIme && v.Lozinka == lozinka)
                {
                    v.Ulogovan = true;
                    HttpContext.Application["Vozac"] = v;
                    return View("vozacWelcome",v);
                }
            }

            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == korisnickoIme && d.Lozinka != lozinka)
                {
                    return View("LozinkaGreska");

                }
                else if (d.KorisnickoIme == korisnickoIme && d.Lozinka == lozinka)
                {
                    d.Ulogovan = true;
                    HttpContext.Application["Dispecer"] = d;
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

        public ActionResult OdjaviDispecer()
        {
            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if(d == HttpContext.Application["Dispecer"])
                {
                    d.Ulogovan = false;
                    HttpContext.Application["Dispecer"] = null;
                }
            }
            return View("Index");
        }

        public ActionResult OdjaviVozac()
        {
            foreach(Vozac v in Registrovani.Vozaci)
            {
                if(v == HttpContext.Application["Vozac"])
                {
                    v.Ulogovan = false;
                    HttpContext.Application["Vozac"] = null;
                }
            }
            return View("Index");
        }

        public ActionResult OdjaviMusterija()
        {
            foreach (Musterija v in Registrovani.Musterije)
            {
                if (v == HttpContext.Application["Musterija"])
                {
                    v.Ulogovan = false;
                    HttpContext.Application["Musterija"] = null;
                }
            }
            return View("Index");
        }

        public ActionResult PromeniLokaciju()
        {
            Vozac v = HttpContext.Application["Vozac"] as Vozac;
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

            Vozac v = HttpContext.Application["Vozac"] as Vozac;

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

        public ActionResult ZatraziVoznjuMusterija()
        {
            return View();
        }

        public ActionResult ZatrazilaMusterija(string ulica,string broj,string mesto, string postanski,string vozilo)
        {
            int br = int.Parse(broj);
            int post = int.Parse(postanski);

            Musterija m = HttpContext.Application["Musterija"] as Musterija;

            Voznja v = new Voznja();
            Adresa a = new Adresa(ulica, br, mesto, post);
            Lokacija l = new Lokacija(1,1,a);
            v.DatumIVremePorudzbine = DateTime.Now;
            v.PocetnaLokacija = l;

            switch(vozilo)
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
            
            foreach(Musterija mu in Registrovani.Musterije)
            {
                if(mu.KorisnickoIme == m.KorisnickoIme)
                {
                    mu.Voznja = m.Voznja;
                    break;
                }
            }

            Sacuvaj(Registrovani.SviZajedno);



            return View("UspesnoZatrazenaVoznja",v);
        }

        public ActionResult VratiProfilMusterija()
        {
            Musterija m = HttpContext.Application["Musterija"] as Musterija;

            return View("musterijaWelcome", m);
        }

        public ActionResult VratiProfilMusterijaKomentar(string komentar,string datum)
        {
            Musterija m = HttpContext.Application["Musterija"] as Musterija;

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
                if (v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year && v.Musterija.KorisnickoIme == korisnik)
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
                        if(v.DatumIVremePorudzbine.Date == d.Date && v.DatumIVremePorudzbine.Day == d.Day && v.DatumIVremePorudzbine.Year == d.Year)
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
    }
}