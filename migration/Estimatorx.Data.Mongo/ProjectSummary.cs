using System;
using System.ComponentModel;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Estimatorx.Data.Mongo
{
    /// <summary>
    /// A project estimate model
    /// </summary>
    public class ProjectSummary
    {

        /// <summary>
        /// Gets or sets the identifier for the model.
        /// </summary>
        /// <value>
        /// The identifier for the model.
        /// </value>
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [BsonElement("nm")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        /// <value>
        /// The description of the project.
        /// </value>
        [BsonElement("ds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the hours per week to use when calculating an estimates total number of weeks.
        /// </summary>
        /// <value>
        /// The hours per week to use when calculating an estimates total number of weeks.
        /// </value>
        [DefaultValue(0)]
        public int HoursPerWeek { get; set; }

        /// <summary>
        /// Gets or sets the contingency percentage rate for the project. Contingency rate is multiplied
        /// by the <see cref="TotalHours"/> to give the <see cref="ContingencyHours"/>.
        /// </summary>
        /// <value>
        /// The contingency percentage rate for the project.
        /// </value>
        [DefaultValue(0)]
        public double ContingencyRate { get; set; }

        /// <summary>
        /// Gets or sets the total number tasks estimated for the project.
        /// </summary>
        /// <value>
        /// The total number tasks estimated for the project.
        /// </value>
        [DefaultValue(0)]
        public int TotalTasks { get; set; }

        /// <summary>
        /// Gets or sets the total number hours estimated for the project.
        /// </summary>
        /// <value>
        /// The total hours number estimated for the project.
        /// </value>
        [DefaultValue(0)]
        public int TotalHours { get; set; }

        /// <summary>
        /// Gets or sets the total number weeks estimated for the project.
        /// </summary>
        /// <value>
        /// The total number weeks estimated for the project.
        /// </value>
        [DefaultValue(0)]
        public double TotalWeeks { get; set; }

        /// <summary>
        /// Gets or sets the total contingency hours for the project. Contingency hours are calculated 
        /// by multiplying the <see cref="TotalHours"/> by the <see cref="ContingencyRate"/>.
        /// </summary>
        /// <value>
        /// The total contingency hours for the project.
        /// </value>
        [DefaultValue(0)]
        public int ContingencyHours { get; set; }

        /// <summary>
        /// Gets or sets the total contingency weeks for the project. Contingency weeks are calculated 
        /// by multiplying the <see cref="TotalWeeks"/> by the <see cref="ContingencyRate"/>.
        /// </summary>
        /// <value>
        /// The total contingency weeks for the project.
        /// </value>
        [DefaultValue(0)]
        public double ContingencyWeeks { get; set; }

        /// <summary>
        /// Gets or set the organization this project belongs to.
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the system create date.
        /// </summary>
        /// <value>
        /// The system create date.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the system create user.
        /// </summary>
        /// <value>
        /// The system create user.
        /// </value>
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the system update date.
        /// </summary>
        /// <value>
        /// The system update date.
        /// </value>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the system update user.
        /// </summary>
        /// <value>
        /// The system update user.
        /// </value>
        public string Updater { get; set; }

    }
}
