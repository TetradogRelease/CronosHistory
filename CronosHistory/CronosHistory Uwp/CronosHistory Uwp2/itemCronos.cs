using Gabriel.Cat;
using Gabriel.Cat.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CronosHistory_UWP
{

    public class itemCronos
    {
        string texto;
        Llista<itemHistory> historial;
        #region ContadorTiempo
        bool estaOn;
        DateTime inicioTiempo;
        Action actContadorTiempo;
        Task tskContadorTiempo;
        public ItemCronos ItemParent{get;set; }
        #endregion
        public itemCronos()
        {
            historial = new Llista<itemHistory>();
            actContadorTiempo = new Action(() => { ContarTiempo(); });
            texto = "";
        }



        public itemCronos(XmlNode nodoItemCronos):this()
        {
            Descripcion = nodoItemCronos.FirstChild.FirstChild!=null ?nodoItemCronos.FirstChild.FirstChild.InnerText.DescaparCaracteresXML():"";
            historial.AfegirMolts(itemHistory.LoadXml(nodoItemCronos.LastChild));
        }
        public bool EstaOn
        {
            get { return estaOn; }
            set
            {
                
                if(!EstaOn&&value)
                {
                    estaOn = true;
                    tskContadorTiempo = new Task(actContadorTiempo);
                    tskContadorTiempo.Start();

                }
                else if(EstaOn)
                {
                    estaOn = false;
                    historial.Afegir(new itemHistory(DateTime.Now - inicioTiempo));

                }
            }
        }
        public string Descripcion
        {
            get { return texto; }
            set { texto = value; }
        }
        public Llista<itemHistory> Historial
        {
            get { return historial; }
        }
        public TimeSpan TiempoTotal
        {
            get
            {
                TimeSpan tiempoTotal = new TimeSpan();
                for (int i = 0; i < historial.Count; i++)
                    tiempoTotal += historial[i].Tiempo;
                return tiempoTotal;
            }
        }
        private void ContarTiempo()
        {
            TimeSpan totalTime;
            TimeSpan tiempoQueLleva;

           if(ItemParent!=null)
            {
                totalTime = TiempoTotal;
                inicioTiempo = DateTime.Now;
                while (estaOn)
                {
                    tiempoQueLleva = DateTime.Now - inicioTiempo;
                    if (EstaOn) { 
                        ItemParent.Tiempo=totalTime + tiempoQueLleva;
                    if (EstaOn)
                        Task.Delay(700);//paso un tiempo
                    }
                }
            }
        }
        public  XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string nodos = itemHistory.ToXml(historial).OuterXml;
            string nodoItemCronos = "<ItemCronos>";
            if (!String.IsNullOrEmpty(Descripcion))
                nodoItemCronos += "<DescripcionItem>" + Descripcion.EscaparCaracteresXML() + "</DescripcionItem>";
            else nodoItemCronos += "<DescripcionItem/>";
            nodoItemCronos += nodos + "</ItemCronos>";
            xmlDoc.LoadXml(nodoItemCronos);
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }
        public static XmlDocument ToXml(itemCronos[] items)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string doc = "<CronosHistory>";
            for (int i = 0; i < items.Length; i++)
                doc += items[i].ToNodoXml().OuterXml;
            doc += "</CronosHistory>";
            xmlDoc.InnerXml = doc;
            xmlDoc.Normalize();
            return xmlDoc;
        }
        public static itemCronos[] LoadXml(XmlDocument xmlItems)
        {
            itemCronos[] items = new itemCronos[xmlItems.ChildNodes[1].ChildNodes.Count];
            for (int i = 0; i < items.Length; i++)
                items[i] = new itemCronos(xmlItems.ChildNodes[1].ChildNodes[i]);
            return items;
        }
    }
    public class itemHistory
    {
        DateTime inicio;
        string contenido;
        TimeSpan tiempo;
        public itemHistory(TimeSpan tiempo) : this(DateTime.Now, tiempo,"")
        { }
        public itemHistory(DateTime inicio,TimeSpan tiempo) : this(inicio, tiempo, "")
        { }
        public itemHistory(DateTime inicio, TimeSpan tiempo, string texto)
        {
            Inicio = inicio;
            Tiempo = TimeSpan.FromSeconds(Convert.ToInt32(tiempo.TotalSeconds));
            
            Contenido = texto;
        }
        public itemHistory(XmlNode nodoItemHistory)
        {
            inicio = new DateTime(Convert.ToInt64(nodoItemHistory.ChildNodes[0].InnerText));
            contenido = nodoItemHistory.ChildNodes[1]!=null ?nodoItemHistory.ChildNodes[1].InnerText.DescaparCaracteresXML():"";
            tiempo = new TimeSpan(Convert.ToInt64(nodoItemHistory.ChildNodes[2].InnerText));
        }
        public DateTime Inicio
        {
            get
            {
                return inicio;
            }

            set
            {
                inicio = value;
            }
        }

        public string Contenido
        {
            get
            {
                return contenido;
            }

            set
            {
                contenido = value;
            }
        }

        public TimeSpan Tiempo
        {
            get
            {
                return tiempo;
            }

            set
            {
                tiempo = value;
            }
        }
        public XmlNode ToNodoXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string nodo = "<ItemHistory>";
            nodo += "<FechaInicio>" + Inicio.Ticks + "</FechaInicio>";
            if (String.IsNullOrEmpty(Contenido))
                nodo += "<Descripcion>" + Contenido.EscaparCaracteresXML() + "</Descripcion>";
            else nodo += "</Descripcion>";
            nodo += "<Tiempo>" + Tiempo.Ticks + "</Tiempo>";
            nodo += "</ItemHistory>";
            xmlDoc.InnerXml = nodo;
            return xmlDoc.FirstChild;
        }
        public static XmlNode ToXml(IEnumerable<itemHistory> itemsHistorial)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string nodo = "<Historial>";
            if(itemsHistorial!=null)
            foreach (itemHistory item in itemsHistorial)
                nodo += item.ToNodoXml().OuterXml;
            nodo += "</Historial>";
            xmlDoc.LoadXml(nodo);
            xmlDoc.Normalize();
            return xmlDoc.FirstChild;
        }

        public static itemHistory[] LoadXml(XmlNode lastChild)
        {
            itemHistory[] history = new itemHistory[lastChild.ChildNodes.Count];
            for (int i = 0; i < history.Length; i++)
                history[i] = new itemHistory(lastChild.ChildNodes[i]);
            return history;
        }
    }
}
