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
    public sealed partial class ItemCronos : UserControl
    {
         readonly Image imgHistorial = new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/menuLineal.png")) };
         readonly Image[] imgsEliminar = new Image[] {
             new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/BasuraOff.png")) },
            new Image() { Source = new BitmapImage(new Uri(MainPage.MainBaseUri,"/Assets/BasuraOn.png")) } };
        Historial lstHistorialTiempos;
        DateTime inicio;
        TimeSpan totalTiempo;
        Task hiloCambiaHora;
        bool estaOn;
        bool estaVisibleElHistorial;

        public ItemCronos()
        {
            estaOn = false;
            InitializeComponent();
            swbtnTime.Label.TextAlignment = TextAlignment.Center;
            swbtnTime.IndexChanged += (s, e) => { EstaEncendido = !EstaEncendido; };
            HistorialVisible = true;
            lstHistorialTiempos = new Historial();
            swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds();
        }
        public ItemCronos(XmlNode nodoItemCronos) : this()
        {
            txtNombreElemento.Text= nodoItemCronos.FirstChild.FirstChild.InnerText.DescaparCaracteresXML();
            lstHistorialTiempos = new Historial(nodoItemCronos.LastChild);
            swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds();
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
            get { return DateTime.MinValue.Ticks != inicio.Ticks; }
            set
            {

                if (value)
                {
                    //si no esta on lo enciendo
                    if (!EstaEncendido)
                    {
                        //lo enciendo
                        inicio = DateTime.Now;
                        totalTiempo = lstHistorialTiempos.TotalTime;
                        hiloCambiaHora = new Task(() => QueCorraElTiempo());
                        hiloCambiaHora.Start();

                    }
                }
                else
                {
                    if (EstaEncendido)
                    {
                        //lo apago
                        lstHistorialTiempos.Add(new ItemHistorial(lstHistorialTiempos, inicio));
                        if (hiloCambiaHora != null && hiloCambiaHora.Status == TaskStatus.Running)
                            estaOn = false;
                        swbtnTime.Label.Text = lstHistorialTiempos.TotalTime.ToHoursMinutesSeconds();
                        inicio = DateTime.MinValue;
                    }
                }
            }
        }

        private  void QueCorraElTiempo()
        {
            Action act = () => { swbtnTime.Label.Text = (totalTiempo + (DateTime.Now - inicio)).ToHoursMinutesSeconds(); };
            estaOn = true;
            while (estaOn)
            {
                
                Dispatcher.BeginInvoke(act);
                if(estaOn)
                    Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
            }
        }
        public XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string nodos = lstHistorialTiempos.ToNodoXml().OuterXml;
            string nodoItemCronos = "<ItemCronos><DescripcionItem>" + txtNombreElemento.Text.EscaparCaracteresXML() + "</DescripcionItem>";
            nodoItemCronos += nodos + "</ItemCronos>";
            xmlDoc.LoadXml(nodoItemCronos);
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }


        private void btnHistoryOrDelete_ChangeIndex(object sender, Gabriel.Cat.Uwp.ToggleButtonArgs e)
        {
            //cuando hacen clic
            if (HistorialVisible)
            {
                //abro el historial
             //   lstHistorialTiempos.ShowDialog();
                totalTiempo = lstHistorialTiempos.TotalTime;
                swbtnTime.Text = totalTiempo.ToHoursMinutesSeconds();
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
