using JohnD.Interfaces;
using JohnD.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JohnD.TestSuit
{
    [TestClass]
    public class StudentGradeManagerFixture
    {
        [TestMethod]
        [ExpectedException(typeof(DependancyNotConfigured), "Student data store is not configured")]
        public void ShouldThrow_DependancyNotConfiguredException()
        {
            //Setup
            var gradeManager = new StudentGradeManager(null);

            //Execute
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();
        }

        #region GetAllGraduatedStudentNames Tests
        [TestMethod]
        [ExpectedException(typeof(DataRetrievalError), "Error while accessing student data store")]
        public void GetAllGraduatedStudentNames_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Throws(new DataRetrievalError());
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();
        }

        [TestMethod]
        [ExpectedException(typeof(NoStudentsInClassException), "There are no students in class")]
        public void GetAllGraduatedStudentNames_WithNoStudents_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns<List<Student>>(null);
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetAllGraduatedStudentNames();
        }

        [TestMethod]
        public void GetAllGraduatedStudentNames_ValidateGPAComparison_LessThanRequired()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var student = new Student("Student1", 1.0F);
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>() { student });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();

            //Verify
            Assert.IsTrue(graduatedStudents != null, "GetAllGraduatedStudentNames returned null");
            Assert.IsTrue(graduatedStudents.Count == 0, "Student not eligible for graduation returned");
        }

        [TestMethod]
        public void GetAllGraduatedStudentNames_ValidateGPAComparison_EqualRequired()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var student = new Student("Student1", 2.0F);
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>() { student });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();

            //Verify
            Assert.IsTrue(graduatedStudents != null, "GetAllGraduatedStudentNames returned null");
            Assert.IsTrue(graduatedStudents.Count == 1, "Student eligible for graduation not returned");
            Assert.IsTrue(string.Equals(graduatedStudents.First(), student.Name), "Student eligible for graduation not returned");
        }

        [TestMethod]
        public void GetAllGraduatedStudentNames_ValidateGPAComparison_GreatherThanRequired()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var student = new Student("Student1", 2.01F);
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>() { student });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();

            //Verify
            Assert.IsTrue(graduatedStudents != null, "GetAllGraduatedStudentNames returned null");
            Assert.IsTrue(graduatedStudents.Count == 1, "Student eligible for graduation not returned");
            Assert.IsTrue(string.Equals(graduatedStudents.First(), student.Name), "Student eligible for graduation not returned");
        }

        [TestMethod]
        public void GetAllGraduatedStudentNames_Negative_ShouldNotReturnGraduatedStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 1.9F), //Not Graduated
                new Student("John 2", 1.4F), //Not Graduated
            });

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();


            //Verify
            Assert.IsTrue(graduatedStudents != null, "GetAllGraduatedStudentNames failed");
            Assert.IsTrue(graduatedStudents.Count == 0, "None of the students graduated!");
        }

        [TestMethod]
        public void GetAllGraduatedStudentNames_ShouldReturnGraduatedStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var students = new List<Student>()
            {
                new Student("John 1", 3.4F), //Graduated
                new Student("John 2", 3.5F), //Cum Laude
                new Student("John 3", 3.9F), //Summa Cum Laude
                new Student("John 4", 4.0F), //Summa Cum Laude
            };
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(students);

            var expectedGraduatedStudents = students.FindAll(s => s.Grade >= 2.0);

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var graduatedStudents = gradeManager.GetAllGraduatedStudentNames();


            //Verify
            Assert.IsTrue(graduatedStudents != null, "GetAllGraduatedStudentNames failed");
            Assert.IsTrue(graduatedStudents.Count == graduatedStudents.Count, "Graduated students count is not matching!");
            foreach (var student in expectedGraduatedStudents)
            {
                Assert.IsTrue(graduatedStudents.Exists(s => string.Equals(s, student.Name, System.StringComparison.OrdinalIgnoreCase)), $"Student {student.Name} should have graduated!");
            }

            foreach (var student in graduatedStudents)
            {
                Assert.IsTrue(expectedGraduatedStudents.Exists(s => string.Equals(s.Name, student, System.StringComparison.OrdinalIgnoreCase)), $"Student {student} should not be graduated!");
            }
        }
        #endregion

        #region GetAllCumLaudeStudentNames Tests
        [TestMethod]
        [ExpectedException(typeof(DataRetrievalError), "Error while accessing student data store")]
        public void GetAllCumLaudeStudentNames_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Throws(new DataRetrievalError());
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllCumLaudeStudentNames();
        }

        [TestMethod]
        [ExpectedException(typeof(NoStudentsInClassException), "There are no students in class")]
        public void GetAllCumLaudeStudentNames_WithNoStudents_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns<List<Student>>(null);
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetAllCumLaudeStudentNames();
        }

        [TestMethod]
        public void GetAllCumLaudeStudentNames_Negative_ShouldNotReturnCumLaudeStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 3.4F), //Graduated
                new Student("John 2", 3.1F), //Graduated
            });

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var cumLaudeStudents = gradeManager.GetAllCumLaudeStudentNames();

            //Verify
            Assert.IsTrue(cumLaudeStudents != null, "GetAllCumLaudeStudentNames failed");
            Assert.IsTrue(cumLaudeStudents.Count == 0, "There should not be any students with Cum Laude");
        }

        [TestMethod]
        public void GetAllCumLaudeStudentNames_Negative_WithSummaCumLaudeOnly_ShouldNotReturnCumLaudeStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 3.4F), //Graduated
                new Student("John 3", 3.9F), //Summa Cum Laude
                new Student("John 4", 4.0F), //Summa Cum Laude
            });

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var cumLaudeStudents = gradeManager.GetAllCumLaudeStudentNames();

            //Verify
            Assert.IsTrue(cumLaudeStudents != null, "GetAllCumLaudeStudentNames failed");
            Assert.IsTrue(cumLaudeStudents.Count == 0, "There should not be any students with Cum Laude");
        }

        [TestMethod]
        public void GetAllCumLaudeStudentNames_ShouldReturnCumLaudeStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var mockStudents = new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 3.4F), //Graduated
                new Student("John 2", 3.5F), //Cum Laude
                new Student("John 3", 3.9F), //Summa Cum Laude
                new Student("John 4", 4.0F), //Summa Cum Laude
            };
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(mockStudents);
            var expectedCumLaudeStudents = mockStudents.FindAll(s => s.Grade >= 3.5 && s.Grade < 3.9F);

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var cumLaudeStudents = gradeManager.GetAllCumLaudeStudentNames();


            //Verify
            Assert.IsTrue(cumLaudeStudents != null, "GetAllCumLaudeStudentNames failed");
            Assert.IsTrue(cumLaudeStudents.Count > 0, "There should be students with Cum Laude");
            foreach (var student in expectedCumLaudeStudents) //Verify if all expected cum laude students are returned
            {
                Assert.IsTrue(cumLaudeStudents.Exists(s => string.Equals(s, student.Name, System.StringComparison.OrdinalIgnoreCase)), $"Student {student.Name} should have got Cum Laude!");
            }

            foreach (var student in cumLaudeStudents) //Verify if there are only cum laude students are returned
            {
                Assert.IsTrue(expectedCumLaudeStudents.Exists(s => string.Equals(s.Name, student, System.StringComparison.OrdinalIgnoreCase)), $"Student {student} should not get Cum Laude!");
            }
        }
        #endregion

        #region GetAllSummaCumLaudeStudentNames Tests
        [TestMethod]
        [ExpectedException(typeof(DataRetrievalError), "Error while accessing student data store")]
        public void GetAllSummaCumLaudeStudentNames_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Throws(new DataRetrievalError());
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var graduatedStudents = gradeManager.GetAllSummaCumLaudeStudentNames();
        }

        [TestMethod]
        [ExpectedException(typeof(NoStudentsInClassException), "There are no students in class")]
        public void GetAllSummaCumLaudeStudentNames_WithNoStudents_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns<List<Student>>(null);
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetAllSummaCumLaudeStudentNames();
        }

        [TestMethod]
        public void GetAllSummaCumLaudeStudentNames_Negative_ShouldNotReturnSummaCumLaudeStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 1.5F), //Not Graduated
                new Student("John 2", 1.95F), //Not Graduated
            });

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var summalCumLaudeStudents = gradeManager.GetAllSummaCumLaudeStudentNames();

            //Verify
            Assert.IsTrue(summalCumLaudeStudents != null, "GetAllSummaCumLaudeStudentNames failed");
            Assert.IsTrue(summalCumLaudeStudents.Count == 0, "There should not be any students with Summa Cum Laude");
        }

        [TestMethod]
        public void GetAllSummaCumLaudeStudentNames_ShouldReturnSummaCumLaudeStudents_ExactlyTwoStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var mockStudents = new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 2.0F), //Graduated
                new Student("John 2", 3.5F), //Cum Laude
                new Student("John 3", 3.9F), //Summa Cum Laude
                new Student("John 4", 4.0F), //Summa Cum Laude
            };
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(mockStudents);
            var expectedSummaCumLaudeStudents = mockStudents.FindAll(s => s.Grade >= 3.9F);

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var cumLaudeStudents = gradeManager.GetAllSummaCumLaudeStudentNames();


            //Verify
            Assert.IsTrue(cumLaudeStudents != null, "GetAllSummaCumLaudeStudentNames failed");
            Assert.IsTrue(cumLaudeStudents.Count > 0, "There should be students with Summa Cum Laude");
            Assert.IsTrue(cumLaudeStudents.Count == 2, "There should be only 2 students with Summa Cum Laude");
            foreach (var student in expectedSummaCumLaudeStudents) //Verify if all expected summa cum laude students are returned
            {
                Assert.IsTrue(cumLaudeStudents.Exists(s => string.Equals(s, student.Name, System.StringComparison.OrdinalIgnoreCase)), $"Student {student.Name} should have got Summa Cum Laude!");
            }

            foreach (var student in cumLaudeStudents) //Verify only the summa cum laude students are returned
            {
                Assert.IsTrue(expectedSummaCumLaudeStudents.Exists(s => string.Equals(s.Name, student, System.StringComparison.OrdinalIgnoreCase)), $"Student {student} should not get Summa Cum Laude!");
            }
        }

        [TestMethod]
        public void GetAllSummaCumLaudeStudentNames_ShouldReturnSummaCumLaudeStudents_MoreThanTwoStudents()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            var mockStudents = new List<Student>()
            {
                new Student("John", 1.9F),   //Not Graduated
                new Student("John 0", 1.4F), //Not Graduated
                new Student("John 1", 3.4F), //Graduated
                new Student("John 2", 3.5F), //Cum Laude
                new Student("John 3", 3.9F), //Summa Cum Laude
                new Student("John 4", 3.9F), //Summa Cum Laude
                new Student("John 5", 4.0F), //Summa Cum Laude
                new Student("John 6", 4.0F), //Summa Cum Laude
            };
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(mockStudents);
            var expectedSummaCumLaudeStudents = mockStudents.FindAll(s => s.Grade >= 3.9F);

            //Execute
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);
            var cumLaudeStudents = gradeManager.GetAllSummaCumLaudeStudentNames();


            //Verify
            Assert.IsTrue(cumLaudeStudents != null, "GetAllSummaCumLaudeStudentNames failed");
            Assert.IsTrue(cumLaudeStudents.Count > 0, "There should be students with Summa Cum Laude");
            Assert.IsTrue(cumLaudeStudents.Count == 4, "There should be more than 2 students with Summa Cum Laude");
            foreach (var student in expectedSummaCumLaudeStudents) //Verify if all expected summa cum laude students are returned
            {
                Assert.IsTrue(cumLaudeStudents.Exists(s => string.Equals(s, student.Name, System.StringComparison.OrdinalIgnoreCase)), $"Student {student.Name} should have got Summa Cum Laude!");
            }

            foreach (var student in cumLaudeStudents) //Verify only the summa cum laude students are returned
            {
                Assert.IsTrue(expectedSummaCumLaudeStudents.Exists(s => string.Equals(s.Name, student, System.StringComparison.OrdinalIgnoreCase)), $"Student {student} should not get Summa Cum Laude!");
            }
        }
        #endregion

        #region Report tests
        [TestMethod]
        [ExpectedException(typeof(DataRetrievalError), "Error while accessing student data store")]
        public void GetClassGPAReport_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Throws(new DataRetrievalError());
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();
        }

        [TestMethod]
        [ExpectedException(typeof(NoStudentsInClassException), "There are no students in class")]
        public void GetClassGPAReport_WithNoStudents_ShouldThrowException()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns<List<Student>>(null);
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();
        }

        [TestMethod]
        public void GetClassGPAReport_Above3GPA_Negative_ShouldReturnZero()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 2.4F), 
                new Student("John 2", 2.0F), 
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Above3GPAStudentCount == 0, "Above 3 GPA student count should be zero");
            Assert.IsTrue(report.Above3GPAAverage == 0, "Above 3 GPA students average GPA should be zero");
        }

        [TestMethod]
        public void GetClassGPAReport_Above3GPA_WithExclusion()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 3.0F), //should exclude
                new Student("John 2", 3.4F),
                new Student("John 3", 3.4F),
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Above3GPAStudentCount == 2, "There should be only 2 students above 3 GPA");
            Assert.IsTrue(report.Above3GPAAverage == 3.4F, "Above 3 GPA students average GPA is not correct");
        }

        [TestMethod]
        public void GetClassGPAReport_Above2GPA_Negative_ShouldReturnZero()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 1.4F),
                new Student("John 2", 1.0F),
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Above2GPAStudentCount == 0, "Above 2 GPA student count should be zero");
            Assert.IsTrue(report.Above2GPAAverage == 0, "Above 2 GPA students average GPA should be zero");
        }

        [TestMethod]
        public void GetClassGPAReport_Above2GPA_WithInclusion()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 1.0F), 
                new Student("John 2", 2.0F), //should include
                new Student("John 3", 2.6F),
                new Student("John 3", 2.4F),
                new Student("John 4", 3.0F), //should include
                new Student("John 5", 3.4F), 
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Above2GPAStudentCount == 4, "There should be only 4 students above 2 GPA");
            Assert.IsTrue(report.Above2GPAAverage == 2.5F, "Above 2 GPA students average is not correct");
        }

        [TestMethod]
        public void GetClassGPAReport_Below2GPA_Negative_ShouldReturnZero()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 3.4F),
                new Student("John 2", 3.0F),
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Below2GPAStudentCount == 0, "Below 2 GPA student count should be zero");
            Assert.IsTrue(report.Below2GPAAverage == 0, "Below 2 GPA students average GPA should be zero");
        }

        [TestMethod]
        public void GetClassGPAReport_Below2GPA_WithExclusion()
        {
            //Setup
            var mockStudentStore = new Mock<IStudentDataStore>();
            mockStudentStore.Setup(store => store.GetAllStudents()).Returns(new List<Student>()
            {
                new Student("John 1", 1.0F),
                new Student("John 2", 2.0F), //should exclude
                new Student("John 3", 1.0F),
            });
            var gradeManager = new StudentGradeManager(mockStudentStore.Object);

            //Execute
            var report = gradeManager.GetClassGPAReport();

            //Verify
            Assert.IsTrue(report != null, "Report should be returned!");
            Assert.IsTrue(report.Below2GPAStudentCount == 2, "There should be only 2 students below 2 GPA");
            Assert.IsTrue(report.Below2GPAAverage == 1.0F, "Below 2 GPA students average is not correct");
        }
        #endregion
    }
}
