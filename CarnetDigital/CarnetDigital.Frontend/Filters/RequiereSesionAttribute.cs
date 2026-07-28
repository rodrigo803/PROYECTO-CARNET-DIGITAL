using CarnetDigital.Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CarnetDigital.Frontend.Filters
{
    public class RequiereSesionAttribute : TypeFilterAttribute
    {
        public RequiereSesionAttribute() : base(typeof(RequiereSesionFilter))
        {
        }
    }

    internal class RequiereSesionFilter : IActionFilter
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public RequiereSesionFilter(ITokenProvider tokenProvider, ITempDataDictionaryFactory tempDataFactory)
        {
            _tokenProvider = tokenProvider;
            _tempDataFactory = tempDataFactory;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_tokenProvider.IsAuthenticated())
                return;

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = "Por favor inicie sesión para utilizar el sistema";

            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
