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
        DateTime inicio;
        TimeSpan tiempo;
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
        public ItemHistorialTime(HistoryTime parent,DateTime fecha, TimeSpan tiempo):this(parent)
        {
            Inicio = fecha;
            Tiempo = tiempo;
        }
        public ItemHistorialTime(HistoryTime parent,XmlNode nodoData):this(parent)
        {
            Inicio = new DateTime(Convert.ToInt64(nodoData.ChildNodes[(int)XmlCampos.Inicio].InnerText));
            Tiempo =new TimeSpan(Convert.ToInt64(nodoData.ChildNodes[(int)XmlCampos.Tiempo].InnerText)); 
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
            get { return  inicio; }
            set
            {
                inicio = value;
                txtFechaInicio.Text = inicio.ToString().Replace(" ","\n");
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
            nodo &= (text)"<FechaInicio>" & Inicio.Ticks & "</FechaInicio>";
            nodo&= (text)"<Descripcion>" & RichDescription.EscaparCaracteresXML() & "</Descripcion>";
            nodo &= (text)"<Tiempo>" & Tiempo.Ticks & "</Tiempo>";
            nodo &= "</ItemHistory>";
            xmlDoc.InnerXml = nodo;
            return xmlDoc.FirstChild;
        }
        public static void AddItemsXml(HistoryTime parent,XmlNode nodoItemCronos)
        {
            if (parent == null || nodoItemCronos == null)
                throw new ArgumentNullException();

            for (int i = 0, f = nodoItemCronos.ChildNodes.Count; i < f; i++)
                try
                {
                    parent.Add(new ItemHistorialTime(parent,nodoItemCronos.ChildNodes[i]));
                }
                catch { }
            
        }
    }
}
