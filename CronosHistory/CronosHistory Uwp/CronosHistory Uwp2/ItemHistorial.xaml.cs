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
using Gabriel.Cat;
using Gabriel.Cat.Extension;
using Windows.UI.Popups;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CronosHistory_UWP
{
    public sealed partial class ItemHistorial : UserControl
    {
        enum XmlCampos
        {
            Inicio, Descripcion, Tiempo
        }
        Historial parent;
        DateTimeOffset inicio;
        TimeSpan tiempo;
        public event EventHandler Eliminar;
        public ItemHistorial(Historial parent)
        {

            this.parent = parent;
            InitializeComponent();

        }
        public ItemHistorial(Historial parent, DateTimeOffset inicio):this(parent)
        {
            Inicio = inicio;
            Tiempo = DateTime.Now - inicio;
        }
        public ItemHistorial(Historial parent, DateTimeOffset fecha, TimeSpan tiempo):this(parent)
        {
            Inicio = fecha;
            Tiempo = tiempo;
        }
        public ItemHistorial(Historial parent, XmlNode nodoData):this(parent)
        {
            Inicio = new DateTimeOffset(new DateTime(Convert.ToInt64(nodoData.ChildNodes[(int)XmlCampos.Inicio].InnerText)));
            Tiempo = new TimeSpan(Convert.ToInt64(nodoData.ChildNodes[(int)XmlCampos.Tiempo].InnerText));
            Description = nodoData.ChildNodes[(int)XmlCampos.Descripcion].InnerText.DescaparCaracteresXML();

        }


        public DateTimeOffset Inicio
        {
            get { return inicio; }
            set
            {
                inicio = value;
                txtFechaInicio.Text = inicio.ToString().Replace(" ", "\n");
            }
        }
        public TimeSpan Tiempo
        {
            get { return tiempo; }
            set
            {
                tiempo = value;
                txtTiempoHecho.Text = tiempo.ToString();
            }
        }
        public string Description
        {
            get { return txtDescripcion.Text; }
            set
            {

                    txtDescripcion.Text = value;
                
            }
        }
        public XmlNode ToXmlNode()
        {
            XmlDocument xmlDoc = new XmlDocument();
            text nodo = "<ItemHistory>";
            nodo &= (text)"<FechaInicio>" & Inicio.Ticks & "</FechaInicio>";
            nodo &= (text)"<Descripcion>" & Description.EscaparCaracteresXML() & "</Descripcion>";
            nodo &= (text)"<Tiempo>" & Tiempo.Ticks & "</Tiempo>";
            nodo &= "</ItemHistory>";
            xmlDoc.InnerXml = nodo;
            return xmlDoc.FirstChild;
        }
        public static void AddItemsXml(Historial parent, XmlNode nodoItemCronos)
        {
            if (parent == null || nodoItemCronos == null)
                throw new ArgumentNullException();

            for (int i = 0, f = nodoItemCronos.ChildNodes.Count; i < f; i++)
                try
                {
                    parent.Add(new ItemHistorial(parent, nodoItemCronos.ChildNodes[i]));
                }
                catch { }

        }

        private async void EliminarAlPresionar(object sender, PointerRoutedEventArgs e)
        {
            MessageBox.MessageResult result;


            if (Eliminar != null)
                if (parent.SaltarConfirmacion )
                {
                    Eliminar(this, new EventArgs());
                }
            else{
                result = await MessageBox.Show("Estas seguro que quieres borrarlo?", "Se necesita su atención", MessageBox.MessageButtons.YesNo);
                if(result == MessageBox.MessageResult.Yes)
                    {
                        Eliminar(this, new EventArgs());
                    }
            }
        }
    }
}
