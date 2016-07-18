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
        static bool EstaCargado = false;
        readonly string nombreArchivo ="CronosHistory.xml";
        List<itemCronos> items;
        List<ItemCronos> itemsControls;
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        public MainPage()
        {
           
            Image imgCronosPlus = new Image(), imgCronosMinus = new Image(), imgCronosOK = new Image();
            items = new List<itemCronos>();
            itemsControls = new List<ItemCronos>();
            InitializeComponent();
            MainBaseUri = this.BaseUri;
            imgCronosPlus.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/Cronos+.png"));
            imgCronosMinus.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/Cronos-.png"));
            imgCronosOK.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/CronosOK.png"));
            btnAñadir.Images.Afegir(imgCronosPlus);
            btnQuitarOOK.Images.Afegir(imgCronosMinus);
            btnQuitarOOK.Images.Afegir(imgCronosOK);
            btnQuitarOOK.Index = 0;
            imgBarra.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/barra.jpg"));
            imgReloj.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/cronosReloj.jpg"));
            Application.Current.Suspending +=  (s, e) => {   SaveXml(); };
            Application.Current.Resuming += (s, e) => {  LoadXml(); };
            if(!EstaCargado)
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
                if (items[i].EstaOn)
                    items[i].EstaOn = false;
            //genero el xml
            if (items.Count != 0)
               itemCronos.ToXml(items.ToArray()).Save(folder,nombreArchivo);
            else if (folder.Exist(nombreArchivo))
                folder.DeleteFile(nombreArchivo);
        }

        private  void LoadXml()
        {
            System.Xml.XmlDocument xml;
            itemCronos[] itemsCargados;
            if (folder.Exist(nombreArchivo))
            {
                try
                {
                    xml = new System.Xml.XmlDocument();
                    xml.Load(folder, nombreArchivo);
                    itemsCargados = itemCronos.LoadXml(xml);
                    itemsControls.AddRange(ItemCronos.ToControl(itemsCargados));
                    stkTiempos.Children.AddRange(itemsControls);
                    items.AddRange(itemsCargados);
                    ActualizaBackGroundItems();
                    for (int i = 0; i < itemsCargados.Length; i++)
                        itemsControls[i].OpenHistory += OpenHistoryEvent;
                }
                catch (Exception e){

                }
                finally
                {
                    EstaCargado = true;
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<itemCronos> todosLosItems =e.Parameter as List<itemCronos>;
            ItemCronos item;
            base.OnNavigatedTo(e);
            if (todosLosItems != null)
            {
                items = todosLosItems;//restauro los items que tenia
                for (int i = 0; i < todosLosItems.Count; i++)
                {
                   
                    item = new ItemCronos(todosLosItems[i]);
                    itemsControls.Add(item);
                    stkTiempos.Children.Add(item);
                    item.OpenHistory += OpenHistoryEvent;
                }
            }

        }


        private void OpenHistoryEvent(object sender, ItemCronosOpenHistoriArgs e)
        {

            Frame.Navigate(typeof(Historial),new Object[] { e.Historial, items});
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkTiempos.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemCronos)stkTiempos.Children[i]).Backcolor = Windows.UI.Colors.LightBlue;
                else ((ItemCronos)stkTiempos.Children[i]).Backcolor = Windows.UI.Colors.White;
        }

        private void btnAñadir_ChangeIndex(object sender, Gabriel.Cat.Uwp.ToggleButtonArgs e)
        {
            ItemCronos newItem = new ItemCronos();
            newItem.OpenHistory += OpenHistoryEvent;
            items.Insert(0,newItem.Item);
            itemsControls.Insert(0, newItem);
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
                        {
                            itemsPerTreure.Add(item);
                            items.Remove(item.Item);
                        }

                        }
                        itemsControls.RemoveRange(itemsPerTreure);
                        stkTiempos.Children.RemoveRange(itemsPerTreure);
                        ActualizaBackGroundItems();
                    }
                }
                for (int i = 0; i < items.Count; i++)
                    itemsControls[i].HistorialVisible = btnQuitarOOK.Index == 0;
                btnAñadir.Visibility = btnQuitarOOK.Index == 0 ? Visibility.Visible : Visibility.Collapsed;

        }

        private bool HayParaQuitar()
        {

            bool hay = false;
            itemsControls.WhileEach((item) =>
            {
                hay = item.PendienteDeEliminar;
                return !hay;
            });
            return hay;
        }
    }
}

