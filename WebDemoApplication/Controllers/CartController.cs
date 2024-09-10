using App.Core.Commons;
using App.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBaseRepository<App.Core.Entities.CartItem> _cartItemRepository;
        private readonly ICurrentUser _currentUser;

        public CartController(IBaseRepository<App.Core.Entities.CartItem> cartItemRepository, ICurrentUser currentUser)
        {
            _cartItemRepository = cartItemRepository;
            _currentUser = currentUser;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromRoute]int id, int quantity = 1)
        {
            var userId = _currentUser.GetUserId();

            await _cartItemRepository.AddAsync(new App.Core.Entities.CartItem
            {
                ProductId = id,
                Quantity = quantity,
                UserId = userId
            });

            await _cartItemRepository.SaveChangesAsync();
            
            return RedirectToAction("Index", "Cart"); 
        }

        public async Task<IActionResult> Index()
        {
            var userId = _currentUser.GetUserId();
            var cartItems = await _cartItemRepository.GetAll().Where(x => x.UserId == userId)
                                                     .Include(x => x.Product)
                                                     .ToListAsync();             

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            await _cartItemRepository.DeleteAsync(id);
            await _cartItemRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
