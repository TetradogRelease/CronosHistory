using Gabriel.Cat;
using Gabriel.Cat.Extension;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CronosHistory_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Historial : Page
    {
        public Historial()
        {
            this.InitializeComponent();
        }
        public Historial(XmlNode nodoItemCronos):this()
        {
            ItemHistorial.AddItemsXml(this, nodoItemCronos);
        }
        public TimeSpan TotalTime
        {
            get
            {
                TimeSpan time = new TimeSpan();
                for (int i = 0; i < stkHistorial.Children.Count; i++)
                    time += (stkHistorial.Children[i] as ItemHistorial).Tiempo;
                return time;
            }
        }

        public bool SaltarConfirmacion {
            get { return ckSaltarConfirmacion.IsChecked != null && ckSaltarConfirmacion.IsChecked.Value; }
        }

        public void Add(ItemHistorial item)
        {
            item.Eliminar += (s, e) =>
            {
                Action act = () =>
                {
                    stkHistorial.Children.Remove(s as ItemHistorial);
                    ActualizaBackGroundItems();
                };
             //   Dispatcher.BeginInvoke(act);
            };
            stkHistorial.Children.Add(item);
            ActualizaBackGroundItems();
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemHistorial)stkHistorial.Children[i]).Background = Windows.UI.Colors.LightBlue.ToBrush();
                else ((ItemHistorial)stkHistorial.Children[i]).Background = Windows.UI.Colors.White.ToBrush();
        }

        public XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            text nodo = "<HistoryCronos>";
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                nodo &= (stkHistorial.Children[i] as ItemHistorial).ToXmlNode().OuterXml;
            nodo &= "</HistoryCronos>";
            xmlDoc.InnerXml = nodo;
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }


        private void btnAñadirCustom_Click(object sender, RoutedEventArgs e)
        {
            //hacen clic
            winAñadirItemManual winPostItem = new winAñadirItemManual();
            //lo abro
            if (winPostItem.GuardarItem)
            {
                try
                {
                    Add(new ItemHistorial(this, winPostItem.Fecha, winPostItem.Tiempo));

                }
                catch
                {
                   
                    MessageBox.Show("No se ha añadido nada", "Atención", MessageBox.MessageButtons.Ok);
                }
            }
        }
    }
}
