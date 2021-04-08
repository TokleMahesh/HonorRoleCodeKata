namespace JohnD
{
    public class Student
    {
        public Student(string name, float grade)
        {
            Name = name;
            Grade = grade;
        }
        public string Name { get; set; }
        public float Grade { get; set; }
    }
}