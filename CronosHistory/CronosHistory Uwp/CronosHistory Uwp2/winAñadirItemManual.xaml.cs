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
        public winAñadirItemManual()
        {
            GuardarItem = false;
            this.InitializeComponent();
            dpFecha.Date = DateTimeOffset.Now;
        }
        
        public bool GuardarItem
        {
            get;

            private set;
        }
        public TimeSpan Tiempo
        {
            get { return tpTiempoHecho.Time; }
        }
        public DateTimeOffset Fecha
        {
            get
            {
                return dpFecha.Date + tpTiempoFecha.Time;
            }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            GuardarItem = true;
          //  Close(); cierro y muestro el historial
        }

    }
}
