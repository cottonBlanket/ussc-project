﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USSC.Dto;
using USSC.Services;


namespace USSC.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("testcase")]
public class TestCaseController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITestCaseService _testCaseService;
    public TestCaseController(ITestCaseService testCaseService, IUserService userService)
    {
        _testCaseService = testCaseService;
        _userService = userService;
    }
    
    [Microsoft.AspNetCore.Mvc.HttpGet("download")]
    public FileStreamResult DownloadFile(string namePractices)
    {
        var path = $"G:\\USSC project\\USSC-project\\USSC\\USSC\\Files\\{namePractices}.zip";
        var fileType="application/octet-stream";
        var fileStream = new FileStream(path, FileMode.Open);
        var fileName = $"{namePractices}.zip";
        return File(fileStream, fileType, fileName);
    }

    /// <summary>
    /// Требуется добавить юзера, чтобы создавать папку отдельно для каждого юзера, или называть файл id юзера
    /// также добавить [Authorize]
    /// Еще возможно сделать обработку сценария, когда файл с таким именем уже есть на сервере
    /// </summary>
    [Microsoft.AspNetCore.Mvc.HttpPost("upload")]
    public async void UploadFile(IFormFile file)
    {
        
         var response = HttpContext.Response;
         var name = file.FileName.Split(".");
         if (name.Length != 2 || name[1] != "zip")
         {
            // throw new Exception("Invalid format file");
            await response.WriteAsync("Invalid format file or file name");
            return;
         }
         // здесь вместо file.FileName должно быть {user.Id}.zip 
         var uploadPath = $"G:\\USSC project\\USSC-project\\USSC\\USSC\\Upload\\{file.FileName}.zip";
        
         using (var fileStream = new FileStream(uploadPath, FileMode.Create))
         {
             await file.CopyToAsync(fileStream);
         }

         await response.WriteAsync("Файлы успешно загружены");
    }

    [HttpPost("addTest")]
    public IActionResult AddTestCase(AddedTestCase testCase)
    {
        var user = _userService.GetById(testCase.UserId);
        var response = _testCaseService.Upload(user, testCase.DirectionId, testCase.Path);
        return Ok(new SuccessResponse(true));
    }
    
    [Authorize(Roles="Admin")]
    [HttpGet("getAll")]
     public IActionResult GetAll()
     {
         var testCases = _testCaseService.GetAll();
         return Ok(testCases);
     }

     /// <summary>
     /// возвращает путь к файлу решен
     /// </summary>
     // [Authorize(Roles = "Admin")]
     [HttpGet("downloadPractices")]
     public IActionResult DownLoadPractice(Guid userId, Guid directionId)
     {
         var user = _userService.GetById(userId);
         if (user.TestCase.Count == 0)
             return BadRequest(new { Message = "Пользователь не предоставил решения" });
         var testCasePath = _testCaseService.DownLoad(userId, directionId);
         
         return Ok(testCasePath);
     }

     /// <summary>
     /// проверка практики куратором
     /// </summary>
     [HttpPost("reviewPractice")]
     public IActionResult ReviewPractice(ReviewedTestCase reviewTestcase)
     {
         var user = _userService.GetById(reviewTestcase.UserId);
         var testCase = user.TestCase.FirstOrDefault(x => x.DirectionId == reviewTestcase.DirectionId);
         var response = _testCaseService.ReviewTestCaseAsync(user, testCase, reviewTestcase);
         if (response.Result.Success)
             return Ok(response.Result);
         else
             return BadRequest(response.Result); //здесь тоже логика с racticeService
     }
}