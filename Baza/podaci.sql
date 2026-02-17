
-- Tipovi clanstva

INSERT INTO TipoviClanstava
(Ime, Cena, Trajanje, MaxSatiPoMesecu, PristupSali, BrojSatiUSaliMesecno)
VALUES
('Dnevno', 12.00, 1, 8, 0, NULL),
('Hot Desk', 120.00, 30, 80, 1, 10),
('Dedicated Desk', 180.00, 30, 120, 1, 20),
('Premium', 250.00, 30, 200, 1, 40);


-- Lokacije

INSERT INTO Lokacije
(Ime, Adresa, Grad, RadniSati, MaxBrojKorisnika)
VALUES
('Hub Kragujevac Center', 'Kralja Aleksandra 12', 'Kragujevac', '08:00–22:00', 120),
('Hub Beograd', 'Nemanjina 4', 'Beograd', '00:00–24:00', 300),
('Hub Novi Sad', 'Bulevar Oslobodjenja 88', 'Novi Sad', '07:00–23:00', 180);


-- Korisnici

INSERT INTO Korisnici
(Ime, Prezime, Email, Telefon, TipClanstvaId, DatumPocetkaClanstva, DatumKrajaClanstva, StatusNaloga)
VALUES
('Marko', 'Jovanovic', 'marko.j@mail.com', '061111111', 2, '2025-01-01', '2025-01-31', 'aktivan'),
('Ana', 'Petrovic', 'ana.p@mail.com', '062222222', 3, '2025-01-05', '2025-02-04', 'aktivan'),
('Nikola', 'Ilic', 'nikola.i@mail.com', '063333333', 4, '2025-01-01', '2025-01-31', 'aktivan'),
('Jelena', 'Markovic', 'jelena.m@mail.com', '064444444', 1, '2025-01-10', '2025-01-10', 'istekao'),
('Ivan', 'Stojanovic', 'ivan.s@mail.com', '065555555', 2, '2025-01-03', '2025-02-02', 'pauziran'),
('Milica', 'Kovacevic', 'milica.k@mail.com', '066666666', 3, '2025-01-01', '2025-01-31', 'aktivan'),
('Stefan', 'Nikolic', 'stefan.n@mail.com', '067777777', 4, '2025-01-01', '2025-01-31', 'aktivan'),
('Tamara', 'Lazic', 'tamara.l@mail.com', '068888888', 2, '2025-01-12', '2025-02-11', 'aktivan'),
('Petar', 'Djordjevic', 'petar.d@mail.com', '069999999', 3, '2025-01-08', '2025-02-07', 'aktivan'),
('Marija', 'Vasic', 'marija.v@mail.com', '060000000', 1, '2025-01-15', '2025-01-15', 'istekao'),
('Luka', 'Pavlovic', 'luka.p@mail.com', '061101010', 2, '2025-01-02', '2025-02-01', 'aktivan'),
('Sara', 'Milosevic', 'sara.m@mail.com', '062202020', 3, '2025-01-03', '2025-02-02', 'aktivan'),
('Filip', 'Arandjelovic', 'filip.a@mail.com', '063303030', 4, '2025-01-01', '2025-01-31', 'aktivan'),
('Maja', 'Popovic', 'maja.p@mail.com', '064404040', 1, '2025-01-18', '2025-01-18', 'istekao'),
('Nemanja', 'Ristic', 'nemanja.r@mail.com', '065505050', 2, '2025-01-06', '2025-02-05', 'pauziran'),
('Jovana', 'Mitrovic', 'jovana.m@mail.com', '066606060', 3, '2025-01-07', '2025-02-06', 'aktivan'),
('Aleksandar', 'Petkovic', 'aleks.p@mail.com', '067707070', 4, '2025-01-01', '2025-01-31', 'aktivan'),
('Teodora', 'Stankovic', 'teodora.s@mail.com', '068808080', 2, '2025-01-11', '2025-02-10', 'aktivan'),
('Vuk', 'Todorovic', 'vuk.t@mail.com', '069909090', 3, '2025-01-09', '2025-02-08', 'aktivan'),
('Isidora', 'Jaksic', 'isidora.j@mail.com', '060111111', 1, '2025-01-21', '2025-01-21', 'istekao'),
('Ognjen', 'Knezevic', 'ognjen.k@mail.com', '061222333', 2, '2025-01-04', '2025-02-03', 'aktivan'),
('Kristina', 'Obradovic', 'kristina.o@mail.com', '062333444', 3, '2025-01-05', '2025-02-04', 'aktivan'),
('Dusan', 'Maric', 'dusan.m@mail.com', '063444555', 4, '2025-01-01', '2025-01-31', 'aktivan'),
('Anja', 'Simic', 'anja.s@mail.com', '064555666', 2, '2025-01-13', '2025-02-12', 'aktivan'),
('Bojan', 'Zivkovic', 'bojan.z@mail.com', '065666777', 3, '2025-01-14', '2025-02-13', 'aktivan');


-- Resursi

-- Kragujevac
INSERT INTO Resursi
(LokacijaId, Ime, TipResursa, Opis, PodtipStola, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu)
VALUES
(1, 'Sto A-1', 'sto', 'Pored prozora', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(1, 'Sto A-2', 'sto', 'Tih deo zgrade', 'dedicated_desk', NULL, NULL, NULL, NULL, NULL),
(1, 'Sto A-3', 'sto', 'Blizu ulaza', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(1, 'Sto A-4', 'sto', 'Tiha zona', 'dedicated_desk', NULL, NULL, NULL, NULL, NULL),
(1, 'Sala K-1', 'sala', 'Velika sala za sastanke', NULL, 8, 1, 1, 1, 1),
(1, 'Sala K-2', 'sala', 'Mala sala za sastanke', NULL, 4, 0, 1, 1, 0),
(1, 'Sala K-3', 'sala', 'Sala sa projektorom', NULL, 10, 1, 0, 1, 1);

-- Belgrade
INSERT INTO Resursi
(LokacijaId, Ime, TipResursa, Opis, PodtipStola, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu)
VALUES
(2, 'Sto B-1', 'sto', 'Otvoren prostor', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(2, 'Sto B-2', 'sto', 'Otvoren prostor', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(2, 'Sto B-3', 'sto', 'Premium prostor', 'dedicated_desk', NULL, NULL, NULL, NULL, NULL),
(2, 'Sto B-4', 'sto', 'Blizu kuhinje', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(2, 'Sto B-5', 'sto', 'Tihi deo', 'dedicated_desk', NULL, NULL, NULL, NULL, NULL),
(2, 'Sala Y-1', 'sala', 'Premium prostor', NULL, 12, 1, 1, 1, 1),
(2, 'Sala Y-2', 'sala', 'Sala za prezentacije', NULL, 15, 1, 1, 1, 1),
(2, 'Privatna kancelarija G-1', 'privatna_kancelarija', 'Privatna timska kancelarija', NULL, 6, 0, 1, 1, 1);

-- Novi Sad
INSERT INTO Resursi
(LokacijaId, Ime, TipResursa, Opis, PodtipStola, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu)
VALUES
(3, 'Sto C-1', 'sto', 'Pored prozora', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(3, 'Sto C-2', 'sto', 'U uglu', 'dedicated_desk', NULL, NULL, NULL, NULL, NULL),
(3, 'Sto C-3', 'sto', 'Radni sto pored zida', 'hot_desk', NULL, NULL, NULL, NULL, NULL),
(3, 'Sala T-1', 'sala', 'OGROMNA sala', NULL, 6, 1, 0, 1, 1),
(3, 'Sala T-2', 'sala', 'Sala za timske sastanke', NULL, 8, 0, 1, 1, 1);


-- Rezervacije

INSERT INTO Rezervacije
(KorisnikId, ResursId, PocetakVreme, KrajVreme, StatusRezervacije)
VALUES
(1, 1, '2025-01-20 09:00', '2025-01-20 13:00', 'zavrsena'),
(2, 3, '2025-01-22 10:00', '2025-01-22 12:00', 'zavrsena'),
(3, 7, '2025-01-25 14:00', '2025-01-25 18:00', 'aktivna'),
(6, 10, '2025-01-26 09:00', '2025-01-26 11:00', 'aktivna'),
(7, 8, '2025-01-27 15:00', '2025-01-27 17:00', 'otkazana'),
(11, 2, '2025-01-28 09:00', '2025-01-28 12:00', 'zavrsena'),
(12, 5, '2025-01-29 10:00', '2025-01-29 13:00', 'zavrsena'),
(13, 9, '2025-01-30 14:00', '2025-01-30 18:00', 'aktivna'),
(14, 12, '2025-01-31 09:00', '2025-01-31 11:00', 'aktivna'),
(15, 6, '2025-02-01 15:00', '2025-02-01 17:00', 'otkazana'),
(16, 4, '2025-02-02 08:00', '2025-02-02 10:00', 'zavrsena'),
(17, 7, '2025-02-03 12:00', '2025-02-03 14:00', 'aktivna'),
(18, 10, '2025-02-04 16:00', '2025-02-04 18:00', 'aktivna'),
(19, 1, '2025-02-05 09:00', '2025-02-05 12:00', 'zavrsena'),
(20, 8, '2025-02-06 13:00', '2025-02-06 16:00', 'otkazana');
