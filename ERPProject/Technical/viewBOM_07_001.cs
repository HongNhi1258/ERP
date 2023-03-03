/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewBOM_07_001.cs
*/
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_07_001 : MainUserControl
  {
    #region field
    public string currItemCode = string.Empty;
    public string currComp = string.Empty;
    public int currRevision = int.MinValue;
    public int type = int.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadData();
      //this.LoadRD();
      //this.LoadCD();
      //this.LoadProduction();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void LoadData()
    {
      if (this.type == 1)
      {
        string commandTextChair = string.Format(@"SELECT DISTINCT A.DCRCode, I.Name ComponentName, CASE WHEN A.[Status] = 0 THEN 'Lost'
                                                   WHEN A.[Status] = 1 THEN 'Valid' 
                                                   WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, CurrentLocation
                                                FROM TblQADMasterInfo A
                                                INNER JOIN TblBOMItemComponent B ON A.DCRCode = B.ComponentCode
                                                INNER JOIN VBOMComponent C ON (B.ComponentCode = C.Code
			                                                AND ISNULL(B.CompRevision, 0) = ISNULL(C.Revision, 0)) 
                                                LEFT JOIN 
                                                (
	                                                SELECT Code, Name FROM VBOMComponent
                                                  WHERE Code LIKE 'DCR%' OR Code LIKE 'DCW%'
                                                ) I ON A.DCRCode = I.Code 
                                                WHERE Kind = 3 AND B.ItemCode = '{0}' AND B.Revision = {1} AND B.ComponentCode = '{2}'", currItemCode, currRevision, currComp);
        DataTable dtChair = DataBaseAccess.SearchCommandTextDataTable(commandTextChair);
        ultChair.DataSource = dtChair;
      }
      else if (this.type == 2)
      {
        string commandTextProduction = string.Format(@"		SELECT DISTINCT A.FinishingCode, D.Name FinishingName, 
	                                                        CASE WHEN A.[Status] = 0 THEN 'Lost'
								                                                         WHEN A.[Status] = 1 THEN 'Valid' 
								                                                         WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN, A.CategoryPanel
	                                                        FROM TblQADMasterInfo A
		                                                        INNER JOIN TblBOMItemComponent B ON A.FinishingCode = B.ComponentCode
		                                                        INNER JOIN VBOMComponent C ON (B.ComponentCode = C.Code
									                                                      AND ISNULL(B.CompRevision, 0) = ISNULL(C.Revision, 0)) 
		                                                        LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
	                                                        WHERE Kind = 6 AND B.ItemCode = '{0}' AND B.Revision = {1}", currItemCode, currRevision);
        DataTable dtProduction = DataBaseAccess.SearchCommandTextDataTable(commandTextProduction);
        ultChair.DataSource = dtProduction;
      }
    }

    //    private void LoadRD()
    //    {
    //      string commandTextRD = string.Format(@"	SELECT DISTINCT A.FinishingCode, D.Name FinishingName, 
    //	                                            CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                             WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                             WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN
    //	                                            FROM TblQADMasterInfo A
    //		                                            INNER JOIN TblBOMItemInfo B ON A.FinishingCode = B.MainFinish
    //		                                            INNER JOIN TblBOMItemBasic C ON C.ItemCode = B.ItemCode AND C.RevisionActive = B.Revision
    //		                                            LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                            WHERE Kind = 4 AND C.ItemCode = '{0}' AND C.RevisionActive = {1}
    //
    //	                                            UNION
    //
    //	                                            SELECT DISTINCT A.FinishingCode, D.Name FinishingName, 
    //	                                            CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                             WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                             WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN
    //	                                            FROM TblQADMasterInfo A
    //		                                            INNER JOIN TblBOMOtherFinishing B ON A.FinishingCode = B.OtherFinishingCode
    //		                                            LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                            WHERE Kind = 4 AND B.ItemCode = '{0}' AND B.Revision = {1}", currItemCode, currRevision);
    //      DataTable dtRD = DataBaseAccess.SearchCommandTextDataTable(commandTextRD);
    //      ultRD.DataSource = dtRD;
    //    }

    //    private void LoadCD()
    //    {
    //      string commandTextCD = string.Format(@"	SELECT DISTINCT A.CDCode, A.FinishingCode, D.Name FinishingName, 
    //	                                            CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                             WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                             WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN
    //	                                            FROM TblQADMasterInfo A
    //		                                            INNER JOIN TblBOMItemInfo B ON A.FinishingCode = B.MainFinish
    //		                                            INNER JOIN TblBOMItemBasic C ON C.ItemCode = B.ItemCode AND C.RevisionActive = B.Revision
    //		                                            LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                            WHERE Kind = 5 AND C.ItemCode = '{0}' AND C.RevisionActive = {1}
    //
    //	                                            UNION
    //
    //	                                            SELECT DISTINCT A.CDCode, A.FinishingCode, D.Name FinishingName, 
    //	                                            CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                             WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                             WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN
    //	                                            FROM TblQADMasterInfo A
    //		                                            INNER JOIN TblBOMOtherFinishing B ON A.FinishingCode = B.OtherFinishingCode
    //		                                            LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                            WHERE Kind = 5 AND B.ItemCode = '{0}' AND B.Revision = {1}", currItemCode, currRevision);
    //      DataTable dtCD = DataBaseAccess.SearchCommandTextDataTable(commandTextCD);
    //      ultCD.DataSource = dtCD;
    //    }

    //    private void LoadProduction()
    //    {
    //      string commandTextProduction = string.Format(@"	SELECT DISTINCT A.FinishingCode, D.Name FinishingName, 
    //	                                              CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                               WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                               WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN, A.CategoryPanel
    //	                                              FROM TblQADMasterInfo A
    //		                                              INNER JOIN TblBOMItemInfo B ON A.FinishingCode = B.MainFinish
    //		                                              INNER JOIN TblBOMItemBasic C ON C.ItemCode = B.ItemCode AND C.RevisionActive = B.Revision
    //		                                              LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                              WHERE Kind = 6 AND C.ItemCode = '{0}' AND C.RevisionActive = {1}
    //
    //	                                              UNION
    //
    //	                                              SELECT DISTINCT A.FinishingCode, D.Name FinishingName, 
    //	                                              CASE WHEN A.[Status] = 0 THEN 'Lost'
    //								                                               WHEN A.[Status] = 1 THEN 'Valid' 
    //								                                               WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, A.[Type], A.DescriptionEN, A.CategoryPanel
    //	                                              FROM TblQADMasterInfo A
    //		                                              INNER JOIN TblBOMOtherFinishing B ON A.FinishingCode = B.OtherFinishingCode
    //		                                              LEFT JOIN TblBOMFinishingInfo D ON A.FinishingCode = D.FinCode
    //	                                              WHERE Kind = 6 AND B.ItemCode = '{0}' AND B.Revision = {1}", currItemCode, currRevision);
    //      DataTable dtProduction = DataBaseAccess.SearchCommandTextDataTable(commandTextProduction);
    //      ultProduction.DataSource = dtProduction;
    //    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

    }

    #endregion function

    #region event
    public viewBOM_07_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_07_001_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultChair_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    private void ultRD_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    private void ultCD_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    private void ultProduction_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    #endregion event
  }
}
