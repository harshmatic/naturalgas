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
                for (int i = 1; i <= 100; i++)
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
                    if (k == 10)
                        d++;
                    if (k == 25)
                        d++;
                    if (k == 45)
                        d++;
                    if (k == 60)
                        d++;
                    if (k == 70)
                        d++;
                    if (k == 90)
                        d++;
                    if (k == 130)
                        d++;
                    if (k == 180)
                        d++;
                    if (k == 210)
                        d++;
                    if (k == 300)
                        d++;
                    if (k == 340)
                        d++;
                    if (k == 380)
                        d++;
                    if (k == 400)
                        d++;
                    if (k == 430)
                        d++;
                    if (k == 450)
                        d++;
                    if (k == 470)
                        d++;
                    if (k == 490)
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
