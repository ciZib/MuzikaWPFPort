using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Threading;
using System.IO;
using System.Timers;
using AxWMPLib;
using WMPLib;
using System.Threading;

namespace MuzikaWPFPort
{
    /// <summary>
    /// Projekat Medija Player. Tip aplikacije: WPF Aplikacija. Radio: Miroslav Bizic. Godina: 2022.
    /// Dodatne biblioteke: AxWMPLib.dll, WMPLib.dll
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Funkcionalnost Aplikacije
        public MainWindow()
        {
            InitializeComponent();
            citajPesmeIzFajla();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                glavniMeni.Height = e.NewSize.Height - 60;
                lbxListaPesama.Height = e.NewSize.Height - 150;
                lbxListaPesama.Width = e.NewSize.Width - 300;
                kanvas.Width = e.NewSize.Width - 150;
            }
        }
        private void GlavniMeni_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
        void sakrijPanele()
        {
            medijaPanel.Visibility = Visibility.Collapsed;
            plejlistaPanel.Visibility = Visibility.Collapsed;
            dodatneInformacijePanel.Visibility = Visibility.Collapsed;
        }
        private void MedijaDugme_Click(object sender, RoutedEventArgs e)
        {
            if (medijaPanel.Visibility == Visibility.Visible)
            {
                sakrijPanele();
            }
            else
            {
                sakrijPanele();
                if (medijaPanel.Visibility == Visibility.Collapsed) medijaPanel.Visibility = Visibility.Visible;
                else medijaPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void PlejlistaDugme_Click(object sender, RoutedEventArgs e)
        {
            if (plejlistaPanel.Visibility == Visibility.Visible)
            {
                sakrijPanele();
            }
            else
            {
                sakrijPanele();
                if (plejlistaPanel.Visibility == Visibility.Collapsed) plejlistaPanel.Visibility = Visibility.Visible;
                else plejlistaPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void DeveloperDugme_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developer: Miroslav Bizic\nDatum proizvodnje: 2022");
        }
        private void DodatneInformacijeDugme_Click(object sender, RoutedEventArgs e)
        {
            if (dodatneInformacijePanel.Visibility == Visibility.Visible)
            {
                sakrijPanele();
            }
            else
            {
                sakrijPanele();
                if (dodatneInformacijePanel.Visibility == Visibility.Collapsed) dodatneInformacijePanel.Visibility = Visibility.Visible;
                else dodatneInformacijePanel.Visibility = Visibility.Collapsed;
            }
        }
        private void Kucica_MouseDown(object sender, MouseButtonEventArgs e)
        {
                sakrijPanele();
        }
        #endregion

        #region Pesme, ubacivanje, izbacivanje, Title bar
        private List<string> listaZaLbx = new List<string>();
        List<Pesma> listaKlasePesma = new List<Pesma>();
        private List<Pesma> distinctListaPesama;
        public string[] path;
        public string[] citajPath;
        public bool pomeranje;
        int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
         List<Pesma> tempListaKlasePesma = new List<Pesma>();
        private void ObrisiSvePesme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lbxListaPesama.Items.Clear();
                FileStream fsOverwrite = new FileStream($"{dir}\\pesme.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fsOverwrite);
                tempListaKlasePesma.Clear();
                foreach (var pesma in tempListaKlasePesma)
                {
                    sw.WriteLine(pesma.Path);
                }
                sw.Close();
            }
            catch (Exception) { }
        }
        void citajPesmeIzFajla() {
            try
            {
                StreamReader sr = new StreamReader($"{dir}\\pesme.txt");
                int brLinija = TotalLines($"{dir}\\pesme.txt");
                int i = 0;
                try
                {
                    citajPath = new string[brLinija];
                    while (sr.Peek() >= 0)
                    {
                        citajPath[i] = $"{sr.ReadLine()}";
                        //MessageBox.Show("procitao sam");
                        i++;
                    }
                     m.CreateControl();
                    for (int j = 0; j < citajPath.Length; j++)
                    {
                        listaZaLbx.Add(citajPath[j]);
                        media = this.m.newMedia(citajPath[j]);
                        string ime = System.IO.Path.GetFileName(citajPath[j]);
                        string duzina = media.durationString;
                        Pesma pesma = new Pesma(ime, citajPath[j], duzina);
                        listaKlasePesma.Add(pesma);
                    }
                    foreach (var pesmaa in listaKlasePesma)
                    {
                        lbxListaPesama.Items.Add(pesmaa.ToString());
                        tempListaKlasePesma.Add(pesmaa);
                    }
                    distinctListaPesama = listaKlasePesma.Distinct().ToList();
                    listaKlasePesma.Clear();
                }
                catch (Exception) { sr.Close(); }
                sr.Close();
            }
            catch (Exception) { }
        }
        string dir = System.Environment.CurrentDirectory;
        AxWMPLib.AxWindowsMediaPlayer m = new AxWMPLib.AxWindowsMediaPlayer();
        //public MediaPlayer mp = new MediaPlayer();
        WMPLib.IWMPMedia media; 
        public void traziLokaciju()
        {
            FileStream fsOverwrite = new FileStream($"{dir}\\pesme.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fsOverwrite);
            string[] path;
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "MP3 Audio File (*.mp3)|*.mp3|Windows Media File (*.wma)|*.wma|WAV Audio File (*.wav)|*.wav";
            od.Multiselect = true;
            bool? rezultat = od.ShowDialog();
            if (rezultat == true)
            {
                List<string> pathPesme = new List<string>();
                path = od.FileNames;
                for (int i = 0; i < path.Length; i++)
                {
                    listaZaLbx.Add(path[i]);
                    m.CreateControl();
                    media = this.m.newMedia(path[i]);
                    //Uri uri = new Uri(path[i]);
                    //mp.Open(uri);
                    string ime = System.IO.Path.GetFileName(path[i]);
                    string duzina = media.durationString;
                    Pesma pesma = new Pesma(ime, path[i], duzina);
                    //sw.WriteLine($"{pesma.Path}\n");
                    listaKlasePesma.Add(pesma);
                }
                foreach (var pesmaA in listaKlasePesma)
                {
                    pathPesme.Add(pesmaA.Path);
                }
                List<string> pathPesmeD = pathPesme.Distinct().ToList();
                foreach (var pPesme in pathPesmeD)
                {
                    sw.WriteLine(pPesme);
                }
                sw.Close();
            }
        }
        private void DodajPesmeDugme_Click(object sender, RoutedEventArgs e)
        {
            traziLokaciju();
            foreach (var pesma in listaKlasePesma)
            {
                lbxListaPesama.Items.Add(pesma.ToString()); //listbox
            }
            distinctListaPesama = listaKlasePesma.Distinct().ToList();
            listaKlasePesma.Clear();
        }
        private void ZatvoriDugme_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void FullScreenDugme_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else this.WindowState = WindowState.Maximized;
        }
        private void MinimizirajDugme_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        public string iseciString(string s, char c)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    break;
                }
                sb.Append(s[i]);
            }
            return sb.ToString();
        }
        #endregion

        #region Funkcije nad pesmama
        bool shuffle = false, loop = false;
        void pustiShuffle() {
            loop = false;
            try
            {
                Random rnd = new Random();
                int a = rnd.Next(0, lbxListaPesama.Items.Count + 1);
                lbxListaPesama.SelectedIndex = a;
                string aa = iseciString(lbxListaPesama.Items[a].ToString(), ':');
                foreach (var pesma in distinctListaPesama)
                {
                    if (pesma.ImePesme == aa)
                    {
                        m.URL = pesma.Path;
                        m.PlayStateChange += m_PlayStateChange;
                        m.Ctlcontrols.play();
                        jacinaZvukaSlider.Value = m.settings.volume;
                        premotajSlider.Value = 0;
                        //premotajSlider.Maximum = Convert.ToInt32(iseciString(iseciString(aa, ':'), ':')) * 60;
                        premotajSlider.Maximum = duzinaPesme();
                        if (premotajSlider.Maximum == 0)
                        {
                            MessageBox.Show("Ova pesma se nazalost ne moze premotati", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        private void m_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (m.playState==WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                m.Ctlcontrols.currentPosition = 0;
                m.Ctlcontrols.play();
            }
        }
        private void DugmePusti_Click(object sender, RoutedEventArgs e)
        {
                try
                {
                    if (m.playState == WMPLib.WMPPlayState.wmppsPaused)
                    {
                        m.Ctlcontrols.play();
                    }
                    else
                    {
                        try
                        {
                            foreach (var pesma in distinctListaPesama)/**/
                            {
                                string aa = iseciString(lbxListaPesama.Items[lbxListaPesama.SelectedIndex].ToString(), ':');
                                if (pesma.ImePesme == aa)//(iseciString(listaPesama.SelectedIndex.ToString(), ':')))
                                {
                                m.URL = pesma.Path;
                                m.settings.autoStart = true;
                                m.settings.rate = brzinaReprodukcijeVrednost;
                                //m.PlayStateChange += m_PlayStateChange;
                                //premotajSlider.Maximum = (int)Math.Ceiling(Convert.ToDouble(iseciString(m.currentMedia.durationString, ':')));
                                jacinaZvukaSlider.Value = m.settings.volume;
                                //premotajSlider.Maximum = (int)Math.Ceiling(m.currentMedia.duration);
                                premotajSlider.Maximum = duzinaPesme();
                                if (premotajSlider.Maximum == 0)
                                {
                                    MessageBox.Show("Ova pesma se nazalost ne moze premotati", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                premotajSlider.Value = 0;
                                m.Ctlcontrols.play();
                            }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }   
        }
        int duzinaPesme()
        {
            string vreme = lbxListaPesama.Items[lbxListaPesama.SelectedIndex].ToString().Split('\t').Last();
            string vremeMinuti = vreme.Split(':').First();
            string vremeSekunde = vreme.Split(':').Last();
            int minuti = Convert.ToInt32(vremeMinuti);
            int sekunde = Convert.ToInt32(vremeSekunde);
            return minuti * 60 + sekunde;
        }
        private void DugmePauza_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m.Ctlcontrols.pause();
            }
            catch (Exception){ }

        }
        private void DugmePrethodno_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var pesma in distinctListaPesama)/**/
                {
                    string aa = iseciString(lbxListaPesama.Items[lbxListaPesama.SelectedIndex - 1].ToString(), ':');
                    if (pesma.ImePesme == aa)//(iseciString(listaPesama.SelectedIndex.ToString(), ':')))
                    {
                        m.URL = pesma.Path;
                        lbxListaPesama.SelectedIndex -= 1;
                        m.settings.autoStart = true;
                        m.settings.rate = brzinaReprodukcijeVrednost;
                        m.Ctlcontrols.play();
                        jacinaZvukaSlider.Value = m.settings.volume;
                        premotajSlider.Value = 0;
                        premotajSlider.Maximum = duzinaPesme();
                    }
                }
                //lbxListaPesama.SelectedIndex -= 1;
            }
            catch (Exception) { }
        }
        private void DugmeSledece_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle)
            {
                m.settings.rate = brzinaReprodukcijeVrednost;
                pustiShuffle();
            }
            else
            {
                try
                {
                    foreach (var pesma in distinctListaPesama)/**/
                    {
                        string aa = iseciString(lbxListaPesama.Items[lbxListaPesama.SelectedIndex + 1].ToString(), ':');
                        if (pesma.ImePesme == aa)//(iseciString(listaPesama.SelectedIndex.ToString(), ':')))
                        {
                            m.URL = pesma.Path;
                            lbxListaPesama.SelectedIndex += 1;
                            m.settings.autoStart = true;
                            m.settings.rate = brzinaReprodukcijeVrednost;
                            m.Ctlcontrols.play();
                            jacinaZvukaSlider.Value = m.settings.volume;
                            premotajSlider.Value = 0;
                            premotajSlider.Maximum = duzinaPesme();
                        }
                    }
                   
                }
                catch (Exception) { }
            } 
        }
        private void ObrisiPesmeDugme_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                m.Ctlcontrols.stop();
                tempListaKlasePesma.RemoveAt(lbxListaPesama.SelectedIndex);
                lbxListaPesama.Items.RemoveAt(lbxListaPesama.SelectedIndex);
                FileStream fsOverwrite = new FileStream($"{dir}\\pesme.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fsOverwrite);
                foreach (var pesma in tempListaKlasePesma)
                {
                    sw.WriteLine(pesma.Path);
                }
                sw.Close();
            }
            catch (Exception) {  }
            try
            {
                lbxListaPesama.SelectedIndex += 1;
            }
            catch (Exception)
            {
                lbxListaPesama.SelectedIndex = -1;
            }   
        }
        private void PremotajSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                m.Ctlcontrols.currentPosition = premotajSlider.Value;
            }
            catch (Exception){ }
        }
        private void JacinaZvukaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                jacinaVrednost.Visibility = Visibility.Visible;
                double a = jacinaZvukaSlider.Value;
                jacinaVrednost.Text = $"{Convert.ToInt32(a)}";
                m.settings.volume = Convert.ToInt32(a);
            }
            catch (Exception){ }
        }
        private void ShuffleDugme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m.Ctlcontrols.stop();
                shuffle = !shuffle;
                if (shuffle)
                {
                    shuffleDugme.Content = "Shuffle - Upaljen";
                    pustiShuffle();
                }
                else shuffleDugme.Content = "Shuffle - Ugasen";
            }
            catch (Exception) { }
        }
        private void LoopDugme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loop = !loop;
                if (loop)
                {
                    shuffle = false;
                    shuffleDugme.Content = "Shuffle - Ugasen";
                    m.settings.autoStart = true;
                    m.settings.setMode("loop", true);
                    loopDugme.Content = "Loop - Upaljen";
                }
                else
                {
                    m.settings.autoStart = false;
                    m.settings.setMode("loop", false);
                    loopDugme.Content = "Loop - Ugasen";
                }
            }
            catch (Exception) { }
        }
        private double brzinaReprodukcijeVrednost = 1;
        private void BrzinaReprodukcijeDugme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (brzinaReprodukcijeVrednost != 2.0)
                {
                    brzinaReprodukcijeVrednost += 0.5;
                    m.Ctlcontrols.pause();
                    m.settings.rate = brzinaReprodukcijeVrednost;
                    m.Ctlcontrols.play();
                    if (m.settings.rate % 1 == 0)
                    {
                        brzinaReprodukcijeDugme.Content = $"Brzina reprodukcije - x{(double)brzinaReprodukcijeVrednost}.0";
                    }
                    else brzinaReprodukcijeDugme.Content = $"Brzina reprodukcije - x{(double)brzinaReprodukcijeVrednost}";
                }
                else
                {
                    brzinaReprodukcijeVrednost = 0.5;
                    m.Ctlcontrols.pause();
                    m.settings.rate = brzinaReprodukcijeVrednost;
                    m.Ctlcontrols.play();
                    brzinaReprodukcijeDugme.Content = $"Brzina reprodukcije - x{(double)brzinaReprodukcijeVrednost}";
                }
            }
            catch (Exception) { }
        }
        private void PretraziNaNetu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imePesmeZaPretrazivanje = distinctListaPesama[lbxListaPesama.SelectedIndex].ImePesme.Split('.').First();
                System.Diagnostics.Process.Start("http://www.google.com/search?q=" + Uri.EscapeDataString(imePesmeZaPretrazivanje));
            }
            catch (Exception){}
        }
        #endregion

        #region Pomeranje aplikacije
        int posX;
        int posY;
        private void Kanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var relativnaPozicija = e.GetPosition(this);
            var tacka = PointToScreen(relativnaPozicija);
            
                pomeranje = true;
                posX = Convert.ToInt32(tacka.X - this.Left);
                posY = Convert.ToInt32(tacka.Y - this.Top);   
        }
        private void Kanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            pomeranje = false;
        }
        private void Kanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (pomeranje)
            {
                var relativnaPozicija = e.GetPosition(this);
                var tacka = PointToScreen(relativnaPozicija);
                this.Top = System.Windows.Forms.Cursor.Position.Y - posY;
                this.Left = System.Windows.Forms.Cursor.Position.X - posX;  
            }   
        }
        private void Kanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            pomeranje = false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
