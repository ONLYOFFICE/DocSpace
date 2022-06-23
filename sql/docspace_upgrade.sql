ALTER TABLE files_thirdparty_account ADD folder_id TEXT NULL COLLATE UTF8_GENERAL_CI;
ALTER TABLE files_thirdparty_account ADD root_folder_type INT NOT NULL;