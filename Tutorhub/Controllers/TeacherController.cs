// 📁 Controllers/TeacherController.cs
// লোকেশন: Tutorbub/Controllers/TeacherController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;

namespace Tutorbub.Controllers
{
    public class TeacherController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public TeacherController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== টিচার ফর্ম দেখা =====
        [HttpGet]
        public IActionResult RequestForm()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var status = _dbHelper.GetUserTeacherRequestStatus(userId);

            if (status == "Pending")
            {
                TempData["Info"] = "You have already submitted a teacher request. Please wait for admin approval.";
                return RedirectToAction("Index", "Profile");
            }
            else if (status == "Approved")
            {
                TempData["Info"] = "You are already a teacher!";
                return RedirectToAction("TeacherDashboard");
            }

            var user = _dbHelper.GetUserById(userId);
            var model = new TeacherRequest
            {
                UserId = userId,
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                MobileNumber = user?.MobileNumber ?? string.Empty
            };

            return View(model);
        }

        // ===== টিচার ফর্ম সাবমিট =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestForm(TeacherRequest model)
        {
            Console.WriteLine("=== Teacher Request Form POST ===");
            Console.WriteLine($"UserId: {model.UserId}");
            Console.WriteLine($"FullName: {model.FullName}");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Education: {model.Education}");
            Console.WriteLine($"SubjectExpertise: {model.SubjectExpertise}");

            if (HttpContext.Session.GetString("UserName") == null)
            {
                Console.WriteLine("User not logged in");
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                model.RequestDate = DateTime.UtcNow;
                model.Status = "Pending";

                Console.WriteLine("Calling CreateTeacherRequest...");
                bool success = _dbHelper.CreateTeacherRequest(model);
                Console.WriteLine($"CreateTeacherRequest result: {success}");

                if (success)
                {
                    TempData["Success"] = "Your teacher request has been submitted successfully! Please wait for admin approval.";
                    Console.WriteLine("Success! Redirecting to RequestForm");
                    return RedirectToAction("RequestForm");
                }
                else
                {
                    ViewBag.Error = "Failed to submit request. Please try again.";
                    Console.WriteLine("Failed! Error set in ViewBag");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model Error: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        // ===== টিচার ড্যাশবোর্ড =====
        public IActionResult TeacherDashboard()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(userId);
            if (user?.Role != "Teacher")
            {
                TempData["Error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Profile");
            }

            var model = new TeacherDashboardViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber ?? string.Empty,
                SubjectExpertise = "Not specified",
                Experience = "Not specified",
                ProfileImageLink = user.ProfileImageLink ?? string.Empty,
                TotalStudents = 0,
                TotalCourses = 0,
                CompletedSessions = 0,
                Rating = 0
            };

            return View(model);
        }
    }
}