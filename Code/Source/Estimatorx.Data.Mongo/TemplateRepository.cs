using System;
using System.Linq;
using System.Linq.Expressions;
using Estimatorx.Core;
using Estimatorx.Core.Providers;
using Estimatorx.Core.Security;
using MongoDB.Abstracts;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog.Fluent;

namespace Estimatorx.Data.Mongo
{
    public class TemplateRepository
        : MongoRepository<Template, string>, ITemplateRepository
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public TemplateRepository()
            : this("EstimatorxMongo")
        {
        }

        public TemplateRepository(string connectionName)
            : base(MongoFactory.GetDatabaseFromConnectionName(connectionName))
        {
        }

        public TemplateRepository(MongoUrl mongoUrl)
            : base(MongoFactory.GetDatabaseFromMongoUrl(mongoUrl))
        {
        }


        public IQueryable<TemplateSummary> Summaries()
        {
            return All().Select(SelectSummary());
        }


        public override string EntityKey(Template entity)
        {
            return entity.Id;
        }

        protected override Expression<Func<Template, bool>> KeyExpression(string key)
        {
            return template => template.Id == key;
        }


        protected override void BeforeInsert(Template entity)
        {
            entity.Created = DateTime.Now;
            entity.Creator = UserName.Current();
            entity.Updated = DateTime.Now;
            entity.Updater = UserName.Current();

            _logger.Debug()
                .Message("Template '{0}' created by '{1}'", entity.Name, entity.Creator)
                .Property("Organization", entity.OrganizationId)
                .Write();

            base.BeforeInsert(entity);
        }

        protected override void BeforeUpdate(Template entity)
        {
            if (entity.Created == DateTime.MinValue)
                entity.Created = DateTime.Now;

            if (string.IsNullOrEmpty(entity.Creator))
                entity.Creator = UserName.Current();

            entity.Updated = DateTime.Now;
            entity.Updater = UserName.Current();

            base.BeforeUpdate(entity);
        }


        protected override void EnsureIndexes(IMongoCollection<Template> mongoCollection)
        {
            base.EnsureIndexes(mongoCollection);

            mongoCollection.Indexes.CreateOne(
                new CreateIndexModel<Template>(
                    Builders<Template>.IndexKeys
                        .Ascending(s => s.OrganizationId)
                        .Descending(s => s.Updated)
                )
            );
        }


        public static Expression<Func<Template, TemplateSummary>> SelectSummary()
        {
            return p => new TemplateSummary
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OrganizationId = p.OrganizationId,
                Created = p.Created,
                Creator = p.Creator,
                Updated = p.Updated,
                Updater = p.Updater
            };
        }

    }
}