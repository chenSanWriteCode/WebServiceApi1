using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServiceApi1.Areas.RealTimeModule.Models;

namespace WebServiceApi1.Areas.RealTimeModule.Controllers
{
    public class RealTimeApiController : ApiController
    {
        public static List<Student> StudentRepository = new List<Student>() {
                new Student() {ID="abc",Name="Tom",Chinese=60,Mathematics=60,English=60,ClassNo=1 },
                new Student() {ID="abd",Name="Jerry",Chinese=60,Mathematics=60,English=60,ClassNo=1 }
            };
        public RealTimeApiController()
        {
            //var random = new Random();
            //StudentRepository = new List<Student>() {
            //    new Student() {ID="abc",Name="Tom",Chinese=60,Mathematics=60,English=60,ClassNo=1 },new Student() {ID="abd",Name="Jerry",Chinese=60,Mathematics=60,English=60,ClassNo=1 }
            //};

        }
        [Authorize]
        [HttpGet]
        public IHttpActionResult getAllStudents()
        {
            return Json<List<Student>>(StudentRepository);
        }
        [HttpGet]
        public IHttpActionResult getStudentByName(string name)
        {
            return Json<Student>(StudentRepository.FirstOrDefault(x => x.Name == name));
        }
        [HttpGet]
        public IHttpActionResult getStudentByNameAndClass(string name, int classNo)
        {
            return Json<Student>(StudentRepository.FirstOrDefault(x => x.Name == name && x.ClassNo == classNo));
        }
        [HttpPost]
        public IHttpActionResult add(Student model)
        {
            StudentRepository.Add(new Student { ID = new Guid().ToString(), Name = model.Name, ClassNo = model.ClassNo, Chinese = model.Chinese, Mathematics = model.Mathematics, English = model.English });
            return Ok<string>("添加成功");
        }
        [HttpPut]
        public IHttpActionResult updateStudent(Student model)
        {
            var model1 = StudentRepository.First(x => x.Name == model.Name);
            model1.ClassNo = model.ClassNo;
            model1.Chinese = model.Chinese;
            model1.Mathematics = model.Mathematics;
            model1.English = model.English;
            var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content= new ObjectContent<Student>(model1,Configuration.Formatters.JsonFormatter)
            };
            return Content(HttpStatusCode.NotFound, model1);
        }
        [HttpDelete]
        public IHttpActionResult delete([FromBody]string id)
        {

            return Ok("删除成功");
        }
    }
}
