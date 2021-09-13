using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class YBankAPI : ControllerBase
    {
        [HttpGet()]
        public double Bakiye(int musteriId)
        {
            return 500.15;
        }
        [HttpGet()]
        public List<string> TumHesaplar(int musteriId)
        {
            return new List<string>()
            {
                "135792468",
                "019283745",
                "085261060"
            };
        }
    }
}
