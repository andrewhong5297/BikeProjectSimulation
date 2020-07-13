using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class DbTestBehaviourScript : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		/*
		ProbabilityDatabase mLocationDb = new ProbabilityDatabase();

		//Add Data
		mLocationDb.addData(new ProbabilityDbEntry("0", "AR", "0.001", "0.007"));
		mLocationDb.addData(new ProbabilityDbEntry("1", "AR", "0.002", "0.006"));
		mLocationDb.addData(new ProbabilityDbEntry("2", "AR", "0.003", "0.005"));
		mLocationDb.addData(new ProbabilityDbEntry("3", "AR", "0.004", "0.004"));
		mLocationDb.addData(new ProbabilityDbEntry("4", "AR", "0.005", "0.003"));
		mLocationDb.addData(new ProbabilityDbEntry("5", "AR", "0.006", "0.002"));
		mLocationDb.addData(new ProbabilityDbEntry("6", "AR", "0.007", "0.001"));
		mLocationDb.close();
		*/

		//Fetch All Data
		ProbabilityDatabase mLocationDb2 = new ProbabilityDatabase();
		System.Data.IDataReader reader = mLocationDb2.getAllData();

		int fieldCount = reader.FieldCount;
		List<ProbabilityDbEntry> myList = new List<ProbabilityDbEntry>();
		while (reader.Read())
		{
			ProbabilityDbEntry entity = new ProbabilityDbEntry(reader[0].ToString(),
									reader[1].ToString(),
									reader[2].ToString(),
									reader[3].ToString(),
									reader[4].ToString());

			Debug.Log("id: " + entity._id);
			myList.Add(entity);
		}

	}

	// Update is called once per frame
	void Update()
	{

	}
}