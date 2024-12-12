using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using ManejoPresupuesto.Servicios;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        //private readonly String connectionString;
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index() 
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var yaExisteCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExisteCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                    $"El nmobre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");  
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta) 
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> BorrarTipoCuenta(int id) 
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            var idsTipoCuentas = tiposCuentas.Select(x => x.Id).ToArray();

            var idsTiposCuentasNoPerteneceAlUsuario = ids.Except(idsTipoCuentas).ToList();

            if (idsTiposCuentasNoPerteneceAlUsuario.Any())
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta { Id = valor, Orden = indice + 1 }).AsEnumerable();
            await repositorioTiposCuentas.Ordener(tiposCuentasOrdenados);
            return Ok();
        }


    }
}