using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Utilities;
using DataLayer.Entities;
using DataLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Repos;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConvertRepository _convertRepository;


        public HomeController(ConvertRepository convertRepository)
        {
            _convertRepository = convertRepository;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _convertRepository.GetListAsync();

            return View(model);
        }


        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> Create(ConvertAddressViewModel convertAddressViewModel, IFormFile input1, IFormFile input2)
        {
            await MFile.Save(input1, "File");
            await MFile.Save(input2, "File");

            await _convertRepository.MapAddAsync(convertAddressViewModel);

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Show(int id)
        {
            var model = await _convertRepository.GetByIdAsync(id);

            return View(model);
        }


    }
}