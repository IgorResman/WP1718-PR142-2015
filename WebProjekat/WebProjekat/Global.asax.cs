using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using WebProjekat.Models;

namespace WebProjekat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Registrovani r = new Registrovani();

            List<Korisnik> registrov = new List<Korisnik>();
            Voznje sveVoznje = new Voznje();

            Ucitavanje();
        }

        #region Ucitavanje
        private void Ucitavanje()
        {

            if (File.Exists(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\korisnici.xml"))
            {
                string ime = "";
                string prezime = "";
                string korisnicko = "";
                string lozinka = "";
                long jmbg = 0;
                string telefon = "";
                string mail = "";
                Pol pol = Pol.Muski;
                Uloga uloga = Uloga.Dispecer;
                int x = 0;
                int y = 0;
                int brul = 0;
                int posta = 0;
                string ulica = "";
                string grad = "";
                int godiste = 0;
                string reg = "";
                TipVozila tip = TipVozila.Kombi;
                int taxiBr = 0;

                using (XmlReader reader = XmlReader.Create(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\korisnici.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && reader.Name.Equals("Korisnik"))
                        {
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            ime = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            prezime = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            jmbg = long.Parse(reader.Value);
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            korisnicko = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            lozinka = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            switch (reader.Value)
                            {
                                case "Muski":
                                    pol = Pol.Muski;
                                    break;
                                case "Zenski":
                                    pol = Pol.Zenski;
                                    break;
                            }
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            mail = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            telefon = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            switch (reader.Value)
                            {
                                case "Dispecer":
                                    uloga = Uloga.Dispecer;
                                    break;
                                case "Vozac":
                                    uloga = Uloga.Vozac;
                                    break;
                                case "Musterija":
                                    uloga = Uloga.Musterija;
                                    break;
                            }

                            if (uloga == Uloga.Musterija)
                            {
                                Musterija m = new Musterija(korisnicko, lozinka, ime, prezime, pol, jmbg, telefon, mail, uloga);
                                Registrovani.Musterije.Add(m);
                                Korisnik k = m;
                                Registrovani.SviZajedno.Add(k);
                            }
                            else if (uloga == Uloga.Dispecer)
                            {
                                Dispecer m = new Dispecer(korisnicko, lozinka, ime, prezime, pol, jmbg, telefon, mail, uloga);
                                Registrovani.Dispeceri.Add(m);
                                Korisnik k = m;
                                Registrovani.SviZajedno.Add(k);
                            }
                            else if (uloga == Uloga.Vozac)
                            {
                                Vozac v = new Vozac(korisnicko, lozinka, ime, prezime, pol, jmbg, telefon, mail, uloga);


                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                godiste = int.Parse(reader.Value);
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reg = reader.Value;
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                taxiBr = int.Parse(reader.Value);
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                switch (reader.Value)
                                {
                                    case "Putnicko":
                                        tip = TipVozila.Putnicko;
                                        break;
                                    case "Kombi":
                                        tip = TipVozila.Kombi;
                                        break;
                                }
                                Automobil a = new Automobil(v, godiste, reg, taxiBr, tip);

                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                ulica = reader.Value;
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                brul = int.Parse(reader.Value);
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                grad = reader.Value;
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                posta = int.Parse(reader.Value);
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                x = int.Parse(reader.Value);
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                y = int.Parse(reader.Value);
                                Adresa adresa = new Adresa(ulica, brul, grad, posta);
                                Lokacija l = new Lokacija(x, y, adresa);

                                v.Automobil = a;
                                v.Lokacija = l;

                                Registrovani.Vozaci.Add(v);
                                Korisnik k = v;
                                Registrovani.SviZajedno.Add(k);
                            }
                        }
                    }
                }
            }


                string line;

                if (Registrovani.Dispeceri.Count == 0)
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\dispeceri.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        Dispecer d = new Dispecer();
                        string[] words = line.Split(',');

                        d.KorisnickoIme = words[0];
                        d.Lozinka = words[1];
                        d.Ime = words[2];
                        d.Prezime = words[3];

                        if (words[4] == "Muski")
                        {
                            d.Pol = Pol.Muski;
                        }
                        else if (words[4] == "Zenski")
                        {
                            d.Pol = Pol.Zenski;
                        }

                        d.Jmbg = long.Parse(words[5]);
                        d.BrTelefona = words[6];
                        d.Mail = words[7];

                        switch (words[8])
                        {
                            case "Dispecer":
                                d.Uloga = Uloga.Dispecer;
                                break;
                            case "Musterija":
                                d.Uloga = Uloga.Musterija;
                                break;
                            case "Vozac":
                                d.Uloga = Uloga.Vozac;
                                break;
                        }

                        Registrovani.Dispeceri.Add(d);
                        Registrovani.SviZajedno.Add(d);
                    }

                    file.Close();

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
                        foreach (Korisnik m in Registrovani.SviZajedno)
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
                            int i = 1;
                            foreach (Voznja v in m.Voznja)
                            {
                                writer.WriteStartElement("VoznjaBroj" + i.ToString());
                                writer.WriteElementString("DatumPorudzbine", v.DatumIVremePorudzbine.ToString());
                                writer.WriteElementString("PocetnaLokacija", v.PocetnaLokacija.ToString());
                                writer.WriteElementString("KrajnjaLokacija", v.Odrediste.ToString());
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
            else
            {
                UcitajVoznje();
            }
        }

        private void UcitajVoznje()
        {
            if (File.Exists(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\voznje.xml"))
            {
                string datum = "";
                string pocetna = "";
                string krajnja = "";
                TipVozila tip = TipVozila.Kombi;
                string musterijaI = "";
                string musterijaP = "";
                string vozacI = "";
                string vozacP = "";
                string dispecerI = "";
                string dispecerP = "";
                StatusVoznje status = StatusVoznje.Formirana;
                string komentar = "";
                string komentarDatum = "";
                Ocena ocena = Ocena.neocenjen;
                int iznos = 0;

                using (XmlReader reader = XmlReader.Create(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\voznje.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && reader.Name.Equals("Voznja"))
                        {
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            datum = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            pocetna = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            krajnja = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            switch(reader.Value)
                            {
                                case "Putnicko":
                                    tip = TipVozila.Putnicko;
                                    break;
                                case "Kombi":
                                    tip = TipVozila.Kombi;
                                    break;
                            }
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            musterijaI = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            musterijaP = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            vozacI = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            vozacP = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            dispecerI = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            dispecerP = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            switch(reader.Value)
                            {
                                case "Kreirana":
                                    status = StatusVoznje.Kreirana;
                                    break;
                                case "Formirana":
                                    status = StatusVoznje.Formirana;
                                    break;
                                case "Neuspesna":
                                    status = StatusVoznje.Neuspesna;
                                    break;
                                case "Obradjena":
                                    status = StatusVoznje.Obradjena;
                                    break;
                                case "Otkazana":
                                    status = StatusVoznje.Otkazana;
                                    break;
                                case "Prihvacena":
                                    status = StatusVoznje.Prihvacena;
                                    break;
                                case "Uspesna":
                                    status = StatusVoznje.Uspesna;
                                    break;
                            }
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            komentar = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            komentarDatum = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            switch(reader.Value)
                            {
                                case "neocenjen":
                                    ocena = Ocena.neocenjen;
                                    break;
                                case "dobro":
                                    ocena = Ocena.dobro;
                                    break;
                                case "lose":
                                    ocena = Ocena.lose;
                                    break;
                                case "odlicno":
                                    ocena = Ocena.odlicno;
                                    break;
                                case "veomaDobro":
                                    ocena = Ocena.veomaDobro;
                                    break;
                                case "veomaLose":
                                    ocena = Ocena.veomaLose;
                                    break;
                            }
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            iznos = int.Parse(reader.Value);

                            SacuvajVoznju(datum, pocetna, krajnja, tip, musterijaI, musterijaP, vozacI, vozacP, dispecerI, dispecerP, status, komentar, komentarDatum, ocena, iznos);
                        }
                    }
                }
            }
        }

        private void SacuvajVoznju(string datum, string pocetna, string krajnja, TipVozila tip, string musterijaI, string musterijaP, string vozacI, string vozacP, string dispecerI, string dispecerP, StatusVoznje status, string komentar, string komentarDatum, Ocena ocena, int iznos)
        {
            DateTime date = ToDate(datum);
            string[] podela = pocetna.Split(',');
            string[] ulica = podela[0].Split('_');
            string[] grad = podela[1].Split('_');

            Adresa adresa = new Adresa(ulica[0], int.Parse(ulica[1]), grad[0], int.Parse(grad[1]));
            Lokacija poc = new Lokacija(1, 1, adresa);

            podela = krajnja.Split(',');
            ulica = podela[0].Split('_');
            grad = podela[1].Split('_');

            Adresa adresaa = new Adresa(ulica[0], int.Parse(ulica[1]), grad[0], int.Parse(grad[1]));
            Lokacija kraj = new Lokacija(1, 1, adresaa);

            Musterija musterija = new Musterija();
            Vozac vozac = new Vozac();
            Dispecer dispecer = new Dispecer();
            if(musterijaI != "nema" && musterijaP != "nema")
            {
                foreach(Musterija m in Registrovani.Musterije)
                {
                    if(musterijaI == m.Ime && musterijaP == m.Prezime)
                    {
                        musterija = m;
                        break;
                    }
                }
            }
            else
            {
                musterija = new Musterija("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Musterija);
            }

            if(vozacI != "nema" && vozacP != "nema")
            {
                foreach (Vozac m in Registrovani.Vozaci)
                {
                    if (vozacI == m.Ime && vozacP == m.Prezime)
                    {
                        vozac = m;
                        break;
                    }
                }
            }
            else
            {
                vozac = new Vozac("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Vozac);
            }

            if(dispecerI != "nema" && dispecerP != "nema")
            {
                foreach (Dispecer m in Registrovani.Dispeceri)
                {
                    if (dispecerI == m.Ime && dispecerP == m.Prezime)
                    {
                        dispecer = m;
                        break;
                    }
                }
            }
            else
            {
                dispecer = new Dispecer("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Dispecer);
            }
            Voznja v = new Voznja();
            DateTime kom = ToDate(komentarDatum);
            Komentar k = new Komentar();
            if(status == StatusVoznje.Otkazana)
            {
                k = new Komentar(komentar, kom, musterija, v, ocena);
            }
            else if(status == StatusVoznje.Neuspesna)
            {
                k = new Komentar(komentar, kom, vozac, v, ocena);
            }
            else if(status == StatusVoznje.Uspesna)
            {
                k = new Komentar(komentar, kom, musterija, v, ocena);
            }
            else
            {
                k = new Komentar("bez opisa", kom, new Korisnik("nema", "nema", "nema", "nema", Pol.Muski, 0000000000, "nema", "nema", Uloga.Dispecer),v,Ocena.neocenjen);
            }

            v = new Voznja(date,poc,tip,musterija,kraj,dispecer,vozac,iznos,k,status);

            k.Voznja = v;
            v.Komentar = k;

            if (musterijaI != "nema" && musterijaP != "nema")
            {
                foreach (Musterija m in Registrovani.Musterije)
                {
                    if(m.KorisnickoIme == musterija.KorisnickoIme)
                    {
                        m.Voznja.Add(v);
                    }
                }
            }


            if (vozacI != "nema" && vozacP != "nema")
            {
                foreach (Vozac m in Registrovani.Vozaci)
                {
                    if(m.KorisnickoIme == vozac.KorisnickoIme)
                    {
                        m.Voznja.Add(v);
                    }
                }
            }


            if (dispecerI != "nema" && dispecerP != "nema")
            {
                foreach (Dispecer m in Registrovani.Dispeceri)
                {
                    if(m.KorisnickoIme == dispecer.KorisnickoIme)
                    {
                        m.Voznja.Add(v);
                    }
                }
            }

            Voznje.SveVoznje.Add(v);

            foreach(Korisnik kor in Registrovani.SviZajedno)
            {
                if(kor.KorisnickoIme == musterija.KorisnickoIme)
                {
                    kor.Voznja = musterija.Voznja;
                }
                else if(kor.KorisnickoIme == vozac.KorisnickoIme)
                {
                    kor.Voznja = vozac.Voznja;
                }
                else if(kor.KorisnickoIme == dispecer.KorisnickoIme)
                {
                    kor.Voznja = dispecer.Voznja;
                }
            }
        }

        public DateTime ToDate(string datum)
        {
            DateTime ret;

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

            ret = new DateTime(year, month, day, hour, minute, second);



            return ret;

        }
        #endregion
    }
}
