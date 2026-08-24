using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
  [ApiController]
  [Route("api/room-types")]
  public class RoomTypeController : ControllerBase
  {
    private readonly IRepository<RoomType> _roomTypeRepository;

    public RoomTypeController(IRepository<RoomType> roomTypeRepository)
    {
      _roomTypeRepository = roomTypeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoomTypes()
    {
      var roomTypes = await _roomTypeRepository.GetAllAsync();
      var response = roomTypes.Select(rt => new
      {
        rt.Id,
        rt.Name,
        rt.BasePrice,
        rt.Capacity,
        rt.Description,
        rt.ImageUrl
      });
      return Ok(response);
    }

        [HttpGet("{id:long}")]
    public async Task<IActionResult> GetRoomTypeDetails(long id)
    {
      var roomType = await _roomTypeRepository.GetByIdAsync(id, "Rooms");
      if (roomType == null)
      {
        return NotFound(new { message = "Tipe Kamar tidak ditemukan." });
      }
      return Ok(new
      {
        roomType.Id,
        roomType.Name,
        roomType.BasePrice,
        roomType.Capacity,
        roomType.Description,
        roomType.ImageUrl,
        Rooms = roomType.Rooms
            .Where(r => r.Status == Models.Enum.RoomStatus.AVAILABLE)
            .Select(r => new { r.Id, r.RoomNumber })
      });
    }
  }
}