using System;
using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingHeaderController : ControllerBase
    {
        private readonly IBookingHeaderService _bookingService;

        public BookingHeaderController(IBookingHeaderService bookingService)
        {
            _bookingService = bookingService;
        }

        [Route("AddBooking")]
        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync(BookingHeaderAddEdit booking)
        {
            var response = await _bookingService.CreateBookingAsync(booking);
            return Ok(response);
        }

        [HttpGet("GetAllBooking/paged")]
        public async Task<IActionResult> GetAllBooking(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _bookingService.GetBookingsAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetAllBooking")]
        public async Task<IActionResult> GetAllBooking()
        {
            var data = await _bookingService.GetBookingsAsync();
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [Route("EditBooking")]
        [HttpPost]
        public async Task<IActionResult> EditBooking(BookingHeaderAddEdit booking)
        {
            var response = await _bookingService.UpdateBookingAsync(booking);
            return Ok(response);
        }

        [HttpDelete("DeleteBooking")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            await _bookingService.DeleteBookingAsync(id);
            return NoContent();
        }

        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(long amenityId, long amenityUnitId, DateTime bookingDate, long? bookingId)
        {
            var slots = await _bookingService.GetAvailableSlotsAsync(amenityId, amenityUnitId, bookingDate, bookingId);
            return Ok(slots);
        }
    }
}
