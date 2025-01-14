using coworking.Entities;
using coworking.UnitOfWork;
using AutoFixture;
using System;

namespace coworking.Test.Helpers;

public class UnitOfWorkMock : BaseTest
{
    public const string GUID_UNIQUE = "5d3c7112-d5b3-46e2-89ea-52dd69659671";

    public coworking.UnitOfWork.Interfaces.Base.IUnitOfWork CreateUnitOfWork()
    {
        var coworkingContext = new CoworkingDbMock().CreateDbContext();
        //var evt = _fixture.Create<Event>();
        //evt.StartDate = DateTime.Now.AddDays(1); //needed this config for some test executed ok
        //evt.Id = Guid.Parse(GUID_UNIQUE);
        //evt.City.StateId = Guid.Parse(GUID_UNIQUE);
        //evt.City.Id = Guid.Parse(GUID_UNIQUE);
        //evt.City.State.Id = Guid.Parse(GUID_UNIQUE);
        //evt.CityId = Guid.Parse(GUID_UNIQUE);
        //coworkingContext.Events.Add(evt);
        //coworkingContext.SaveChanges();
        return new coworking.UnitOfWork.Repositories.Base.UnitOfWork(coworkingContext);
    }
}
