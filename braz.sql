-- phpMyAdmin SQL Dump
-- version 4.1.14
-- http://www.phpmyadmin.net
--
-- Хост: 127.0.0.1
-- Час створення: Лют 20 2016 р., 17:31
-- Версія сервера: 5.6.17
-- Версія PHP: 5.5.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- База даних: `braz`
--

-- --------------------------------------------------------

--
-- Структура таблиці `cart`
--

CREATE TABLE IF NOT EXISTS `cart` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `List` text COLLATE utf8_unicode_ci NOT NULL,
  `User` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=4 ;

--
-- Дамп даних таблиці `cart`
--

INSERT INTO `cart` (`Id`, `List`, `User`) VALUES
(1, '', 0),
(2, '', 0),
(3, '', 3);

-- --------------------------------------------------------

--
-- Структура таблиці `categories`
--

CREATE TABLE IF NOT EXISTS `categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `catid` int(11) NOT NULL,
  `Name` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=66 ;

--
-- Дамп даних таблиці `categories`
--

INSERT INTO `categories` (`Id`, `catid`, `Name`) VALUES
(0, 0, 'Полоса'),
(9, 9, 'Профиль для натяжных потолков'),
(11, 11, 'Мебельный профиль'),
(12, 12, 'Торгово-выставочные системы'),
(13, 13, 'Профиль для конструкций теплиц'),
(16, 16, 'Отлив алюминиевый'),
(17, 1, 'Труба круглая'),
(18, 1, 'Труба квадратная'),
(19, 1, 'Труба прямоугольная'),
(20, 1, 'Труба трехгранная'),
(21, 1, 'Труба овальная'),
(22, 1, 'Труба специальной формы'),
(23, 2, 'Уголок равносторонний'),
(24, 2, 'Уголок разносторонний'),
(25, 3, 'Пруток круглый'),
(26, 3, 'Пруток овальный'),
(27, 3, 'Пруток шестигранный'),
(28, 4, 'Швеллер'),
(29, 4, 'Тавр'),
(30, 4, 'Z-Профиль'),
(31, 4, 'Двутавр'),
(32, 4, 'Отбортованный швеллер'),
(33, 5, 'Вентиляционный профиль'),
(34, 5, 'Коробка для электропроводки'),
(35, 5, 'Корпуса приборов'),
(36, 5, 'Радиаторный профиль'),
(37, 5, 'Роторный профиль'),
(38, 5, 'Светодиодный профиль'),
(39, 5, 'Станочный профиль'),
(40, 5, 'Тепловый трубы'),
(41, 6, 'Врезной профиль'),
(42, 6, 'Карнизы'),
(43, 6, 'Порожки'),
(44, 7, 'Витражный профиль'),
(45, 7, 'Оконно-дверной профиль'),
(46, 7, 'Клемный профиль'),
(47, 7, 'Антимаскитный профиль'),
(48, 7, 'Система офисных перегородок'),
(49, 7, 'Фасадный профиль'),
(50, 8, 'Вагонка'),
(51, 8, 'Опалубка'),
(52, 8, 'Правило строительное'),
(53, 10, 'Автомобильный профиль'),
(54, 10, 'Яхтенный и мачтовый профиль'),
(55, 10, 'Алюминиевые полы'),
(56, 14, 'Вентиляционные решетки'),
(57, 14, 'Декоративные решетки'),
(58, 14, 'Грязезащитные решетки'),
(59, 15, 'Клик-профиль'),
(60, 15, 'Рамочный профиль'),
(61, 15, 'Рекламный профиль'),
(62, 17, 'Петели'),
(63, 17, 'Плашечные зажимы'),
(64, 17, 'Соеденители квадратных труб'),
(65, 17, 'Угловые вкладыши');

-- --------------------------------------------------------

--
-- Структура таблиці `products`
--

CREATE TABLE IF NOT EXISTS `products` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Article` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `catId` int(11) NOT NULL,
  `S` float NOT NULL,
  `Q` float NOT NULL,
  `D` float NOT NULL,
  `P` float NOT NULL,
  `A` float NOT NULL,
  `B` float NOT NULL,
  `R` float NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Структура таблиці `usertable`
--

CREATE TABLE IF NOT EXISTS `usertable` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `Password` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `Email` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `Phone` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=4 ;

--
-- Дамп даних таблиці `usertable`
--

INSERT INTO `usertable` (`Id`, `Name`, `Password`, `Email`, `Phone`) VALUES
(1, 'asd', 'b2ef9c7b10eb0985365f913420ccb84a', 'asd@asd', 'asd'),
(2, 'дичь', 'a637d91093e07c661749a681730000dd', 'asd@asd', '123'),
(3, 'dichara', '698d51a19d8a121ce581499d7b701668', 'bigunok@lax', '1111');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
