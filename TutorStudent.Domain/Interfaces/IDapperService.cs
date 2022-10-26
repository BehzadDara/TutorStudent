using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TutorStudent.Domain.Interfaces
{
    public interface IDapperService
    {
        IEnumerable<TEntity> QuerySP<TEntity>(string storedProcedure,
                                            dynamic param = null,
                                            dynamic outParam = null,
                                            SqlTransaction transaction = null,
                                            bool buffered = true,
                                            int? commandTimeout = null) where TEntity : class;

    }
}
