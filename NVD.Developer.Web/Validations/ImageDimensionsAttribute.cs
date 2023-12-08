using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace NVD.Developer.Web.Validations
{
	public class ImageDimensionsAttribute : ValidationAttribute
	{
		private readonly int _maxWidth;
		private readonly int _maxHeight;
		public ImageDimensionsAttribute(int maxWidth, int maxHeight)
		{
			_maxWidth = maxWidth;
			_maxHeight = maxHeight;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;
			if (file != null)
			{
				using (Image myImage = Image.FromStream(file.OpenReadStream()))
				{
					if (myImage.Height > _maxHeight || myImage.Width > _maxWidth)
					{
						return new ValidationResult(GetErrorMessage());
					}
				}				
			}

			return ValidationResult.Success;
		}

		public string GetErrorMessage()
		{
			return $"Expected file dimensions are width <= {_maxWidth}px and height <= {_maxHeight}px.";
		}
	}
}
