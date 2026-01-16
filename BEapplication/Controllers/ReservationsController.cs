using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BEapplication.Controllers
{
    [ApiController]
    [Route("api/Reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationLogic _reservationLogic;

        public ReservationsController(IReservationLogic reservationLogic)
        {
            _reservationLogic = reservationLogic;
        }

        // YEAR → AVAILABLE MONTHS
        [HttpGet("availability/{year}/months")]
        public async Task<IActionResult> GetAvailableMonths(int year)
        {
            var months = await _reservationLogic.GetAvailableMonths(year);
            return Ok(months);
        }

        // MONTH → AVAILABLE DAYS
        [HttpGet("availability/{year}/{month}/days")]
        public async Task<IActionResult> GetAvailableDays(int year, int month)
        {
            var days = await _reservationLogic.GetAvailableDays(year, month);
            return Ok(days);
        }

        // DAY → AVAILABLE HOURS
        [HttpGet("availability/{year}/{month}/{day}/hours")]
        public async Task<IActionResult> GetAvailableHours(int year, int month, int day)
        {
            var hours = await _reservationLogic.GetAvailableHours(year, month, day);
            return Ok(hours);
        }

        [HttpPost("addReservation")]
        [Authorize]
        public async Task<IActionResult> AddReservation(RequestNewReservation request)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            await _reservationLogic.AddReservation(request, email);

            return Ok("Rezervare creată.");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            await _reservationLogic.DeleteReservation(id);
            return NoContent();
        }

        [HttpGet("myReservations")]
        [Authorize]
        public async Task<IActionResult> GetMyReservations()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var reservations = await _reservationLogic.GetMyReservations(userEmail);
            return Ok(reservations);
        }

    }
}
