using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using ClienteVuelosa181G0250.Models;
using ClienteVuelosa181G0250.Views;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace ClienteVuelosa181G0250.ViewModels
{

    public class ClienteViewModel: INotifyPropertyChanged
    {
        public string Url { get; set; } = "http://vuelos.itesrc.net/Tablero";

        public List<VueloClase> Vuelos { get; set; } 
            = new List<VueloClase>();

        private string origen, destino, estado, hora;

        private TimeSpan spa;
        public TimeSpan Spa { get { return spa; } set { spa = value; } }

        private VueloClase vueloSeleccionado;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Horax { get { return hora = Spa.ToString(@"hh\:mm"); } set { hora = value; } }
        public string Destinox { get { return destino; } set { destino = value; } }
        public string Vuelox { get { return origen; } set { origen = value; } }
        public string Estadox { get { return estado; } set { estado = value; } }

        public VueloClase VueloSeleccionado { get { return vueloSeleccionado; } set { vueloSeleccionado = value; } }


        public ICommand AgregarVuelo { get; set; }
        public ICommand EditarVuelo { get; set; }
        public ICommand EliminarVuelo { get; set; }
        public ICommand ConectarCommand { get; set; }
        public ICommand AgregarPage { get; set; }
        public ICommand EditarPage { get; set; }
        public ICommand EliminarPage { get; set; }

        public string Mensaje { get; set; }

        public ClienteViewModel()
        {
            VueloSeleccionado = new VueloClase();

            ConectarCommand = new Command(Connect);
            AgregarVuelo = new Command(Agregar);
            EditarVuelo = new Command(Editar);
            EliminarVuelo = new Command(Eliminar);
            AgregarPage = new Command(AddPage);
            EditarPage = new Command(EditPage);
            EliminarPage = new Command(DeletePage);
        }
        
        private async void AddPage()
        {
            ActualizarLista();
            AgregarPage page = new AgregarPage() { BindingContext = this };
            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        private async void EditPage()
        {
            ActualizarLista();
            EditarPage page = new EditarPage() { BindingContext = this };
            await App.Current.MainPage.Navigation.PushAsync(page);

        }

        private async void DeletePage()
        {
            ActualizarLista();
            EliminarPage page = new EliminarPage() { BindingContext = this };
            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        private async void ActualizarLista()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Url);
            var json = await response.Content.ReadAsStringAsync();
            var vuelos = JsonConvert.DeserializeObject<List<VueloClase>>(json);

            Vuelos = vuelos;

            Mensaje = "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));
        }

        private async void Connect()
        {
            Uri uri;
            try
            {
                if (Uri.TryCreate(Url, UriKind.Absolute, out uri))
                {
                    VueloPage page = new VueloPage() { BindingContext = this };
                    await App.Current.MainPage.Navigation.PushAsync(page);

                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(Url);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var vuelos = JsonConvert.DeserializeObject<List<VueloClase>>(json);

                        Vuelos = vuelos;

                    }
                }
                Mensaje = "La URL especificada es incorrecta";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));
            }
            catch (Exception ex) when (ex is Exception) { Mensaje = ex.ToString(); }
            catch (Exception ex) when (ex is IOException) { Mensaje = ex.ToString(); }
        }

        private async void Agregar()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(Url);


            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            { 

                var DatoVuelo = new VueloClase
                {
                    Vuelo = Vuelox,
                    Destino = Destinox,
                    Estado = Estadox,
                    Hora = Horax
                };


                var vuelo = JsonConvert.SerializeObject(DatoVuelo);
                StringContent vuelocontent = new StringContent(vuelo, Encoding.UTF8, "application/json");
                await client.PostAsync(Url, vuelocontent);
                resp.EnsureSuccessStatusCode();



                var content = await resp.Content.ReadAsStringAsync();
                ActualizarLista();

                AgregarPage page = new AgregarPage() { BindingContext = this };
                await App.Current.MainPage.Navigation.PopAsync();
                Vuelox = "";
                Estadox = "";
                Destinox = "";
                Horax = "";
            }
        }

        private async void Editar()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(Url);


            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {

                if (vueloSeleccionado != null)
                {
                    var DatoVuelo = new VueloClase
                    {
                        Vuelo = VueloSeleccionado.Vuelo,
                        Destino = Destinox,
                        Estado = Estadox,
                        Hora = VueloSeleccionado.Hora
                    };

                    var vuelo = JsonConvert.SerializeObject(DatoVuelo);
                    StringContent vuelocontent = new StringContent(vuelo, Encoding.UTF8, "application/json");
                    await client.PutAsync(Url, vuelocontent);
                    HttpResponseMessage res = await client.PutAsJsonAsync(Url, DatoVuelo);
                    resp.EnsureSuccessStatusCode();

                    var content = await resp.Content.ReadAsStringAsync();
                    ActualizarLista();

                    EditarPage page = new EditarPage() { BindingContext = this };
                    await App.Current.MainPage.Navigation.PopAsync();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vuelos)));

                    Vuelox = "";
                    Estadox = "";
                    Destinox = "";
                    Horax = "";
                }
                Mensaje = "Seleccione un vuelo a editar";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));
            }

        }


        private async void Eliminar()
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Url);
            var json = await response.Content.ReadAsStringAsync();
            var vuelos = JsonConvert.DeserializeObject<List<VueloClase>>(json);

            if(vueloSeleccionado != null)
            {

                var DatoVuelo = new VueloClase
                {
                    Vuelo = VueloSeleccionado.Vuelo,
                    Destino = VueloSeleccionado.Destino,
                    Estado = VueloSeleccionado.Estado,
                    Hora = VueloSeleccionado.Hora
                };

                HttpMethod del = HttpMethod.Delete;

                await client.DeleteAsJsonAsync(Url, DatoVuelo);
                HttpResponseMessage res = await client.DeleteAsJsonAsync(Url, DatoVuelo);
                ActualizarLista();

                EliminarPage page = new EliminarPage() { BindingContext = this };
                await App.Current.MainPage.Navigation.PopAsync();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vuelos)));
            }
            Mensaje = "Seleccione un vuelo a eliminar";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));

        }

    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) });
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = Serialize(data) });
        private static HttpContent Serialize(object data) => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
    }
}
