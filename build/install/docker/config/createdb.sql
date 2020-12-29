CREATE DATABASE IF NOT EXISTS `DB_NAME` CHARACTER SET utf8 COLLATE 'utf8_general_ci';
use `DB_NAME`;
set @@global.max_allowed_packet = 104857600;
set @@global.group_concat_max_len = 20971520;
