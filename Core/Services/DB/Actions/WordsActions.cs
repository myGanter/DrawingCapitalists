using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Core.Models.DB;
using Core.Expansions;

namespace Core.Services.DB.Actions
{
    public class WordsActions : BaseActions
    {
        private readonly Random Rnd;

        public WordsActions(AppDBContext context, ActionsBuilder builder) : base(context, builder)
        {
            Rnd = new Random();
        }

        public async Task<List<Word>> GetWordsFromWordCollectionAsync(int wordCollectionId)
        {
            var wordCollection = await Context.WordCollections.Include(x => x.Words).Where(x => x.Id == wordCollectionId).FirstOrDefaultAsync();
            return wordCollection?.Words;
        }

        public async Task<List<Word>> GetWordsFromRandomWordCollectionAsync()
        {
            var collectionsCount = await Context.WordCollections.CountAsync();
            var rndCollection = await Context.WordCollections.Skip(Rnd.Next(collectionsCount)).Take(1).FirstOrDefaultAsync();

            if (rndCollection.IsNull())
                return null;

            return await GetWordsFromWordCollectionAsync(rndCollection.Id);
        }
    }
}
