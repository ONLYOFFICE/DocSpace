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
	
	IF NOT EXISTS(SELECT * FROM information_schema.`TABLES` WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'hosting_instance_registration') THEN
        CREATE TABLE hosting_instance_registration (
		  instance_registration_id VARCHAR(255) NOT NULL COLLATE 'utf8_general_ci',
		  last_updated DATETIME NULL DEFAULT NULL,
		  worker_type_name VARCHAR(255) NOT NULL COLLATE 'utf8_general_ci',
		  is_active TINYINT(3) NOT NULL,
		  PRIMARY KEY (instance_registration_id) USING BTREE,
		  INDEX worker_type_name (worker_type_name) USING BTREE
		)
		COLLATE='utf8mb4_0900_ai_ci'
		ENGINE=InnoDB;
    END IF;
	
END DLM00
		
CALL docspace_upgrade1() DLM00

DELIMITER ;