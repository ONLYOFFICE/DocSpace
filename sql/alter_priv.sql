/* UPDATE   SET email = 'paul.bannov@gmail.com';  */
/* CREATE USER IF  NOT EXISTS 'onlyoffice_user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'onlyoffice_pass';

    mysql -D "onlyoffice" -e 'CREATE USER IF NOT EXISTS "onlyoffice_user"@"localhost" IDENTIFIED WITH mysql_native_password BY "onlyoffice_pass";' && \
    mysql -D "onlyoffice" -e 'GRANT ALL PRIVILEGES ON *.* TO 'onlyoffice_user'@'localhost';' && \
    mysql -D "onlyoffice" -e 'UPDATE core_user SET email = "paul.bannov@gmail.com";' && \
    mysql -D "onlyoffice" -e 'UPDATE core_usersecurity SET pwdhash = "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", pwdhashsha512 = "USubvPlB+ogq0Q1trcSupg==";'
*/
 CREATE USER IF NOT EXISTS 'onlyoffice_user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'onlyoffice_pass';
 GRANT ALL PRIVILEGES ON *.* TO 'onlyoffice_user'@'localhost';
 UPDATE core_user SET email = 'paul.bannov@gmail.com';
 UPDATE core_usersecurity SET pwdhash = 'vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=', pwdhashsha512 = 'USubvPlB+ogq0Q1trcSupg==';