using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Business.Types;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Enums;
using BilgeShop.Data.Repository;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Managers
{
	public class UserManager : IUserService
	{
		private readonly IRepository<UserEntity> _userRepository;
		private readonly IDataProtector _dataProtector;
		public UserManager(IRepository<UserEntity> userRepository, IDataProtectionProvider dataProtectionProvider)
		{
			_userRepository = userRepository;
			_dataProtector = dataProtectionProvider.CreateProtector("security");
		}

		public ServiceMessage AddUser(AddUserDto addUserDto)
		{
			var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == addUserDto.Email.ToLower()).ToList();
			// Eğer küçültme işlemi başka yerde yapıldıysa bu kodlarda ToLower() kısımlarına gerek olmayabilir. Fakat projenin diğer taraflarının bir gün değişmeyeceğinin garantisi olmadığından mümkün olduğunda önlemleri her noktada almakta fayda var.
			//  hasmail içersinde dolu mu yoksa null mı kontrol etmem.

			if (hasMail.Any()) // hasMail is not null
			{
				return new ServiceMessage
				{
					IsSucceed = false,
					Message = "Bu Eposta adresli kullanıcı zaten mevcut"
				};
			}

			var userEntity = new UserEntity()
			{
				FirstName = addUserDto.FirstName,
				LastName = addUserDto.LastName,
				Email = addUserDto.Email,
				Password = _dataProtector.Protect(addUserDto.Password),
				UserType = UserTypeEnum.User
				// yeni kayıtlar direkt User ( admin değil)
			};

			_userRepository.Add(userEntity);

			return new ServiceMessage()
			{
				IsSucceed = true
			};
		}

		public UserInfoDto LoginUser(LoginUserDto loginUserDto)
		{
			var userEntity = _userRepository.Get(x => x.Email == loginUserDto.Email);

			if(userEntity is null)
			{
				return null; // form üzerinde gönderile mail adresi ile eşleşen bir veri tabloda yoksa, oturum açılamayacağı için geriye hiçbir bilgi dönmüyor.
			}

			var rawPassword = _dataProtector.Unprotect(userEntity.Password);

			if(loginUserDto.Password == rawPassword)
			{
				return new UserInfoDto()
				{
					Id = userEntity.Id,
					FirstName = userEntity.FirstName,
					LastName = userEntity.LastName,
					UserType = userEntity.UserType,
					Email = userEntity.Email,
				};
			}
			else
			{
				return null;
			}
		}
	}
}
