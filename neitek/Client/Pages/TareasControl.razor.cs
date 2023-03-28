using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using neitek.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace neitek.Client.Pages
{
    public partial class TareasControl : ComponentBase
    {
        [Inject]
        public Tareas tareas { get; set; }
        public List<Tareas> listaTareas { get; set; } = new List<Tareas>();
        public List<Tareas> filterTareas { get; set; } = new List<Tareas>();
        [Inject]
        public HttpClient httpclient { get; set; } = new HttpClient();
        public string TitlePage { get; set; }
        [CascadingParameter]
        public int metaSeleccionada { get; set; }
        [Parameter]
        public Modal ModalDelete { get; set; } = new Modal();
        [Parameter]
        public Tareas tareasModal { get; set; } = new Tareas();
        [Parameter]
        public Modal ModalEliminarTarea { get; set; } = new Modal();
        [Parameter]
        public Modal ModalTareaNueva { get; set; } = new Modal();
        [Parameter]
        public Tareas eliminarTareaModal { get; set; } = new Tareas();
        public List<int> tareasAEliminar { get; set; } = new List<int>();
        public Tareas emptyTareas { get; set; } = new Tareas();
        public List<bool> checkTareas { get; set; } = new List<bool>();
        public string titleModal { get; set; }
        [Parameter]
        public EventCallback<string> OnClickCallback { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }

       
        protected override async Task OnParametersSetAsync()
        {
            if (metaSeleccionada > 0)
            {
                //var data = await httpclient.GetFromJsonAsync<IEnumerable<Tareas>>("api/Tareas/"+ metaSeleccionada);
                var data = await httpclient.GetFromJsonAsync<IEnumerable<Tareas>>($"/api/Tareas/{metaSeleccionada}");
                if (data != null)
                {
                    listaTareas = data.ToList();
                    filterTareas = listaTareas;
                    //tareasAEliminar = new List<int>();
                    checkTareas.Clear();
                    checkTareas.AddRange(new List<bool>(new bool[listaTareas.Count()]));
                }
                TitlePage = "Tareas";
            }
        }
        private void ShowModal(bool editar, Tareas tarea)
        {
            if (editar)
            {
                titleModal = "Editar Tarea";
                tareasModal = tarea;
            }
            else
            {
                titleModal = "Nueva Tarea";
                tareasModal = new Tareas();
            }
            ModalTareaNueva.Show();
        }
        private void HideModal()
        {
            ModalTareaNueva.Hide();
        }

        private void ShowModalEliminar()
        {
            ModalEliminarTarea.Show();
        }
        private void HideModalEliminar()
        {
            ModalEliminarTarea.Hide();
        }
        protected async void AgregarTarea(Tareas tareaNueva)
        {
            tareaNueva.FechaCreacion = DateTime.Now;
            tareaNueva.MetaID = metaSeleccionada;
            //api para agregar
            if (tareasModal.ID == 0)
            {
                var insertar = await httpclient.PostAsJsonAsync("/api/Tareas/", tareaNueva);
                var respuesta = insertar.Content.ReadAsStringAsync().Result;

                dynamic json = JsonConvert.DeserializeObject(respuesta);
                if (json["success"] == "false")
                {
                    await JsRuntime.InvokeVoidAsync("alert", "Las Tareas no admiten nombres repetidos.");

                }
            }
            else
            {
                var editar = await httpclient.PutAsJsonAsync($"/api/Tareas/{tareasModal.ID}", tareasModal);
            }
           
            listaTareas = await httpclient.GetFromJsonAsync<List<Tareas>>($"/api/Tareas/{metaSeleccionada}");
            filterTareas = listaTareas;
            checkTareas.Clear();
            checkTareas.AddRange(new List<bool>(new bool[listaTareas.Count()]));
            StateHasChanged();
            await ModalTareaNueva.Hide();
            await OnClickCallback.InvokeAsync();
        }

        protected void deleteTareasID(int id)
        {
            if (tareasAEliminar.Contains(id))
            {
                tareasAEliminar.Remove(id);
            }
            else
            {
                tareasAEliminar.Add(id);
            }
        }

        protected async void EliminarTarea()
        {
            foreach(var tareaID in tareasAEliminar)
            {
               await httpclient.DeleteAsync($"/api/Tareas/{tareaID}");
            }
            tareasAEliminar.Clear();
            listaTareas = await httpclient.GetFromJsonAsync<List<Tareas>>($"/api/Tareas/{metaSeleccionada}");
            filterTareas = listaTareas;
            checkTareas.Clear();
            checkTareas.AddRange(new List<bool>(new bool[listaTareas.Count()]));
            StateHasChanged();
            await ModalEliminarTarea.Hide();
            await OnClickCallback.InvokeAsync();
        }

        protected async void CompletarTareas()
        {

            foreach (var tareaID in tareasAEliminar)
            {
                //await httpclient.DeleteAsync($"/api/Tareas/{tareaID}");
                var editar = await httpclient.PutAsJsonAsync($"/api/Tareas/Completar/{tareaID}", new Tareas { Nombre="edit"});
            }
            tareasAEliminar.Clear();
            listaTareas = await httpclient.GetFromJsonAsync<List<Tareas>>($"/api/Tareas/{metaSeleccionada}");
            filterTareas = listaTareas;
            checkTareas.Clear();
            checkTareas.AddRange(new List<bool>(new bool[listaTareas.Count()]));

            StateHasChanged();
            await ModalEliminarTarea.Hide();
            await OnClickCallback.InvokeAsync();
        }

        protected async void ImportanteTarea(int tareaID)
        {
                //await httpclient.DeleteAsync($"/api/Tareas/{tareaID}");
                var editar = await httpclient.PutAsJsonAsync($"/api/Tareas/Importante/{tareaID}", new Tareas { Nombre = "edit" });
          var tarea =  filterTareas.Select(x => x).Where(y => y.ID == tareaID).FirstOrDefault();
            tarea.Importante = !tarea.Importante;
            StateHasChanged();

        }

        protected void FilterByTask(string InputValue)
        {
            filterTareas = listaTareas.Where(x => x.Nombre.ToLower().Contains(InputValue.ToLower())).ToList();
        }

        protected void FilterByDate(string InputValue)
        {
            filterTareas = listaTareas.Where(x => x.FechaCreacion.ToString().Contains(InputValue)).ToList();
        }

        protected void FilterByStatus(string InputValue)
        {
            //revisar
            string completada = "completada";
            string abierta = "abierta";

            if (completada.Contains(InputValue.ToLower()) && abierta.Contains(InputValue.ToLower()))
            {
                filterTareas = listaTareas;
            }

            else if (completada.Contains(InputValue.ToLower()))
            {
                filterTareas = listaTareas.Where(x => x.Status == true).ToList();
            }

            else if (abierta.Contains(InputValue.ToLower()))
            {
                filterTareas = listaTareas.Where(x => x.Status == false).ToList();
            }

        }

        protected void CheckboxClicked(Tareas tarea, object seleccionado)
        {
            if ((bool)seleccionado)
            {
            }
        }
    }
}
