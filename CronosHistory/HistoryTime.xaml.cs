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
using System.Windows.Shapes;
using System.Xml;
using Gabriel.Cat.Extension;
using Gabriel.Cat;

namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for HistoryTime.xaml
    /// </summary>
    public partial class HistoryTime : Window
    {
        public HistoryTime()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                e.Cancel = true;
                if(!MainWindow.IsClosing)
                   Hide();
            };
        }
        public HistoryTime(XmlNode nodoItemCronos):this()
        {
           ItemHistorialTime.AddItemsXml(this, nodoItemCronos);
        }
        public TimeSpan TotalTime
        {
            get
            {
                TimeSpan time = new TimeSpan();
                for (int i = 0; i < stkHistorial.Children.Count; i++)
                    time += (stkHistorial.Children[i] as ItemHistorialTime).Tiempo;
                return time;
            }
        }
        public void Add(ItemHistorialTime item)
        {
            item.Eliminar += (s,e) =>
            {
                Action act = () =>
                {
                    stkHistorial.Children.Remove(s as ItemHistorialTime);
                    ActualizaBackGroundItems();
                };
                Dispatcher.BeginInvoke(act);
            };
            stkHistorial.Children.Add(item);
            ActualizaBackGroundItems();
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemHistorialTime)stkHistorial.Children[i]).Background = Brushes.LightBlue;
                else ((ItemHistorialTime)stkHistorial.Children[i]).Background = Brushes.White;
        }

        public XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            text nodo = "<HistoryCronos>";
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                nodo &= (stkHistorial.Children[i] as ItemHistorialTime).ToXmlNode().OuterXml;
            nodo &= "</HistoryCronos>";
            xmlDoc.InnerXml = nodo;
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }


        private void btnAñadirCustom_Click(object sender, RoutedEventArgs e)
        {
            //hacen clic
            winPostItem winPostItem = new winPostItem();
            winPostItem.ShowDialog();
            if (winPostItem.GuardarItem)
            {
                try
                {
                    Add(new ItemHistorialTime(this, winPostItem.Fecha, winPostItem.Tiempo));

                }
                catch
                {
                    MessageBox.Show("No se ha añadido nada", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
