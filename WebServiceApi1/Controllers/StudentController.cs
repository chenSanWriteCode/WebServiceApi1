using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebServiceApi1.Areas.RealTimeModule.Models;

namespace WebServiceApi1.Controllers
{

    [RoutePrefix("api/student/info")]
    public class StudentController : ApiController
    {
        public static List<Student> StudentRepository = new List<Student>() {
                new Student() {ID="abc",Name="Tom",Chinese=60,Mathematics=60,English=60,ClassNo=1 },
                new Student() {ID="abd",Name="Jerry",Chinese=60,Mathematics=60,English=60,ClassNo=1 }
            };

        public IHttpActionResult Get()
        {
            return Json(StudentRepository);
        }

        public IHttpActionResult Post(Student student)
        {
            return Ok("ok");
        }

    }
}
