﻿using DFE.SIP.API.SharePointOnline.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;

namespace DFE.SIP.API.SharePointOnline.Controllers
{
    
    public class ValuesController : ApiController
    {
        // GET api/values


         [CustomAuthorize("SpContributor")]
        public IEnumerable<string> Get()
        {

             

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
