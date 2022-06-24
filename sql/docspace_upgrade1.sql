DELIMITER DLM00

CREATE PROCEDURE docspace_upgrade1()
BEGIN
	IF EXISTS (SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'files_thirdparty_account' AND COLUMN_NAME = 'root_folder_type') THEN
		ALTER TABLE files_thirdparty_account CHANGE root_folder_type room_type INT NOT NULL;
	END IF;
	
	IF NOT EXISTS (SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'files_thirdparty_account' AND COLUMN_NAME = 'room_type') THEN
		ALTER TABLE files_thirdparty_account ADD room_type INT NOT NULL;
	END IF;
	
	IF NOT EXISTS (SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'files_thirdparty_account' AND COLUMN_NAME = 'folder_Id') THEN
		ALTER TABLE files_thirdparty_account ADD folder_id TEXT NULL COLLATE UTF8_GENERAL_CI;
	END IF;
	
END DLM00
		
CALL docspace_upgrade1() DLM00

DELIMITER ;onlyoffice_app