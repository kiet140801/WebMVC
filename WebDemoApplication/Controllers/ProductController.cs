using App.Core.Commons;
using App.Core.Entities;
using App.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDemoApplication.Models.Dtos;

namespace WebDemoApplication.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly ICurrentUser _currentUser;
        public ProductController(IWebHostEnvironment environment, IBaseRepository<Product> productRepository, ICurrentUser currentUser)
        {
            _environment = environment;
            _productRepository = productRepository;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["IsAmin"] = _currentUser.IsAdmin();
            var products = await _productRepository.GetAll().ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto ballDto)
        {
            if(ballDto.ImageFile == null)
            {
                ModelState.AddModelError("error", "vui long nhap thong tin");
            }
            if(!ModelState.IsValid)
            {
                return View(ballDto);
            }
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ballDto.ImageFile!.FileName);

            // ~/image/item1.jpg
            string imageFullPath = _environment.WebRootPath + "/Image/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ballDto.ImageFile.CopyTo(stream);
            }

            var ball = new Product()
            {
                Name = ballDto.Name,
                Price = ballDto.Price,
                ImageFile = newFileName,
            };
            
            await _productRepository.AddAsync(ball);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Price = product.Price,
            };
            ViewData["BallId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFile;

            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDto ProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            if(!ModelState.IsValid)
            {
                ViewData["BallId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFile;

                return View(ProductDto);
            }

            string newFileName = product.ImageFile;

            if(ProductDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ProductDto.ImageFile!.FileName);

                string imageFullPath = _environment.WebRootPath + "/Image/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ProductDto.ImageFile.CopyTo(stream);
                }

                string oldBallPath = _environment.WebRootPath + product.ImageFile;
                System.IO.File.Delete(oldBallPath);
            }

            product.Name = ProductDto.Name;
            product.Price = ProductDto.Price;
            product.ImageFile = newFileName;

            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            string imageBallPath = _environment.WebRootPath + "/Image/" + product.ImageFile;
            System.IO.File.Delete(imageBallPath);

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }
    }
}
