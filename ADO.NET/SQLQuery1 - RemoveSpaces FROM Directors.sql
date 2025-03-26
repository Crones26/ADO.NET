UPDATE Directors SET last_name = TRIM(last_name);
UPDATE Directors SET first_name = TRIM(first_name);
UPDATE Movies SET title = LTRIM(RTRIM(title));