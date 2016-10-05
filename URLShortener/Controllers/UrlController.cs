using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using URLShortener.Binding_Models;
using URLShortener.Data_Transfer_Objects;
using URLShortener.Models;

namespace URLShortener.Controllers
{
    public class UrlController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        //The Dependency Resolver specified in the App_Start and WebApiConfig files handles the DI here
        public UrlController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
        }
        // POST - /api/Url
        [System.Web.Http.HttpPost]
        public IHttpActionResult Create(UrlBindingModel urlBindingModel)
        {
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            if (!ModelState.IsValid)
            {
                BadRequest();
            }
            if (!HasHttpProtocol(urlBindingModel.OriginalUrl))
            {
                urlBindingModel.OriginalUrl = "http://" + urlBindingModel.OriginalUrl;
            }
            int lastEntryId = _unitOfWork.Urls.GetAll().OrderByDescending(u => u.UrlId).Select(u => u.UrlId).FirstOrDefault();
            //int lastEntryId = (from u in _unitOfWork.Urls
            //               orderby u.UrlId descending
            //               select u.UrlId).FirstOrDefault();
            if (lastEntryId == null)
            {
                return InternalServerError();
            }
            int newId = lastEntryId + 1;
            var newUrl = new Url()
            {
                OriginalUrl = urlBindingModel.OriginalUrl,
                OurUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/" + newId
            };
            

            _unitOfWork.Urls.Add(newUrl);
            try //Try to save changes to DB, catch any exceptions. If succcessful, return the new short URL.
            {
                _unitOfWork.Complete();
                return Created(new Uri(newUrl.OurUrl), newUrl);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        // GET - Gets the Url with the specified id - /api/Url/1
        public IHttpActionResult Get(int id)
        {
            var url = _unitOfWork.Urls.Get(id);
            if (url == null)
            {
                NotFound();
            }
            return Ok(url);
        }
        // PUT - Updates to a new originalUrl /api/Url/1
        [System.Web.Http.HttpPut]
        public IHttpActionResult Update(int id, UrlDto urlDto)
        {
            var urlInDb = _unitOfWork.Urls.Get(id);

            if (urlInDb == null)
            {
                return NotFound();
            }
            urlInDb.OriginalUrl = urlDto.OriginalUrl;
            try
            {
                _unitOfWork.Complete();
                return Ok(urlDto);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
        [System.Web.Http.HttpGet]
        //GET - List of all URLs in database /api/Url
        public IEnumerable<Url> GetUrls()
        {
            return _unitOfWork.Urls.GetAll();
        }
        //DELETE - Deletes the specified URL by id - /api/url/1
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var urlInDb = _unitOfWork.Urls.Get(id);

            if (urlInDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Urls.Remove(urlInDb);
            try
            {
                _unitOfWork.Complete();
                return Ok();
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Checks to see if the given url string contains http or https protocols.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool HasHttpProtocol(string url)
        {
            url = url.ToLower();
            if (url.Length > 5)
            {
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
