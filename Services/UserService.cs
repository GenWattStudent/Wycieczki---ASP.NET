using AutoMapper;
using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;

namespace Book.App.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _userImageFolder = "users";
    private readonly IFileService _fileService;
    private readonly IAgencyService _agencyService;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IFileService fileService, IAgencyService agencyService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _agencyService = agencyService;
        _mapper = mapper;
    }

    public async Task Register(RegisterViewModel registerModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);

        registerModel.Password = PasswordHash;

        var userModel = _mapper.Map<UserModel>(registerModel);

        if (registerModel.Image != null)
        {
            userModel.ImagePath = await _fileService.SaveFile(registerModel.Image, _userImageFolder);
        }

        _unitOfWork.userRepository.Add(userModel);
        await _unitOfWork.SaveAsync();
    }

    public async Task EditUserInfo(EditUserViewModel userViewModel, UserModel userModel)
    {
        if (userModel == null) return;

        userModel = _mapper.Map(userViewModel, userModel);

        if (userViewModel.Image != null)
        {
            if (userModel.ImagePath != null) _fileService.DeleteFile(userModel.ImagePath);
            userModel.ImagePath = await _fileService.SaveFile(userViewModel.Image, _userImageFolder);
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task RegisterAdmin(UserModel userModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(userModel.Password);

        userModel.Password = PasswordHash;

        _unitOfWork.userRepository.Add(userModel);
        await _unitOfWork.SaveAsync();
    }

    public async Task<UserModel?> GetByUsername(string username) => await _unitOfWork.userRepository.GetSingleBySpec(new UserSpecification(username));

    public async Task Delete(string username)
    {
        var user = await GetByUsername(username);
        if (user == null) return;


        if (user.TravelAgency != null)
        {
            await _agencyService.LeaveAsync(user.Id);
        }

        await _unitOfWork.userRepository.Remove(user.Id);
        if (user.ImagePath != null) _fileService.DeleteFile(user.ImagePath);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteImage(string? username)
    {
        var user = await GetByUsername(username ?? "");
        if (user == null) return;

        if (user.ImagePath != null) _fileService.DeleteFile(user.ImagePath);
        user.ImagePath = null;
        await _unitOfWork.SaveAsync();
    }
}