using BTMBackend.Data.Repos;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;



namespace BTMBackend.Data
{
    public class PrepDb
    {
        //private readonly IEncryptRepo _encryptRepo;

        //public PrepDb(IEncryptRepo encryptRepo)
        //{
        //    _encryptRepo = encryptRepo;
        //}

        //public string GetEncryptedKey(string key)
        //{
        //    return _encryptRepo.EncryptPassword(key);
        //}

        public static void PrepData(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    if (serviceScope == null)
                        return;
                    if (serviceScope.ServiceProvider == null)
                        return;
                    SeedData(serviceScope.ServiceProvider.GetService<DataContext>(), isProduction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not SeedData:" + ex.Message);
                }
            }
        }

        public static async void SeedData(DataContext? context, bool isProduction)
        {
            if (context == null)
                return;

            if (isProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not run migrations:" + ex.Message);
                }
            }
            //==========================================
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new()
                    {
                        TitleAr = "مسؤول نظام",
                        TitleEn = "Adminstrator",
                        IsActive = true,
                    },

                    new()
                    {
                        TitleAr = "مدخل بيانات",
                        TitleEn = "Data Entery",
                        IsActive = true,
                    },

                    new()
                    {
                        TitleAr = "مشرف",
                        TitleEn = "Supervisor",
                        IsActive = true,
                    },

                    new()
                    {
                        TitleAr = "مركز اتصال",
                        TitleEn = "Call Center",
                        IsActive = true,
                    },

                    new()
                    {
                        TitleAr = "فني",
                        TitleEn = "Technician",
                        IsActive = true,
                    },

                    new()
                    {
                        TitleAr = "عميل",
                        TitleEn = "Customer",
                        IsActive = true,
                    },

                };
                await context.Roles.AddRangeAsync(roles);
                context.SaveChanges();
                var getRole = context.Roles.FirstOrDefault(x => x.TitleEn == "Adminstrator");
                if (getRole == null)
                {
                    Console.WriteLine("Roles Not Found");
                }
                else
                {
                    var user = new User()
                    {
                        Username = "772226327",
                        Password = "HR9T3orXDYvEm3ukwkX/ag==",
                        IsActive = true,
                        RoleId = getRole.Id,
                        IsVerified = true
                    };
                    context.Users.Add(user);
                    context.SaveChanges();

                    var employee = new Employee()
                    {
                        FirstName = "عبدالرحمن",
                        LastName = "الحميري",
                        PhoneNumber = "772226327",
                        Address = "Yemen , Sana'a",
                        City = 1,
                        County = 1,
                        UserId = user.Id,
                    };
                    context.Employees.Add(employee);

                    context.SaveChanges();
                }

                context.SaveChanges();

            }

            if (!context.PublicLists.Any())
            {
                var county = new List<PublicList>
                    {
                        new()
                        {
                            NameAR = "الرياض",
                            NameEN = "Riyadh",
                            Code = "County_RIY",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "مكة المكرمة",
                            NameEN = "Makkah",
                            Code = "County_MAK",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "المدينة المنورة",
                            NameEN = "Al-Madinah",
                            Code = "County_MAD",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "القصيم",
                            NameEN = "Qassim",
                            Code = "County_QAS",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "الشرقية",
                            NameEN = "Eastern",
                            Code = "County_EAS",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "عسير",
                            NameEN = "Aseer",
                            Code = "County_ASE",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "تبوك",
                            NameEN = "Tabouk",
                            Code = "County_TAB",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "حائل",
                            NameEN = "Hail",
                            Code = "County_HAI",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "الحدود الشمالية",
                            NameEN = "Northern Borders",
                            Code = "County_BOR",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "جازان",
                            NameEN = "Jazan",
                            Code = "County_JAZ",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "نجران",
                            NameEN = "Najran",
                            Code = "County_NAJ",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "الباحة",
                            NameEN = "Al-Baha",
                            Code = "County_BAH",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "الجوف",
                            NameEN = "Al-Jouf",
                            Code = "County_JOU",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "شكاوى",
                            NameEN = "Complaint",
                            Code = "Complaint",
                            Status = true,
                            Type = 0
                        },
                        new()
                        {
                            NameAR = "اقتراحات",
                            NameEN = "Suggition",
                            Code = "Suggition",
                            Status = true,
                            Type = 0
                        },


                    };
                await context.PublicLists.AddRangeAsync(county);
                context.SaveChanges();


                //if (!context.ExternalLinks.Any())
                //{
                //    var link = new List<ExternalLink>
                //    {
                //        new()
                //        {
                //            NameAr="فيسبوك",
                //            NameEn="Facebook",
                //            Link="https://www.facebook.com/",
                //            UserId=User.
                //        },
                //    };
                //};

            }

            else
            {
                Console.WriteLine("No Data Were Added");
            }

        }
    }
}
