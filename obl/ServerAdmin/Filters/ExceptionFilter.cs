using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerAdmin.Exceptions;

namespace WebApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            try
            {
                throw context.Exception;
            }
            catch (GameException ex)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = ex.Message
                };
            }
            catch (UserException ex)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = ex.Message
                };
            }
            catch (Exception)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 500,
                    Content = "Sorry,internal error"
                };
            }
        }
    }
}