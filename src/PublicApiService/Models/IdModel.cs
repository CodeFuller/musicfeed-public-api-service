using System;

namespace PublicApiService.Models
{
	public record IdModel
	{
		public string Value { get; }

		public IdModel(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Id is null or empty", nameof(value));
			}

			Value = value;
		}
	}
}
