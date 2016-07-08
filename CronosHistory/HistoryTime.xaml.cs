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
                };
                Dispatcher.BeginInvoke(act);
            };
            stkHistorial.Children.Add(item);
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
    }
}
