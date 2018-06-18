using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Lokacija
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Adresa Adresa { get; set; }

        public Lokacija()
        {

        }

        public Lokacija(int x, int y,Adresa a)
        {
            X = x;
            Y = y;
            Adresa = a;
        }
    }
}