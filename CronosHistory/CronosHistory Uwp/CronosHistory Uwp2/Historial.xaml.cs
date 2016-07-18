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
        Object todosLosItems;
        itemCronos item;
        public Historial()
        {
            this.InitializeComponent();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            Cargar(e.Parameter as Object[]);


        }

        private void Cargar(object[] items)
        {
      

            if (items != null)
            {
                todosLosItems = items[1];
                item = items[0] as itemCronos;
                for (int i = 0; i < item.Historial.Count; i++)
                    Add(new ItemHistorial(this, item.Historial[i]), false);

            }
        }
        public void Add(ItemHistorial item,bool añadir=true)
        {
            Action act;
                item.Eliminar += (s, e) =>
                {
                     act = () =>
                    {
                        ItemHistorial itemHistorial = s as ItemHistorial;
                        stkHistorial.Children.Remove(itemHistorial);
                        this.item.Historial.Elimina(itemHistorial.ItemHistory);//lo quito para que conste
                        ActualizaBackGroundItems();
                    };
                    Dispatcher.BeginInvoke(act);
                };
                if(añadir)
                this.item.Historial.Afegir(item.ItemHistory);//lo añado para que se guarde
            act = () =>
              {
                  stkHistorial.Children.Add(item);
                  ActualizaBackGroundItems();
              };
            Dispatcher.BeginInvoke(act);

        }

        private void ActualizaBackGroundItems()
        {
            for (int i = 0; i < stkHistorial.Children.Count; i++)
                if (i % 2 == 0)
                    ((ItemHistorial)stkHistorial.Children[i]).BackColor = Windows.UI.Colors.LightBlue;
                else ((ItemHistorial)stkHistorial.Children[i]).BackColor = Windows.UI.Colors.White;
        }




        private void btnAñadirCustom_Click(object sender, RoutedEventArgs e)
        {
            //hacen clic
            Frame.Navigate(typeof(winAñadirItemManual),new object[] { item,todosLosItems});

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage),todosLosItems);
        }
    }
}
