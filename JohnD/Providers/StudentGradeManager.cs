using JohnD.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnD.Providers
{
    public class StudentGradeManager : IStudentGradeManager
    {
        private IStudentDataStore _store = null;
        public StudentGradeManager(IStudentDataStore store)
        {
            if (store == null) throw new DependancyNotConfigured();
            _store = store;
        }
        public List<string> GetAllGraduatedStudentNames()
        {
            var students = _store.GetAllStudents();
            if (students == null || students.Count == 0) throw new NoStudentsInClassException();

            return students.FindAll(s => s.Grade >= 2.0).ConvertAll(s => s.Name);
        }

        public List<string> GetAllCumLaudeStudentNames()
        {
            var students = _store.GetAllStudents();
            if (students == null || students.Count == 0) throw new NoStudentsInClassException();

            var cumLaudeStudents = students
                .FindAll(s => s.Grade >= 3.5)
                .Select(s => s.Name).ToList();

            //Exclude students who got Summa Cum Laude
            var summaCumLaudeStudents = GetAllSummaCumLaudeStudentNames();
            cumLaudeStudents.RemoveAll(cumLaude => summaCumLaudeStudents.Exists(summaCumLaude => string.Equals(cumLaude, summaCumLaude)));

            return cumLaudeStudents;
        }

        //Assuming there is no criteria for Summa Cum Laude for the minimum GPA. i.e. There will be always Summa Cum Laude students amoung the graduated students(>2.0 GPA)
        public List<string> GetAllSummaCumLaudeStudentNames()
        {
            var students = _store.GetAllStudents();

            if (students == null || students.Count == 0) throw new NoStudentsInClassException();

            //Get the minimum GPA for Summa Cum Laude 
            float firstHighestGPA = -1;
            float secondHighestGPA = -1;
            foreach(var s in students)
            {
                if(firstHighestGPA < s.Grade)
                {
                    secondHighestGPA = firstHighestGPA;
                    firstHighestGPA = s.Grade;
                }
            }

            //Return all students who are eligible for graduation and with GPA >= second highest student.
            return students.FindAll(s => s.Grade >= 2.0 && s.Grade >= secondHighestGPA).Select(s => s.Name).ToList();
        }

        public GradeReport GetClassGPAReport()
        {
            var report = new GradeReport();
            var students = _store.GetAllStudents();

            if (students == null || students.Count == 0) throw new NoStudentsInClassException();

            //Set above 3 GPA stats
            var above3GPAStudents = students.FindAll(s => s.Grade > 3F);
            report.Above3GPAStudentCount = above3GPAStudents.Count;
            report.Above3GPAAverage = above3GPAStudents.Any() ? above3GPAStudents.Average(s => s.Grade) : 0;

            //Set above 2 GPA stats
            var above2GPAStudents = students.FindAll(s => s.Grade >= 2F && s.Grade <= 3.0F);
            report.Above2GPAStudentCount = above2GPAStudents.Count;
            report.Above2GPAAverage = above2GPAStudents.Any() ? above2GPAStudents.Average(s => s.Grade) : 0;

            //Set below 2 GPA stats
            var below2GPAStudents = students.FindAll(s => s.Grade < 2F);
            report.Below2GPAStudentCount = below2GPAStudents.Count;
            report.Below2GPAAverage = below2GPAStudents.Any() ? below2GPAStudents.Average(s => s.Grade) : 0;

            return report;
        }
    }
}
