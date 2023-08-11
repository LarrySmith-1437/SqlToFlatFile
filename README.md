# SqlToFlatFile
Allows the output of any arbitrary sql statement to a flat file (or flat files if there are multiple result sets). Queries can be run via Sql Server, ODBC, or OLEDB.

The project includes a dotNet Standard 2.0 library, and a C# console app, so you can call it programmatically or easily from a script.

Command Line Example: 

running the following command...

```
sqltoflatfile --ConnectionString "Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=True;" --OutputFilePath "output.csv" --QueryFile "ReturnAllCommonDataTypesQuery.sql" --TextEnclosure """" --Header
```

... executes a trusted connection to a local sql server instance, outputs the dataset to output.csv, running the query in file ReturnAllCommonDataTypesQuery.sql.
The default delimter is a comma, but it can be any desiered character or string.
Further it will enclose any string fields in a single double-quote character(").  Note that here, there are 4 " characters, since the inner 2 form an escape sequence. 
Finally it outputs the column names as a header line on the first line of the file.

The output from the exeuction looks like this: 

![image](https://github.com/LarrySmith-1437/SqlToFlatFile/assets/22043765/b60cbab8-8828-4762-b3fd-35222cadbc88)


The output file looks like this: 

![image](https://github.com/LarrySmith-1437/SqlToFlatFile/assets/22043765/cef96fb9-4a00-42cc-ab2d-56b95754b681)
