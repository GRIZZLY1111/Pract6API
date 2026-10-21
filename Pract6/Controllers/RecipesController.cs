using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pract6.Models;

namespace Pract6.Controllers
{
    [Route("api/v1/[controller]")] 
    public class RecipesController : Controller
    {
        private readonly WebApiDatabaseContext _context;

        public RecipesController(WebApiDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? cuisine, [FromQuery] bool? vegetarian)
        {
            var query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrEmpty(cuisine))
            {
                query = query.Where(r => r.Cuisine == cuisine);
            }

            if (vegetarian.HasValue)
            {
                query = query.Where(r => r.IsVegetarian == vegetarian.Value);
            }

            return Ok(query.ToList());
        }

        [HttpGet("{id:int}")]
        public IActionResult Get([FromRoute] int id)
        {
            Recipe? recipe = _context.Recipes.Find(id);

            if (recipe == null)
                return NotFound();

            return Ok(recipe);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Recipe newRecipe)
        {
            _context.Recipes.Add(newRecipe);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = newRecipe.Id }, newRecipe);
        }

        [HttpPut("{id:int}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Recipe updatedRecipe)
        {
            if (id != updatedRecipe.Id)
                return BadRequest(); 

            var exist = _context.Recipes.Any(r => r.Id == id);
            if (!exist)
                return NotFound(); 

            _context.Recipes.Update(updatedRecipe);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult Patch([FromRoute] int id, [FromBody] RecipePatch updatedRecipe)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe == null) 
                return NotFound();

            if(updatedRecipe.Calories>0)
                recipe.Calories = updatedRecipe.Calories;

            if(updatedRecipe.IsVegetarian == false || updatedRecipe.IsVegetarian == true) 
                recipe.IsVegetarian = Convert.ToBoolean(updatedRecipe.IsVegetarian);

            if(!string.IsNullOrEmpty(updatedRecipe.Name))
                recipe.Name = updatedRecipe.Name;

            if(!string.IsNullOrEmpty(updatedRecipe.Cuisine))
                recipe.Cuisine = updatedRecipe.Cuisine;

            if(updatedRecipe.CookingTime>0)
                recipe.CookingTime = updatedRecipe.CookingTime;

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var recipe = _context.Recipes.Find(id);

            if (recipe == null)
                return NotFound(); 

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return NoContent(); 
        }
    }
}