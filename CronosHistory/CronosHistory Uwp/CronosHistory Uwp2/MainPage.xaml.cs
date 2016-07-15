using CronosHistory_UWP;
using Gabriel.Cat;
using Gabriel.Cat.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CronosHistory_Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly string nombreArchivo ="CronosHistory.xml";
        List<ItemCronos> items;
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        public MainPage()
        {
           
            Image imgCronosPlus = new Image(), imgCronosMinus = new Image(), imgCronosOK = new Image();
            items = new List<ItemCronos>();
            InitializeComponent();
            
            MainBaseUri = this.BaseUri;
            imgCronosPlus.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets3/Cronos+.png"));
            imgCronosMinus.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets3/Cronos-.png"));
            imgCronosOK.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets3/CronosOK.png"));
            btnAñadir.Images.Afegir(imgCronosPlus);
            btnQuitarOOK.Images.Afegir(imgCronosMinus);
            btnQuitarOOK.Images.Afegir(imgCronosOK);
            btnQuitarOOK.Index = 0;
            imgBarra.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets3/barra.jpg"));
            imgReloj.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets3/cronosReloj2.jpg"));
            Application.Current.Suspending +=  (s, e) => {  SaveXml(); };
            Application.Current.Resuming += (s, e) => { LoadXml(); };
            LoadXml();
        }


        public static Uri MainBaseUri
        {
            get;
            private set;
        }

        private void SaveXml(object sender = null, EventArgs e = null)
        {
            //paro los que esten encendidos
            for (int i = 0; i < items.Count; i++)
                if (items[i].EstaEncendido)
                    items[i].EstaEncendido = false;
            //genero el xml
            if (items.Count != 0)
               ItemCronos.ToXml(items.ToArray()).Save(folder,nombreArchivo);
            else if (File.Exists(nombreArchivo))
                File.Delete(nombreArchivo.ToString());
        }

        private  void LoadXml()
        {
            System.Xml.XmlDocument xml;
            ItemCronos[] itemsCargados;
            if (File.Exists(folder.Path + Path.AltDirectorySeparatorChar + nombreArchivo))
            {
                try
                {
                    xml = new System.Xml.XmlDocument();
                    xml.Load(folder, nombreArchivo);
                    itemsCargados = ItemCronos.LoadItemsFromXml(xml);
                    stkTiempos.Children.AddRange(itemsCargados);
                    items.AddRange(itemsCargados);
                    ActualizaBackGroundItems();
                    for (int i = 0; i < itemsCargados.Length; i++)
                        itemsCargados[i].OpenHistory += OpenHistoryEvent;
                }
                catch (Exception e){

                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage main=e.Parameter as MainPage;
            base.OnNavigatedTo(e);
            if(main!=null)
            for(int i=0;i<main.items.Count;i++)
            {
                items.Add(main.items[i]);
                main.stkTiempos.Children.Remove(items[i]);
                stkTiempos.Children.Add(items[i]);
                items[i].OpenHistory += OpenHistoryEvent;
            }

        }


        private void OpenHistoryEvent(object sender, ItemCronosOpenHistoriArgs e)
        {
            for (int i = 0; i < items.Count; i++)//quito los eventos
                items[i].OpenHistory -= OpenHistoryEvent;

            Frame.Navigate(typeof(Historial),new Page[] { this, e.Historial });
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkTiempos.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemCronos)stkTiempos.Children[i]).Background = Windows.UI.Colors.LightBlue.ToBrush();
                else ((ItemCronos)stkTiempos.Children[i]).Background = Windows.UI.Colors.White.ToBrush();
        }

        private void btnAñadir_ChangeIndex(object sender, Gabriel.Cat.Uwp.ToggleButtonArgs e)
        {
            ItemCronos newItem = new ItemCronos();
            newItem.OpenHistory += OpenHistoryEvent;
            items.Insert(0, newItem);
            stkTiempos.Children.Insert(0, newItem);
            ActualizaBackGroundItems();
        }

        private async void btnQuitarOOK_ChangeIndex(object sender, Gabriel.Cat.Uwp.ToggleButtonArgs e)
        {
            List<ItemCronos> itemsPerTreure;
            ItemCronos item;
            bool esperar;
            MessageBox.MessageResult result;

            esperar = btnQuitarOOK.Index == 0 && HayParaQuitar();
                if (esperar)
                {
                    result = await MessageBox.Show("Estas seguro que quieres eliminarlos ?", "Atención", MessageBox.MessageButtons.YesNo);
                    if (result == MessageBox.MessageResult.Yes)
                    {
                        itemsPerTreure = new List<ItemCronos>();

                        for (int i = 0; i < stkTiempos.Children.Count; i++)
                        {
                            item = stkTiempos.Children[i] as ItemCronos;
                            if (item.PendienteDeEliminar)
                                itemsPerTreure.Add(item);

                        }
                        items.RemoveRange(itemsPerTreure);
                        stkTiempos.Children.RemoveRange(itemsPerTreure);
                        ActualizaBackGroundItems();
                    }
                }
                for (int i = 0; i < items.Count; i++)
                    items[i].HistorialVisible = btnQuitarOOK.Index == 0;
                btnAñadir.Visibility = btnQuitarOOK.Index == 0 ? Visibility.Visible : Visibility.Collapsed;

        }

        private bool HayParaQuitar()
        {

            bool hay = false;
            items.WhileEach((item) =>
            {
                hay = item.PendienteDeEliminar;
                return !hay;
            });
            return hay;
        }
    }
}

