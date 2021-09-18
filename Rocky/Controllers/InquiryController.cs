using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inquiryDetailRep;
        private readonly IInquiryHeaderRepository _inquiryHeaderilRep;

        public InquiryController(IInquiryDetailRepository inquiryDetailRep, IInquiryHeaderRepository inquiryHeaderilRep)
        {
            _inquiryDetailRep = inquiryDetailRep;
            _inquiryHeaderilRep = inquiryHeaderilRep;
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
