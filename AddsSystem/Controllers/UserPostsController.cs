using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AddsSystem.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace AddsSystem.Controllers
{
    public class UserpostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: UserPosts
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userid = User.Identity.GetUserId();
                var userPostModel = db.UserPosts.Include(c => c.PostImages).Where(s => s.UserId == userid);
                return View(userPostModel.ToList());
            }

            else
            {
                var userPostModel = db.UserPosts.Include(c => c.PostImages).OrderBy(s => s.UserId);
                return View(userPostModel.ToList());

            }
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserPost UserPost)
        {
            if (ModelState.IsValid)
            {
                List<PostImage> postImages = new List<PostImage>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        PostImage postImage = new PostImage()
                        {
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName),
                            Id = Guid.NewGuid()
                        };
                        postImages.Add(postImage);

                        var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), postImage.Id + postImage.Extension);
                        file.SaveAs(path);
                    }
                }

                string userid = User.Identity.GetUserId();
                UserPost.UserId = userid;

                UserPost.PostImages = postImages;
                db.UserPosts.Add(UserPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(UserPost);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserPost userpost = db.UserPosts.Include(s => s.PostImages).SingleOrDefault(x => x.UserPostId == id);
            if (userpost == null)
            {
                return HttpNotFound();
            }
            return View(userpost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserPost userpost)
        {
            if (ModelState.IsValid)
            {
                List<PostImage> postImages = new List<PostImage>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);


                    PostImage postImage = new PostImage()
                    {
                        FileName = fileName,
                        Extension = Path.GetExtension(fileName),
                        Id = Guid.NewGuid(),
                        UserPostId = userpost.UserPostId
                    };

                    var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), postImage.Id + postImage.Extension);
                    file.SaveAs(path);
                    //postImage.UserPostId = userpost.UserPostId
                    db.PostImages.Add(postImage);
                }

                string userid = User.Identity.GetUserId();
                userpost.UserId = userid;

                userpost.PostImages = postImages;
                db.Entry(userpost).State = EntityState.Modified;
                db.SaveChanges();
                // return RedirectToAction("Index");
            }
            //return View(support);
            return RedirectToAction("Index");
        }

        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/App_Data/Upload/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                Guid guid = new Guid(id);
                PostImage postImage = db.PostImages.Find(guid);
                if (postImage == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //Remove from database
                db.PostImages.Remove(postImage);
                db.SaveChanges();

                //Delete file from the file system
                var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), postImage.Id + postImage.Extension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                UserPost userpost = db.UserPosts.Find(id);
                if (userpost == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //delete files from the file system

                foreach (var item in userpost.PostImages)
                {
                    String path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), item.Id + item.Extension);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                db.UserPosts.Remove(userpost);
                db.SaveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
