use autoreviewerdb;
go

drop table if exists Registration;
drop table if exists UserSkills;
drop table if exists Skill;
drop table if exists GitHubUser;
drop table if exists Repository;
go

create table GitHubUser(
	gitHubUserID int identity(1, 1),
	gitHubUsername varchar(39) not null,
	gitHubUserEmail varchar(255) not null,

	constraint pk_gitHubUserID primary key (gitHubUserID),
	constraint uq_gitHubUsername unique (gitHubUsername),
	constraint uq_gitHubUserEmail unique (gitHubUserEmail)
);
go

create table Skill(
	skillID int identity(1, 1),
	skillName varchar(20) not null,

	constraint pk_skillID primary key (skillID),
	constraint uq_skillName unique (skillName)
);
go

create table Repository(
	repositoryID int identity(1, 1),
	repositoryName varchar(100) not null,
	repositoryOwnerUsername varchar(39) not null,

	constraint pk_repositoryID primary key (repositoryID),
	constraint uq_repositoryName unique (repositoryName),
);
go

create table Registration(
	registrationID  int identity(1, 1),
	gitHubUserID int not null,
	repositoryID int not null,

	constraint pk_registrationID primary key (registrationID),
	constraint fk_gitHubUserID_Registration foreign key (gitHubUserID) references GitHubUser (gitHubUserID),
	constraint fk_repositoryID_Registration foreign key (repositoryID) references Repository (repositoryID)
);
go

create table UserSkills(
	userSkillID int identity(1, 1),
	gitHubUserID int not null,
	skillID int not null,

	constraint pk_userSkillID primary key (userSkillID),
	constraint fk_gitHubUserID_UserSkills foreign key (gitHubUserID) references GitHubUser (gitHubUserID),
	constraint fk_skillID_UserSkills foreign key (skillID) references Skill (skillID)
);
go