﻿using System.Collections.Generic;
using Antix.Data.Keywords;
using Antix.Data.Keywords.Entities;
using Antix.Data.Keywords.Processing;
using Xunit;

namespace Antix.Tests.Data.Keywords
{
    public class KeywordManagerTests
    {
        static KeywordsManager GetService()
        {
            return new KeywordsManager(
                new SplitByWhitespaceKeywordProcessor());
        }

        [Fact]
        public void GetsAllKeywords()
        {
            var entity = new Entity
                {
                    Text = "aa bb",
                    SubCollection = new[]
                        {
                            new SubEntity {Text = "cc dd ee"},
                            new SubEntity {Text = "ff"}
                        }
                };

            var manager = GetService();

            manager
                .IndexEntity<Entity>()
                .Index(e => e.Text)
                .ForEach(e => e.SubCollection, b => b.Index(e => e.Text));

            var result = manager.GetKeywords(entity);

            Assert.Equal(new[] {"aa", "bb", "cc", "dd", "ee", "ff"}, result);
        }

        class Entity
        {
            public string Text { get; set; }
            public ICollection<SubEntity> SubCollection { get; set; }

            public IEnumerable<EntityKeyword> Keywords { get; set; }
        }

        class EntityKeyword : IEntityKeyword
        {
            public IKeyword Keyword { get; set; }
            public int Frequency { get; set; }
        }

        class SubEntity
        {
            public string Text { get; set; }
        }
    }
}