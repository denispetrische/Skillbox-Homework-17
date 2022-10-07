INSERT INTO localDBTable([Фамилия], [Имя], [Отчество], [Номер_телефона],[Email]) Values (N'Сидоров', N'Иван', N'Иванович','+3242414','ivan228@mail.ru');
INSERT INTO localDBTable([Фамилия], [Имя], [Отчество], [Номер_телефона],[Email]) Values (N'Иванов', N'Макс', N'Александрович','+32123414','max228@mail.ru');
INSERT INTO localDBTable([Фамилия], [Имя], [Отчество], [Номер_телефона],[Email]) Values (N'Петров', N'Ник', N'Игоревич','+34363414','nick228@mail.ru');
INSERT INTO localDBTable([Фамилия], [Имя], [Отчество], [Номер_телефона],[Email]) Values (N'Иванова', N'Сая', N'Аркадьевич','+37478914','arcadia228@mail.ru');


UPDATE localDBTable SET ID = 1 where ID = 11;

SELECT * FROM localDBTable;

DELETE FROM localDBTable WHERE [ID]<13;

Drop Table localDBTable;