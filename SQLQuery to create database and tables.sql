CREATE DATABASE University;
GO

USE University;

CREATE TABLE COURSES
(
	COURSE_ID INT CONSTRAINT PK_COURSE_ID PRIMARY KEY IDENTITY,
	NAME NVARCHAR(20) CONSTRAINT UQ_COURSE_NAME UNIQUE NOT NULL,
	DESCRIPTION NVARCHAR(30) NOT NULL
);

CREATE TABLE GROUPS
(
	GROUP_ID INT IDENTITY,
	COURSE_ID INT,
	NAME NVARCHAR(20) NOT NULL,
	CONSTRAINT PK_GROUP_ID PRIMARY KEY (GROUP_ID),
	CONSTRAINT FK_GROUPS_To_COURSES FOREIGN KEY (COURSE_ID) REFERENCES COURSES (COURSE_ID),
	CONSTRAINT UQ_GROUP_NAME UNIQUE (NAME)
);

CREATE TABLE STUDENTS
(
	STUDENT_ID INT IDENTITY,
	GROUP_ID INT,
	FIRST_NAME NVARCHAR(20) NOT NULL,
	LAST_NAME NVARCHAR(20) NOT NULL,
	CONSTRAINT PK_STUDENT_ID PRIMARY KEY (STUDENT_ID),
	CONSTRAINT FK_STUDENTS_To_GROUPS FOREIGN KEY (GROUP_ID) REFERENCES GROUPS (GROUP_ID)
);