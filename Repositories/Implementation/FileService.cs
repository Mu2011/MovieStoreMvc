using Microsoft.VisualBasic;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation;

public class FileService(IWebHostEnvironment environment) : IFileService
{
  private readonly IWebHostEnvironment _environment = environment;
  public bool DeleteImage(string imageFileName)
  {
    try
    {
      var wwwPath = _environment.WebRootPath;
      // var path = Path.Combine(wwwPath, "Uploads//" + imageFileName);
      var path = Path.Combine(wwwPath, "Uploads");
      var filePath = Path.Combine(path, imageFileName);
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
        return true;
      }
      return false;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public Tuple<int, string> SaveImage(IFormFile imageFile)
  {
    try
    {
      var wwwPath = _environment.WebRootPath;
      var path = Path.Combine(wwwPath, "Uploads");

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      // Check if the allowed extension is valid
      var extension = Path.GetExtension(imageFile.FileName);
      var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

      if (!allowedExtensions.Contains(extension.ToLower()))
      {
        string msg = string.Format("Only {0} extensions are allowed", string.Join(", ", allowedExtensions));
        return new Tuple<int, string>(0, msg);
      }

      string uniqueString = Guid.NewGuid().ToString();
      // we are trying to create a unique filename here
      var newFileName = uniqueString + extension;
      var filePath = Path.Combine(path, newFileName);
      var stream = new FileStream(filePath, FileMode.Create);
      imageFile.CopyTo(stream);
      stream.Close();
      return new Tuple<int, string>(1, newFileName);
    }
    catch (Exception)
    {
      return new Tuple<int, string>(0, "Error occurred");
    }
  }

  public Tuple<int, string> SaveImage(IFormFile imageFile, string path)
  {
    try
    {
      // Check if the allowed extension is valid
      var extension = Path.GetExtension(imageFile.FileName);
      var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

      if (!allowedExtensions.Contains(extension.ToLower()))
      {
        string msg = string.Format("Only {0} extensions are allowed", string.Join(", ", allowedExtensions));
      }

      // Save the image
      var fileName = Path.GetFileName(imageFile.FileName);
      var filePath = Path.Combine(path, fileName);
      imageFile.CopyTo(new FileStream(filePath, FileMode.Create));
      return new Tuple<int, string>(1, fileName);
    }
    catch (Exception)
    {
      return new Tuple<int, string>(0, string.Empty);
    }
  }
}