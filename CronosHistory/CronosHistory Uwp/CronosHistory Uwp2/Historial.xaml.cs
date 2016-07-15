using CronosHistory_Uwp;
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
        itemCronos[] todosLosItems;
        itemCronos item;
        public Historial()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //cargo la pagina que tenia que cargar y cojo el main para volver atrás
            Object[] items = e.Parameter as Object[];
            if (items != null)
            {
                todosLosItems = items[1] as itemCronos[];
                item = items[0] as itemCronos;
                for (int i = 0; i < item.Historial.Count; i++)
                    stkHistorial.Children.Add(new ItemHistorial(this, item.Historial[i]));

            }

            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ItemHistorial item;
            base.OnNavigatedFrom(e);
            for(int i=0;i<stkHistorial.Children.Count;i++)
            {
                item = stkHistorial.Children[i] as ItemHistorial;
                stkHistorial.Children.Remove(item);
                nextPage.stkHistorial.Children.Add(item);
            }
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
            try
            {
                item.Eliminar += (s, e) =>
                {
                    Action act = () =>
                    {
                        stkHistorial.Children.Remove(s as ItemHistorial);
                        ActualizaBackGroundItems();
                    };
                    Dispatcher.BeginInvoke(act);
                };
                stkHistorial.Children.Add(item);
                ActualizaBackGroundItems();
            }
            catch { }
        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemHistorial)stkHistorial.Children[i]).Background = Windows.UI.Colors.LightBlue.ToBrush();
                else ((ItemHistorial)stkHistorial.Children[i]).Background = Windows.UI.Colors.White.ToBrush();
        }




        private void btnAñadirCustom_Click(object sender, RoutedEventArgs e)
        {
            //hacen clic
            Frame.Navigate(typeof(winAñadirItemManual),this);

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage),todosLosItems);
        }
    }
}
