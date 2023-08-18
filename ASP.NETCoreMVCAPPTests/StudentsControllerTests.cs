using ASP.NETCoreMVCAPP.Controllers;
using ASP.NETCoreMVCAPP.Data;
using ASP.NETCoreMVCAPP.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ASP.NETCoreMVCAPPTests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _controller = new StudentsController(_mockStudentRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfStudents()
        {
            // Arrange
            List<Student> students = getTestStudents();
            _mockStudentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(students);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Equal(students.Count, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenNoStudentsAreAvailable()
        {
            // Arrange
            var groups = new List<Student>();
            _mockStudentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(groups);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task ListStudentsForGroup_ReturnsViewResult_WithListOfStudents()
        {
            // Arrange
            List<Student> students = getTestStudents();
            int groupId = 10;
            _mockStudentRepository.Setup(repo => repo.GetStudentsWithGroupForGroupAsync(groupId)).ReturnsAsync(students);

            // Act
            var result = await _controller.ListStudentsForGroup(groupId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Equal(students.First().GroupId, model.First().GroupId);
            Assert.Equal(students.Count, model.Count());
        }

        [Fact]
        public async Task ListStudentsForGroup_ReturnsViewResult_WhenNoStudentsAreAvailable()
        {
            // Arrange
            var students = new List<Student>();
            int groupId = 20;
            _mockStudentRepository.Setup(repo => repo.GetStudentsWithGroupForGroupAsync(groupId)).ReturnsAsync(students);

            // Act
            var result = await _controller.ListStudentsForGroup(groupId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task ListStudentsForGroup_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.ListStudentsForGroup(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenStudentIsNull()
        {
            // Arrange
            int? id = 1;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithStudent()
        {
            // Arrange
            int? id = 1;
            Student student = getTestStudents().First();
            _mockStudentRepository.Setup(repo => repo.GetStudentWithGroupByIdAsync(id.Value)).ReturnsAsync(student);

            // Act
            var result = await _controller.Details(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Student>(viewResult.ViewData.Model);
            Assert.Equal(id, model.StudentId);
        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName));
        }

        [Fact]
        public async Task Create_ReturnRedirectToIndex_ValidModel()
        {
            // Arrange
            Student student = getTestStudents().First();
            _mockStudentRepository.Setup(repo => repo.CreateAsync(It.IsAny<Student>()));

            // Act
            var result = await _controller.Create(student) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockStudentRepository.Verify(repo => repo.CreateAsync(student), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnViewResult_InvalidModel()
        {
            // Arrange
            Student student = getTestStudents().First();
            _mockStudentRepository.Setup(repo => repo.CreateAsync(student));
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(student) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.Equal(student, result.Model);
            _mockStudentRepository.Verify(repo => repo.CreateAsync(student), Times.Never);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenStudentIsNull()
        {
            // Arrange
            int? id = 1;
            _mockStudentRepository.Setup(repo => repo.GetByIdAsync(id.Value));

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdAndStudentIdAreNotEqual()
        {
            // Arrange
            int id = 1;
            int nonexistentId = getTestStudents().Last().StudentId + 1;
            Student student = getTestStudents().First();
            _mockStudentRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(student);

            // Act
            var result = await _controller.Edit(nonexistentId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenIdAndStudentAreNotNull()
        {
            // Arrange
            int? id = 1;
            List<Student> students = getTestStudents();
            _mockStudentRepository.Setup(repo => repo.GetByIdAsync(id.Value)).ReturnsAsync(students.First());

            // Act
            var result = await _controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Student>(viewResult.ViewData.Model);
            Assert.Equal(students.First(), model);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            Student student = getTestStudents().First();
            int studentId = student.StudentId;
            _mockStudentRepository.Setup(repo => repo.UpdateAsync(student)).Verifiable();

            // Act
            var result = await _controller.Edit(studentId, student) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockStudentRepository.Verify(repo => repo.UpdateAsync(student), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFoundResult_WhenIdAndStudentIdAreNotEqual()
        {
            // Arrange
            int nonexistentId = getTestStudents().Last().StudentId + 1;
            Student student = getTestStudents().First();

            // Act
            var result = await _controller.Edit(nonexistentId, student);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockStudentRepository.Verify(repo => repo.UpdateAsync(student), Times.Never);
        }

        [Fact]
        public async Task EditPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            Student student = getTestStudents().First();
            int id = student.StudentId;
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Edit(id, student);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(student, viewResult.Model);
            _mockStudentRepository.Verify(repo => repo.UpdateAsync(student), Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenNoStudentsAreAvailable()
        {
            // Arrange
            int? id = 1;
            _mockStudentRepository.Setup(repo => repo.GetStudentWithGroupByIdAsync(id.Value));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenStudentIsNotFound()
        {
            // Arrange
            int id = 1;
            int? nonexistentId = getTestStudents().Last().StudentId + 1;
            var students = getTestStudents();

            _mockStudentRepository.Setup(repo => repo.GetStudentWithGroupByIdAsync(id)).ReturnsAsync(students.First());

            // Act
            var result = await _controller.Delete(nonexistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenStudentIsFound()
        {
            // Arrange
            int? id = 1;
            var students = getTestStudents();
            _mockStudentRepository.Setup(repo => repo.GetStudentWithGroupByIdAsync(id.Value)).ReturnsAsync(students.First());

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Student>(viewResult.ViewData.Model);
            Assert.Equal(id, model.StudentId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_RemovesStudentAndRedirectsToIndex()
        {
            // Arrange
            Student student = getTestStudents().Last();
            int id = student.StudentId;
            _mockStudentRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(student);
            _mockStudentRepository.Setup(repo => repo.DeleteAsync(student)).Verifiable();

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(StudentsController.Index), viewResult.ActionName);
            _mockStudentRepository.Verify(repo => repo.DeleteAsync(student), Times.Once());
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsNotFoundResult_WhenStudentIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockStudentRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Student>()), Times.Never);
        }

        private List<Student> getTestStudents()
        {
            var students = new List<Student>
            {
                new Student { StudentId = 1, FirstName = "First Name Student 1", LastName = "Last Name Student 1", GroupId = 10 },
                new Student { StudentId = 2, FirstName = "First Name Student 2", LastName = "Last Name Student 2", GroupId = 10 },
                new Student { StudentId = 3, FirstName = "First Name Student 3", LastName = "Last Name Student 3", GroupId = 10 },
            };
            return students;
        }
    }
}
