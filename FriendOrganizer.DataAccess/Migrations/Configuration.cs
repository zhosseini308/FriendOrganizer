namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(
              f => f.FirstName,
                 new Friend { FirstName = "asa", LastName = "bb" },
                 new Friend { FirstName = "sss", LastName = "dd" },
                 new Friend { FirstName = "esse", LastName = "hh" },
                 new Friend { FirstName = "bsb", LastName = "ll" }
                );
            context.ProgrammingLanguages.AddOrUpdate(
              pl => pl.Name,
              new ProgrammingLanguage { Name = "Css#" },
              new ProgrammingLanguage { Name = "TypeSscript" },
              new ProgrammingLanguage { Name = "Fs#" },
              new ProgrammingLanguage { Name = "Swsift" },
              new ProgrammingLanguage { Name = "Javsa" });

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(pn => pn.Number,
              new FriendPhoneNumber { Number = "+49 123456578", FriendId = context.Friends.First().ID });


            context.Meetings.AddOrUpdate(m => m.Title,
                new Meeting
                {
                    Title = "Watching Socccer",
                    DateFrom = new DateTime(2018, 5, 26),
                    DateTo = new DateTime(2018, 5, 26),
                    Friends = new List<Friend>
                    {
                        context.Friends.First(f => f.FirstName == "asa" ),
                        context.Friends.First(f => f.FirstName == "bsb" )
                    }
                });

            context.SaveChanges();
        }
    }
}
