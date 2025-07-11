USE [HR_department]
GO

-- Очистка данных с временным отключением ограничений
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO

DELETE FROM [dbo].[Users]
DELETE FROM [dbo].[VacationRequests]
DELETE FROM [dbo].[SickLeaves]
DELETE FROM [dbo].[ErrorReports]
DELETE FROM [dbo].[Certifications]
DELETE FROM [dbo].[Achievements]
DELETE FROM [dbo].[Staff]
DELETE FROM [dbo].[Positions]
DELETE FROM [dbo].[Departments]
DELETE FROM [dbo].[Status_user]
DELETE FROM [dbo].[CertificationsValues]
DELETE FROM [dbo].[LeaveStatus]
GO

EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'
GO

-- Вставка справочных данных
INSERT INTO [dbo].[Departments] ([DepartmentID], [DepartmentName])
VALUES 
(3, 'Отдел кадров'),
(4, 'Бухгалтерия'),
(5, 'IT-отдел'),
(6, 'Отдел продаж'),
(7, 'Отдел маркетинга')
GO

INSERT INTO [dbo].[Positions] ([PositionID], [PositionName], [Salary], [Description])
VALUES 
(1, 'HR-менеджер', 70000.00, 'Специалист по подбору персонала'),
(2, 'Бухгалтер', 60000.00, 'Ведение финансовой отчетности'),
(3, 'Программист', 120000.00, 'Разработка программного обеспечения'),
(4, 'Менеджер по продажам', 80000.00, 'Работа с клиентами и продажи'),
(5, 'Маркетолог', 75000.00, 'Продвижение продуктов компании'),
(6, 'Директор', 150000.00, 'Руководство отделом')
GO

INSERT INTO [dbo].[Status_user] ([StatusID], [Name_status])
VALUES 
(4, 'Администратор'),
(5, 'Руководитель'),
(6, 'Сотрудник')
GO

INSERT INTO [dbo].[CertificationsValues] ([StatusID], [StatusName])
VALUES 
(1, 'Запланирована'),
(2, 'Пройдена'),
(3, 'Не пройдена'),
(4, 'Отменена')
GO

INSERT INTO [dbo].[LeaveStatus] ([StatusID], [StatusName])
VALUES 
(1, 'На рассмотрении'),
(2, 'Одобрено'),
(3, 'Отклонено'),
(4, 'Одобрено с правками')
GO

-- Вставка данных о сотрудниках
INSERT INTO [dbo].[Staff] (
    [LastName], 
    [FirstName], 
    [MiddleName], 
    [BirthDate], 
    [ContactInfo], 
    [Education], 
    [HireDate], 
    [PositionID], 
    [DepartmentID]
)
VALUES 
('Иванов', 'Иван', 'Иванович', '1985-05-15', '+79161234567', 'Высшее техническое образование', '2020-01-10', 6, 5),
('Петров', 'Петр', 'Петрович', '1990-07-22', '+79162223344', 'Высшее экономическое', '2021-03-15', 2, 4),
('Сидорова', 'Анна', 'Сергеевна', '1992-11-05', '+79163334455', 'Высшее гуманитарное', '2019-05-20', 1, 3),
('Кузнецов', 'Алексей', NULL, '1988-09-14', '+79165556677', 'Высшее техническое', '2018-02-10', 3, 5)
GO

-- Вставка пользователей (пароли: admin123 и user123)
INSERT INTO [dbo].[Users] (
    [Login_user], 
    [Password_user], 
    [EmployeeID], 
    [StatusID]
)
VALUES 
('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 
 (SELECT EmployeeID FROM Staff WHERE LastName = 'Иванов'), 4),
('petrov', '3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1', 
 (SELECT EmployeeID FROM Staff WHERE LastName = 'Петров'), 6),
('sidorova', '3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1', 
 (SELECT EmployeeID FROM Staff WHERE LastName = 'Сидорова'), 5),
('kuznetsov', '3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1', 
 (SELECT EmployeeID FROM Staff WHERE LastName = 'Кузнецов'), 6)
GO