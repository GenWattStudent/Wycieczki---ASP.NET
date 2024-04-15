using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack,
    Souper
}

public class MealModel : BaseEntity
{
    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(0, 15000)]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Price { get; set; }
    public int MealTypeId { get; set; }
    public MealType MealType { get; set; }
    public int TourId { get; set; }
    public TourModel Tour { get; set; }
    public DateTime Start { get; set; } = DateTime.Now;
    public DateTime End { get; set; } = DateTime.Now;

    public void Update(MealModel mealModel)
    {
        Name = mealModel.Name;
        Description = mealModel.Description;
        Price = mealModel.Price;
        MealTypeId = mealModel.MealTypeId;
        MealType = mealModel.MealType;
        TourId = mealModel.TourId;
        Tour = mealModel.Tour;
        Start = mealModel.Start;
        End = mealModel.End;
    }
}