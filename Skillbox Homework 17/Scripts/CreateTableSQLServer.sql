Create Table localDBTable
(
	ID int not null IDENTITY(1,1),
	Фамилия nvarchar(255) not null,
	Имя nvarchar(255) not null,
	Отчество nvarchar(255) not null,
	[Номер_телефона] nvarchar(255),
	Email nvarchar(255) not null,
)