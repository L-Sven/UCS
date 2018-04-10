DELETE FROM Kunder;
DELETE FROM Leverantörer;
DELETE FROM KundFakHuvud_KundFakRad;
DELETE FROM LevFakHuvud_LevFakRad;
DELETE FROM KundFakturaHuvud;
DELETE FROM LevFakturaHuvud;
DELETE FROM KundFakturaRad;
DELETE FROM LevFakturaRad;

DBCC CHECKIDENT (KundFakturaRad, RESEED, 0)
DBCC CHECKIDENT (LevFakturaRad, RESEED, 0)
DBCC CHECKIDENT (KundFakHuvud_KundFakRad, RESEED, 0)
DBCC CHECKIDENT (LevFakHuvud_LevFakRad, RESEED, 0)