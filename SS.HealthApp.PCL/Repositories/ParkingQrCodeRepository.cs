using PCLStorage;
using System.Threading.Tasks;
using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;
using System.IO;
using System;

namespace SS.HealthApp.PCL.Repositories
{
    class ParkingQrCodeRepository : BaseRepository<object>
    {
        protected override string GetRepositoryFilename()
        {
            return "parkingqrcode.txt";
        }

    }
}
