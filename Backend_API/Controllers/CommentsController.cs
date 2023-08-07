using ASP_API.Helpers;
using ASP_API.Models;
using ASP_API.Models.Comments;
using AutoMapper;
using Backend_API.Data;
using Backend_API.Data.Entities;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AppEFContext _appEFContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public CommentsController(AppEFContext appEFContext, IConfiguration configuration, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _hostingEnvironment = hostEnvironment;
            _appEFContext = appEFContext;
            _configuration = configuration;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<IActionResult> Get(int thoughtId)
        {
            var res = await _appEFContext.Comments
                .Include(c => c.User)
                .Include(c => c.CommentParent)
                .Where(c => c.TweetId == thoughtId)
                .ToListAsync();

            return Ok(res);

        }


        [HttpPost("CreateComment")]
        public async Task<IActionResult> Post([FromForm] CommentsCreateViewModel model)
        {
            if (model.CommentText != null)
            {
                var comment = new CommentEntity
                {
                    CommentText = model.CommentText,
                    CreatedAt = DateTime.UtcNow,
                    CommentParentId = model.CommentParentId,
                    UserId = model.UserId,
                    TweetId = model.TweetId,
                };

                _appEFContext.Add(comment);
                _appEFContext.SaveChanges();

                foreach (var idImg in model.ImagesID)
                {
                    var image = await _appEFContext.CommentsMedias.SingleOrDefaultAsync(x => x.Id == idImg);
                    if(image != null)
                       image.CommentId = comment.Id;
                }

                _appEFContext.SaveChanges();
                return Ok(comment);
            }
            return BadRequest(404);
        }
        
        [HttpPost("uploadMedia")]
        public async Task<IActionResult> UploadMedia([FromForm] CommentsUploadImageModel model)
        {
            //fdasfsdfsafssssssss
            string imageName = string.Empty;
            if (model.Media != null)
            {
                var fileExp = Path.GetExtension(model.Media.FileName);
                var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                imageName = Path.GetRandomFileName() + fileExp;
                using (var ms = new MemoryStream())
                {
                    await model.Media.CopyToAsync(ms);
                    var bmp = new AnyBitmap(AnyBitmap.FromStream(ms).GetBytes());
                    string[] sizes = ((string)_configuration.GetValue<string>("ImageSizes")).Split(" ");
                    foreach (var s in sizes)
                    {
                        int size = Convert.ToInt32(s);
                        NetVips.Image saveImage = ImageWorker.CompressImage(bmp, size, size, false);
                        saveImage.WriteToFile(Path.Combine(dirSave, s + "_" + imageName));
                    }
                }
                var entity = new CommentMediaEntity();
                entity.Path = imageName;
                entity.CreatedAt = DateTime.UtcNow;

                _appEFContext.CommentsMedias.Add(entity);
                _appEFContext.SaveChanges();
                return Ok(_mapper.Map<CommentsViewImageModel>(entity));

            }
            return BadRequest();
        }

        [HttpDelete("RemoveComment/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var image = _appEFContext.CommentsMedias.Where(x => x.Id == id).ToList();

            if (image != null)
            {

                var dirSave = Path.Combine(_hostingEnvironment.ContentRootPath, "images");
                string[] sizes = ((string)_configuration.GetValue<string>("ImageSizes")).Split(" ");
                foreach (var i in image)
                {
                    foreach (var s in sizes)
                    {
                        var imgDelete = Path.Combine(dirSave, s + "_" + i.Path);
                        if (System.IO.File.Exists(imgDelete))
                        {
                            System.IO.File.Delete(imgDelete);
                        }
                    }
                    _appEFContext.CommentsMedias.Remove(i);
                    await _appEFContext.SaveChangesAsync();
                }
            }
            _appEFContext.Comments.Remove(_appEFContext.Comments.SingleOrDefault(x => x.Id == id));
            await _appEFContext.SaveChangesAsync();
            return Ok();
        }


    }
}
