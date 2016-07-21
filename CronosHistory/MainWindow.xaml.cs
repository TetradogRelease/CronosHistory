using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Gabriel.Cat.Extension;
namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string NOMBREARCHIVO = "CronosHistory.xml";
        List<ItemCronos> items;

        public static bool IsClosing { get; private set; }

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            IsClosing = false;
            items = new List<ItemCronos>();
            InitializeComponent();
            LoadXml();
            Closing += SaveXml;
            btnAñadir.ImagenesButton.Afegir(Imagenes.CronosPlus.ToImage());
           
            btnQuitarOOK.ImagenesButton.Afegir(Imagenes.CronosMinus.ToImage());
            btnQuitarOOK.ImagenesButton.Afegir(Imagenes.CronosOK.ToImage());
            btnQuitarOOK.Index = 0;
            imgBarra2.SetImage(Imagenes.barra);
            imgReloj.SetImage(Imagenes.reloj);
        }

        private void SaveXml(object sender, EventArgs e)
        {
            IsClosing = true;
            //paro los que esten encendidos
            for (int i = 0; i < items.Count; i++)
                if(items[i].EstaEncendido)
                  items[i].EstaEncendido = false;
            //genero el xml
            if (items.Count != 0)
                ItemCronos.ToXml(items.ToArray()).Save(NOMBREARCHIVO);
            else if(File.Exists(NOMBREARCHIVO))
                File.Delete(NOMBREARCHIVO);
        }

        private void LoadXml()
        {
            System.Xml.XmlDocument xml;
            ItemCronos[] itemsCargados;
            if (File.Exists(NOMBREARCHIVO))
            {
                xml = new System.Xml.XmlDocument();
                xml.Load(NOMBREARCHIVO);
                itemsCargados = ItemCronos.LoadItemsFromXml(xml);
                stkTiempos.Children.AddRange(itemsCargados);
                items.AddRange(itemsCargados);
                ActualizaBackGroundItems();
            }
        }

        private void Eliminado(object sender, ItemCronosEventArgs e)
        {
                Action act = () =>
                {
                    items.Remove(e.Item);
                    stkTiempos.Children.Remove(e.Item);

                };
                Dispatcher.BeginInvoke(act);
          
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkTiempos.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemCronos)stkTiempos.Children[i]).Background = Brushes.LightBlue;
                else ((ItemCronos)stkTiempos.Children[i]).Background = Brushes.White;
        }

        private void btnAñadir_ChangeIndex(object sender, Gabriel.Cat.Wpf.ToggleButtonArgs e)
        {
            ItemCronos newItem = new ItemCronos();
            items.Add( newItem);
            stkTiempos.Children.Add(newItem);
            ActualizaBackGroundItems();
        }

        private void btnQuitarOOK_ChangeIndex(object sender, Gabriel.Cat.Wpf.ToggleButtonArgs e)
        {
            List<ItemCronos> itemsPerTreure;
            ItemCronos item;
            if (btnQuitarOOK.Index == 0&&HayParaQuitar() && MessageBox.Show("Estas seguro que quieres eliminarlos ?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
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

            for (int i = 0; i < items.Count; i++)
                items[i].HistorialVisible = btnQuitarOOK.Index == 0;
            btnAñadir.Visibility = btnQuitarOOK.Index == 0 ? Visibility.Visible : Visibility.Hidden;
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
