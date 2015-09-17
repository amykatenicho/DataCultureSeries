package com.microsoft.ted.storm;

import com.google.gson.Gson;
import com.microsoft.ted.storm.Messages.HumidityMessage;
import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;

/**
 * Unit test for simple App.
 */
public class AppTest 
    extends TestCase
{
    /**
     * Create the test case
     *
     * @param testName name of the test case
     */
    public AppTest( String testName )
    {
        super( testName );
    }

    /**
     * @return the suite of tests being tested
     */
    public static Test suite()
    {
        return new TestSuite( AppTest.class );
    }

    /**
     * Rigourous Test :-)
     */
    public void testApp()
    {
        HumidityMessage msg = new Gson().fromJson("{\"Humidity\":49.0,\"Deviceid\":\"humidity0\",\"Timestamp\":\"2015-01-27T10:36:19.0806594Z\",\"RoomNumber\":0}", HumidityMessage.class);
        System.out.println("test");
    }
}
