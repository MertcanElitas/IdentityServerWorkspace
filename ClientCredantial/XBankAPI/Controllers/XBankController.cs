using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class XBankController : ControllerBase
    {
        [HttpGet()]
        //[Authorize(Policy = "AllXBank")]
        public double Bakiye(int musteriId) 
        {
            return 1000;
        }

        [HttpGet()]
        public List<string> TumHesaplar(int musteriId)
        {
            return  new List<string>()
            {
                "123456789",
                "987654321",
                "564738291"
            };
        }
    }
}
