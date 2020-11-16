using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace randomszemely {
    class Program {
        public static List<Szemely> szemelyek = new List<Szemely>();
        public static List<string> knevek_noi = new List<string>();
        public static List<string> knevek_ffi = new List<string>();
        public static List<string> vnevek = new List<string>();
        public static List<string> telep = new List<string>();
        public static List<string> existingIDs = new List<string>();
        static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            // keresztnevek (női, férfi), vezetéknevek, települések, és megyék lekérése 
            WebClient web = new WebClient();
            foreach(string ffi_knev in web.DownloadString("https://srv-store6.gofile.io/download/HCOO3L/ffiknevek.txt").Split("\r\n")) { knevek_ffi.Add(ffi_knev); }
            foreach(string noi_knev in web.DownloadString("https://srv-store1.gofile.io/download/GZMRuq/noiknevek.txt").Split("\r\n")) { knevek_noi.Add(noi_knev); }
            foreach(string veznev in web.DownloadString("https://srv-store6.gofile.io/download/abQFvv/vnevek.txt").Split("\r\n")) { vnevek.Add(veznev); }
            foreach(string telep_megy in web.DownloadString("https://srv-store6.gofile.io/download/B6yFTT/telepulesek-megyek.txt").Split("\r\n")) { telep.Add(telep_megy); }
            // addig generál embereket amíg escape-et nem üt a felhasználó, utána vége van a programnak és bazárul
            ConsoleKey key;
            do {
                Console.Clear();
                Szemely sz = new Szemely();
                Console.WriteLine($"\n   [{sz.ID}] {sz.Vnev} {sz.Knev} ({sz.Kor}, {(sz.Nem ? "férfi" : "nő")}): {sz.Magassag} cm\n   Lakhely: {sz.Lakas.Megye}, {sz.Lakas.Telepules}, {sz.Lakas.Utca} {sz.Lakas.Szam}. ({sz.Lakas.Terulet:F2} m2)");
                key = Console.ReadKey().Key;
            }
            while(key!=ConsoleKey.Escape);
        }
    }

    public class Haz {
        public string Megye { get; set; }
        public string Telepules { get; set; }
        public string Utca { get; set; }
        public int Szam { get; set; }
        public double Terulet { get; set; }
        public Haz() {
            Random r = new Random();
            string telep_megye =  Program.telep[r.Next(0,Program.telep.Count()-1)];
            Megye = telep_megye.Split(" - ")[1];
            Telepules = telep_megye.Split(" - ")[0];
            Utca = string.Join(' ',Program.vnevek[r.Next( 0, Program.vnevek.Count()-1)],Program.knevek_ffi.Concat(Program.knevek_noi).ElementAt( r.Next( 0, Program.knevek_noi.Count()-1+(Program.knevek_ffi.Count()-1)) ),"utca");
            Szam = r.Next(1,300);
            Terulet = r.Next(100,30000)*0.981;
        }
    }
    public class Szemely {
        public string Knev { get; set; }
        public string Vnev { get; set; }
        public int Kor { get; set; }
        public bool Nem { get; set; } //   true: férfi   false: nő
        public int Magassag { get; set; }
        public string ID { get; set; }
        public Haz Lakas { get; set; }
        public Szemely() {
            Random r = new Random();
            bool nem = r.Next(0,1)>0;
            Knev = nem ? Program.knevek_ffi[r.Next(0, Program.knevek_ffi.Count() - 1)] : Program.knevek_noi[r.Next(0, Program.knevek_noi.Count() - 1)];
            Vnev = Program.vnevek[r.Next(0, Program.vnevek.Count() - 1)];
            Kor = r.Next(1, 99);
            Nem = nem;
            Magassag = Kor>16 ? r.Next(156,209) : r.Next(30,155);
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            do {
                ID = "";
                for (int a=0; a<=15; a++) {
                    ID += chars[r.Next(0, chars.Length - 1)];
                }
            }
            while (Program.existingIDs.Contains(ID)); // a lehetséges duplikáció elkerülése (addig generál új ID-kat amíg talál egy olyat, ami még nem fordult elő)
            Program.existingIDs.Add(ID);
            Lakas = new Haz();
        }
    }
}
