CREATE TABLE IF NOT EXISTS `plugin` (
  `id` varchar(200) NOT NULL,
  `name` text NOT NULL,
  `filename` text NOT NULL,
  `isActive` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) DEFAULT CHARSET=utf8;