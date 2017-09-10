using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet.Server.Mvc
{
    [OicController]
    public class OicController
    {
        private ActionContext _controllerContext;

        // TODO: Still deciding how to discover data for a Resource Directory
        public virtual IEnumerable<IOicResource> DiscoverableResources
        {
            get
            {
                yield break;
            }
        }


        #region Properties

        public OicContext OicContext => ControllerContext.OicContext;

        public OicRequest Request => OicContext?.Request;

        public OicResponse Response => OicContext?.Response;

        public ActionContext ControllerContext
        {
            get => _controllerContext ?? (_controllerContext = new ActionContext());
            set => _controllerContext = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion


        public virtual Task<IActionResult> GetAsync()
        {
            return Task.FromResult(Get());
        }

        public virtual IActionResult Get()
        {
            return MethodNotAllowed();
        }

        public virtual Task<IActionResult> PutAsync()
        {
            return Task.FromResult(Put());
        }

        public virtual IActionResult Put()
        {
            return MethodNotAllowed();
        }

        public virtual Task<IActionResult> PostAsync()
        {
            return Task.FromResult(Post());
        }

        public virtual IActionResult Post()
        {
            return MethodNotAllowed();
        }

        public virtual Task<IActionResult> DeleteAsync()
        {
            return Task.FromResult(Delete());
        }

        public virtual IActionResult Delete()
        {
            return MethodNotAllowed();
        }


        #region Response Actions

        public virtual IActionResult Content(IOicResource content)
        {
            return new ContentResult(content);
        }

        public virtual IActionResult Valid(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.Valid, message);
        }

        public virtual IActionResult Deleted(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.Deleted, message);
        }

        // TODO: Create an IActionResult for OicResponseCode.Created and OicResponseCode.Changed

        public virtual IActionResult StatusCode(OicResponseCode code, string message = default(string))
        {
            return new StatusCodeResult(code, message);
        }

        public virtual IActionResult MethodNotAllowed(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.MethodNotAllowed, message);
        }

        public virtual IActionResult BadRequest(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.BadRequest, message);
        }

        public virtual IActionResult Unauthorized(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.Unauthorized, message);
        }

        public virtual IActionResult BadOption(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.BadOption, message);
        }
            
        public virtual IActionResult Forbidden(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.Forbidden, message);
        }
    
        public virtual IActionResult NotFound(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.NotFound, message);
        }

        public virtual IActionResult NotAcceptable(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.NotAcceptable, message);
        }

        public virtual IActionResult PreconditionFailed(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.PreconditionFailed, message);
        }

        public virtual IActionResult RequestEntityTooLarge(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.RequestEntityTooLarge, message);
        }

        public virtual IActionResult UnsupportedContentFormat(string message = default(string))
        {
            return new StatusCodeResult(OicResponseCode.UnsupportedContentFormat, message);
        }

        #endregion


    }
}