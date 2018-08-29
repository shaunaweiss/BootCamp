using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;


/// <summary>
///		Sample application to demonstrate a C# API
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class BootCampAPI : System.Web.Services.WebService {

    #region ######################################################################################################################################################## Notes
    /*	Dynamic object Example ...
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = new Dictionary<string, object>();
        row.Add("Make", "G35");
        row.Add("Model", "25 -18 Turbo");
        row.Add("Year", 2014);
        rows.Add(row);

        row = new Dictionary<string, object>();
        row.Add("Make", "Honda");
        row.Add("Model", "Accord");
        row.Add("Year", 2015);
        rows.Add(row);

        serialize(rows);
    */
    #endregion

    #region ######################################################################################################################################################## Wrapper Methods [DON'T MODIFY]
        Helper helper = new Helper();

		// Database
		private void addParam(string name, object value)						{ helper.addParam(name, value);							}
		private DataSet sqlExecDataSet(string sql)								{	return helper.sqlExecDataSet(sql);						}
		private DataTable sqlExecDataTable(string sql)							{	return helper.sqlExecDataTable(sql);					}
		private DataTable sqlExec(string sql)									{	return helper.sqlExec(sql);								}
		private DataTable sqlExec(string sql, DataTable dt, string udtblParam)	{	return helper.sqlExec(sql, dt, udtblParam);				}
		private DataTable sqlExecQuery(string sql)								{	return helper.sqlExecQuery(sql);						}

		// Serializer
		private void streamJson(string jsonString)								{	helper.streamJson(jsonString);							}
		private void serialize(Object obj)										{	helper.serialize(obj);									}
		private void serializeSingleDataTableRow(DataTable dt)					{	helper.serializeSingleDataTableRow(dt);					}
		private void serializeDataTable(DataTable dt)							{	helper.serializeDataTable(dt);							}
		private void serializeDataSet(DataSet ds)								{	helper.serializeDataSet(ds);							}
		private void serializeXML<T>(T value)									{	helper.serializeXML(value);								}
		private void serializeDictionary(Dictionary<object, object> dic)		{	helper.serializeDictionary(dic);						}
		private void serializeObject(Object obj)								{   helper.serializeObject(obj);							}

		// Going to leave this out so we don't need to import Newtonsoft.Json package
		//private T _download_serialized_json_data<T>(string url) where T : new()	{	return Helper._download_serialized_json_data<T>(url);	}

	#endregion


	//=== Web Service Methods Follow Below
    [WebMethod(Description = "Returns Classes On A Specific Day Of The Week.")]
    public void getClassByDay(string classDOW) {
        classDOW = classDOW.Trim();
		addParam("@ClassDOW", classDOW);
        serializeDataTable(sqlExec("spGetClassByDay"));
    }
    
    [WebMethod(Description = "Returns Classes With A specific Instructor.")]
    public void getClassByInstructor(string firstName, string lastName)
    {
        firstName = firstName.Trim();
        lastName = lastName.Trim();
        addParam("@FirstName", firstName);
        addParam("@LastName", lastName);
        serializeDataTable(sqlExec("spGetClassByInstructor"));
    }

    [WebMethod(Description = "Returns Classes With A Particular Level.")]
    public void getClassByLevel(int LevelID)
    {
        addParam("@LevelID", LevelID);
        serializeDataTable(sqlExec("spGetClassByLevel"));
    }

    [WebMethod(Description = "Returns Classes With A Particular Focus Area.")]
    public void getClassByFocus(int ClassFocusID)
    {
        addParam("@ClassFocusID", ClassFocusID);
        serializeDataTable(sqlExec("spGetClassByFocus"));
    }

    [WebMethod(Description = "Returns The Last Weigh In For A Customer.")]
    public void getLatestWeighIn(string EmailAddress)
    {
        EmailAddress = EmailAddress.Trim();
        addParam("@EmailAddress", EmailAddress);
        serializeDataTable(sqlExec("spGetLatestWeighIn"));
    }

    [WebMethod(Description = "Returns The Customers That Are In A Particular Class.")]
    public void getCustomersByClass(string ClassName)
    {
        ClassName = ClassName.Trim();
        addParam("@ClassName", ClassName);
        serializeDataTable(sqlExec("spGetCustomersByClass"));
    }

    [WebMethod(Description = "Adds a new customer metric")]
    public void addCustomerMetric(int CustomerID, float weightInLbs, float heightInInches, String weighInDate)
    {
        weighInDate = weighInDate.Trim();
        addParam("@CustomerID", CustomerID);
        addParam("@weightInLbs", weightInLbs);
        addParam("@heightInInches", heightInInches);
        addParam("@weighInDate", weighInDate);
        serializeDataTable(sqlExec("spAddCustomerMetric"));
    }


    [WebMethod(Description = "Add/ Updates OR Deletes a customer")]
    public void addUpdateDeleteCustomer(int CustomerID, string EmailAddress, string FirstName, string LastName, int hardDelete)
    {
        EmailAddress = EmailAddress.Trim();
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
        addParam("@CustomerID", CustomerID);
        addParam("@EmailAddress", EmailAddress);
        addParam("@FirstName", FirstName);
        addParam("@LastName", LastName);
        addParam("@hardDelete", hardDelete);
        serializeDataTable(sqlExec("spAddUpdateDelete_Customer"));
    }



    /*
	[WebMethod(Description = "TEST API CALL FOR JQUERY CONSUMPTION - max retun count = 30, filter by make, model or year.  Set count=0 and filter = \"\" for random cars<br /><br />possible makes: Honda, Toyota, BMW, Ford<br />possible models: Truck, Sport, Hatchback, Coupe, Sedan, SU, MiniVan<br />possible years: 2000-2018")]
	public void Cars(int count, string filter) {
		List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
		Dictionary<string, object> row;

		// Set up random cars and add them to the list to return to user. If a filter is added
		// just add cars that fit into the filter
		string[] makes = { "Honda", "Toyota", "BMW", "Ford" };
		string[] models = { "Truck", "Sport", "Hatchback", "Coupe", "Sedan", "SUV", "MiniVan" };
		Random rnd = new Random();
		int attempt = 0;
		int maxRows = 30;
		int num = (count <= 0 ? maxRows : Math.Min(count, maxRows));
		filter = filter.Trim();
		while (true) {
			row = new Dictionary<string, object>();
			row.Add("Make", makes[rnd.Next(makes.Length)]);
			row.Add("Model", models[rnd.Next(models.Length)]);
			row.Add("Year", rnd.Next(19) + 2000);

			// Note: The code line: String.Join("-",row) would produce a string that looks like:
			//
			//		[Make, Honda]-[Model, SUV]-[Year, 2010]

			if (filter.Length == 0 ||  String.Join("-",row).ToLower().Contains(" " + filter.ToLower() + "]")) { 
				rows.Add(row);
			}
			if (rows.Count == num || ++attempt > 1000) break;
		}

		serialize(rows);
	}
    */
}
