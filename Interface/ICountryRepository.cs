﻿using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interface
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int id);
        Country GetCountryByOwner(int ownerId);
        ICollection<Owner> GetOwnersFromCountryId(int id);
        bool CountryExists(int id);
        bool CreateCountry(Country country);
        bool UpdateCountry(Country country); 
        bool DeleteCountry(Country country); 
        bool Save();
    }
}
