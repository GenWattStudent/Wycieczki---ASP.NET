using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;

namespace Book.App.Services;

public class AgencyService : IAgencyService
{
    private readonly IUnitOfWork _unitOfWork;

    public AgencyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AcceptAsync(int id, string reason)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(id);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        agency.IsAccepted = true;
        agency.Reason = reason;

        _unitOfWork.agencyRepository.Update(agency);
        await _unitOfWork.SaveAsync();
    }

    public async Task<TravelAgencyModel> CreateAsync(TravelAgencyViewModel travelAgencyViewModel, int userId)
    {
        if (travelAgencyViewModel.TravelAgency == null)
        {
            throw new Exception("Travel agency is null");
        }

        var user = await _unitOfWork.userRepository.GetById(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (user.TravelAgencyId != null)
        {
            throw new Exception("User already has an agency");
        }

        user.Role = Role.AgencyAdmin;
        travelAgencyViewModel.TravelAgency.Users.Add(user);
        _unitOfWork.agencyRepository.Add(travelAgencyViewModel.TravelAgency);
        await _unitOfWork.SaveAsync();

        return travelAgencyViewModel.TravelAgency;
    }

    public async Task DeleteAsync(int id)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(id);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        _unitOfWork.agencyRepository.Remove(agency);
        await _unitOfWork.SaveAsync();
    }

    public Task<List<TravelAgencyModel>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TravelAgencyModel>> GetAllByUserIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<TravelAgencyModel> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TravelAgencyModel>> GetNotAcceptedAsync()
    {
        var agencySpecification = new AgencySpecification();
        agencySpecification.SetNotAccepted();
        return await _unitOfWork.agencyRepository.GetBySpec(agencySpecification);
    }

    public async Task<bool> HasUserAgencyAsync(int userId)
    {
        var agencySpecification = new AgencySpecification();
        agencySpecification.HasAgency(userId);
        return await _unitOfWork.agencyRepository.GetSingleBySpec(agencySpecification) != null;
    }

    public async Task LeaveAsync(int userId)
    {
        var agencySpecification = new AgencySpecification();
        agencySpecification.ByUserId(userId);

        var agency = await _unitOfWork.agencyRepository.GetSingleBySpec(agencySpecification);
        var user = await _unitOfWork.userRepository.GetById(userId);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (agency.Users.Count == 1)
        {
            await DeleteAsync(agency.Id);
            return;
        }
        else if (user.Role == Role.AgencyAdmin && agency.Users.Count > 1)
        {
            // promote another user to admin
            var newAdmin = agency.Users.FirstOrDefault(u => u.Id != userId);
            if (newAdmin != null)
            {
                newAdmin.Role = Role.AgencyAdmin;
            }
        }

        agency.Users.RemoveAll(u => u.Id == userId);
        await _unitOfWork.SaveAsync();
    }

    public async Task PromoteAsync(int userId, int currentUserId, Role agencyRole)
    {
        var currentUser = await _unitOfWork.userRepository.GetById(currentUserId);

        if (currentUser != null && currentUser.TravelAgency != null && currentUser.Role == Role.AgencyAdmin)
        {
            var user = await _unitOfWork.userRepository.GetById(userId);
            if (user != null)
            {
                user.Role = agencyRole;
                await _unitOfWork.SaveAsync();
            }
        }
    }

    public async Task RejectAsync(int id, string reason)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(id);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        agency.IsAccepted = false;
        agency.Reason = reason;

        _unitOfWork.agencyRepository.Update(agency);
        await _unitOfWork.SaveAsync();
    }

    public Task UpdateAsync(TravelAgencyViewModel travelAgencyModel)
    {
        throw new NotImplementedException();
    }
}