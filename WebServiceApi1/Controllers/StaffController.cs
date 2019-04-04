using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServiceApi1.Areas.RealTimeModule.Models;

namespace WebServiceApi1.Controllers
{
    public class StaffController : ApiController
    {
        public static List<Student> staffRepository = new List<Student>() {
                new Student() {ID="bbb",Name="Tom",Chinese=60,Mathematics=60,English=60,ClassNo=1 },
                new Student() {ID="ccc",Name="Jerry",Chinese=60,Mathematics=60,English=60,ClassNo=1 }
            };
        public IHttpActionResult Get()
        {
            return Json(staffRepository);
        }

        public IHttpActionResult Post(Student student)
        {
            return Ok("ok");
        }
    }
}
