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
using System.Net;
using Windows.Data.Xml.Dom;
using System.Diagnostics;

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
            TextBlock txt = new TextBlock();
            txt.Text = "Hue";
            txt.FontSize = 14.0;
            Codes.Children.Add(txt);

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

                Debug.WriteLine(stringKursSredni);

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

                Debug.WriteLine(stringKursKupnaSprzedazy);
                CzytajXML();
            }
        }

        private void CzytajXML() {
            srednie.LoadXml(stringKursSredni);
            ks.LoadXml(stringKursKupnaSprzedazy);
            var avRoots = srednie.GetElementsByTagName("tabela_kursow");
            var avKursy = avRoots[0].ChildNodes;
            
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}