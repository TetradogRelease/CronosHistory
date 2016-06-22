using Gabriel.Cat;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Gabriel.Cat.Extension;
namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for ItemHistorialTime.xaml
    /// </summary>
    public partial class ItemHistorialTime : UserControl
    {
        enum XmlCampos
        {
            Inicio,Descripcion,Tiempo
        }
        HistoryTime parent;
        public event EventHandler Eliminar;
        public ItemHistorialTime(HistoryTime parent)
        {
           
            this.parent = parent;
            InitializeComponent();
            txtTiempoHecho.MouseLeftButtonUp += EliminarEvent;
            txtFechaInicio.MouseLeftButtonUp += EliminarEvent;
        }
        public ItemHistorialTime(HistoryTime parent, DateTime inicio):this(parent)
        {
            Inicio = inicio;
            Tiempo = DateTime.Now-inicio;
        }
        public ItemHistorialTime(HistoryTime parent,XmlNode nodoData):this(parent)
        {
            Inicio = DateTime.Parse(nodoData.ChildNodes[(int)XmlCampos.Inicio].InnerText);
            Tiempo = TimeSpan.Parse(nodoData.ChildNodes[(int)XmlCampos.Tiempo].InnerText);
            RichDescription = nodoData.ChildNodes[(int)XmlCampos.Descripcion].InnerText.DescaparCaracteresXML();

        }

        private void EliminarEvent(object sender, MouseButtonEventArgs e)
        {
            if(Eliminar!=null)
            if (parent.ckSaltarConfirmacion.IsChecked.Value || MessageBox.Show("Estas seguro que quieres borrarlo?", "Se necesita su atención", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                    Eliminar(this, new EventArgs());
            }

        }

        public DateTime Inicio
        {
            get { return  DateTime.Parse(txtFechaInicio.Text); }
            set
            {
                txtFechaInicio.Text = value.ToShortDateString();
            }
        }
        public TimeSpan Tiempo
        {
            get { return TimeSpan.Parse(txtTiempoHecho.Text); }
            set
            {
                txtTiempoHecho.Text = value.ToString();
            }
        }
        public string RichDescription
        {
            get { return txtDescripcion.TextWithFormat; }
            set
            {
                try
                {
                    //si no tiene el formato hace una excepcion asi que lo pongo como texto plano
                    txtDescripcion.TextWithFormat = value;
                }catch
                {
                    txtDescripcion.Text = value;
                }
            }
        }
        public XmlNode ToXmlNode()
        {
            XmlDocument xmlDoc = new XmlDocument();
            text nodo = "<ItemHistory>";
            nodo &= (text)"<FechaInicio>" & txtFechaInicio.Text & "</FechaInicio>";
            nodo&= (text)"<Descripcion>" & RichDescription.EscaparCaracteresXML() & "</Descripcion>";
            nodo &= (text)"<Tiempo>" & txtTiempoHecho.Text & "</Tiempo>";
            nodo &= "</ItemHistory>";
            xmlDoc.InnerXml = nodo;
            return xmlDoc.FirstChild;
        }
        public static void AddItemsXml(HistoryTime parent,XmlNode nodoItemCronos)
        {
            if (parent == null || nodoItemCronos == null)
                throw new ArgumentNullException();

            for (int i = 1, f = nodoItemCronos.ChildNodes.Count; i < f; i++)//el nodo 0 es la descripcion del item :D
                try
                {
                    parent.Add(new ItemHistorialTime(parent,nodoItemCronos.ChildNodes[i]));
                }
                catch { }
            
        }
    }
}
