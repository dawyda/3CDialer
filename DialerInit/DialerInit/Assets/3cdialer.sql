-- MySQL dump 10.13  Distrib 5.6.14, for Win64 (x86_64)
--
-- Host: localhost    Database: 3cdialer
-- ------------------------------------------------------
-- Server version	5.6.14-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `call_2_userid`
--

DROP TABLE IF EXISTS `call_2_userid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `call_2_userid` (
  `userid` int(5) NOT NULL,
  `callid` int(8) NOT NULL,
  `called` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `call_2_userid`
--

LOCK TABLES `call_2_userid` WRITE;
/*!40000 ALTER TABLE `call_2_userid` DISABLE KEYS */;
INSERT INTO `call_2_userid` VALUES (14,76,1),(14,77,1),(14,78,1),(16,61,1),(16,62,1),(16,63,1),(16,79,1),(16,80,1),(16,81,1),(20,82,0),(20,83,0),(20,84,0);
/*!40000 ALTER TABLE `call_2_userid` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `call_list_data`
--

DROP TABLE IF EXISTS `call_list_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `call_list_data` (
  `id` mediumint(8) unsigned NOT NULL AUTO_INCREMENT,
  `calllistID` smallint(5) NOT NULL,
  `fname` varchar(50) NOT NULL,
  `lname` varchar(50) DEFAULT NULL,
  `tel1` varchar(30) NOT NULL,
  `tel2` varchar(30) DEFAULT NULL,
  `status` varchar(50) NOT NULL,
  `language` varchar(100) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `custom1` varchar(100) DEFAULT NULL,
  `custom2` varchar(100) DEFAULT NULL,
  `custom3` varchar(100) DEFAULT NULL,
  `custom4` varchar(100) DEFAULT NULL,
  `custom5` varchar(100) DEFAULT NULL,
  `custom6` varchar(100) DEFAULT NULL,
  `custom7` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index` (`status`)
) ENGINE=InnoDB AUTO_INCREMENT=85 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `call_list_data`
--

LOCK TABLES `call_list_data` WRITE;
/*!40000 ALTER TABLE `call_list_data` DISABLE KEYS */;
INSERT INTO `call_list_data` VALUES (49,7,'esther','','0712594456','','assigned','','home','','','','','','',''),(50,7,'boniface','','0711071132','','assigned','','work','','','','','','',''),(51,7,'gerald','','0707988885','','assigned','','home','','','','','','',''),(52,7,'esther','','0712594456','','assigned','','home','','','','','','',''),(53,7,'boniface','','0711071132','','assigned','','work','','','','','','',''),(54,7,'gerald','','0707988885','','assigned','','home','','','','','','',''),(55,7,'esther','','0712594456','','assigned','','home','','','','','','',''),(56,7,'boniface','','0711071132','','assigned','','work','','','','','','',''),(57,7,'gerald','','0707988885','','assigned','','home','','','','','','',''),(58,8,'David','Syengo','0712594022','','assigned','','Home','022','','','','','',''),(59,8,'Damaris','Mutinda','0707996985','','assigned','','Home','985','','','','','',''),(60,8,'Other','Guy','020514322','','assigned','','Work','322','','','','','',''),(61,9,'David','Syengo','0712594022','','assigned','Home','022','','','','','','',''),(62,9,'Damaris','Mutinda','0707996985','','assigned','Home','985','','','','','','',''),(63,9,'Other','Guy','020514322','','assigned','Work','322','','','','','','',''),(64,10,'esther','','0712594456','','new','','home','','','','','','',''),(65,10,'boniface','','0711071132','','new','','work','','','','','','',''),(66,10,'gerald','','0707988885','','new','','home','','','','','','',''),(67,10,'esther','','0712594456','','new','','home','','','','','','',''),(68,10,'boniface','','0711071132','','new','','work','','','','','','',''),(69,10,'gerald','','0707988885','','new','','home','','','','','','',''),(70,10,'esther','','0712594456','','new','','home','','','','','','',''),(71,10,'boniface','','0711071132','','new','','work','','','','','','',''),(72,10,'gerald','','0707988885','','new','','home','','','','','','',''),(76,12,'David','Syengo','0712594022','','assigned','Home','022','','','','','','',''),(77,12,'Damaris','Mutinda','0712594022','','assigned','Home','985','','','','','','',''),(78,12,'Other','Guy','0712594022','','assigned','Work','322','','','','','','',''),(79,13,'David','Syengo','0712594022','','assigned','Home','022','','','','','','',''),(80,13,'Damaris','Mutinda','0712594022','','assigned','Home','985','','','','','','',''),(81,13,'Other','Guy','0712594022','','assigned','Work','322','','','','','','',''),(82,14,'David','Syengo','0712594022','','assigned','Home','022','','','','','','',''),(83,14,'Damaris','Mutinda','0712594022','','assigned','Home','985','','','','','','',''),(84,14,'Other','Guy','0712594022','','assigned','Work','322','','','','','','','');
/*!40000 ALTER TABLE `call_list_data` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `call_lists`
--

DROP TABLE IF EXISTS `call_lists`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `call_lists` (
  `id` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `descr` text,
  `campaignID` smallint(5) unsigned NOT NULL,
  `date_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `call_lists`
--

LOCK TABLES `call_lists` WRITE;
/*!40000 ALTER TABLE `call_lists` DISABLE KEYS */;
INSERT INTO `call_lists` VALUES (9,'List for another campaign','blah blah',5,'2016-01-14 15:41:05'),(10,'custo3','',11,'2016-01-19 22:41:58'),(11,'Test demo','demo list',1,'2016-02-05 18:06:39'),(12,'test_list2','another list',1,'2016-02-08 12:46:55'),(13,'badder','',5,'2016-02-08 17:51:13'),(14,'Beta list','for beta test',18,'2016-02-10 18:44:06');
/*!40000 ALTER TABLE `call_lists` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `call_lists_view`
--

DROP TABLE IF EXISTS `call_lists_view`;
/*!50001 DROP VIEW IF EXISTS `call_lists_view`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `call_lists_view` (
  `name` tinyint NOT NULL,
  `id` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `campaigns`
--

DROP TABLE IF EXISTS `campaigns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `campaigns` (
  `id` smallint(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `descr` text,
  `teamID` smallint(4) NOT NULL,
  `date_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `script` text,
  PRIMARY KEY (`id`),
  KEY `teamID` (`teamID`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `campaigns`
--

LOCK TABLES `campaigns` WRITE;
/*!40000 ALTER TABLE `campaigns` DISABLE KEYS */;
INSERT INTO `campaigns` VALUES (1,'Default Campaign','This is the default campaign',1,'2016-01-08 20:39:00','Enter script here'),(2,'Another Campaign','another one',3,'2016-01-14 12:20:12','Enter script here'),(4,'test camp','no descr',1,'2016-01-14 12:56:41','Enter script here'),(12,'test campaign 2','mon see mon do',1,'2016-01-14 15:10:42','Enter script here'),(13,'Agrresive Campaign','wfgfwg',3,'2016-01-14 15:15:59','Enter script here'),(14,'some campaign','',1,'2016-01-19 13:11:44','Enter script here'),(18,'New Campaigns','Good script',4,'2016-02-11 12:24:47','blah blah');
/*!40000 ALTER TABLE `campaigns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `outcome`
--

DROP TABLE IF EXISTS `outcome`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `outcome` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `callid` mediumint(8) unsigned NOT NULL COMMENT 'this is the id of the call in list data',
  `status` varchar(200) NOT NULL COMMENT 'Either called/failed/non-existent',
  `notes` text COMMENT 'any additional notes by agent',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `outcome`
--

LOCK TABLES `outcome` WRITE;
/*!40000 ALTER TABLE `outcome` DISABLE KEYS */;
INSERT INTO `outcome` VALUES (1,76,'successful','Enter notes about the call here...'),(2,77,'unsuccessful','Enter notes about the call here...'),(3,78,'unsuccessful','Enter notes about the call here...'),(4,61,'unsuccessful','Enter notes about the call here...'),(5,62,'unsuccessful','Enter notes about the call here...'),(6,63,'unsuccessful','Enter notes about the call here...'),(7,79,'successful','Enter notes about the call here...'),(8,80,'unsuccessful','Enter notes about the call here...'),(9,81,'successful','pusha t');
/*!40000 ALTER TABLE `outcome` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `id` tinyint(2) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `descr` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'User','normal user'),(2,'Admin','Administrator...can also dial');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sessions`
--

DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sessions` (
  `id` smallint(5) NOT NULL AUTO_INCREMENT,
  `userid` smallint(5) NOT NULL,
  `logintime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `lastaccess` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `userid` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=114 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sessions`
--

LOCK TABLES `sessions` WRITE;
/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
INSERT INTO `sessions` VALUES (32,1,'2016-02-05 12:54:34','2016-02-05 12:54:34'),(55,13,'2016-02-05 17:41:57','2016-02-05 17:41:57'),(99,14,'2016-02-08 17:41:55','2016-02-08 17:41:55'),(106,16,'2016-02-08 21:48:30','2016-02-08 21:48:30'),(113,20,'2016-02-11 11:29:44','2016-02-11 11:29:44');
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teams`
--

DROP TABLE IF EXISTS `teams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `teams` (
  `id` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `descr` text,
  `date_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teams`
--

LOCK TABLES `teams` WRITE;
/*!40000 ALTER TABLE `teams` DISABLE KEYS */;
INSERT INTO `teams` VALUES (1,'Default Team','This is the default team','2016-01-08 20:37:13'),(3,'Third Team','Another team for test','2016-01-13 21:31:01'),(4,'Team Kukula','','2016-02-11 11:14:25');
/*!40000 ALTER TABLE `teams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `name` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `roleID` tinyint(2) unsigned NOT NULL,
  `date_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `campaignID` smallint(3) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`),
  KEY `roleID` (`roleID`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`roleID`) REFERENCES `roles` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','Administrator','5baa61e4c9b93f3f0682250b6cf8331b7ee68fd8',2,'2016-01-08 20:42:49',1),(18,'user1','Default User','b3daa77b4c04a9551b8781d03191fe098f325e67',1,'2016-02-09 10:59:37',18),(19,'newuser10479','New User','9d4e1e23bd5b727046a9e3b4b7db57bd8d6ee684',1,'2016-02-09 11:01:37',18),(21,'test','Testing','a94a8fe5ccb19ba61c4c0873d391e987982fbbd3',1,'2016-02-11 12:10:55',4),(22,'syengo','David Syengo','08d2369d022d944867fa50de09c239b2be015c83',1,'2016-02-11 12:22:08',1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `call_lists_view`
--

/*!50001 DROP TABLE IF EXISTS `call_lists_view`*/;
/*!50001 DROP VIEW IF EXISTS `call_lists_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = cp850 */;
/*!50001 SET character_set_results     = cp850 */;
/*!50001 SET collation_connection      = cp850_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `call_lists_view` AS select `call_lists`.`name` AS `name`,`call_lists`.`id` AS `id` from `call_lists` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-02-12 17:53:51
