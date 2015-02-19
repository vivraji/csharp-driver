﻿using System;
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Cassandra.Tests.Mapping.Pocos;
using NUnit.Framework;

namespace Cassandra.Tests.Mapping
{
    [TestFixture]
    public class AttributeBasedTypeDefinitionTests
    {
        [Test]
        public void AttributeBasedTypeDefinition_Defaults_Tests()
        {
            //Non decorated Poco
            var tableConfig = new TableMappingConfig(typeof (AllTypesEntity));
            var definition = new AttributeBasedTypeDefinition(typeof (AllTypesEntity));
            definition.ApplyTo(tableConfig);

            Assert.False(tableConfig.CaseSensitive);
            Assert.False(tableConfig.CompactStorage);
            Assert.False(tableConfig.AllowFiltering);
            Assert.False(tableConfig.ExplicitColumns);
            Assert.AreEqual(0, tableConfig.ClusteringKeys.Count);
            Assert.AreEqual(0, tableConfig.PartitionKeys.Count);
            Assert.Null(tableConfig.KeyspaceName);
            Assert.AreEqual("AllTypesEntity", tableConfig.TableName);
            Assert.AreEqual(typeof(AllTypesEntity), definition.PocoType);
        }

        [Test]
        public void AttributeBased_Single_PartitionKey_Test()
        {
            var tableConfig = new TableMappingConfig(typeof (DecoratedUser));
            var definition = new AttributeBasedTypeDefinition(typeof (DecoratedUser));
            definition.ApplyTo(tableConfig);

            Assert.False(tableConfig.CaseSensitive);
            Assert.False(tableConfig.CompactStorage);
            Assert.False(tableConfig.AllowFiltering);
            Assert.False(tableConfig.ExplicitColumns);
            Assert.AreEqual(0, tableConfig.ClusteringKeys.Count);
            CollectionAssert.AreEqual(new[] { "userid" }, tableConfig.PartitionKeys);
        }

        [Test]
        public void AttributeBased_Composite_PartitionKey_Test()
        {
            var tableConfig = new TableMappingConfig(typeof (DecoratedTimeSeries));
            var definition = new AttributeBasedTypeDefinition(typeof (DecoratedTimeSeries));
            definition.ApplyTo(tableConfig);

            Assert.True(tableConfig.CaseSensitive);
            Assert.False(tableConfig.CompactStorage);
            Assert.False(tableConfig.AllowFiltering);
            Assert.False(tableConfig.ExplicitColumns);
            CollectionAssert.AreEqual(new[] { Tuple.Create("Time", SortOrder.Unspecified) }, tableConfig.ClusteringKeys);
            CollectionAssert.AreEqual(new[] { "name", "Slice" }, tableConfig.PartitionKeys);
        }
    }
}
