select 
     DateTimeType = CAST('2017-06-01 13:55:05.223' as datetime) 
	,DateType = cast('2017-06-15' as date)
    ,CharacterType = 'Testing CharacterType'
    ,NCharacterType = N'Testing CharacterType'	
    ,IntType = 5
    ,DecimalType = 5.5
    ,BitType = CAST(1 as bit)
    ,TinyIntType = CAST(2 as tinyint)
    ,BigIntType = CAST(12345 as bigint)
    ,SmallIntType = CAST(12 as smallint)
union 
select 
     DateTimeType = CAST('2017-06-01 13:55:05.223' as datetime) 
	,DateType = cast('2017-06-15' as date)
    ,CharacterType = 'Testing CharacterType 2'
    ,NCharacterType = N'Testing CharacterType 2'	
    ,IntType = 10
    ,DecimalType = 10.10
    ,BitType = CAST(2 as bit)
    ,TinyIntType = CAST(4 as tinyint)
    ,BigIntType = CAST(12345678 as bigint)
    ,SmallIntType = CAST(1234 as smallint)
