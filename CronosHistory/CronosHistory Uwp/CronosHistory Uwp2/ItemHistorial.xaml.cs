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
        itemHistory itemHistory;
        Historial parent;
        public event EventHandler Eliminar;
        public ItemHistorial(Historial parent,itemHistory itemHistory)
        {
           
            this.parent = parent;
            InitializeComponent();
            this.ItemHistory = itemHistory;

        }
        public itemHistory ItemHistory
        {
            get { return itemHistory; }
            set
            {
                itemHistory = value;
                txtDescripcion.Text = itemHistory.Contenido;
                txtFechaInicio.Text = itemHistory.Inicio.ToString();
                txtTiempoHecho.Text = itemHistory.Tiempo.ToHoursMinutesSeconds();
            }
        }
        public DateTime Inicio
        {
            get { return itemHistory.Inicio; }
            set
            {
                itemHistory.Inicio = value;
                txtFechaInicio.Text = Inicio.ToString().Replace(" ", "\n");
            }
        }
        public TimeSpan Tiempo
        {
            get { return itemHistory.Tiempo; }
            set
            {
                itemHistory.Tiempo = value;
                txtTiempoHecho.Text = Tiempo.ToHoursMinutesSeconds();
            }
        }
        public string Description
        {
            get { return itemHistory.Contenido; }
            set
            {
                itemHistory.Contenido = value;
                txtDescripcion.Text = itemHistory.Contenido;
            }
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

        private void txtDescripcion_TextChanged(object sender, TextChangedEventArgs e)
        {
            itemHistory.Contenido = txtDescripcion.Text;
        }
    }
}
