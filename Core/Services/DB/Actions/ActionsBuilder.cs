using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Expansions;

namespace Core.Services.DB.Actions
{
    public class ActionsBuilder
    {
        public readonly AppDBContext Context;

        public ActionsBuilder(AppDBContext context)
        {
            Context = context;
        }

        private UserStateActions _UserStateActions;
        public UserStateActions GetUserStateActions
        { 
            get 
            {
                if (_UserStateActions.IsNull())
                    _UserStateActions = new UserStateActions(Context, this);

                return _UserStateActions;
            } 
        }

        internal void SetNotSaveChangesMode(bool mode)
        {
            if (_UserStateActions.IsNotNull())
                _UserStateActions.SaveChangesMode = mode;
        }
    }
}
