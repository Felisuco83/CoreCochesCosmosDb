using CoreCochesCosmosDb.Models;
using CoreCochesCosmosDb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCochesCosmosDb.Controllers
{
    public class CosmosController : Controller
    {
        ServiceCosmosDb servicecosmos;
        public CosmosController(ServiceCosmosDb service)
        {
            this.servicecosmos = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            await this.servicecosmos.CrearBbddVehiculosAsync();
            await this.servicecosmos.CrearColeccioVehiculosAsync();
            List<Vehiculo> coches = this.servicecosmos.CrearCoches();
            foreach (Vehiculo car in coches)
            {
                await this.servicecosmos.InsertarVehiculo(car);
            }
            ViewBag.Mensaje = "TODO INICIADO EN COSMOS";
            return View();
        }

        public IActionResult ListCoches()
        {
            return View(this.servicecosmos.GetVehiculos());
        }

        public async Task<IActionResult> Details(string id)
        {
            return View(await this.servicecosmos.FindVehiculosAsync(id)); 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create( Vehiculo car, string motor)
        {
            if (motor != null)
            {
                car.Motor = new Motor { Tipo = "Diesel", Caballos = 999, Potencia = 999 };
            }
            await this.servicecosmos.InsertarVehiculo(car);
            return RedirectToAction("ListCoches");
        }

        public async Task<IActionResult> Delete (string id)
        {
            await this.servicecosmos.EliminarVehiculo(id);
            return RedirectToAction("ListCoches");
        }

        public async Task<IActionResult> Edit (string id)
        {
            return View(await this.servicecosmos.FindVehiculosAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit (Vehiculo car, string motor)
        {
            if (motor != null)
            {
                car.Motor = new Motor { Tipo = "Diesel", Caballos = 999, Potencia = 999 };
            }
            await this.servicecosmos.ModificarVehiculo(car);
            return RedirectToAction("ListCoches");
        }
    }
}
