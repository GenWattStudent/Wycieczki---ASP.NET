using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;

namespace Book.App.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoService _geoService;

    public ReservationService(IUnitOfWork reservationUnitOfWork, IGeoService geoService)
    {
        _unitOfWork = reservationUnitOfWork;
        _geoService = geoService;
    }

    public async Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.ActiveAndFutureReservations();
        return await _unitOfWork.reservationRepository.GetBySpec(reservationSpec);
    }

    public async Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.HistoryReservations();
        return await _unitOfWork.reservationRepository.GetBySpec(reservationSpec);
    }

    public async Task<ReservationModel?> GetClosestReservation(int userId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.ClosestReservation();
        return await _unitOfWork.reservationRepository.GetSingleBySpec(reservationSpec);
    }

    public async Task AddTourToUser(int userId, int tourId)
    {
        var user = await _unitOfWork.userRepository.GetById(userId);
        var reservations = await _unitOfWork.reservationRepository.GetBySpec(new ReservationSpecification(userId));
        var tour = await _unitOfWork.tourRepository.GetSingleBySpec(new TourSpecification(tourId));

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
            Tour = tour,
            TravelAgency = tour.TravelAgency
        };

        user.Reservations.Add(reservation);
        await _unitOfWork.SaveAsync();
    }

    public async Task Cancel(int userId, int tourId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.FilterByTour(tourId);
        var reservation = await _unitOfWork.reservationRepository.GetSingleBySpec(reservationSpec);

        if (reservation == null)
        {
            throw new Exception("Reservation not found");
        }

        if (reservation.Tour.StartDate < DateTime.Now && reservation.Tour.EndDate > DateTime.Now)
        {
            throw new Exception("Tour already started");
        }

        await _unitOfWork.reservationRepository.Remove(reservation.Id);
        await _unitOfWork.SaveAsync();
    }

    public async Task<TourModel?> GetTourById(int userId, int tourId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.FilterByTour(tourId);
        var reservation = await _unitOfWork.reservationRepository.GetSingleBySpec(reservationSpec);

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

    public async Task Delete(int userId, int tourId)
    {
        var reservationSpec = new ReservationSpecification(userId);
        reservationSpec.FilterByTour(tourId);
        var reservation = await _unitOfWork.reservationRepository.GetSingleBySpec(reservationSpec);

        if (reservation != null)
        {
            await _unitOfWork.reservationRepository.Remove(reservation.Id);
            await _unitOfWork.SaveAsync();
        }
        else
        {
            throw new Exception("Reservation not found");
        }
    }
}