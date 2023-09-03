using ASP_API.Helpers;
using ASP_API.Models;
using ASP_API.Models.Comments;
using AutoMapper;
using Backend_API.Data;
using Backend_API.Data.Entities;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        private readonly UserManager<UserEntity> _userManager;
        public CommentsController(AppEFContext appEFContext, IConfiguration configuration, IMapper mapper, IWebHostEnvironment hostEnvironment, UserManager<UserEntity> userManager)
        {
            _hostingEnvironment = hostEnvironment;
            _appEFContext = appEFContext;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet("{thoughtId}")]
        public async Task<IActionResult> Get(int thoughtId)
        {
            var comments = _appEFContext.Comments
                    .Include(c => c.User)
                    .Include(c => c.CommentsChildren)
                    .Include(c => c.ReplyTo)
                    .Where(c => c.TweetId == thoughtId && c.CommentParentId == null && c.IsComment == true)
                    .OrderBy(c => c.CreatedAt)
                    .ToList();


            var commentsWithChildren = new List<CommentViewModel>();

            foreach (var comment in comments)
            {
                var commentModel = new CommentViewModel
                {
                    // Заповнюємо дані з коментаря, які не змінюються

                    Id = comment.Id,
                    CommentText = comment.CommentText,
                    Medias = _appEFContext.CommentsMedias.Where(m => m.CommentId == comment.Id).Select(m => _mapper.Map<CommentsViewImageModel>(m)).ToList(),
                    User = _mapper.Map<UserViewModel>(comment.User),
                    ThoughtId = comment.TweetId,
                    CommentParentId = comment.CommentParentId,
                    CreatedAt = comment.CreatedAt,
                    CreatedAtStr = HelperFunctions.ConvertDateTimeToStr(comment.CreatedAt),
                    Children =  HelperFunctions.GetCommentChildren(comment.Id, _appEFContext, _mapper),
                    IsComment = comment.IsComment,
                    IsReply = comment.IsReply,
                    ReplyTo = comment.ReplyTo == null ? null : HelperFunctions.CommentEntityToViewModel(comment.ReplyTo, _appEFContext, _mapper),
                };

                commentsWithChildren.Add(commentModel);
            }

            return Ok(commentsWithChildren);

        }

        

        [HttpPost("CreateComment")]
        public async Task<IActionResult> Post([FromForm] CommentsCreateViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (model.CommentText != null)
            {
                var comment = new CommentEntity
                {
                    CommentText = model.CommentText,
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id,
                    TweetId = model.TweetId,
                    CommentParentId = null,
                    IsComment = true,
                    IsReply = false,
                    ReplyToId = null,
                };

                _appEFContext.Add(comment);
                _appEFContext.SaveChanges();

                if (model.MediaIds != null)
                {
                    foreach (var idImg in model.MediaIds)
                    {
                        var image = await _appEFContext.CommentsMedias.SingleOrDefaultAsync(x => x.Id == idImg);
                        if (image != null)
                            image.CommentId = comment.Id;
                    }
                }

                    _appEFContext.SaveChanges();
                return Ok(comment);
            }
            return BadRequest(404);
        }

        [HttpPost("ReplyComment")]
        public async Task<IActionResult> ReplyToComment([FromForm] CommentReplyCreateViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (model.CommentText != null)
            {
                var comment = new CommentEntity
                {
                    CommentText = model.CommentText,
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id,
                    TweetId = model.TweetId,
                    CommentParentId = model.CommentParentId != null ? model.CommentParentId : null,
                    IsComment = false,
                    IsReply = true,
                    ReplyToId = model.ReplyToChild && model.ReplyToId != null ? model.ReplyToId : null,
                };

                _appEFContext.Add(comment);
                _appEFContext.SaveChanges();

                if (model.MediaIds != null)
                {
                    foreach (var idImg in model.MediaIds)
                    {
                        var image = await _appEFContext.CommentsMedias.SingleOrDefaultAsync(x => x.Id == idImg);
                        if (image != null)
                            image.CommentId = comment.Id;
                    }
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
