namespace Domain;

public record QuoteRequest(string Operation, string Instrument, int Quantity);