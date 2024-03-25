using Book.App.Models;
using Book.App.Repositories;
using Book.App.Repositories.UnitOfWork;
using Book.App.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class BookService
{
    private readonly ReservationUnitOfWork _reservationUnitOfWork;
    private readonly GeoService _geoService;

    public BookService(ReservationUnitOfWork reservationUnitOfWork, GeoService geoService)
    {
        _reservationUnitOfWork = reservationUnitOfWork;
        _geoService = geoService;
    }

    public async Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId)
    {
        return await _reservationUnitOfWork.reservationRepository.GetUserActiveAndFutureReservations(userId);
    }

    public async Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId)
    {
        return await _reservationUnitOfWork.reservationRepository.GetReservationsHistoryByUserId(userId);
    }

    public async Task<ReservationModel?> GetClosestReservation(int userId)
    {
        return await _reservationUnitOfWork.reservationRepository.GetClosestReservation(userId);
    }

    public async Task AddTourToUser(int userId, int tourId)
    {
        var user = await _reservationUnitOfWork.userRepository.GetById(userId);
        var reservations = await _reservationUnitOfWork.reservationRepository.GetAllReservations(userId);
        var tour = await _reservationUnitOfWork.tourRepository.GetTour(tourId);

        if (user == null || tour == null)
        {
            throw new Exception("User or tour not found");
        }

        if (reservations.Any(r => r.Tour.Id == tourId))
        {
            throw new Exception("User already booked this tour");
        }

        if (tour.StartDate < DateTime.Now)
        {
            throw new Exception("Tour already started");
        }

        if (tour.MaxUsers <= tour.Users.Count)
        {
            throw new Exception("Tour is full");
        }

        if (user.Tours.Any(t =>
            t.StartDate <= tour.StartDate && t.EndDate >= tour.EndDate
        || t.StartDate >= tour.StartDate && t.EndDate <= tour.EndDate
        || t.StartDate <= tour.StartDate && t.EndDate >= tour.StartDate
        || t.StartDate <= tour.EndDate && t.EndDate >= tour.EndDate))
        {
            throw new Exception("User already booked a tour in this time");
        }

        var reservation = new ReservationModel
        {
            User = user,
            Tour = tour
        };

        user.Reservations.Add(reservation);
        await _reservationUnitOfWork.SaveAsync();
    }

    public async Task DeleteCancelTour(int userId, int tourId)
    {
        var reservation = await _reservationUnitOfWork.reservationRepository.GetReservationByTourAndUserId(tourId, userId);

        if (reservation == null)
        {
            throw new Exception("Reservation not found");
        }

        if (reservation.Tour.StartDate < DateTime.Now && reservation.Tour.EndDate > DateTime.Now)
        {
            throw new Exception("Tour already started");
        }

        await _reservationUnitOfWork.reservationRepository.Delete(reservation.Id);
        await _reservationUnitOfWork.SaveAsync();
    }

    public async Task<TourModel?> GetTourById(int userId, int tourId)
    {
        var reservation = await _reservationUnitOfWork.reservationRepository.GetReservationWithAllJoinsByTourAndUserId(tourId, userId);

        return reservation != null ? reservation.Tour : null;
    }

    public BookViewModel GetBookViewModel(TourModel tour)
    {
        var bookViewModel = new BookViewModel { TourModel = tour, Distance = _geoService.CalculateDistance(tour.Waypoints) };

        // the trip started and not ended calculate dsitance and which waypoint you are
        if (tour.StartDate <= DateTime.Now && tour.EndDate >= DateTime.Now)
        {
            bookViewModel.PercentOfTime = (DateTime.Now - tour.StartDate).TotalDays / (tour.EndDate - tour.StartDate).TotalDays;
            var nextWaypointData = _geoService.CalculateDistanceToNextWaypoint(tour.Waypoints, bookViewModel.PercentOfTime);
            bookViewModel.NextWaypointData = nextWaypointData;
        }
        else if (tour.StartDate > DateTime.Now)
        {
            bookViewModel.PercentOfTime = 0;
        }
        else
        {
            bookViewModel.PercentOfTime = 1;
        }

        return bookViewModel;
    }

    public async Task DeleteReservation(int userId, int tourId)
    {
        var reservation = await _reservationUnitOfWork.reservationRepository.GetReservationByTourAndUserId(tourId, userId);

        if (reservation != null)
        {
            await _reservationUnitOfWork.reservationRepository.Delete(reservation.Id);
            await _reservationUnitOfWork.SaveAsync();
        }
        else
        {
            throw new Exception("Reservation not found");
        }
    }
}