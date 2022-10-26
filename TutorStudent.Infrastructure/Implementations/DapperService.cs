using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using TutorStudent.Domain.DependencyInjectionAttribute;
using TutorStudent.Domain.Interfaces;

namespace TutorStudent.Infrastructure.Implementations
{
    [SingletonDependency(ServiceType = (typeof(IDapperService)))]
    public class DapperService : IDapperService
    {
        private readonly int defaultCommandTimeout = 3000;

        public IEnumerable<TEntity> QuerySP<TEntity>(string storedProcedure,
                                            object param = null,
                                            dynamic outParam = null,
                                            SqlTransaction transaction = null,
                                            bool buffered = true,
                                            int? commandTimeout = null) where TEntity : class
        {

            using IDbConnection _db = new SqlConnection
                ("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TutorStudent;");

            return _db.Query<TEntity>(storedProcedure,
                                    param: param,
                                    transaction: transaction,
                                    buffered: buffered,
                                    commandTimeout: (commandTimeout > 0 ? commandTimeout : defaultCommandTimeout),
                                    commandType: CommandType.StoredProcedure);
        }
    }
}
