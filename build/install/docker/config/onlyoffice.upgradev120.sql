DELIMITER DLM00

DROP PROCEDURE IF EXISTS upgrade120 DLM00

CREATE PROCEDURE upgrade120()
BEGIN

    IF EXISTS(SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tenants_quota' AND COLUMN_NAME = 'price2') THEN
        ALTER TABLE `tenants_quota` DROP COLUMN `price2`;
    END IF;

    IF EXISTS(SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'core_usersecurity' AND COLUMN_NAME = 'pwdhashsha512') THEN
        ALTER TABLE `core_usersecurity` DROP COLUMN `pwdhashsha512`;
    END IF;

    CREATE TABLE IF NOT EXISTS `core_userdav` (
    `tenant_id` int(11) NOT NULL,
    `user_id` varchar(255) NOT NULL,
    PRIMARY KEY (`user_id`),
    KEY `tenant_id` (`tenant_id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8;

    IF NOT EXISTS(SELECT * FROM information_schema.`STATISTICS` WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'tenants_tenants' AND `INDEX_NAME` = 'status') THEN
        ALTER TABLE `tenants_tenants` ADD INDEX `status` (`status`);
    END IF;

    IF NOT EXISTS(SELECT * FROM information_schema.`STATISTICS` WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'files_thirdparty_account' AND `INDEX_NAME` = 'tenant_id') THEN
        ALTER TABLE `files_thirdparty_account` ADD INDEX `tenant_id` (`tenant_id`);
    END IF;

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.docx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.odt');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.pdf');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.rtf');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.txt');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.doc', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docm', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docx', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dot', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotm', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotx', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fb2', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodt', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.html', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.mht', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odt', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ott', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.rtf', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.txt', '.epub');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.epub');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.doc', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docm', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docx', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dot', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotm', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotx', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.epub', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodt', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.html', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.mht', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odt', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ott', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.rtf', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.txt', '.fb2');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.fb2');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.doc', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docm', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docx', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dot', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotm', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotx', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.epub', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fb2', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodt', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odt', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ott', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.rtf', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.txt', '.html');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.html');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.doc', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docm', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docx', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dot', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotm', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.epub', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fb2', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodt', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.html', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.mht', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odt', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ott', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.rtf', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.txt', '.dotx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.dotx');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.doc', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docm', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.docx', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dot', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotm', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.dotx', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.epub', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fb2', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodt', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.html', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.mht', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odt', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.rtf', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.txt', '.ott');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xml', '.ott');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodp', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odp', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.otp', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pot', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.potm', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pps', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppsm', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppsx', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppt', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pptm', '.potx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pptx', '.potx');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fodp', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.odp', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pot', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.potm', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.potx', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pps', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppsm', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppsx', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ppt', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pptm', '.otp');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.pptx', '.otp');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.csv', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fods', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ods', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ots', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xls', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlsm', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlsx', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlt', '.xltx');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xltm', '.xltx');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.csv', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.fods', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.ods', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xls', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlsm', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlsx', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xlt', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xltm', '.ots');
    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.xltx', '.ots');

    INSERT IGNORE INTO `files_converts` (`input`, `output`) VALUES ('.oxps', '.pdf');

    CREATE TABLE IF NOT EXISTS `files_properties` (
        `tenant_id` int(10) NOT NULL,
        `entry_id` varchar(32) NOT NULL,
        `data` MEDIUMTEXT NOT NULL,
        PRIMARY KEY (`tenant_id`, `entry_id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8;

    INSERT IGNORE INTO `crm_currency_info` (`resource_key`, `abbreviation`, `symbol`, `culture_name`, `is_convertable`, `is_basic`) values ('Currency_EthiopianBirr', 'ETB', 'Br', 'ET', 0, 0);

    IF NOT EXISTS(SELECT * FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'login_events' AND COLUMN_NAME = 'active') THEN
        ALTER TABLE `login_events` ADD COLUMN `active` INT(10) NOT NULL DEFAULT '0' AFTER `description`;
    END IF;

END DLM00

CALL upgrade120() DLM00

DELIMITER ;
