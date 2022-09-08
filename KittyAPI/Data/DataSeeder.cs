using KittyAPI.Models;

namespace KittyAPI.Data;

public class DataSeeder
{
    private readonly DataContext _dbContext;

    public DataSeeder(DataContext userDbContext)
    {
        _dbContext = userDbContext;
    }

    public void Seed()
    {
        if (!_dbContext.Users.Any())
        {
            var users = new List<User>()
            {
                new User()
                {
                    UserId=Guid.NewGuid().ToString(), Username = "alomi136", Email = "a", PasswordHash = "MFvfOBskejHs/KNaRZvEf8On3/qlI/JUPrlOznKr4oxHS15MMVnSENkV8xmxo3uI3Cu/GOHtQWKbjalnnNTrbA==", PasswordSalt = "hZNSSOJ8Qqy3Iqp0G2p1exkPcWnWySD5A9qxahVbJ40rm37twvbM8k2AemfmMSjy04kQQBe0g+CF18PDr17hmoXEo3iCVdKNgWxAvr4guV+fLqyuTK0dcDR8Ai4tEy53AZzyEQHTbateHzwPDlx0fHgy6O/GH2ST/R64yLCfqrc=", FirstName = "Alojz",
                    LastName = "Milicevic", Role = "Administrator"
                },
                new User()
                {
                    UserId=Guid.NewGuid().ToString(), 
                    Username = "almce", Email = "alma", PasswordHash = "FDxRO4rMTxEmKu9YAhu929SLjYxwsPrg/uXJhUW11fE+cTlUbK+9mH07e1sf62kShOwUoSGVzbLjgXV85WQLoQ==", PasswordSalt="aoQ2sK9yu92pKLv4OoLXW+12JD0pzjGEEzX01lWsKJdRXq83P+8ZrZD36v5397PVhygyPnZfLgV62LP6bbb8owPCDzPBrrAwI8S+UsCODX4Ku+bg7nc46uK3s0AIEMGItPZrH69hbDHehRkCU91uGKTBgwxINVuTB/21rcSvEQk=", FirstName = "Alma",
                    LastName = "Cederblad", Role = "User",
                }
            };

            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
        }
    }
}
