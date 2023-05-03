CREATE TABLE [Reservations] (
    [Id] int IDENTITY (1,1) NOT NULL, 
    [Name] nvarchar(255) NOT NULL, 
    [MemberId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [ReservationDate] datetime2 NOT NULL,
    [ReservationEndDate] datetime2 NOT NULL,
    [UniqueNumber] int NOT NULL,
    [IsReservationClosed] bit NOT NULL,
);