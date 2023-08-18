using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NETCoreMVCAPP.Data;
using ASP.NETCoreMVCAPP.Models;

namespace ASP.NETCoreMVCAPP.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var groups = await _groupRepository.GetAllAsync();
            return View(groups);
        }

        public async Task<IActionResult> ListGroupsForCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _groupRepository.GetGroupsWithCourseForCourseAsync(id.Value);
            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetGroupWithCourseByIdAsync(id.Value);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_groupRepository.GetCourses(), "CourseId", "Name");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,CourseId,Name")] Group @group)
        {
            if (ModelState.IsValid)
            {
                await _groupRepository.CreateAsync(@group);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_groupRepository.GetCourses(), "CourseId", "Name", @group.CourseId);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetByIdAsync(id.Value);
            if (@group == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_groupRepository.GetCourses(), "CourseId", "Name", @group.CourseId);
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,CourseId,Name")] Group @group)
        {
            if (id != @group.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _groupRepository.UpdateAsync(@group);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_groupRepository.GetCourses(), "CourseId", "Name", @group.CourseId);
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetGroupWithCourseAndStudentsByIdAsync(id.Value);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _groupRepository.GetByIdAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            
            await _groupRepository.DeleteAsync(@group);
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
          return _groupRepository.IsExists(id);
        }
    }
}
