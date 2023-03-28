using Blazorise;
using Microsoft.AspNetCore.Components;
using neitek.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace neitek.Client.Pages
{
    public partial class MetasControl: ComponentBase
    {
        public List<Metas> listaMetas { get; set; }
        public List<int> porcentajes { get; set; } = new List<int>();
        public Dictionary<int, int> porcentajeCumplimiento { get; set; } = new Dictionary<int, int>();
        [Inject]
        public HttpClient httpclient { get; set; } = new HttpClient();
        [Parameter]
        public Modal ModalMetaNueva { get; set; } = new Modal();

        [Parameter]
        public Modal ModalEliminarMeta { get; set; } = new Modal();
        [Parameter]
        public TareasControl TareasControl { get; set; }
        [Parameter]
        public Metas metasModal { get; set; } = new Metas();
        [Parameter]
        public Metas eliminarMetasModal { get; set; } = new Metas();
        public Metas emptyMetas { get; set; } = new Metas();
        public int metaSeleccionada{ get; set; }
        public bool mostrarTareas { get; set; } = false;
        public string titleModal { get; set; }

        protected override async Task OnInitializedAsync()
        {

            //ProviderTypes = ProviderTypeDataProvider.getProviderTypes();

            GetTareasRealizadas();
            listaMetas = await httpclient.GetFromJsonAsync<List<Metas>>("/api/Metas");

        }
        private void ShowModal(bool editar,Metas meta)
        {
            if (editar)
            {
                titleModal = "Editar Meta";
                metasModal = meta;
            }
            else
            {
                titleModal = "Nueva Meta";
                metasModal = new Metas();
            }
            ModalMetaNueva.Show();
        }
        private void HideModal()
        {
            ModalMetaNueva.Hide();
        }

        private void ShowModalEliminar(int id, string nombre)
        {
            eliminarMetasModal.Nombre = nombre;
            eliminarMetasModal.ID = id;
            ModalEliminarMeta.Show();
        }
        private void HideModalEliminar()
        {
            ModalEliminarMeta.Hide();
        }

        protected async void AgregarMeta(Metas metaNueva)
        {
            metaNueva.FechaCreacion = DateTime.Now;

            //api para agregar
            if (metasModal.ID == 0)
            {
                var insertar = await httpclient.PostAsJsonAsync("/api/Metas/", metaNueva);
            }
            else
            {
               var editar=  await httpclient.PutAsJsonAsync($"/api/Metas/{metasModal.ID}", metasModal);
            }
            listaMetas = await httpclient.GetFromJsonAsync<List<Metas>>("/api/Metas");
            StateHasChanged();
            await ModalMetaNueva.Hide();
        }

        protected async void EliminarMeta(Metas meta)
        {
            await httpclient.DeleteAsync($"/api/Metas/{meta.ID}");
            
            listaMetas = await httpclient.GetFromJsonAsync<List<Metas>>("/api/Metas");
            StateHasChanged();
            mostrarTareas = false;
            await ModalEliminarMeta.Hide();
        }

        protected async void GetTareasPorMeta(int id)
        {
            mostrarTareas = true;
            metaSeleccionada = id;
            StateHasChanged();
        }


        protected async void GetTareasRealizadas()
        {
             porcentajeCumplimiento = await httpclient.GetFromJsonAsync<Dictionary<int, int>>("/api/Metas/tareasRealizadas");
            porcentajes = new List<int>(porcentajeCumplimiento.Values);
        }
    }
}
