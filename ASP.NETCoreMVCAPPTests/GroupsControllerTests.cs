using ASP.NETCoreMVCAPP.Controllers;
using ASP.NETCoreMVCAPP.Data;
using ASP.NETCoreMVCAPP.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ASP.NETCoreMVCAPPTests
{
    public class GroupsControllerTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly GroupsController _controller;

        public GroupsControllerTests()
        {
            _mockGroupRepository = new Mock<IGroupRepository>();
            _controller = new GroupsController(_mockGroupRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfGroups()
        {
            // Arrange
            List<Group> groups = getTestGroups();
            _mockGroupRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(groups);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData.Model);
            Assert.Equal(groups.Count, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenNoGroupsAreAvailable()
        {
            // Arrange
            var groups = new List<Group>();
            _mockGroupRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(groups);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task ListGroupsForCourse_ReturnsViewResult_WithListOfGroups()
        {
            // Arrange
            List<Group> groups = getTestGroups();
            int courseId = 10;
            _mockGroupRepository.Setup(repo => repo.GetGroupsWithCourseForCourseAsync(courseId)).ReturnsAsync(groups);

            // Act
            var result = await _controller.ListGroupsForCourse(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData.Model);
            Assert.Equal(groups.First().CourseId, model.First().CourseId);
            Assert.Equal(groups.Count, model.Count());
        }

        [Fact]
        public async Task ListGroupsForCourse_ReturnsViewResult_WhenNoGroupsAreAvailable()
        {
            // Arrange
            var groups = new List<Group>();
            int courseId = 20;
            _mockGroupRepository.Setup(repo => repo.GetGroupsWithCourseForCourseAsync(courseId)).ReturnsAsync(groups);

            // Act
            var result = await _controller.ListGroupsForCourse(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task ListGroupsForCourse_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.ListGroupsForCourse(id);

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
        public async Task Details_ReturnsNotFoundResult_WhenGroupIsNull()
        {
            // Arrange
            int? id = 1;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithGroup()
        {
            // Arrange
            int? id = 1;
            Group group = getTestGroups().First();
            _mockGroupRepository.Setup(repo => repo.GetGroupWithCourseByIdAsync(id.Value)).ReturnsAsync(group);

            // Act
            var result = await _controller.Details(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Group>(viewResult.ViewData.Model);
            Assert.Equal(id, model.GroupId);
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
            Group group = getTestGroups().First();
            _mockGroupRepository.Setup(repo => repo.CreateAsync(It.IsAny<Group>()));

            // Act
            var result = await _controller.Create(group) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockGroupRepository.Verify(repo => repo.CreateAsync(group), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnViewResult_InvalidModel()
        {
            // Arrange
            Group group = getTestGroups().First();
            _mockGroupRepository.Setup(repo => repo.CreateAsync(group));
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(group) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.Equal(group, result.Model);
            _mockGroupRepository.Verify(repo => repo.CreateAsync(group), Times.Never);
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
        public async Task Edit_ReturnsNotFound_WhenGroupIsNull()
        {
            // Arrange
            int? id = 1;
            _mockGroupRepository.Setup(repo => repo.GetByIdAsync(id.Value));

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdAndGroupIdAreNotEqual()
        {
            // Arrange
            int id = 1;
            int nonexistentId = getTestGroups().Last().GroupId + 1;
            Group group = getTestGroups().First();
            _mockGroupRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(group);

            // Act
            var result = await _controller.Edit(nonexistentId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenIdAndGroupAreNotNull()
        {
            // Arrange
            int? id = 1;
            List<Group> groups = getTestGroups();
            _mockGroupRepository.Setup(repo => repo.GetByIdAsync(id.Value)).ReturnsAsync(groups.First());

            // Act
            var result = await _controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Group>(viewResult.ViewData.Model);
            Assert.Equal(groups.First(), model);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            Group group = getTestGroups().First();
            int groupId = group.GroupId;
            _mockGroupRepository.Setup(repo => repo.UpdateAsync(group)).Verifiable();

            // Act
            var result = await _controller.Edit(groupId, group) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _mockGroupRepository.Verify(repo => repo.UpdateAsync(group), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFoundResult_WhenIdAndGroupIdAreNotEqual()
        {
            // Arrange
            int nonexistentId = getTestGroups().Last().GroupId + 1;
            Group group = getTestGroups().First();

            // Act
            var result = await _controller.Edit(nonexistentId, group);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGroupRepository.Verify(repo => repo.UpdateAsync(group), Times.Never);
        }

        [Fact]
        public async Task EditPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            Group group = getTestGroups().First();
            int id = group.GroupId;
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Edit(id, group);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(group, viewResult.Model);
            _mockGroupRepository.Verify(repo => repo.UpdateAsync(group), Times.Never);
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
        public async Task Delete_ReturnsNotFoundResult_WhenNoGroupsAreAvailable()
        {
            // Arrange
            int? id = 1;
            _mockGroupRepository.Setup(repo => repo.GetGroupWithCourseAndStudentsByIdAsync(id.Value));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenGroupIsNotFound()
        {
            // Arrange
            int id = 1;
            int? nonexistentId = getTestGroups().Last().GroupId + 1;
            var groups = getTestGroups();

            _mockGroupRepository.Setup(repo => repo.GetGroupWithCourseAndStudentsByIdAsync(id)).ReturnsAsync(groups.First());

            // Act
            var result = await _controller.Delete(nonexistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenGroupIsFound()
        {
            // Arrange
            int? id = 1;
            var groups = getTestGroups();
            _mockGroupRepository.Setup(repo => repo.GetGroupWithCourseAndStudentsByIdAsync(id.Value)).ReturnsAsync(groups.First());

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.ViewData.Model);
            Assert.Equal(id, model.GroupId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_RemovesGroupAndRedirectsToIndex()
        {
            // Arrange
            Group group = getTestGroups().Last();
            int id = group.GroupId;
            _mockGroupRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(group);
            _mockGroupRepository.Setup(repo => repo.DeleteAsync(group)).Verifiable();

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(GroupsController.Index), viewResult.ActionName);
            _mockGroupRepository.Verify(repo => repo.DeleteAsync(group), Times.Once());
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsNotFoundResult_WhenGroupIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGroupRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Group>()), Times.Never);
        }

        private List<Group> getTestGroups()
        {
            var groups = new List<Group>
            {
                new Group { GroupId = 1, Name = "Group 1", CourseId = 10, Course = new Course { CourseId = 10 } },
                new Group { GroupId = 2, Name = "Group 2", CourseId = 10, Course = new Course { CourseId = 10 } },
                new Group { GroupId = 3, Name = "Group 3", CourseId = 10, Course = new Course { CourseId = 10 } }
            };
            return groups;
        }
    }
}