﻿using BusinessLayer.Interface;
using DataLayer.Dtos;
using DataLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly AfrostichContext _context;
        private readonly string defualtPassword = "1234567";

        public UserService(IConfiguration configuration, AfrostichContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<ResponseModel> PostUser(AddUserDto userDto)
        {
            ResponseModel response = new ResponseModel();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                User user = new User();
                Person person = new Person()
                {
                    Surname = userDto.Surname,
                    Firstname = userDto.Firstname,
                    Othername = userDto.Othername,
                    Email = userDto.Email,
                };
                _context.Add(person);
                await _context.SaveChangesAsync();

                Utility.CreatePasswordHash(defualtPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.Username = userDto.Email;
                user.RoleId = userDto.RoleId;
                user.IsVerified = true;
                user.Active = true;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.PersonId = person.Id;
                _context.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return response;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }

        }
        public async Task<UserDto> AuthenticateUser(LoginDto dto, string injectkey)
        {
            UserDto userDto = new UserDto();
            var user = await _context.USER
               .Include(r => r.Role)
               .Include(r => r.Person)
               .Where(f => f.Active && f.Username == dto.UserName).FirstOrDefaultAsync();

            if (user == null)
                return null;
            if (!user.IsVerified)
                throw new Exception("Account has not been verified!");
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(injectkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.Name),

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            user.LastLogin = DateTime.UtcNow;
            _context.Update(user);
            await _context.SaveChangesAsync();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            userDto.Token = tokenHandler.WriteToken(token);
            userDto.Password = null;
            userDto.UserName = user.Username;
            userDto.RoleName = user.Role.Name;
            userDto.UserId = user.Id;
            userDto.PersonId = user.PersonId;
            userDto.FullName = user.Person.Surname + " " + user.Person.Firstname + " " + user.Person.Othername;

            return userDto;
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
}
