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
using System.IO;

namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for ItemCronos.xaml
    /// </summary>
    public partial class ItemCronos : UserControl
    {

        static readonly Image imgHistorial = Imagenes.menuLineal.ToImage();
        static readonly Image[] imgsEliminar =new Image[] { Imagenes.BasuraOff.ToImage(),Imagenes.BasuraOn.ToImage() };
        HistoryTime lstHistorialTiempos;
        DateTime inicio;
        TimeSpan totalTiempo;
        Thread hiloCambiaHora;
        bool estaVisibleElHistorial;

        public ItemCronos()
        {
            InitializeComponent();
            swbtnTime.Label.TextAlignment = TextAlignment.Center;
            swbtnTime.Changed += (s, e) => { EstaEncendido = !EstaEncendido; };
            HistorialVisible = true;
            lstHistorialTiempos = new HistoryTime();
            swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds();       
        }
        public ItemCronos(XmlNode nodoItemCronos):this()
        {
            txtNombreElemento.TextWithFormat = nodoItemCronos.FirstChild.FirstChild.InnerText.DescaparCaracteresXML();
            lstHistorialTiempos = new HistoryTime(nodoItemCronos);
            swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds();
        }
        public bool PendienteDeEliminar
        {
            get
            {
                return !HistorialVisible&&btnHistoryOrDelete.Index>0;
            }
        }
        public bool HistorialVisible {

            get
            {
                return estaVisibleElHistorial;
            }
            set
            {
                btnHistoryOrDelete.ImagenesButton.Buida();
                if(value)
                {
                        
                  btnHistoryOrDelete.ImagenesButton.Afegir(imgHistorial);
                    
                }
                else
                {
                    btnHistoryOrDelete.ImagenesButton.AfegirMolts(imgsEliminar);
                }
                estaVisibleElHistorial = value;
                btnHistoryOrDelete.Index = btnHistoryOrDelete.Index;
            }
        }
        public bool EstaEncendido
        {
            get { return swbtnTime.EstaOn; }
            set
            {

                if(value)
                {
                    //si no esta on lo enciendo
                    if(!EstaEncendido)
                    {
                        //lo enciendo
                        inicio = DateTime.Now;
                        totalTiempo = lstHistorialTiempos.TotalTime;
                        hiloCambiaHora = new Thread(()=> QueCorraElTiempo());
                        hiloCambiaHora.Start();

                    }
                }else
                {
                    if(EstaEncendido)
                    {
                        //lo apago
                        lstHistorialTiempos.Add(new ItemHistorialTime(lstHistorialTiempos, inicio));
                        if(hiloCambiaHora!=null&&hiloCambiaHora.IsAlive)
                        hiloCambiaHora.Abort();
                        swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds(); 

                    }
                }
            }
        }

        private void QueCorraElTiempo()
        {
            Action act = () => {
               
                swbtnTime.Label.Text = (totalTiempo + (DateTime.Now - inicio)).ToHoursMinutesSeconds();
            };
            while (true)
            {
                Thread.Sleep(500);//me espero un segundo
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


        private void btnHistoryOrDelete_ChangeIndex(object sender, Gabriel.Cat.Wpf.ToggleButtonArgs e)
        {
            //cuando hacen clic
            if (HistorialVisible)
            {
                //abro el historial
                lstHistorialTiempos.ShowDialog();
                totalTiempo = lstHistorialTiempos.TotalTime;
                swbtnTime.Label.Text = totalTiempo.ToHoursMinutesSeconds();
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
