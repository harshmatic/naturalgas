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
            context.Employee.RemoveRange(context.Employee);
            context.SaveChanges();

            //UpdateEmployee(context);
        }
        // private static void UpdateEmployee(ApplicationContext context)
        // {


        //     var employee = new List<Employee>() {
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             FirstName = "John",
        //             LastName = "Doe",
        //             EmployeeCode = "Emp001",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9876543210",
        //             Email = "john.doe@kenyapolice.com",
        //             ResidencePhone = "020-22665544",
        //             Address1 = "Westlands Commercial Centre, Ring Road",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef9579"),	//Lasi
        //     		DesignationID = new Guid("2b72f829-5195-46c3-a6a4-06f817f11093"),	//IG
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709471111"),	//GSU
        //     		ShiftID = new Guid("B5FEDC70-D3A0-4806-BCF4-D1A30CE90555"),			//General Officers
        //             UserID= "56c385ae-ce46-41d4-b7fe-08df9aef7203",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef2222"),
        //             FirstName = "Jack",
        //             LastName = "Sparrow",
        //             EmployeeCode = "Emp002",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9823654170",
        //             Email = "jack.sparrow@kenyapolice.com",
        //             ResidencePhone = "020-22665544",
        //             Address1 = "Ngong, Olkeri, FOrest Line Road",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("411bfab2-0d44-4fb9-8835-184db90f44fa"),	//LKPC
        //     		DesignationID = new Guid("f6b0d655-5afd-44e1-a1d4-5d6bec3a7c81"),	//DIG
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709472222"),	//ASTU
        //     		ShiftID = new Guid("B5FEDC70-D3A0-4806-BCF4-D1A30CE90555"),			//General Officers
        //             UserID= "56c385ae-ce46-41d4-b7fe-08df9aef7202",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef3333"),
        //             FirstName = "Angelina",
        //             LastName = "Jolie",
        //             EmployeeCode = "Emp003",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Female",
        //             Mobile = "95135782460",
        //             Email = "angelina.jolie@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = "Salama House, Wabera Street Nairobi.",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("89234f93-6a6a-4960-a7d3-20f98f2760a8"),	//LSMS
        //     		DesignationID = new Guid("aff1592e-ba8e-4791-831c-5df49da69054"),	//SAIG
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("95998825-255A-401F-AAB1-5EF4C2A56285"),			//officers morning
        //             UserID= "56c385ae-ce46-41d4-b7fe-08df9aef7201",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef4444"),
        //             FirstName = "Brad",
        //             LastName = "Pitt",
        //             EmployeeCode = "Emp004",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9654781230",
        //             Email = "brad.pitt@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = "Nejo plaza, Kasarani",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("89234f93-6a6a-4960-a7d3-20f98f2760a8"),	//LSMS
        //     		DesignationID = new Guid("1f573249-6ee2-4506-97a6-cb0d9ce14ab9"),	//SGT
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("B5FEDC70-D3A0-4806-BCF4-D1A30CE90222"),
        //             UserID= "56c385ae-ce46-41d4-b7fe-08df9aef7303",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef5555"),
        //             FirstName = "Steve",
        //             LastName = "Rogers",
        //             EmployeeCode = "Emp005",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "8796541230",
        //             Email = "steve.rogers@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = "Kilimani Business Centre,Kirichwa Road",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("89234f93-6a6a-4960-a7d3-20f98f2760a8"),	//LSMS
        //     		DesignationID = new Guid("57bf3249-6ee2-4506-97a6-cb0d9ce14896"),	//Constable
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("B5FEDC70-D3A0-4806-BCF4-D1A30CE90333"),			//reg night
        //             UserID = "56c385ae-ce46-41d4-b7fe-08df9aef7302",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef6666"),
        //             FirstName = "Tony",
        //             LastName = "Stark",
        //             EmployeeCode = "Emp006",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9632587410",
        //             Email = "tony.stark@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = " Limuru Rd/1st Parklands Ave, Parklands, Nairobi",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("89234f93-6a6a-4960-a7d3-20f98f2760a8"),	//LSMS
        //     		DesignationID = new Guid("57bf3249-6ee2-4506-97a6-cb0d9ce14896"),	//Constable
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("95998825-255A-401F-AAB1-5EF4C2A56111"),		//reg mid day
        //             UserID = "56c385ae-ce46-41d4-b7fe-08df9aef7301",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef7777"),
        //             FirstName = "Johny",
        //             LastName = "Depp",
        //             EmployeeCode = "Emp007",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9632587412",
        //             Email = "johny.depp@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = " Limuru Rd/1st Sandlands Ave, Sandlands, Nairobi",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("411bfab2-0d44-4fb9-8835-184db90f44fa"),	//LKPC
        //     		DesignationID = new Guid("836bf2d2-7eb2-454a-a298-72a9d6aea480"),	//ASP
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("95998825-255A-401F-AAB1-5EF4C2A56111"),		//reg mid day
        //             UserID = "56c385ae-ce46-41d4-b7fe-08df9aef7204",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //         new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef8888"),
        //             FirstName = "Nick",
        //             LastName = "jones",
        //             EmployeeCode = "Emp008",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9632587412",
        //             Email = "nick.jones@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = " Limuru Rd/1st Sandlands Ave, Sandlands, Nairobi",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("411bfab2-0d44-4fb9-8835-184db90f44fa"),	//LKPC
        //     		DesignationID = new Guid("57bf3249-6ee2-4506-97a6-cb0d9ce14897"),	//Admin
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("95998825-255A-401F-AAB1-5EF4C2A56111"),		//reg mid day
        //             UserID="56c385ae-ce46-41d4-b7fe-08df9aef7101",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },
        //          new Employee{
        //             EmployeeID = new Guid("56c385ae-ce46-41d4-b7fe-08df9aef9999"),
        //             FirstName = "Tom",
        //             LastName = "Cruise",
        //             EmployeeCode = "Emp009",
        //             DateOfBirth = DateTime.Now.AddYears(-30),
        //             Gender = "Male",
        //             Mobile = "9632587412",
        //             Email = "tom.cruise@kenyapolice.com",
        //             ResidencePhone = "020-22565784",
        //             Address1 = " Limuru Rd/1st Sandlands Ave, Sandlands, Nairobi",
        //             OrganizationJoiningDate = DateTime.Now.AddYears(-5),
        //             ServiceJoiningDate = DateTime.Now.AddYears(-5),
        //             AreaID = new Guid("411bfab2-0d44-4fb9-8835-184db90f44fa"),	//LKPC
        //     		DesignationID = new Guid("57bf3249-6ee2-4506-97a6-cb0d9ce14898"),	//Admin
        //     		DepartmentID = new Guid("a1da1d8e-1111-4634-b538-a01709473333"),	//CID
        //     		ShiftID = new Guid("95998825-255A-401F-AAB1-5EF4C2A56111"),		//reg mid day
        //             UserID="56c385ae-ce46-41d4-b7fe-08df9aef7102",
        //             CreatedBy=new Guid("56c385ae-ce46-41d4-b7fe-08df9aef1111"),
        //             CreatedOn=DateTime.Now.AddHours(-6)
        //         },

        //     };

        //     context.Employee.AddRange(employee);
        //     context.SaveChanges();
        // }

    }
}
