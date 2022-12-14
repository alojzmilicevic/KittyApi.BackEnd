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
                    UserId="alojzId",
                    Username = "a",
                    FirstName = "Alojz",
                    LastName = "Milicevic",
                    PasswordHash = "MFvfOBskejHs/KNaRZvEf8On3/qlI/JUPrlOznKr4oxHS15MMVnSENkV8xmxo3uI3Cu/GOHtQWKbjalnnNTrbA==",
                    PasswordSalt = "hZNSSOJ8Qqy3Iqp0G2p1exkPcWnWySD5A9qxahVbJ40rm37twvbM8k2AemfmMSjy04kQQBe0g+CF18PDr17hmoXEo3iCVdKNgWxAvr4guV+fLqyuTK0dcDR8Ai4tEy53AZzyEQHTbateHzwPDlx0fHgy6O/GH2ST/R64yLCfqrc=",
                },
                new User()
                {
                    UserId="almaId",
                    Username = "almce",
                    LastName = "Cederblad",
                    PasswordHash = "FDxRO4rMTxEmKu9YAhu929SLjYxwsPrg/uXJhUW11fE+cTlUbK+9mH07e1sf62kShOwUoSGVzbLjgXV85WQLoQ==",
                    PasswordSalt="aoQ2sK9yu92pKLv4OoLXW+12JD0pzjGEEzX01lWsKJdRXq83P+8ZrZD36v5397PVhygyPnZfLgV62LP6bbb8owPCDzPBrrAwI8S+UsCODX4Ku+bg7nc46uK3s0AIEMGItPZrH69hbDHehRkCU91uGKTBgwxINVuTB/21rcSvEQk=", FirstName = "Alma",
                },
                new User()
                {
                    UserId="stringId",
                    Username = "string",
                    FirstName = "s",
                    LastName = "stringson",
                    PasswordHash = "7cofrx8jFs2QzXzoPpZV/FcPkdXocfqKsFVZt+VgC9xYpISzmmheZ0CKMOrgm+tpotsAcq9T0GqqYe14t2To8Q==",
                    PasswordSalt = "nqD//P9Xx+tBi3tciLyRUy/nfiDs4qrRfOUHDBWHd9ELKENSC0TV3yfm0Ryxu81Gfr7ac6fdZbYqjzWy4VK/gpFLdHfzIPIedanuG9UNPfKk1/Bi3iU6MpDv/IRg/OLap65DbdqbdiFNt473lw6VaLHTjf6ZcZjdHR1j6hmorzA="
                }
            };

            _dbContext.Users.AddRange(users);
        }

        if (!_dbContext.Thumbnails.Any())
        {
            var thumbnails = new List<Thumbnail>()
            {
                new Thumbnail()
                {
                    ThumbnailName = "Cat Tree Cam", ThumbnailPath = "/images/stream-types/tree.jpg"
                },
                new Thumbnail()
                {
                    ThumbnailName = "Food Cam", ThumbnailPath = "/images/stream-types/eat.jpg"
                },
                new Thumbnail()
                {
                    ThumbnailName = "Sleep Cam", ThumbnailPath = "/images/stream-types/sleep.jpg"
                },
                new Thumbnail()
                {
                    ThumbnailName = "Toilet Cam", ThumbnailPath = "/images/stream-types/wc.jpg"
                },
                new Thumbnail()
                {
                    ThumbnailName = "NotFound", ThumbnailPath = "/images/stream-types/404-cat.png"
                },
            };
            _dbContext.Thumbnails.AddRange(thumbnails);

        }
        _dbContext.SaveChanges();
    }


}
