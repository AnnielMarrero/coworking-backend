using coworking.Entities;

namespace coworking.Data
{
    public class DataSeeder
    {
        private readonly CoworkingManager _dbContext;


        public DataSeeder(CoworkingManager dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            if (!_dbContext.Room.Any())
            {
                Room[] rooms = new Room[5];
                for (int i = 0; i < rooms.Length; i++)
                {
                    rooms[i] = new Room
                    {
                        Location = "lOC " + ('A' + i),
                        Capacity = 10,
                        IsAvailable = true,
                        CreatedBy = "test",
                        UpdatedBy = "test"
                    };
                };

                _dbContext.Room.AddRange(rooms);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Rol.Any())
            {
                Rol[] rols = new Rol[2];
                rols[0] = new Rol { 
                    Name = "Usuario Estandar",
                    CreatedBy = "test",
                    UpdatedBy = "test"
                };
                rols[1] = new Rol
                {
                    Name = "administrador",
                    CreatedBy = "test",
                    UpdatedBy = "test"
                };
                

                _dbContext.Rol.AddRange(rols);
                _dbContext.SaveChanges();
            }

        }
    }
}
