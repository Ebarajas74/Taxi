﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Juan", "Zuluaga", "ebarajas@fordmontes.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("1111", "Enrique", "Barajas", "enbace74@gmail.com", "461 120 4923", "Cto. San Isidro", UserType.Admin);

            var driver = await CheckUserAsync("2020", "Juan", "Zuluaga", "enbace@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Driver);
            var user1 = await CheckUserAsync("3030", "Juan", "Zuluaga", "carlos.zuluaga@globant.com.mx", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            var user2 = await CheckUserAsync("4040", "Juan", "Zuluaga", "juanzuluaga2480@correo.itm.mx", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            
            await CheckTaxisAsync(driver, user1, user2);
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<UserEntity> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }
        private async Task CheckTaxisAsync(
            UserEntity driver,
            UserEntity user1,
            UserEntity user2)
        {
            if (_dataContext.Taxis.Any())
            {
                return;
            }

            _dataContext.Taxis.Add(new TaxiEntity
            {
                Plaque = "TPQ123",
                Trips = new List<TripEntity>
                {
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = 4.5f,
                        Source = "ITM Fraternidad",
                        Target = "ITM Robledo",
                        Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer convallis finibus nisl in fringilla. Fusce et odio dolor. Aliquam venenatis libero tortor, at tincidunt neque auctor vitae. Morbi lobortis eget quam eget convallis. Morbi id magna neque. Pellentesque sit amet imperdiet lacus. Aliquam ac facilisis elit, in aliquet felis. Morbi lacinia nunc ut tristique ornare. Integer felis risus, tempor eu placerat vel, lacinia nec ipsum. Cras eleifend orci ac dictum lacinia. Cras id porta ex, quis interdum ex. Vivamus facilisis feugiat diam, vitae euismod quam dignissim sit amet. Integer egestas a velit sit amet auctor. Pellentesque mollis nisl in nibh viverra posuere. Nulla facilisi.",
                    },
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = 4.8f,
                        Source = "ITM Robledo",
                        Target = "ITM Fraternidad",
                        Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer convallis finibus nisl in fringilla. Fusce et odio dolor. Aliquam venenatis libero tortor, at tincidunt neque auctor vitae. Morbi lobortis eget quam eget convallis. Morbi id magna neque. Pellentesque sit amet imperdiet lacus. Aliquam ac facilisis elit, in aliquet felis. Morbi lacinia nunc ut tristique ornare. Integer felis risus, tempor eu placerat vel, lacinia nec ipsum. Cras eleifend orci ac dictum lacinia. Cras id porta ex, quis interdum ex. Vivamus facilisis feugiat diam, vitae euismod quam dignissim sit amet. Integer egestas a velit sit amet auctor. Pellentesque mollis nisl in nibh viverra posuere. Nulla facilisi.",
                    }
                }
            });

            _dataContext.Taxis.Add(new TaxiEntity
            {
                Plaque = "THW321",
                Trips = new List<TripEntity>
                {
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = 4.5f,
                        Source = "ITM Fraternidad",
                        Target = "ITM Robledo",
                        Remarks = "Muy buen servicio"
                    },
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = 4.8f,
                        Source = "ITM Robledo",
                        Target = "ITM Fraternidad",
                        Remarks = "Conductor muy amable"
                    }
                }
            });

            await _dataContext.SaveChangesAsync();
        }
    }
}
