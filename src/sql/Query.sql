SELECT
    b."BusType",
    b."CompanyName",
    ss."SeatNumber",
    ss."Price" AS "SeatPrice",
    bs."JourneyDate"
FROM
    "SeatStatuses" ss 
JOIN
    "BusSchedules" bs ON ss."BusScheduleId" = bs."BusScheduleId"  -- Replace with the EXACT name
JOIN
    "Buses" b ON bs."BusId" = b."BusId"  -- Replace with the EXACT name
ORDER BY
    b."BusType", bs."JourneyDate", ss."SeatNumber";



SELECT DISTINCT "BusType", "CompanyName"
FROM "Buses";



UPDATE "SeatStatuses"
SET "Price" = 700.00
FROM 
    "BusSchedules" bs, 
    "Buses" b
WHERE 
    "SeatStatuses"."BusScheduleId" = bs."BusScheduleId" 
    AND bs."BusId" = b."BusId"
    -- 🎯 CRITICAL FIX: Target the exact string 'Non AC'
    AND b."BusType" ILIKE 'Non AC';

SELECT * FROM "SeatStatuses"
WHERE "BusScheduleId" = '30000000-0000-0000-0000-000000000101';


UPDATE "SeatStatuses"
SET "Price" = 700.00
FROM 
    "BusSchedules" bs, 
    "Buses" b
WHERE 
    "SeatStatuses"."BusScheduleId" = bs."BusScheduleId" 
    AND bs."BusId" = b."BusId"
    AND b."BusType" ILIKE 'Non AC';


	SELECT
    b."BusType",
    b."CompanyName",
    ss."SeatNumber",
    ss."Price" AS "SeatPrice",
    bs."JourneyDate"
FROM
    "SeatStatuses" ss 
JOIN
    "BusSchedules" bs ON ss."BusScheduleId" = bs."BusScheduleId"
JOIN
    "Buses" b ON bs."BusId" = b."BusId"
WHERE
    b."BusType" ILIKE 'AC' -- 🎯 Filter specifically for Non AC buses
ORDER BY
    b."CompanyName", bs."JourneyDate", ss."SeatNumber";



UPDATE "SeatStatuses"
SET "Price" = 700.00
FROM 
    "BusSchedules" AS bs, 
    "Buses" AS b
WHERE 
    "SeatStatuses"."BusScheduleId" = bs."BusScheduleId"
    AND bs."BusId" = b."BusId"
    -- Target any BusType that contains 'Non AC' (case-insensitive)
    AND b."BusType" ILIKE '%Non AC%';


SELECT
    b."BusType",
    b."CompanyName",
    ss."Price" AS "SeatPrice",
    ss."BusScheduleId",
    ss."SeatNumber"
FROM
    "SeatStatuses" ss 
JOIN
    "BusSchedules" bs ON ss."BusScheduleId" = bs."BusScheduleId"
JOIN
    "Buses" b ON bs."BusId" = b."BusId"
WHERE
    b."BusType" ILIKE 'Non AC' -- Filters for Non AC buses
    AND ss."Price" = 1200.00;   -- Filters for the incorrect price



UPDATE "SeatStatuses"
SET "Price" = 700.00
FROM 
    "BusSchedules" AS bs, 
    "Buses" AS b
WHERE 
    "SeatStatuses"."BusScheduleId" = bs."BusScheduleId"
    AND bs."BusId" = b."BusId"
    AND b."BusType" ILIKE '%Non AC%'; -- Ensures all variations of 'Non AC' are caught

SELECT * FROM "BoardingPoints"
SELECT * FROM "BusSchedules"
SELECT * FROM "BusSeatLayouts"
SELECT * FROM "Buses"
SELECT * FROM "Routes"
SELECT * FROM "SeatStatuses"
SELECT * FROM "Tickets"
SELECT * FROM "__EFMigrationsHistory"

SELECT COUNT(*)
FROM "SeatStatuses" ss
JOIN "BusSchedules" bs ON ss."BusScheduleId" = bs."BusScheduleId"
JOIN "Buses" b ON bs."BusId" = b."BusId"
WHERE (b."BusType" ILIKE 'AC' OR b."BusType" ILIKE 'NON AC')
  AND ss."Price" = 1200.00;


SELECT COUNT(*)
FROM "SeatStatuses" ss
JOIN "BusSchedules" bs ON ss."BusScheduleId" = bs."BusScheduleId"
JOIN "Buses" b ON bs."BusId" = b."BusId"
WHERE b."BusType" ILIKE 'NON AC'
  AND ss."Price" = 1200.00;


SELECT
    "BusName",
    "BusType",
    b."BusSeatLayoutId",
    l."LayoutName"
FROM
    "Buses" b
JOIN
    "BusSeatLayouts" l ON b."BusSeatLayoutId" = l."BusSeatLayoutId"
ORDER BY
    "BusType" DESC;



SELECT
    bs."BusScheduleId",
    b."BusName",
    b."BusType",
    b."BusSeatLayoutId",
    l."LayoutName"
FROM
    "BusSchedules" bs
JOIN
    "Buses" b ON bs."BusId" = b."BusId"
JOIN
    "BusSeatLayouts" l ON b."BusSeatLayoutId" = l."BusSeatLayoutId"
WHERE
    bs."BusScheduleId" = '30000000-0000-0000-0000-000000000110'; -- Replace with your Schedule ID



select * FROM "SeatStatuses"
WHERE "BusScheduleId" = '30000000-0000-0000-0000-000000000128';



SELECT 
    b."BusName",
    bs."BusScheduleId",
    COUNT(ss."SeatNumber") AS "AvailableSeats"
FROM "Buses" b
JOIN "BusSchedules" bs ON b."BusId" = bs."BusId"
JOIN "SeatStatuses" ss ON ss."BusScheduleId" = bs."BusScheduleId"
WHERE ss."Status" = 1   -- 1 = Available
GROUP BY b."BusName", bs."BusScheduleId"
ORDER BY b."BusName", bs."BusScheduleId";


SELECT 
    b."BusName",
    b."BusType",
    l."TotalSeats" AS "TotalSeatsPerBus"
FROM "Buses" b
JOIN "BusSeatLayouts" l ON b."BusSeatLayoutId" = l."BusSeatLayoutId"
ORDER BY b."BusName";

