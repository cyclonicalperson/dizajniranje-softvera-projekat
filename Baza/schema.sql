-- Tipovi clanstva

CREATE TABLE TipoviClanstava (
    Id INT IDENTITY PRIMARY KEY,
    Ime NVARCHAR(50) NOT NULL UNIQUE,
    Cena DECIMAL(10,2) NOT NULL,
    Trajanje INT NOT NULL,
    MaxSatiPoMesecu INT NOT NULL,
    PristupSali BIT NOT NULL,
    BrojSatiUSaliMesecno INT NULL
);


-- Lokacije

CREATE TABLE Lokacije (
    Id INT IDENTITY PRIMARY KEY,
    Ime NVARCHAR(100) NOT NULL,
    Adresa NVARCHAR(200) NOT NULL,
    Grad NVARCHAR(50) NOT NULL,
    RadniSati NVARCHAR(100) NOT NULL,
    MaxBrojKorisnika INT NOT NULL
);


-- Korisnici

CREATE TABLE Korisnici (
    Id INT IDENTITY PRIMARY KEY,
    Ime NVARCHAR(50) NOT NULL,
    Prezime NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Telefon NVARCHAR(30),

    TipClanstvaId INT NOT NULL,
    DatumPocetkaClanstva DATE NOT NULL,
    DatumKrajaClanstva DATE NOT NULL,

    StatusNaloga NVARCHAR(20) NOT NULL
        CHECK (StatusNaloga IN ('aktivan', 'pauziran', 'istekao')),

    CONSTRAINT FK_Korisnici_TipoviClanstava
        FOREIGN KEY (TipClanstvaId)
        REFERENCES TipoviClanstava(Id)
);


-- Resursi

CREATE TABLE Resursi (
    Id INT IDENTITY PRIMARY KEY,
    LokacijaId INT NOT NULL,
    Ime NVARCHAR(100) NOT NULL,
    TipResursa NVARCHAR(30) NOT NULL
        CHECK (TipResursa IN ('sto', 'sala', 'privatna_kancelarija')),
    Opis NVARCHAR(300),

    -- Za radna mesta
    PodtipStola NVARCHAR(30) NULL
        CHECK (PodtipStola IN ('hot_desk', 'dedicated_desk')),

    -- Za sale za sastanke
    Kapacitet INT NULL,
    ImaProjektor BIT NULL,
    ImaTV BIT NULL,
    ImaTablu BIT NULL,
    ImaOnlineOpremu BIT NULL,

    CONSTRAINT FK_Resursi_Lokacije
        FOREIGN KEY (LokacijaId)
        REFERENCES Lokacije(Id)
);

-- U slucaju da dodamo jos podtipova:
-- Za svaki tip resursa odrediti cutom parametre kao što je u primeru za radno mesto salu za sastanke


-- Rezervacije

CREATE TABLE Rezervacije (
    Id INT IDENTITY PRIMARY KEY,
    KorisnikId INT NOT NULL,
    ResursId INT NOT NULL,

    PocetakVreme DATETIME2 NOT NULL,
    KrajVreme DATETIME2 NOT NULL,

    StatusRezervacije NVARCHAR(20) NOT NULL
        CHECK (StatusRezervacije IN ('aktivna', 'zavrsena', 'otkazana')),

    CONSTRAINT FK_Rezervacije_Korisnici
        FOREIGN KEY (KorisnikId)
        REFERENCES Korisnici(Id),

    CONSTRAINT FK_Rezervacije_Resursi
        FOREIGN KEY (ResursId)
        REFERENCES Resursi(Id),

    CONSTRAINT CHK_Rezervacija_Vreme
        CHECK (KrajVreme > PocetakVreme)
);


-- Indekseri

CREATE INDEX IX_Korisnici_Status ON Korisnici(StatusNaloga);
CREATE INDEX IX_Korisnici_TipClanstva ON Korisnici(TipClanstvaId);

CREATE INDEX IX_Resursi_Lokacija ON Resursi(LokacijaId);
CREATE INDEX IX_Resursi_Tip ON Resursi(TipResursa);

CREATE INDEX IX_Rezervacije_Korisnik ON Rezervacije(KorisnikId);
