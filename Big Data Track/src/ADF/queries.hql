DROP TABLE IF EXISTS DeviceReadings;
DROP TABLE IF EXISTS AverageReadingByMinute;

CREATE EXTERNAL TABLE DeviceReadings (
		type string, sensorDateTime string, deviceId string, roomNumber int, reading float
	)
	ROW FORMAT DELIMITED FIELDS TERMINATED BY '\054'
        STORED AS TEXTFILE
        LOCATION 'wasb:///input';

CREATE TABLE AverageReadingByMinute (type string, sensorDateTime string, roomNumber int, reading float)
    row format delimited 
    fields terminated by '\t' 
    lines terminated by '\n' 
    stored as textfile location 'wasb:///output/myaverageByMinute';

INSERT INTO TABLE AverageReadingByMinute SELECT TYPE,  concat(substr(sensorDateTime, 1, 16), ":00.0000000Z"), roomNumber, avg(reading) 
    FROM DeviceReadings 
    WHERE roomNumber IS NOT NULL 
    GROUP BY TYPE, concat(substr(sensorDateTime, 1, 16), ":00.0000000Z"), roomNumber;
	