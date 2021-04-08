using System;
using System.Collections.Generic;
using System.Text;

namespace JohnD
{
    public class GradeReport
    {
        public int Above3GPAStudentCount { get; set; }
        public float Above3GPAAverage { get; set; }
        public int Above2GPAStudentCount { get; set; }
        public float Above2GPAAverage { get; set; }
        public int Below2GPAStudentCount { get; set; }
        public float Below2GPAAverage { get; set; }
    }
}
