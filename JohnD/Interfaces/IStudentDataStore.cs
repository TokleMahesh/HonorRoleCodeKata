using System;
using System.Collections.Generic;
using System.Text;

namespace JohnD.Interfaces
{
    public interface IStudentDataStore
    {
        List<Student> GetAllStudents();
    }
}
