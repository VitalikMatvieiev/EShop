using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inquiryDetailRep;
        private readonly IInquiryHeaderRepository _inquiryHeaderilRep;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inquiryDetailRep, IInquiryHeaderRepository inquiryHeaderilRep)
        {
            _inquiryDetailRep = inquiryDetailRep;
            _inquiryHeaderilRep = inquiryHeaderilRep;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderilRep.FirstOrDefault(u => u.Id == id),
                inquiryDetail = _inquiryDetailRep.GetAll(u => u.InquiryHeaderId == id,includeProperties:"Product")
            };

            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            InquiryVM.inquiryDetail = _inquiryDetailRep.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
            
            foreach (var detail in InquiryVM.inquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
                
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set<List<ShoppingCart>>(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set<int>(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inquiryHeaderilRep.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inquiryDetailRep.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inquiryDetailRep.RemoveRange(inquiryDetails);
            _inquiryHeaderilRep.Remove(inquiryHeader);

            _inquiryHeaderilRep.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS 
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderilRep.GetAll() });
        }
        #endregion

    }
}
