using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Connection Test: "+ ConnectionTest());
    }

    public bool ConnectionTest()
    {
        string conStr = string.Format("Server={0};Database={1};Uid={2};Pwd={3};SslMode=None;",
            "localhost", "atm", "root", "pcj52300!");

        try
        {
            using (MySqlConnection con = new MySqlConnection(conStr))
            {
                con.Open();
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("e: " + e.ToString());
            return false;
        }
    }
}