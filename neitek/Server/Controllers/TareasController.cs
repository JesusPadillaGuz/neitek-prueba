using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neitek.Server.Database;
using neitek.Server.Helpers;
using neitek.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace neitek.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private AppDbContext context;

        public TareasController(AppDbContext Context)
        {
            context = Context;
        }
        // GET: api/Tareas
        [HttpGet("{id}")]
        public async Task<List<Tareas>> GetTareasByMetaID(int id)
        {
            return await context.Tareas.Where(x => x.MetaID == id).ToListAsync();
        }

        //// GET api/Tareas/5
        //[HttpGet("{id}")]
        //public async Task<Metas> GetMetaByID(int id)
        //{
        //    return await context.Metas.Where(x => x.ID == id).FirstOrDefaultAsync();
        //}

        // POST api/<MetasController>
        [HttpPost]
        public async Task<Respuesta> Post(Tareas tarea)
        {
            await context.AddAsync(tarea);
            var response = await context.SaveChangesAsync();
            if (response > 0)
            {
                return new Respuesta
                {
                    success = true,
                    message = "la tarea fue agregada correctamente."
                };
            }
            else
            {
                return new Respuesta
                {
                    success = false,
                    message = "no se pudo agregar la meta."
                };
            }
        }

        // PUT api/Tareas/5
        [HttpPut("{id}")]
        public async Task<Respuesta> EditarTarea(int id, Tareas tarea)
        {
            try
            {
                if (context.Tareas.Any(x => x.ID == id))
                {
                    context.Tareas.Update(tarea);
                    await context.SaveChangesAsync();
                    return new Respuesta
                    {
                        success = true,
                        message = "tarea editada exitosamente."
                    };
                }
                else
                {
                    return new Respuesta
                    {
                        success = false,
                        message = "no existe la tarea."
                    };
                }

            }
            catch (Exception e)
            {
                return new Respuesta
                {
                    success = false,
                    message = "falló el intentar agregar la tarea."
                };
            }

        }

        // PUT api/Tareas/5
        [HttpPut("Completar/{id}")]
        public async Task<Respuesta> CompletarTarea(int id, Tareas tarea)
        {
            try
            {
                if (context.Tareas.Any(x => x.ID == id))
                {
                    var tar = context.Tareas.Where(x => x.ID == id).FirstOrDefault();
                    tar.Status = true;
                    context.Tareas.Update(tar);
                    await context.SaveChangesAsync();
                    return new Respuesta
                    {
                        success = true,
                        message = "tarea editada exitosamente."
                    };
                }
                else
                {
                    return new Respuesta
                    {
                        success = false,
                        message = "no existe la tarea."
                    };
                }

            }
            catch (Exception e)
            {
                return new Respuesta
                {
                    success = false,
                    message = "falló el intentar agregar la tarea."
                };
            }

        }

        // DELETE api/Tareas>/5
        [HttpDelete("{id}")]
        public async Task<Respuesta> Delete(int id)
        {
            var tarea = await context.Tareas.Where(x => x.ID == id).FirstOrDefaultAsync();
            context.Tareas.Remove(tarea);
            await context.SaveChangesAsync();

            return new Respuesta
            {
                success = true,
                message = "tarea eliminada."
            };
        }

    }
}
