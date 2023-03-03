/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 18-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using DaiCo.Transaction;
using System.Data;
namespace DaiCo.TransactionImplement
{

  public class TransactionFactoryImpl : TransactionFactory
  {
    #region Contructors
    public TransactionFactoryImpl()
    {
    }
    #endregion Contructors

    #region Methods
    /// <summary>
    /// This method is used to Insert a records into database (Ham nay dung de insert mot record bat ky vao database)
    /// </summary>
    /// <param name="obj">This object need to insert into database (Doi tuong nay dung de insert vao database)</param>
    /// <returns>a IObject (Ket qua tra ve doi tuong kieu IObject tuong ung voi record vua insert vao database)</returns>
    public override ITransaction InsertObject(IObject obj)
    {
      return new InsertObjectTransaction(obj);
    }

    /// <summary>
    /// This method is used to Update a record in database by primary key. (Ham nay dung de update mot record trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object need to update in database. (Doi tuong can cap nhat) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    /// <returns>If update successfully Then return true Else return false. (Neu update thanh cong thi tra ve TRUE, nguoc lai tra ve FALSE</returns>
    public override ITransaction UpdateObject(IObject obj, params string[] primaryKeyNames)
    {
      return new UpdateObjectTransaction(obj, primaryKeyNames);
    }

    /// <summary>
    /// This method is used to Update a record in database by primary key. (Ham nay dung de update mot record trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object need to update in database. (Doi tuong can cap nhat) </param>
    /// <returns>If update successfully Then return true Else return false. (Neu update thanh cong thi tra ve TRUE, nguoc lai tra ve FALSE</returns>
    public override ITransaction UpdateObject(IObject obj)
    {
      return new UpdateObjectByKeyTransaction(obj);
    }

    /// <summary>
    /// This method is used to Load a record from database by primary key (Ham nay dung de load mot object tu database theo primary key)
    /// </summary>
    /// <param name="obj">This object is used to get data and transit the condition parammeter too. (Doi tuong nay vua duoc dung de lay du lieu ve va dong thoi cung de truyen tham so dieu kien) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    /// <returns>a first Object satisfy the condition. (Ket qua tra ve doi tuong dau tien thoa man dieu kien)</returns>
    public override ITransaction LoadObject(IObject obj, params string[] primaryKeyNames)
    {
      return new LoadObjectTransaction(obj, primaryKeyNames);
    }

    /// <summary>
    /// This method is used to Load a record from database by primary key (Ham nay dung de load mot object tu database theo primary key)
    /// </summary>
    /// <param name="obj">This object is used to get data and transit the condition parammeter too. (Doi tuong nay vua duoc dung de lay du lieu ve va dong thoi cung de truyen tham so dieu kien) </param>
    /// <returns>a first Object satisfy the condition. (Ket qua tra ve doi tuong dau tien thoa man dieu kien)</returns>
    public override ITransaction LoadObject(IObject obj)
    {
      return new LoadObjectByKeyTransaction(obj);
    }

    /// <summary>
    /// This method is used to Load all records from database or by some condition. (Ham nay dung de load tat ca record tu database hoac theo mot vai dieu kien nao do)
    /// </summary>
    /// <param name="obj">This object is used to transit the condition parammeter. (Doi tuong nay dung de truyen tham so dieu kien) </param>
    /// <param name="typeOutput">Type of result. If iTpyeOutput == 1 then result is a list object Else the result is a SqlDataAdapter (Bien de cho biet kieu tra ve. Neu iTpyeOutput == 1 thi keu qua tra ve la list object, nguoc lai thi tra ve mot SqlDataAdapter)</param>
    /// <param name="whereClause">The where clause</param>
    /// <param name="inputParameters">Array list paramater in where clause. (Danh sach paramater trong menh de where )</param>
    /// <returns>If iTpyeOutput == 1 then result is a list object Else the result is a SqlDataAdapter (Neu iTpyeOutput == 1 thi keu qua tra ve la list object, nguoc lai thi tra ve mot SqlDataAdapter)</returns>
    public override ITransaction ListObject(IObject obj, int typeOutput, string whereClause, params DBParameter[] inputParameters)
    {
      return new ListObjectTransaction(obj, typeOutput, whereClause, inputParameters);
    }

    /// <summary>
    /// This method is used to Delete a record in database by primary key (Ham nay dung de delete mot record cua mot Object trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object is needed to delete. (Doi tuong de delete) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    public override ITransaction DeleteObject(IObject obj, params string[] primaryKeyNames)
    {
      return new DeleteObjectTransaction(obj, primaryKeyNames);
    }

    /// <summary>
    /// This method is used to Delete a record in database by primary key (Ham nay dung de delete mot record cua mot Object trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object is needed to delete. (Doi tuong de delete) </param>
    public override ITransaction DeleteObject(IObject obj)
    {
      return new DeleteObjectByKeyTransaction(obj);
    }

    /// <summary>
    /// This method is used to Execute a CommandText. (Ham nay dung de thuc thi mot cau lenh sql)
    /// </summary>E
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>A first column of first record. (Ket qua tra ve Column dau tien cua record dau tien tim duoc))</returns>
    public override ITransaction ExecuteCommandText(string commandText, DBParameter[] inputParameters)
    {
      return new ExecuteCommandText(commandText, inputParameters);
    }

    /// <summary>
    /// This method is used to Execute a Store Procedure. (Ham nay dung de thuc thi mot Store Procedure)
    /// </summary>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    public override ITransaction ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters)
    {
      return new ExecuteStoreProcedure(storeProcedureName, inputParameters, outputParameters);
    }
    public override ITransaction ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters, int commandTimeout)
    {
      return new ExecuteStoreProcedure(storeProcedureName, inputParameters, outputParameters, commandTimeout);
    }
    public override ITransaction ExecuteStoreProcedure(IStoreObject obj)
    {
      return new Execute_StoreProcedure(obj);
    }
    public override ITransaction ExecuteStoreProcedure(IStoreObject obj, int commandTimeout)
    {
      return new Execute_StoreProcedure(obj, commandTimeout);
    }

    /// <summary>
    /// This method is used to find records using command text. (Ham nay dung de tim kiem du lieu theo command text)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT trong cau command text) </param>
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>List object satisfy the condition. (Ket qua tra ve danh sach doi tuong thoa man dieu kien)</returns>
    public override ITransaction SearchCommandText(IObject obj, string commandText, DBParameter[] inputParameters)
    {
      return new SearchObjectTransaction(obj, CommandType.Text, commandText, inputParameters, null);
    }

    /// <summary>
    /// This method is used to find records using command text. (Ham nay dung de tim kiem du lieu theo command text)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT trong cua command text) </param>
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>A SqlDataAdapter. (Ket qua tra ve doi tuong SqlDataAdapter)</returns>
    public override ITransaction SearchCommandText(string commandText, DBParameter[] inputParameters)
    {
      return new SearchDataAdapterTransaction(CommandType.Text, commandText, inputParameters, null);
    }

    /// <summary>
    /// This method is used to find records using store procedure. (Ham nay dung de tim kiem du lieu theo store procedure)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT cua store procedure) </param>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    /// <returns>The result list object affter executed the store procedure. (Ket qua tra ve danh sach doi tuong duoc tra ve khi thuc thi store procedure)</returns>
    public override ITransaction SearchStoreProcedure(IObject obj, string storeProcedureName, DBParameter[] inParameters, DBParameter[] outParameters)
    {
      return new SearchObjectTransaction(obj, CommandType.StoredProcedure, storeProcedureName, inParameters, outParameters);
    }
    public override ITransaction SearchStoreProcedure(IObject obj, string storeProcedureName, DBParameter[] inParameters, DBParameter[] outParameters, int commandTimeOut)
    {
      return new SearchObjectTransaction(obj, CommandType.StoredProcedure, storeProcedureName, inParameters, outParameters, commandTimeOut);
    }
    /// <summary>
    /// This method is used to find records using store procedure. (Ham nay dung de tim kiem du lieu theo store procedure)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT cua store procedure) </param>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    /// <returns>A SqlDataAdapter. (Ket qua tra ve doi tuong SqlDataAdapter)</returns>
    public override ITransaction SearchStoreProcedure(string storeProcedureName, DBParameter[] inParameters, DBParameter[] outParameters)
    {
      return new SearchDataAdapterTransaction(CommandType.StoredProcedure, storeProcedureName, inParameters, outParameters);
    }
    public override ITransaction SearchStoreProcedure(string storeProcedureName, DBParameter[] inParameters, DBParameter[] outParameters, int commandTimeOut)
    {
      return new SearchDataAdapterTransaction(CommandType.StoredProcedure, storeProcedureName, inParameters, outParameters, commandTimeOut);
    }
    public override ITransaction SearchStoreProcedure(IStoreObject obj)
    {
      return new SearchStoreProcedure(obj);
    }
    public override ITransaction SearchStoreProcedure(IStoreObject obj, int commandTimeOut)
    {
      return new SearchStoreProcedure(obj, commandTimeOut);
    }
    #endregion Methods
  }
}