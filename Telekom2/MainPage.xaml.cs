using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telekom2.Resources;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using Windows.Data.Xml.Dom;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Telekom2 {
    public partial class MainPage : PhoneApplicationPage {
        XmlDocument srednie = new XmlDocument();
        XmlDocument ks = new XmlDocument();
        Dictionary<string, ExchangeInfo> dane = new Dictionary<string, ExchangeInfo>();

        WebClient wc;

        Uri adresSredni = new Uri("http://www.nbp.pl/kursy/xml/a082z150429.xml");
        Uri adresKS = new Uri("http://www.nbp.pl/kursy/xml/c082z150429.xml");
        string stringKursSredni;
        string stringKursKupnaSprzedazy;

        // Constructor
        public MainPage() {
            InitializeComponent();
            

            PobierzSrednie();
        }

        public void PobierzSrednie() {
            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ZapiszSrednie);
            wc.DownloadStringAsync(adresSredni);
        }

        private void ZapiszSrednie(Object sender, DownloadStringCompletedEventArgs e) {
            if (!e.Cancelled && e.Error == null) {
                stringKursSredni = (string)e.Result;

                //Debug.WriteLine(stringKursSredni);

                PobierzKS();
            }
        }

        public void PobierzKS() {
            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ZapiszKS);
            
            
            wc.DownloadStringAsync(adresKS);
        }

        private void ZapiszKS(Object sender, DownloadStringCompletedEventArgs e) {
            if (!e.Cancelled && e.Error == null) {
                stringKursKupnaSprzedazy = (string)e.Result;

                //Debug.WriteLine(stringKursKupnaSprzedazy);
                CzytajXML();
            }
        }

        private void CzytajXML() {
            srednie.LoadXml(stringKursSredni);
            ks.LoadXml(stringKursKupnaSprzedazy);

            InterpretAverage();
            InterpretSB();

            PrintExchange();
        }

        private void InterpretAverage() {
            var avRoots = srednie.GetElementsByTagName("tabela_kursow");
            var avKursy = avRoots[0].ChildNodes;

            foreach (var n in avKursy) {
                //Debug.WriteLine(n.GetXml() + " DOTDOT");
                string kod = null;
                double kurs = 0;
                if (n.HasChildNodes()) {
                    var w = n.SelectSingleNode("kod_waluty");
                    if (w != null) {

                        kod = w.InnerText;
                        Debug.WriteLine(kod);
                    }

                    var k = n.SelectSingleNode("kurs_sredni");
                    if (k != null) {
                        kurs = double.Parse(k.InnerText);
                        Debug.WriteLine(kurs);
                    }


                }

                if (kod != null && kurs != 0)
                    dane.Add(kod, new ExchangeInfo(kurs));
            }

        }

        private void InterpretSB() {
            var ksRoots = ks.GetElementsByTagName("tabela_kursow");
            var ksKursy = ksRoots[0].ChildNodes;

            foreach (var n in ksKursy) {
                //Debug.WriteLine(n.GetXml() + " DOTDOT");
                string kod = null;
                double kursS = 0;
                double kursB = 0;
                if (n.HasChildNodes()) {
                    var w = n.SelectSingleNode("kod_waluty");
                    if (w != null) {

                        kod = w.InnerText;
                        Debug.WriteLine(kod);
                    }

                    var k = n.SelectSingleNode("kurs_sprzedazy");
                    if (k != null) {
                        kursS = double.Parse(k.InnerText);
                        Debug.WriteLine(kursS);
                    }

                    var kk = n.SelectSingleNode("kurs_kupna");
                    if (kk != null) {
                        kursB = double.Parse(kk.InnerText);
                        Debug.WriteLine(kursB);
                    }


                }

                if (kod != null && dane[kod] != null && kursS != 0 && kursB != 0) {
                    dane[kod].Buy = kursB;
                    dane[kod].Sell = kursS;
                }
            }
        }

        public void PrintExchange() {
            double fontSize = 15.0;
            foreach (var i in dane) {
                TextBlock kod = new TextBlock();
                kod.Text = i.Key;
                kod.FontSize = fontSize;
                Codes.Children.Add(kod);

                TextBlock sell = new TextBlock();
                sell.Text = (i.Value.Sell == 0) ? "-" : i.Value.Sell.ToString("#0.0000");
                sell.FontSize = fontSize;
                Sell.Children.Add(sell);

                TextBlock buy = new TextBlock();
                buy.Text = (i.Value.Buy == 0) ? "-" : i.Value.Buy.ToString("#0.0000");
                buy.FontSize = fontSize;
                Buy.Children.Add(buy);

                TextBlock av = new TextBlock();
                av.Text = i.Value.Average.ToString("#0.0000");
                av.FontSize = fontSize;
                Average.Children.Add(av);
            }
            
        }
    }
}