using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class winAñadirItemManual : Page
    {
        Object[] sesionData;
        public winAñadirItemManual()
        {
            this.InitializeComponent();
            dpFecha.Date = DateTimeOffset.Now;
            tpTiempoHecho.Time = new TimeSpan();
            tpTiempoHecho.MinuteIncrement = 1;
            
        }

        public TimeSpan Tiempo
        {
            get { return tpTiempoHecho.Time; }
        }
        public DateTime Fecha
        {
            get
            {
                DateTime fecha = new DateTime(dpFecha.Date.Year, dpFecha.Date.Month, dpFecha.Date.Day, tpTiempoFecha.Time.Hours, tpTiempoFecha.Time.Minutes, tpTiempoFecha.Time.Seconds, tpTiempoFecha.Time.Milliseconds);
                return fecha;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            sesionData = e.Parameter as Object[];
            base.OnNavigatedTo(e);
        }
        private void Back()
        {
            Frame.Navigate(typeof(Historial), sesionData);
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            itemCronos parent = (sesionData[0] as itemCronos);
            parent.Historial.Afegir(new itemHistory( Fecha, Tiempo));
            Back();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
    }
}
