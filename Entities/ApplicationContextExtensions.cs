using System;
using System.Collections.Generic;
using ESPL.NG.Entities.Core;
using ESPL.NG.Enums;
namespace ESPL.NG.Entities
{
    public static class ApplicationContextExtensions
    {
        public static void EnsureSeedDataForContext(this ApplicationContext context)
        {
            // context.Customer.RemoveRange(context.Customer);
            // context.SaveChanges();
            // UpdateCustomer(context);
        }

        private static void UpdateCustomer(ApplicationContext context)
        {

            int k = 1, d = 0;
            for (int j = 0; j < 5; j++)
            {

                var customers = new List<Customer>();
                for (int i = 1; i <= 1000; i++)
                {
                    var cust = new Customer
                    {
                        NationalID = GenrateRandomNumber(),
                        SerialNumber = GenrateRandomNumber(),
                        CustomerID = Guid.NewGuid(),
                        Firstname = "John" + k.ToString(),
                        Surname = "Doe" + k.ToString(),
                        Mobile = "9876543210",
                        Email = "john.doe" + k.ToString() + "@gmail.com",
                        Gender = k % 10 == 0 ? "F" : "M",
                        DateOfBirth = DateTime.Now.AddYears(-30).AddDays(k),
                        Address = "Westlands Commercial Centre, Ring Road",
                        Status = true,
                        DistributorName = "Natural Gas Agency",
                        DistributorAddress = "Westlands Commercial Centre, DP Road",
                        DistributorContact = "9898989898",
                        CreatedOn = DateTime.Now.AddMonths(d).AddYears(-1)
                    };
                    customers.Add(cust);
                    k++;
                    if (k == 100)
                        d++;
                    else if (k == 350)
                        d++;
                    else if (k == 700)
                        d++;
                    else if (k == 1200)
                        d++;
                    else if (k == 1600)
                        d++;
                    else if (k == 1900)
                        d++;
                    else if (k == 2245)
                        d++;
                    else if (k == 2468)
                        d++;
                    else if (k == 2600)
                        d++;
                    else if (k == 2900)
                        d++;
                    else if (k == 3102)
                        d++;
                    else if (k == 3400)
                        d++;
                    else if (k == 3800)
                        d++;
                    else if (k == 4250)
                        d++;
                    else if (k == 4754)
                        d++;
                    else if (k == 4850)
                        d++;
                    else if (k == 4902)
                        d++;

                };
                context.Customer.AddRange(customers);
                context.SaveChanges();


            }

        }

        private static string GenrateRandomNumber()
        {
            Random rnd = new Random();
            var result = rnd.Next(10000000, 99999999).ToString();
            return result;
        }
    }
}
