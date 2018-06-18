using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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

            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(@"D:\fakultet\III godina\WEB\Projekat\WP1718-PR142-2015\WebProjekat\WebProjekat\dispeceri.txt");
            while ((line = file.ReadLine()) != null)
            {
            Dispecer d = new Dispecer();
                string[] words = line.Split(',');

                d.KorisnickoIme = words[0];
                d.Lozinka = words[1];
                d.Ime = words[2];
                d.Prezime = words[3];
                
                if(words[4] == "Muski")
                {
                    d.Pol = Pol.Muski;
                }
                else if(words[4] == "Zenski")
                {
                    d.Pol = Pol.Zenski;
                }

                d.Jmbg = long.Parse(words[5]);
                d.BrTelefona = words[6];
                d.Mail = words[7];

                switch(words[8])
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
            }

            file.Close();

        }
    }
}
