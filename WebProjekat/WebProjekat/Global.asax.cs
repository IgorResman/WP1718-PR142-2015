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


            // Read the file and display it line by line.
            if(File.Exists(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\baza.xml"))
            {
                string ime;
                int x = 0;
                int y = 0;
                string ulica = "";
                int broj = 0;
                string grad = "";
                int posta = 0;
                string prezime;
                string korisnicko;
                string lozinka;
                long jmbg;
                string brTel;
                Uloga uloga = Uloga.Dispecer;
                Pol pol = Pol.Muski;
                string mail;
                List<Voznja> voznje = new List<Voznja>();

                using (XmlReader reader = XmlReader.Create(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\baza.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && reader.Name.Equals("Korisnik"))
                        {
                            bool postoji = false;
                            bool postojid = false;
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
                            if (reader.Value == "Muski")
                            {
                                pol = Pol.Muski;
                            }
                            else
                            {
                                pol = Pol.Zenski;
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
                            brTel = reader.Value;
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            if (reader.Value == "Dispecer")
                            {
                                uloga = Uloga.Dispecer;
                            }
                            else if (reader.Value == "Musterija")
                            {
                                uloga = Uloga.Musterija;
                            }
                            else if (reader.Value == "Vozac")
                            {
                                uloga = Uloga.Vozac;
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
                                broj = int.Parse(reader.Value);
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
                            }

                            if (uloga == Uloga.Dispecer)
                            {
                                Korisnik k = new Dispecer(korisnicko, lozinka, ime, prezime, pol, jmbg, brTel, mail, uloga);

                                foreach (Dispecer d in Registrovani.SviZajedno)
                                {
                                    if (k.KorisnickoIme == d.KorisnickoIme)
                                    {
                                        postoji = true;
                                    }

                                }

                                if (!postoji)
                                {
                                    Registrovani.SviZajedno.Add(k);
                                }

                                foreach (Dispecer d in Registrovani.Dispeceri)
                                {
                                    if (k.KorisnickoIme == d.KorisnickoIme)
                                    {
                                        postojid = true;
                                    }
                                }

                                if (!postojid)
                                {
                                    Registrovani.Dispeceri.Add(k as Dispecer);
                                }
                            }
                            else if (uloga == Uloga.Musterija)
                            {
                                Musterija m = new Musterija(korisnicko, lozinka, ime, prezime, pol, jmbg, brTel, mail, uloga);
                                Korisnik k = m;
                                Registrovani.SviZajedno.Add(k);
                                Registrovani.Musterije.Add(m);
                            }
                            else if (uloga == Uloga.Vozac)
                            {
                                Vozac v = new Vozac(korisnicko,lozinka,ime,prezime,pol,jmbg,brTel,mail,uloga);
                                Adresa a = new Adresa(ulica,broj,grad,posta);
                                Lokacija l = new Lokacija(x, y, a);
                                v.Lokacija = l;
                                
                                Korisnik k = new Vozac(korisnicko, lozinka, ime, prezime, pol, jmbg, brTel, mail, uloga);
                                Registrovani.SviZajedno.Add(k);
                                Registrovani.Vozaci.Add(v);
                            }
                        }
                    }
                }

            }


            string line;

            if(Registrovani.Dispeceri.Count == 0)
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
            

        }
    }
}
