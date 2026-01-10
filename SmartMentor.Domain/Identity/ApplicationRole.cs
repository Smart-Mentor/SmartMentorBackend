using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Persistence.Identity
{
    public class ApplicationRole:IdentityRole<Guid>
    {
        public string  Description { get; set; }
    }
}
