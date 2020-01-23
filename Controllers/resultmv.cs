using Microsoft.AspNetCore.Mvc;

namespace SecureAfrica.Controllers
{
    internal class resultmv : ActionResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}