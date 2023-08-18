using ASP.NETCoreMVCAPP.Controllers;
using ASP.NETCoreMVCAPP.Data;
using ASP.NETCoreMVCAPP.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ASP.NETCoreMVCAPPTests
{
    public class CoursesControllerTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _controller = new CoursesController(_mockCourseRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfCourses()
        {
            // Arrange
            List<Course> courses = getTestCourses();
            _mockCourseRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(courses);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
            Assert.Equal(courses.Count, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenNoCoursesAreAvailable()
        {
            // Arrange
            var courses = new List<Course>();
            _mockCourseRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(courses);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
            Assert.Empty(model);
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
        public async Task Details_ReturnsNotFoundResult_WhenCourseIsNull()
        {
            // Arrange
            int? id = 1;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task Details_ReturnsViewResult_WithCourse()
        {
            // Arrange
            int? id = 1;
            Course course = getTestCourses().First();
            _mockCourseRepository.Setup(repo => repo.GetByIdAsync(id.Value)).ReturnsAsync(course);

            // Act
            var result = await _controller.Details(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(id, model.CourseId);
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
            Course course = getTestCourses().First();
            _mockCourseRepository.Setup(repo => repo.CreateAsync(It.IsAny<Course>()));

            // Act
            var result = await _controller.Create(course) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockCourseRepository.Verify(repo => repo.CreateAsync(course), Times.Once);
        }
        
        [Fact]
        public async Task Create_ReturnViewResult_InvalidModel()
        {
            // Arrange
            Course course = getTestCourses().First();
            _mockCourseRepository.Setup(repo => repo.CreateAsync(course));
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(course) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.Equal(course, result.Model);
            _mockCourseRepository.Verify(repo => repo.CreateAsync(course), Times.Never);
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
        public async Task Edit_ReturnsNotFound_WhenCourseIsNull()
        {
            // Arrange
            int? id = 1;
            _mockCourseRepository.Setup(repo => repo.GetByIdAsync(id.Value));

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdAndCourseIdAreNotEqual()
        {
            // Arrange
            int id = 1;
            int nonexistentId = getTestCourses().Last().CourseId + 1;
            Course course = getTestCourses().First();
            _mockCourseRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(course);

            // Act
            var result = await _controller.Edit(nonexistentId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenIdAndCourseAreNotNull()
        {
            // Arrange
            int? id = 1;
            List<Course> courses = getTestCourses();
            _mockCourseRepository.Setup(repo => repo.GetByIdAsync(id.Value)).ReturnsAsync(courses.First());

            // Act
            var result = await _controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(courses.First(), model);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            Course course = getTestCourses().First();
            int courseId = course.CourseId;
            _mockCourseRepository.Setup(repo => repo.UpdateAsync(course)).Verifiable();

            // Act
            var result = await _controller.Edit(courseId, course) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockCourseRepository.Verify(repo => repo.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFoundResult_WhenIdAndCourseIdAreNotEqual()
        {
            // Arrange
            int nonexistentId = getTestCourses().Last().CourseId + 1;
            Course course = getTestCourses().First();

            // Act
            var result = await _controller.Edit(nonexistentId, course);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCourseRepository.Verify(repo => repo.UpdateAsync(course), Times.Never);
        }

        [Fact]
        public async Task EditPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            Course course = getTestCourses().First();
            int id = course.CourseId;
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Edit(id, course);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(course, viewResult.Model);
            _mockCourseRepository.Verify(repo => repo.UpdateAsync(course), Times.Never);
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
        public async Task Delete_ReturnsNotFoundResult_WhenNoCoursesAreAvailable()
        {
            // Arrange
            int? id = 1;
            _mockCourseRepository.Setup(repo => repo.GetCourseWithGroupsByIdAsync(id.Value));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenCourseIsNotFound()
        {
            // Arrange
            int id = 1;
            int? nonexistentId = getTestCourses().Last().CourseId + 1;
            var courses = getTestCourses();

            _mockCourseRepository.Setup(repo => repo.GetCourseWithGroupsByIdAsync(id)).ReturnsAsync(courses.First());

            // Act
            var result = await _controller.Delete(nonexistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenCourseIsFound()
        {
            // Arrange
            int? id = 1;
            var courses = getTestCourses();
            _mockCourseRepository.Setup(repo => repo.GetCourseWithGroupsByIdAsync(id.Value)).ReturnsAsync(courses.First());

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Course>(viewResult.ViewData.Model);
            Assert.Equal(id, model.CourseId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_RemovesCourseAndRedirectsToIndex()
        {
            // Arrange
            Course course = getTestCourses().Last();
            int id = course.CourseId;
            _mockCourseRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(course);
            _mockCourseRepository.Setup(repo => repo.DeleteAsync(course)).Verifiable();

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.Index), viewResult.ActionName);
            _mockCourseRepository.Verify(repo => repo.DeleteAsync(course), Times.Once());
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsNotFoundResult_WhenCourseIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCourseRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Course>()), Times.Never);
        }

        private List<Course> getTestCourses()
        {
            var courses = new List<Course>
            {
                new Course { CourseId = 1, Name = "Course 1", Description = "Description 1" },
                new Course { CourseId = 2, Name = "Course 2", Description = "Description 2" },
                new Course { CourseId = 3, Name = "Course 3", Description = "Description 3" }
            };
            return courses;
        }
    }
}
