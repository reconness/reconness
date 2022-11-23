

namespace ReconNess.Presentation.Api.Mappers.Resolvers;

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class UserProfileResolver : IValueResolver<User, UserDto, string>
{
    private readonly UserManager<User> userManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    public UserProfileResolver(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userDto"></param>
    /// <param name="destMember"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string Resolve(User user, UserDto userDto, string destMember, ResolutionContext context)
    {
        return userManager.GetRolesAsync(user).Result.FirstOrDefault();
    }
}
