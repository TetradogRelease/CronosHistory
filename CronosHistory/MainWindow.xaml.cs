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
        const string ACABADO = "Acabado", QUITAR = "Quitar";
        const string NOMBREARCHIVO = "CronosHistory.xml";
        List<ItemCronos> items;
        public MainWindow()
        {
            items = new List<ItemCronos>();
            InitializeComponent();
            LoadXml();
            Closing += SaveXml;
        }

        private void SaveXml(object sender, EventArgs e)
        {
            //paro los que esten encendidos
            for (int i = 0; i < items.Count; i++)
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

                for (int i = 0; i < itemsCargados.Length; i++)
                {
                    items.Add(itemsCargados[i]);
                    items[i].Eliminado += Eliminado;
                }
                stkTiempos.Children.AddRange(items);
            }
        }

        private void btnAñadir_Click(object sender, RoutedEventArgs e)
        {
            ItemCronos newItem = new ItemCronos();
            items.Insert(0,newItem);
            stkTiempos.Children.Insert(0, newItem);
            newItem.Eliminado += Eliminado;
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

        private void btnQuitarOOK_Click(object sender, RoutedEventArgs e)
        {

            if (btnAñadir.IsVisible)
            {
                //activo el modo quitar
                btnQuitarOOK.Content = ACABADO;
                btnAñadir.Visibility = Visibility.Hidden;
            }else
            {
                //desactivo el modo quitar
                btnQuitarOOK.Content = QUITAR;
                btnAñadir.Visibility = Visibility.Visible;
            }
            for (int i = 0; i < items.Count; i++)
                items[i].HistorialVisible = btnAñadir.IsVisible;

        }
    }
}
