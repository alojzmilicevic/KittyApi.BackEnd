using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Models;

public class Thumbnail
{
    public int Id { get; set; }
    public string ThumbnailName { get; set; }
    public string ThumbnailPath { get; set; }
}
