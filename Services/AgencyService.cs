using AutoMapper;
using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;

namespace Book.App.Services;

public class AgencyService : IAgencyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    public readonly string logosFolder = "logos";
    public readonly string videosFolder = "videos";

    public AgencyService(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _mapper = mapper;
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

    public async Task<TravelAgencyModel> CreateAsync(CreateAgencyViewModel createAgencyViewModel, int userId)
    {
        var user = await _unitOfWork.userRepository.GetById(userId);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var travelAgencyModel = _mapper.Map<TravelAgencyModel>(createAgencyViewModel);

        if (createAgencyViewModel.LogoFile != null)
        {
            var logoUrl = await _fileService.SaveFile(createAgencyViewModel.LogoFile, logosFolder);
            travelAgencyModel.LogoPath = logoUrl;
        }

        if (createAgencyViewModel.VideoFile != null)
        {
            var videoUrl = await _fileService.SaveFile(createAgencyViewModel.VideoFile, videosFolder);
            travelAgencyModel.VideoPath = videoUrl;
        }

        user.Roles.Add(new RoleModel { Role = Role.AgencyAdmin });
        travelAgencyModel.Users.Add(user);
        _unitOfWork.agencyRepository.Add(travelAgencyModel);
        await _unitOfWork.SaveAsync();

        return travelAgencyModel;
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var agencySpecification = new AgencySpecification();
        agencySpecification.ById(id);
        var agency = await _unitOfWork.agencyRepository.GetSingleBySpec(agencySpecification);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        if (!agency.Users.Any(u => u.Id == userId))
        {
            throw new Exception("You are not allowed to delete this agency");
        }

        foreach (var user in agency.Users)
        {
            user.Roles.RemoveAll(r => r.Role == Role.AgencyAdmin || r.Role == Role.AgencyUser || r.Role == Role.AgencyManager);
        }

        if (agency.LogoPath != null) _fileService.DeleteFile(agency.LogoPath);
        if (agency.VideoPath != null) _fileService.DeleteFile(agency.VideoPath);


        foreach (var image in agency.Images)
        {
            _fileService.DeleteFile(image.ImageUrl);
            _unitOfWork.imageRepository.Remove(image);
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

    public async Task<TravelAgencyModel> GetByIdAsync(int id)
    {
        var agencySpecification = new AgencySpecification();
        agencySpecification.ById(id);
        var agency = await _unitOfWork.agencyRepository.GetSingleBySpec(agencySpecification);
        return agency;
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

        if (agency == null) throw new Exception("Agency not found");
        if (user == null) throw new Exception("User not found");

        if (agency.Users.Count == 1)
        {
            await DeleteAsync(agency.Id, userId);
            return;
        }
        else if (user.Roles.Any(r => r.Role == Role.AgencyAdmin) && agency.Users.Count > 1)
        {
            // promote another user to admin
            var newAdmin = agency.Users.FirstOrDefault(u => u.Id != userId);
            if (newAdmin != null)
            {
                newAdmin.Roles.Add(new RoleModel { Role = Role.AgencyAdmin });
            }

            user.Roles.RemoveAll(r => r.Role == Role.AgencyAdmin);
        }

        agency.Users.RemoveAll(u => u.Id == userId);
        await _unitOfWork.SaveAsync();
    }

    private bool IsAgencyAdmin(UserModel? user) => user != null && user.TravelAgency != null && user.Roles.Any(r => r.Role == Role.AgencyAdmin);
    public async Task<bool> IsUserInAgencyAsync(int userId, int agencyId)
    {
        var agency = await GetByIdAsync(agencyId);
        if (agency == null) return false;
        return agency.Users.Any(u => u.Id == userId);
    }

    public async Task PromoteAsync(int userId, int currentUserId, Role agencyRole)
    {
        var currentUser = await _unitOfWork.userRepository.GetById(currentUserId);

        if (IsAgencyAdmin(currentUser))
        {
            var user = await _unitOfWork.userRepository.GetById(userId);
            if (user != null && user.TravelAgencyId == currentUser?.TravelAgencyId && user.Roles.All(r => r.Role != agencyRole))
            {
                user.Roles.Add(new RoleModel { Role = agencyRole });
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

    public async Task UpdateAsync(CreateAgencyViewModel createAgencyViewModel)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(createAgencyViewModel.Id);

        if (agency == null) throw new Exception("Agency not found");

        if (createAgencyViewModel.LogoFile != null)
        {
            if (agency.LogoPath != null)
            {
                _fileService.DeleteFile(agency.LogoPath);
            }

            var logoUrl = await _fileService.SaveFile(createAgencyViewModel.LogoFile, logosFolder);
            agency.LogoPath = logoUrl;
        }

        if (createAgencyViewModel.VideoFile != null)
        {
            if (agency.VideoPath != null)
            {
                _fileService.DeleteFile(agency.VideoPath);
            }

            var videoUrl = await _fileService.SaveFile(createAgencyViewModel.VideoFile, videosFolder);
            agency.VideoPath = videoUrl;
        }

        agency = _mapper.Map(createAgencyViewModel, agency);

        _unitOfWork.agencyRepository.Update(agency);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteLogoAsync(int id)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(id);

        if (agency == null) throw new Exception("Agency not found");

        if (agency.LogoPath != null)
        {
            _fileService.DeleteFile(agency.LogoPath);
            agency.LogoPath = null;
            _unitOfWork.agencyRepository.Update(agency);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task DeleteVideoAsync(int id)
    {
        var agency = await _unitOfWork.agencyRepository.GetById(id);

        if (agency == null) throw new Exception("Agency not found");

        if (agency.VideoPath != null)
        {
            _fileService.DeleteFile(agency.VideoPath);
            agency.VideoPath = null;
            _unitOfWork.agencyRepository.Update(agency);
            await _unitOfWork.SaveAsync();
        }
    }
}