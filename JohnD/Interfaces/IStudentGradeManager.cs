using System;
using System.Collections.Generic;
using System.Text;

namespace JohnD.Interfaces
{
    public interface IStudentGradeManager
    {
        List<string> GetAllGraduatedStudentNames();
        List<string> GetAllCumLaudeStudentNames();
        GradeReport GetClassGPAReport();
    }
}
