using System;
using System.Collections.Generic;
using System.Linq;

public enum Allergen
{
    Eggs,
    Peanuts,
    Shellfish,
    Strawberries,
    Tomatoes,
    Chocolate,
    Pollen,
    Cats
}

public class AllergenInfo
{
    public Allergen? Allergen { get; set; }
    public int Value { get; set; }
}

public class Allergies
{  
    private List<AllergenInfo> _allergensInfos;
    private int _mask;

    public Allergies(int mask)
    {
        _mask = mask;        
        BuildAllergiensInfos();
       
        if (_mask > 256)
            AddHigherAllergiens();
    }

    private void BuildAllergiensInfos()
    {
        var num = 1;
        var allergens = Enum.GetValues(typeof(Allergen)).Cast<Allergen>().ToList();        
        _allergensInfos = allergens.Select(x => new AllergenInfo
        {
            Allergen = x,
            Value = num *= allergens.IndexOf(x) == 0 ? 1 : 2
        }).ToList();
    }

    private void AddHigherAllergiens()
    {
        while (_allergensInfos.Sum(x => x.Value) < _mask)
        {
            _allergensInfos.Add(new AllergenInfo
            {
                Value = _allergensInfos.Last().Value * 2
            });
        }
    }

    public bool IsAllergicTo(Allergen allergen)
    {
        return IsExactAllergen(out Allergen? exactAllergen) 
            ? exactAllergen == allergen 
            : GetUsedAllergies().Contains(allergen);
    }

    public Allergen[] List()
    {
        if (_mask == 0)
            return new Allergen[0];

        return IsExactAllergen(out Allergen? exactAllergen)
            ? (new Allergen[] { exactAllergen.Value })
            : GetUsedAllergies().OrderBy(x => x).ToArray();
    }

    private bool IsExactAllergen(out Allergen? allergen)
    {
        allergen = _allergensInfos.FirstOrDefault(x => x.Value == _mask)?.Allergen;
        return allergen != null;
    }

    private List<Allergen> GetUsedAllergies()
    {
        var sum = 0;
        var usedAllergies = new List<Allergen>();
        foreach (var item in _allergensInfos.OrderByDescending(x => x.Value))
        {
            if (item.Value + sum > _mask)
                continue;

            if (item.Allergen != null)
                usedAllergies.Add(item.Allergen.Value);

            sum += item.Value;
            if (sum == _mask)
                break;
        }
        return usedAllergies;
    }
}