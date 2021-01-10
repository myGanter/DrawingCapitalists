using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.DB.Actions
{
    public abstract class BaseActions
    {
        protected readonly AppDBContext Context;

        protected readonly ActionsBuilder Builder;

        internal bool SaveChangesMode { get; set; }

        public BaseActions(AppDBContext context, ActionsBuilder builder)
        {
            Context = context;
            Builder = builder;
            SaveChangesMode = true;
        }

        public async Task TransactionAsync(Func<Task> clbk)
        {
            Builder.SetNotSaveChangesMode(false);

            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                await clbk();

                await Context.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            Builder.SetNotSaveChangesMode(true);
        }

        protected async Task SaveChangesAsync()
        {
            if (SaveChangesMode)
                await Context.SaveChangesAsync();
        }
    }
}
