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
using Gabriel.Cat.Extension;
namespace CronosHistory
{
    /// <summary>
    /// Interaction logic for winPostItem.xaml
    /// </summary>
    public partial class winPostItem : Window
    {
        bool guardarItem;
        public winPostItem()
        {
            GuardarItem = false;
            InitializeComponent();
            dpFecha.SelectedDate = DateTime.Now;
        }

        public bool GuardarItem
        {
            get
            {
                return guardarItem;
            }

          private  set
            {
                guardarItem = value;
            }
        }
        public TimeSpan Tiempo
        {
            get { return new TimeSpan(int.Parse(txtHoras.Text), int.Parse(txtMinutos.Text), int.Parse(txtSegundos.Text)); }
        }
        public DateTime Fecha
        {
            get { DateTime fecha= dpFecha.SelectedDate.Value;
                fecha=fecha.AddHours(int.Parse(txtHorasFecha.Text));
                fecha = fecha.AddMinutes(int.Parse(txtMinutosFecha.Text));
                fecha = fecha.AddSeconds(int.Parse(txtSegundosFecha.Text));
                return fecha;
            }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            GuardarItem = true;
            Close();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            try
            {
                int.Parse(txt.Text);
            }catch { txt.Text = "0";MessageBox.Show("Solo caracteres numericos!"); }
        }
    }
}
