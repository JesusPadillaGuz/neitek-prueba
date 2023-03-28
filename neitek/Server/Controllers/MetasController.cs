using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class MetasController : ControllerBase
    {
        private AppDbContext context;

        public MetasController(AppDbContext Context)
        {
            context = Context;
        }
        // GET: api/Metas
        [HttpGet]
        public async Task<List<Metas>> GetMetas()
        {
            return await context.Metas.ToListAsync();
        }

        // GET: api/Metas
        [HttpGet("tareasRealizadas")]
        public async Task<Dictionary<int,int>> GetTareasRealizadas()
        {
            var query = context.Metas.Include(x => x.Tareas).ToList();
            var dictionario = new Dictionary<int, int>();

            foreach (var meta in query)
            {
              var completadas = (from tarea in meta.Tareas where tarea.Status == true select tarea).Count();
              var todas = (from tarea in meta.Tareas select tarea).Count();
                if (todas > 0)
                {
                    double porcentaje = (double)completadas / todas;
                    dictionario.Add(meta.ID, (int)(porcentaje * 100));
                }
                else
                {
                    dictionario.Add(meta.ID, 0);
                }
            }
            //.Select(group => new { METAID = group.Key, Tareas = group })
            //.ToList();

            //var tareasRealizadasPorMeta = context.Metas
            //                         .SelectMany(meta => meta.Tareas).GroupBy(x => x.MetaID).ToList();

            return dictionario;
        }

        // GET api/Metas/5
        [HttpGet("{id}")]
        public async Task<Metas> GetMetaByID(int id)
        {
            return await context.Metas.Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        // POST api/Metas
        [HttpPost]
        public async Task<Respuesta> Post(Metas meta)
        {
            var metaExist = await context.Metas.AnyAsync(x => x.Nombre == meta.Nombre);
            var response = 0;
            if (!metaExist)
            {
                await context.AddAsync(meta);

                 response = await context.SaveChangesAsync();
                return new Respuesta
                {
                    success = true,
                    message = "la meta fue agregada correctamente."
                };
            }
                return new Respuesta
                {
                    success = false,
                    message = "no se pudo agregar la meta."
                };
        }

        // PUT api/Metas/5
        [HttpPut("{id}")]
        public async Task<Respuesta> EditarMeta(int id, Metas meta)
        {
            try
            {
                if (context.Metas.Any(x => x.ID == id))
                {
                    context.Metas.Update(meta);
                    await context.SaveChangesAsync();
                    return new Respuesta
                    {
                        success = true,
                        message = "meta editada exitosamente."
                    };
                }
                else
                {
                    return new Respuesta
                    {
                        success = false,
                        message = "no existe la meta."
                    };
                }

            }
            catch (Exception e)
            {
                return new Respuesta
                {
                    success = false,
                    message = "falló el intentar agregar la meta."
                };
            }
            
        }

        // DELETE api/Metas/5
        [HttpDelete("{id}")]
        public async Task<Respuesta> Delete(int id)
        {
            var tareas = await context.Tareas.Where(x => x.MetaID == id).ToListAsync();
            context.Tareas.RemoveRange(tareas);
            await context.SaveChangesAsync();
            var meta = await context.Metas.Where(x => x.ID == id).FirstOrDefaultAsync();
            context.Metas.Remove(meta);
            await context.SaveChangesAsync();

            return new Respuesta
            {
                success = true,
                message = "meta eliminada."
            };
        }
    }
}
