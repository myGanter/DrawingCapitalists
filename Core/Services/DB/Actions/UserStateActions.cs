using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Core.Models.DB;

namespace Core.Services.DB.Actions
{
    public class UserStateActions : BaseActions
    {      
        public UserStateActions(AppDBContext context, ActionsBuilder builder) : base(context, builder)
        { }

        public async Task<UserState> GetAsync(string name, string fingerPrint)
        {
            return await Context.UserStates.FirstOrDefaultAsync(x => x.Name == name && x.FingerPrint == fingerPrint);            
        }

        public async Task AddAsync(UserState userState)
        {
            await Context.UserStates.AddAsync(userState);
            await SaveChangesAsync();
        }
    }
}
