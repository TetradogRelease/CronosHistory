using Gabriel.Cat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Gabriel.Cat.Extension;
namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for ItemCronos.xaml
    /// </summary>
    public partial class ItemCronos : UserControl
    {
        const string HISTORIAL = "Historial", ELIMINAR = "Eliminar",ON="On",OFF="Off";
        HistoryTime lstHistorialTiempos;
        DateTime inicio;
        TimeSpan totalTiempo;
        Thread hiloCambiaHora;
        public event EventHandler<ItemCronosEventArgs> Eliminado;
        public ItemCronos()
        {
            
            lstHistorialTiempos = new HistoryTime();
            InitializeComponent();
        }
        public ItemCronos(XmlNode nodoItemCronos):this()
        {
            txtNombreElemento.TextWithFormat = nodoItemCronos.FirstChild.FirstChild.InnerText.DescaparCaracteresXML();
            lstHistorialTiempos = new HistoryTime(nodoItemCronos);
            txbTiempo.Text = lstHistorialTiempos.TotalTime.ToString();
        }
        public bool HistorialVisible {

            get
            {
                return btnHistoryOrDelete.Content.ToString()==HISTORIAL;
            }
            set
            {
                if(value)
                {
                    btnHistoryOrDelete.Content = HISTORIAL;
                }
                else
                {
                    btnHistoryOrDelete.Content = ELIMINAR;
                }
            }
        }
        public bool EstaEncendido
        {
            get { return btnOnOff.Content.ToString()==OFF; }
            set
            {
                Action act;
                if(value)
                {
                    //si no esta on lo enciendo
                    if(!EstaEncendido)
                    {
                        //lo enciendo
                        btnOnOff.Content = OFF;
                        inicio = DateTime.Now;
                        totalTiempo = lstHistorialTiempos.TotalTime;
                        hiloCambiaHora = new Thread(()=> QueCorraElTiempo());
                        hiloCambiaHora.Start();
                        act = () => { Background = Brushes.Green; txtNombreElemento.Background = Background; };
                        Dispatcher.BeginInvoke(act);
                    }
                }else
                {
                    if(EstaEncendido)
                    {
                        //lo apago
                        btnOnOff.Content = ON;
                        lstHistorialTiempos.Add(new ItemHistorialTime(lstHistorialTiempos, inicio));
                        hiloCambiaHora.Abort();
                        txbTiempo.Text = lstHistorialTiempos.TotalTime.ToString();
                        act = () => { Background = Brushes.White; txtNombreElemento.Background = Background; };
                        Dispatcher.BeginInvoke(act);
                    }
                }
            }
        }

        private void QueCorraElTiempo()
        {
            Action act = () => {
                txbTiempo.Text = (totalTiempo+(DateTime.Now - inicio)).ToString();
            };
            while (true)
            {
                Thread.Sleep(1000);//me espero un segundo
                Dispatcher.BeginInvoke(act);
            }
        }
        public XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string nodos= lstHistorialTiempos.ToNodoXml().InnerXml;
            string nodoItemCronos = "<ItemCronos><DescripcionItem>" + txtNombreElemento.TextWithFormat.EscaparCaracteresXML() + "</DescripcionItem>";
            nodoItemCronos += nodos+"</ItemCronos>";
            xmlDoc.LoadXml(nodoItemCronos);
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }

        private void btnHistoryOrDelete_Click(object sender, RoutedEventArgs e)
        {
            if(HistorialVisible)
            {
                //abro el historial
                lstHistorialTiempos.ShowDialog();
                txbTiempo.Text = lstHistorialTiempos.TotalTime.ToString();
            }
            else if(Eliminado!=null)
            {
                Eliminado(this, new ItemCronosEventArgs(this));
            }
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            EstaEncendido = !EstaEncendido;
        }
        public static ItemCronos[] LoadItemsFromXml(XmlDocument xml)
        {
            List<ItemCronos> items = new List<ItemCronos>();
            ItemCronos item;
            for (int i = 0; i < xml.FirstChild.ChildNodes.Count; i++)
            {
                item = new ItemCronos(xml.FirstChild.ChildNodes[i]);
            
                items.Add(item);
            }
            return items.ToArray();
        }
        public static XmlDocument ToXml(ItemCronos[] items)
        {
            XmlDocument xmlDoc = new XmlDocument();
            text doc = "<CronosHistory>";
            for (int i = 0; i < items.Length; i++)
                doc &= items[i].ToNodoXml().OuterXml;
            doc &= "</CronosHistory>";
            xmlDoc.InnerXml = doc;
            xmlDoc.Normalize();
            return xmlDoc;
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
