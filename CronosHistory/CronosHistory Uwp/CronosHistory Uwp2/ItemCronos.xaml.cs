using Gabriel.Cat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Gabriel.Cat.Extension;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using CronosHistory_Uwp;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CronosHistory_UWP
{
    public class ItemCronosOpenHistoriArgs : EventArgs
    {
        public itemCronos Historial { get; private set; }
        public ItemCronosOpenHistoriArgs(itemCronos historial)
        { Historial = historial; }
    }
    public sealed partial class ItemCronos : UserControl
    {
         readonly Image imgHistorial = new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/menuLineal.png")) };
         readonly Image[] imgsEliminar = new Image[] {
             new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/BasuraOff.png")) },
            new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/BasuraOn.png")) } };
        itemCronos itemCronos;
        bool estaVisibleElHistorial;
        public event EventHandler<ItemCronosOpenHistoriArgs> OpenHistory;
        public ItemCronos():this(new itemCronos())
        {

        }
        public ItemCronos(itemCronos itemCronos )
        {
            InitializeComponent();
            this.itemCronos = itemCronos;
            this.itemCronos.ItemParent = this;
            txtNombreElemento.TextChanged += (s, e) => { itemCronos.Descripcion = txtNombreElemento.Text; };
            swbtnTime.Label.TextAlignment = TextAlignment.Center;
            swbtnTime.IndexChanged += (s, e) => { EstaEncendido = !EstaEncendido; };
            HistorialVisible = true;
            swbtnTime.Brushes.AfegirMolts(new Brush[] {Windows.UI.Colors.Salmon.ToBrush(),Windows.UI.Colors.LightGreen.ToBrush() });
            txtNombreElemento.Text = itemCronos.Descripcion;
            swbtnTime.Label.Text = itemCronos.TiempoTotal.ToHoursMinutesSeconds();
            txtNombreElemento.TextWrapping = TextWrapping.Wrap;
            
        }
        public Windows.UI.Color Backcolor
        {
            set { grControl.Background = value.ToBrush(); }
        }
        public itemCronos Item
        {
            get { return itemCronos; }
        }
        public bool PendienteDeEliminar
        {
            get
            {
                return !HistorialVisible && btnHistoryOrDelete.Index > 0;
            }
        }
        public bool HistorialVisible
        {

            get
            {
                return estaVisibleElHistorial;
            }
            set
            {
                btnHistoryOrDelete.Images.Buida();
                if (value)
                {

                    btnHistoryOrDelete.Images.Afegir(imgHistorial);

                }
                else
                {
                    btnHistoryOrDelete.Images.AfegirMolts(imgsEliminar);
                }
                estaVisibleElHistorial = value;
                btnHistoryOrDelete.Index = btnHistoryOrDelete.Index;
            }
        }
        public bool EstaEncendido
        {
            get { return itemCronos.EstaOn; }
            set
            {

                itemCronos.EstaOn = value;
                swbtnTime.Text = itemCronos.TiempoTotal.ToHoursMinutesSeconds();
            }
        }

        public static ItemCronos[] ToControl(itemCronos[] itemsCargados)
        {
            ItemCronos[] items = new ItemCronos[itemsCargados.Length];
            for (int i = 0; i < itemsCargados.Length; i++)
                items[i] = new ItemCronos(itemsCargados[i]);
            return items;
        }

        public TimeSpan Tiempo { set {

                swbtnTime.Text = value.ToHoursMinutesSeconds();
            } }

       


        private void btnHistoryOrDelete_ChangeIndex(object sender, Gabriel.Cat.Uwp.ToggleButtonArgs e)
        {
            //cuando hacen clic
            if (HistorialVisible&&OpenHistory!=null)
            {

                //abro el historial
                OpenHistory(this, new ItemCronosOpenHistoriArgs(itemCronos));
                //ya no volverá...será otro nuevo asi que no vale la pena...
            }

        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            EstaEncendido = !EstaEncendido;
        }


    }
    public class ItemCronosEventArgs : EventArgs
    {
        ItemCronos item;

        public ItemCronosEventArgs(ItemCronos item)
        {
            Item = item;
        }

        public ItemCronos Item
        {
            get
            {
                return item;
            }

            private set
            {
                item = value;
            }
        }
    }
}
