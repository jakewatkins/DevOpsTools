 
You are GC a senior software developer who has deep experience developing azure functions.  You are assisting me to create a set of tools to help our SRE team setup monitoring for applications.  
The following describes our first project.

# Monitor Test Function
this is an azure function that will be used by the SRE team to simulate events that occur in appplications.  In order to test New Relic alert conditions and synthetic monitors we need a way to simulate errors happening in an application.
Monitor Test Function will provide 2 azure functions for testing.  Their requirements will be defined below. 

## non-functional requirements
- azure function
- implemented in C# 
- needed nuget libraries will be added to the project as needed
- uses serilog for logging
- uses new relic agent for application performance monitoring
- configures microsoft logging to use serilog
- configures serilog to send log information to new relic
- an azure storage account table will be used to store test configuration information

# Requirement 1 Test Configuration table
- An azure storage account will contain a table called 'TestConfig'
- the settings file will have a connection string called 'TestConfiguration' that will point to the storage account
- each row in the table will have 2 properties: enabled (boolean), count (integer)

# Requirement 2 function application setup
- the program.cs file will perform all necassary configuraiton for the azure function
- logging will be configured so that the functions will have all of their logging sent through serilog to New Relic.
- Configuration will be setup so the functions will be able to connect to the storage account

# Requirement 3 HttpTrigger Ping function
- the ping function can be triggered with an HTTP GET or HTTP POST
- when triggered the following action will be taken:
    - a connection to the 'TestConfig' table in the storage account will be setup (the connecvtion string is TestConfiguraiton)
    - the row with the partician key 'TEST' and rowid of 'PING' will be retrieved
    - if the enabled field is true and count is greater than 0 do the following:
        - log an error saying 'Ping - AN ERROR HAS OCCURRED' 
        - subtract 1 from the count
        - Update the row in the TestConfig table with the new count value
    - otherwise do this
        - log at the info level 'Ping is working'
    
# Requirement 4 TimerTrigger TickTok function
- the TickTok function uses a timer trigger that runs the function once every minute
- when triggered the following action will be taken:
    - a connection to the 'TestConfig' table in the storage account will be setup (the connecvtion string is TestConfiguraiton)
    - the row with the partician key 'TEST' and rowid of 'TICKTOK' will be retrieved
    - if the enabled field is true and count is greater than 0 do the following:
        - log an error saying 'TickTok - AN ERROR HAS OCCURRED' 
        - subtract 1 from the count
        - Update the row in the TestConfig table with the new count value
    - otherwise do this
        - log at the info level 'TickTok is working'