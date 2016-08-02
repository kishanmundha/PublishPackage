using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishPackage.Models;
using System.Collections.Generic;
using Moq;

namespace PublishPackageTest.Models
{
    [TestClass]
    public class SqlTests
    {
        private static readonly string ArgumentNullExceptionMessage = "Value cannot be null.\r\nParameter Name: {0}";
        private static readonly string ArgumentExceptionMessage = "Value cannot be empty.\r\nParameter Name: {0}";

        #region SqlColumn

        [TestMethod]
        public void SqlColumnGetScriptTest()
        {
            List<Tuple<string, SqlColumn>> columns = new List<Tuple<string, SqlColumn>>();

            columns.Add(new Tuple<string, SqlColumn>("[Id] [int] NULL", new SqlColumn { ColumnName = "Id", DataType = "int", IsNullable = true }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [int] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "int", IsNullable = false }));

            // DataType capital test
            columns.Add(new Tuple<string, SqlColumn>("[Id] [int] NULL", new SqlColumn { ColumnName = "Id", DataType = "INT", IsNullable = true }));

            // DataType tests
            columns.Add(new Tuple<string, SqlColumn>("[Id] [bigint] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "bigint" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [binary](50) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "binary", Length = 50 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [bit] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "bit" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [char](10) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "char", Length = 10 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [date] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "date" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [datetime] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "datetime" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [datetime2](7) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "datetime2", DateTimePrec = 7 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [datetimeoffset](7) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "datetimeoffset", DateTimePrec = 7 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [decimal](8,2) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "decimal", Prec = 8, Scale = 2 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [float] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "float" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [geography] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "geography" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [geometry] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "geometry" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [hierarchyid] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "hierarchyid" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [image] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "image" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [int] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "int" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [money] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "money" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [nchar](10) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "nchar", Length = 10 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [ntext] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "ntext" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [numeric](8,2) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "numeric", Prec = 8, Scale = 2 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [nvarchar](50) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "nvarchar", Length = 50 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [nvarchar](max) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "nvarchar", Length = -1 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [real] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "real" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [smalldatetime] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "smalldatetime" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [smallint] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "smallint" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [smallmoney] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "smallmoney" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [sql_variant] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "sql_variant" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [text] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "text" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [time](7) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "time", DateTimePrec = 7 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [timestamp] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "timestamp" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [tinyint] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "tinyint" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [uniqueidentifier] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "uniqueidentifier" }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [varbinary](50) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "varbinary", Length = 50 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [varbinary](max) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "varbinary", Length = -1 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [varchar](50) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "varchar", Length = 50 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [varchar](max) NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "varchar", Length = -1 }));
            columns.Add(new Tuple<string, SqlColumn>("[Id] [xml] NOT NULL", new SqlColumn { ColumnName = "Id", DataType = "xml" }));

            foreach (var column in columns)
            {
                Assert.AreEqual(column.Item1, column.Item2.GetScript());
            }
        }

        [TestMethod]
        public void SqlColumnValueSetTest()
        {
            SqlColumn column = new SqlColumn();

            ExceptionAssert.Throws<ArgumentNullException>(() => column.ColumnName = null, string.Format(ArgumentNullExceptionMessage, "ColumnName"));

            ExceptionAssert.Throws<ArgumentException>(() => column.ColumnName = string.Empty, string.Format(ArgumentExceptionMessage, "ColumnName"));
            ExceptionAssert.Throws<ArgumentException>(() => column.ColumnName = " ", string.Format(ArgumentExceptionMessage, "ColumnName"));


            ExceptionAssert.Throws<ArgumentNullException>(() => column.DataType = null, string.Format(ArgumentNullExceptionMessage, "DataType"));

            ExceptionAssert.Throws<ArgumentException>(() => column.DataType = string.Empty, string.Format(ArgumentExceptionMessage, "DataType"));
            ExceptionAssert.Throws<ArgumentException>(() => column.DataType = " ", string.Format(ArgumentExceptionMessage, "DataType"));
        }

        #endregion

        #region SqlConstraintKey

        [TestMethod]
        public void SqlConstraintKeyGetScriptTest()
        {
            SqlConstraintKey key1 = new SqlConstraintKey { KeyName = "PK_Table1", IsClustred = true, KeyType = SqlConstraintKeyType.PrimaryKey, Columns = new System.Collections.Generic.List<Tuple<string, bool>>() { new Tuple<string, bool>("Id", false) } };

            Assert.AreEqual(@"CONSTRAINT [PK_Table1] PRIMARY KEY CLUSTERED ([Id] ASC)", key1.GetScript());

            SqlConstraintKey key2 = new SqlConstraintKey { KeyName = "IX_Table1", IsClustred = false, KeyType = SqlConstraintKeyType.UniqueKey, Columns = new System.Collections.Generic.List<Tuple<string, bool>>() { new Tuple<string, bool>("Id", false), new Tuple<string, bool>("Age", true) } };

            Assert.AreEqual(@"CONSTRAINT [IX_Table1] UNIQUE NONCLUSTERED ([Id] ASC, [Age] DESC)", key2.GetScript());
        }

        #endregion

        #region SqlDefaultConstraint

        [TestMethod]
        public void SqlDefaultConstraintGetAddScriptTest()
        {
            SqlDefaultConstraint constraint = new SqlDefaultConstraint { TableName = "Table1", ColumnName = "Column1", KeyName = "DF_Table1_Column1", Definition = "((0))" };

            Assert.AreEqual(@"ALTER TABLE [Table1] ADD CONSTRAINT [DF_Table1_Column1] DEFAULT ((0)) FOR [Column1]
GO

", constraint.GetAddScript());
        }

        [TestMethod]
        public void SqlDefaultConstraintGetDropScriptTest()
        {
            SqlDefaultConstraint constraint = new SqlDefaultConstraint { TableName = "Table1", ColumnName = "Column1", KeyName = "DF_Table1_Column1", Definition = "((0))" };

            Assert.AreEqual(@"ALTER TABLE [Table1] DROP CONSTRAINT [DF_Table1_Column1]
GO

", constraint.GetDropScript());
        }

        [TestMethod]
        public void SqlDefaultConstraintGetRenameScriptTest()
        {
            SqlDefaultConstraint constraint = new SqlDefaultConstraint { ColumnName = "Column1", KeyName = "DF_Table1_Column1", Definition = "((0))" };

            Assert.AreEqual(@"EXEC sp_rename N'OLD_KEY', N'DF_Table1_Column1', N'OBJECT'", constraint.GetRenameScript("OLD_KEY"));
        }

        #endregion

        #region SqlForeignKey

        [TestMethod]
        public void SqlForeignKeyGetScriptTest()
        {
            var key = new SqlForeignKey { TableName="Table1", KeyName = "FK_Table1_Table2", KeyColumnName = "Id", ForeignTableName = "Table2", ForeignColumnName = "Id" };

            Assert.AreEqual(@"ALTER TABLE [Table1] WITH CHECK ADD CONSTRAINT [FK_Table1_Table2] FOREIGN KEY ([Id]) REFERENCES [Table2] ([Id])
GO

ALTER TABLE [Table1] CHECK CONSTRAINT [FK_Table1_Table2]
GO

", key.GetScript());
        }

        [TestMethod]
        public void SqlForeignKeyGetDropScriptTest()
        {
            var key = new SqlForeignKey { TableName= "Table1",KeyName = "FK_Table1_Table2", KeyColumnName = "Id", ForeignTableName = "Table2", ForeignColumnName = "Id" };

            Assert.AreEqual("ALTER TABLE [Table1] DROP CONSTRAINT [FK_Table1_Table2]\r\nGO\r\n\r\n", key.GetDropScript());
        }

        #endregion

        #region SqlCheckConstraint

        [TestMethod]
        public void SqlCheckConstraintGetScriptTest()
        {
            var constraint = new SqlCheckConstraint { TableName = "Table1", ConstraintName = "CK_Value", CheckClause = "[Id]>(0)" };

            Assert.AreEqual(@"ALTER TABLE [Table1] WITH CHECK ADD CONSTRAINT [CK_Value] CHECK ([Id]>(0))
GO

ALTER TABLE [Table1] CHECK CONSTRAINT [CK_Value]
GO

", constraint.GetScript());
        }

        #endregion

        #region SqlTable

        [TestMethod]
        public void SqlTableGetCreateScriptTest()
        {
            SqlTable table = new SqlTable();
            table.TableName = "Table1";

            Mock<SqlColumn> column1 = new Mock<SqlColumn>();
            column1.Setup(x => x.GetScript()).Returns("[Id] [int] IDENTITY(1,1) NOT NULL");

            Mock<SqlColumn> column2 = new Mock<SqlColumn>();
            column2.Setup(x => x.GetScript()).Returns("[Name] [varchar](50) NULL");

            table.Columns.Add(column1.Object);
            table.Columns.Add(column2.Object);

            var checkConstraint1 = new Mock<SqlCheckConstraint>();
            var checkConstraint2 = new Mock<SqlCheckConstraint>();
            checkConstraint1.Setup(x => x.GetScript()).Returns("CHECKCONSTRAINT1\r\nGO\r\n\r\n");
            checkConstraint2.Setup(x => x.GetScript()).Returns("CHECKCONSTRAINT2\r\nGO\r\n\r\n");

            table.CheckConstraints.Add(checkConstraint1.Object);
            table.CheckConstraints.Add(checkConstraint2.Object);

            var defaultConstraint1 = new Mock<SqlDefaultConstraint>();
            var defaultConstraint2 = new Mock<SqlDefaultConstraint>();
            defaultConstraint1.Setup(x => x.GetAddScript()).Returns("DEFAULTCONSTRAINT1\r\nGO\r\n\r\n");
            defaultConstraint2.Setup(x => x.GetAddScript()).Returns("DEFAULTCONSTRAINT2\r\nGO\r\n\r\n");

            table.DefaultConstraints.Add(defaultConstraint1.Object);
            table.DefaultConstraints.Add(defaultConstraint2.Object);

            var foreignKey1 = new Mock<SqlForeignKey>();
            var foreignKey2 = new Mock<SqlForeignKey>();
            foreignKey1.Setup(x => x.GetScript()).Returns("FOREIGNKEY1\r\nGO\r\n\r\n");
            foreignKey2.Setup(x => x.GetScript()).Returns("FOREIGNKEY2\r\nGO\r\n\r\n");

            table.ForeignKeys.Add(foreignKey1.Object);
            table.ForeignKeys.Add(foreignKey2.Object);

            var constraintKey1 = new Mock<SqlConstraintKey>();
            var constraintKey2 = new Mock<SqlConstraintKey>();
            constraintKey1.Setup(x => x.GetScript()).Returns("CONSTRAINTKEY1\r\nGO\r\n\r\n");
            constraintKey2.Setup(x => x.GetScript()).Returns("CONSTRAINTKEY2\r\nGO\r\n\r\n");

            //table.ConstraintKeys.Add(constraintKey1.Object);
            //table.ConstraintKeys.Add(constraintKey2.Object);

            Assert.AreEqual(@"CREATE TABLE [Table1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL
)
GO

DEFAULTCONSTRAINT1
GO

DEFAULTCONSTRAINT2
GO

FOREIGNKEY1
GO

FOREIGNKEY2
GO

CHECKCONSTRAINT1
GO

CHECKCONSTRAINT2
GO

", table.GetCreateScript());

        }

        #endregion

        #region DataCompare
        [TestMethod]
        public void CompareTest()
        {
            SqlDatabase db1 = new SqlDatabase
            {
                Tables = new List<SqlTable>()
                {
                    new SqlTable
                    {
                        TableName = "Table1", Columns = new List<SqlColumn>()
                        {
                            new SqlColumn
                            {
                                ColumnName = "Column1",
                                DataType = "int",
                                Identity = new SqlIdentityColumn { IncrementValue = 1, SeedValue = 1}
                            },
                            new SqlColumn
                            {
                                ColumnName = "Column2",
                                DataType = "varchar",
                                Length = 50
                            }
                        }
                    }
                },
                Views = new List<SqlView>() { },
                Procedures = new List<SqlProcedure>() { }
            };

            SqlDatabase db2 = new SqlDatabase()
            {
                Tables = new List<SqlTable>()
                {
                    new SqlTable
                    {
                        TableName = "Table1", Columns = new List<SqlColumn>()
                        {
                            new SqlColumn
                            {
                                ColumnName = "Column1",
                                DataType = "int",
                                Identity = new SqlIdentityColumn { IncrementValue = 1, SeedValue = 1}
                            },
                            new SqlColumn
                            {
                                ColumnName = "Column3",
                                DataType = "varchar",
                                Length = 50
                            }
                        }
                    }
                },
                Views = new List<SqlView>() { },
                Procedures = new List<SqlProcedure>() { }
            };

            var compareResult = SqlDatabaseCompareResult.Compare(db1, db2);

            Console.WriteLine(compareResult.GetScript());
        }

        [TestMethod]
        public void SqlDataReaderTest()
        {
            ISqlReader reader = new SqlReaderByDatabase();
            var database = reader.Get("data source=10.160.0.18;initial catalog=BookMyFood;integrated security=False;User Id=sa;Password=zoomi@123");

            var str = database.GetCreateScript();
        }
        #endregion
    }
}
