﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database
{
    public static class Template
    {
        public static string SyntaxChecker = @"
{query}
Double check the {dialect} query above for common mistakes, including:
- Using NOT IN with NULL values
- Using UNION when UNION ALL should have been used
- Using BETWEEN for exclusive ranges
- Data type mismatch in predicates
- Properly quoting identifiers
- Using the correct number of arguments for functions
- Casting to the correct data type
- Using the proper columns for joins
If there are any of the above mistakes, rewrite the query. If there are no mistakes, just reproduce the original query.
";

        public static string SqlPrefix = @"
You are an agent designed to interact with a SQL database.
Given an input question, create a syntactically correct T-SQL query to run, then look at the results of the query and return the answer.
Always include the TOP({top}) sql statement in the query.
Like in this example SELECT TOP({top}) column1, column2, columnN from Table
You can order the results by a relevant column to return the most interesting examples in the database.
Never query for all the columns from a specific table, only ask for the relevant columns given the question.
You MUST double check your query before executing it. If you get an error while executing a query, rewrite the query and try again.
DO NOT make any DML statements (INSERT, UPDATE, DELETE, DROP etc.) to the database.
If the question does not seem related to the database, just return ""I don't know"" as the answer.
Only use the tables listed below.

{schema}

For questions that include information about about a country for example USA use United States instead, or US use United States or FR use France

Question: {input}

Just provide the SQL statement and nothing more

";

        public static string SqlSufix = @"
You have created the following SQL statement to answer the question
{sql}
This is the result of the query
{result}
if the result of the query is NULL then respond ""I can't answer that question, I may not have the data"" other wise
please provide answer to the following question in no more than 100 characters and make sure to format the numbers
with thousand separator and two decimal points: {input}
";

        public static string SqlChart = @"
You are an agent designed to interact with a SQL database.
You have use the following schema to answer the question
{schema}
You have created the following SQL statement to answer the question
{sql}
This is the result of the query
{result}
here is the question you have answer: {input}
create the JSON configuration for a Chart.js javascript component to visualize the data
Check the syntax of the JSON and make sure it is correct
";

    }
}
