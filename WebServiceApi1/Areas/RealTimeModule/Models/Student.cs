using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceApi1.Areas.RealTimeModule.Models
{
    public class Student
    {
        public string Name { get; set; }
        public string  ID { get; set; }
        public float Chinese { get; set; }
        public float Mathematics { get; set; }
        public float English { get; set; }
        public int ClassNo { get; set; }
    }
}