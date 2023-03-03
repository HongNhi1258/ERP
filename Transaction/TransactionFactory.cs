/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 18-06-2008
   Company :  Dai Co   
 */
using DaiCo.Application;

namespace DaiCo.Transaction
{
  /// <summary>
  /// Summary description for TransactionFactory.
  /// </summary>
  public abstract class TransactionFactory
  {

    #region Fields
    private static TransactionFactory transactionFactory;
    #endregion Fields

    #region Properties
    public static TransactionFactory Factory
    {
      get { return transactionFactory; }
    }
    #endregion Properties

    #region Basic Methods
    public static void SetFactory(TransactionFactory factory)
    {
      transactionFactory = factory;
    }
    // <summary>
    /// This method is used to Insert a records into database. (Ham nay dung de insert mot record bat ky vao database)
    /// </summary>
    /// <param name="obj">This object need to insert into database. (Doi tuong nay dung de insert vao database)</param>
    /// <returns>a Object. (Ket qua tra ve doi tuong kieu tuong ung voi record vua insert vao database)</returns>
    public virtual ITransaction InsertObject(IObject obj) { return null; }

    /// <summary>
    /// This method is used to Update a record in database by primary key. (Ham nay dung de update mot record trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object need to update in database. (Doi tuong can cap nhat) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    /// <returns>If update successfully Then return true Else return false. (Neu update thanh cong thi tra ve TRUE, nguoc lai tra ve FALSE</returns>
    public virtual ITransaction UpdateObject(IObject obj, params string[] primaryKeyNames) { return null; }

    /// <summary>
    /// This method is used to Update a record in database by primary key. (Ham nay dung de update mot record trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object need to update in database. (Doi tuong can cap nhat) </param>
    /// <returns>If update successfully Then return true Else return false. (Neu update thanh cong thi tra ve TRUE, nguoc lai tra ve FALSE</returns>
    public virtual ITransaction UpdateObject(IObject obj) { return null; }

    /// <summary>
    /// This method is used to Load a record from database by primary key (Ham nay dung de load mot object tu database theo primary key)
    /// </summary>
    /// <param name="obj">This object is used to get data and transit the condition parammeter too. (Doi tuong nay vua duoc dung de lay du lieu ve va dong thoi cung de truyen tham so dieu kien) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    /// <returns>a first Object satisfy the condition. (Ket qua tra ve doi tuong dau tien thoa man dieu kien)</returns>
    public virtual ITransaction LoadObject(IObject obj, params string[] primaryKeyNames) { return null; }

    /// <summary>
    /// This method is used to Load a record from database by primary key (Ham nay dung de load mot object tu database theo primary key)
    /// </summary>
    /// <param name="obj">This object is used to get data and transit the condition parammeter too. (Doi tuong nay vua duoc dung de lay du lieu ve va dong thoi cung de truyen tham so dieu kien) </param>
    /// <returns>a first Object satisfy the condition. (Ket qua tra ve doi tuong dau tien thoa man dieu kien)</returns>
    public virtual ITransaction LoadObject(IObject obj) { return null; }

    /// <summary>
    /// This method is used to Load all records from database or by some condition. (Ham nay dung de load tat ca record tu database hoac theo mot vai dieu kien nao do)
    /// </summary>
    /// <param name="obj">This object is used to transit the condition parammeter. (Doi tuong nay dung de truyen tham so dieu kien) </param>
    /// <param name="typeOutput">Type of result. If iTpyeOutput == 1 then result is a list object Else the result is a SqlDataAdapter (Bien de cho biet kieu tra ve. Neu iTpyeOutput == 1 thi keu qua tra ve la list object, nguoc lai thi tra ve mot SqlDataAdapter)</param>
    /// <param name="whereClause">The where clause</param>
    /// <param name="inputParameters">Array list paramater in where clause. (Danh sach paramater trong menh de where )</param>
    /// <returns>If iTpyeOutput == 1 then result is a list object Else the result is a SqlDataAdapter (Neu iTpyeOutput == 1 thi keu qua tra ve la list object, nguoc lai thi tra ve mot SqlDataAdapter)</returns>
    public virtual ITransaction ListObject(IObject obj, int typeOutput, string whereClause, params DBParameter[] inputParameters) { return null; }

    /// <summary>
    /// This method is used to Delete a record in database by primary key (Ham nay dung de delete mot record cua mot Object trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object is needed to delete. (Doi tuong de delete) </param>
    /// <param name="primaryKeyNames">Array list column's name of object's primarykey (condition of where clause).  (Danh sach ten cua cac column - Duoc su dung de dung trong menh de where )</param>
    public virtual ITransaction DeleteObject(IObject obj, params string[] primaryKeyNames) { return null; }

    /// <summary>
    /// This method is used to Delete a record in database by primary key (Ham nay dung de delete mot record cua mot Object trong database theo primary key)
    /// </summary>
    /// <param name="obj">This object is needed to delete. (Doi tuong de delete) </param>
    public virtual ITransaction DeleteObject(IObject obj) { return null; }

    /// <summary>
    /// This method is used to Execute a CommandText. (Ham nay dung de thuc thi mot cau lenh sql)
    /// </summary>E
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>A first column of first record. (Ket qua tra ve Column dau tien cua record dau tien tim duoc))</returns>
    public virtual ITransaction ExecuteCommandText(string commandText, params DBParameter[] inputParameters) { return null; }

    /// <summary>
    /// This method is used to Execute a Store Procedure. (Ham nay dung de thuc thi mot Store Procedure)
    /// </summary>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    /// <returns>A first column of first record. (Ket qua tra ve Column dau tien cua record dau tien tim duoc))</returns>
    public virtual ITransaction ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters) { return null; }
    public virtual ITransaction ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters, int timeout) { return null; }
    public virtual ITransaction ExecuteStoreProcedure(IStoreObject obj) { return null; }
    public virtual ITransaction ExecuteStoreProcedure(IStoreObject obj, int timeout) { return null; }

    /// <summary>
    /// This method is used to find records using command text. (Ham nay dung de tim kiem du lieu theo command text)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT trong cua command text) </param>
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>List object satisfy the condition. (Ket qua tra ve danh sach doi tuong thoa man dieu kien)</returns>
    public virtual ITransaction SearchCommandText(IObject obj, string commandText, params DBParameter[] inputParameters) { return null; }

    /// <summary>
    /// This method is used to find records using command text. (Ham nay dung de tim kiem du lieu theo command text)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT trong cua command text) </param>
    /// <param name="commandText">The sql need to executed</param>
    /// <param name="inputParameters">Array list paramater in the sql. (Danh sach paamater trong cau lenh sql )</param>
    /// <returns>A SqlDataAdapter. (Ket qua tra ve doi tuong SqlDataAdapter)</returns>
    public virtual ITransaction SearchCommandText(string commandText, params DBParameter[] inputParameters) { return null; }

    /// <summary>
    /// This method is used to find records using store procedure. (Ham nay dung de tim kiem du lieu theo store procedure)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT cua store procedure) </param>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    /// <returns>The result list object affter executed the store procedure. (Ket qua tra ve danh sach doi tuong duoc tra ve khi thuc thi store procedure)</returns>
    public virtual ITransaction SearchStoreProcedure(IObject obj, string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters) { return null; }
    public virtual ITransaction SearchStoreProcedure(IObject obj, string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters, int commandTimeOut) { return null; }

    /// <summary>
    /// This method is used to find records using store procedure. (Ham nay dung de tim kiem du lieu theo store procedure)
    /// </summary>
    /// <param name="obj">This object is used to confirm the return values. It's fields must match the fields in the select clauses in the store procedure (Doi tuong nay dung de xac dinh ket qua tra ve. Tat ca ten field cua doi tuong phai giong voi ten field trong menh de SELECT cua store procedure) </param>
    /// <param name="storeProcedureName">The Store Procedure's name. (Ten Store Procedure )</param>
    /// <param name="inputParameters">Array list INPUT PARAMATER in the Store Procedure. (Danh sach các INPUT PARAMATER trong Store Procedure )</param>
    /// <param name="outputParameters">Array list OUTPUT PARAMATER in the Store Procedure. (Danh sach các OUTPUT PARAMATER trong Store Procedure )</param>
    /// <returns>A SqlDataAdapter. (Ket qua tra ve doi tuong SqlDataAdapter)</returns>
    public virtual ITransaction SearchStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters) { return null; }
    public virtual ITransaction SearchStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters, int commandTimeOut) { return null; }
    public virtual ITransaction SearchStoreProcedure(IStoreObject obj) { return null; }
    public virtual ITransaction SearchStoreProcedure(IStoreObject obj, int commandTimeOut) { return null; }

    /// <summary>
    /// This method is like to ExecuteScalar method. (Ham nay dung giong nhu ExecuteScalar method, lay gia tri cua column dau tien cua dong dau tien tim duoc)
    /// </summary>
    /// <param name="commandText">The query</param>
    /// <param name="inputParameters">List of Parameter need to transit to store procedure.(Danh sach cac parameter can truyen vao store procedure.)</param>
    public virtual ITransaction ExecuteScalar(string commandText, params DBParameter[] inputParameters) { return null; }
    #endregion Basic Methods

    #region Extra Methods
    #endregion Extra Methods
  }
  #region Interface
  #endregion Interface
}
