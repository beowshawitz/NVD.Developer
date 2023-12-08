using System.ComponentModel.DataAnnotations;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Core
{
    public class AppPageResult
	{
		public int Start { get; set; } = 0;
		public int PageSize { get; set; }
		public int ItemCount { get; set; } = 0;

		public IEnumerable<Application> Applications { get; set; }

		public AppPageResult()
		{ 
			Applications = new List<Application>();
		}
		public AppPageResult(int pageSize, int start)
		{
			Applications = new Application[0];
			PageSize = pageSize;
			Start = start;
		}

		public int TotalPages
		{
			get { return CalculateTotalPages(); }
		}

		public int CurrentPage
		{
			get
			{
				return CalculateCurrentPage();
			}
		}

		public bool IsFirstPage
		{
			get
			{
				return DetermineIfFirstPage();
			}
		}

		public bool IsLastPage
		{
			get
			{
				return DetermineIfLastPage();
			}
		}

		private int CalculateCurrentPage()
		{
			if(PageSize <= 0) 
			{ 
				return 0;	
			}
            else
            {
				return (Start / PageSize) + 1;
			}            
		}

		private int CalculateTotalPages()
		{
			if (ItemCount <= 0 || PageSize <= 0)
			{
				return 0;
			}
			else
			{
				return (int)Math.Ceiling((decimal)ItemCount / (decimal)PageSize);
			}
		}

		private bool DetermineIfFirstPage()
		{
			if (PageSize <= 0)
			{
				return true;
			}
			else
			{
				return (Start - 1) % PageSize <= 0;
			}
		}

		private bool DetermineIfLastPage()
		{
			if (ItemCount <= 0 || PageSize <= 0)
			{
				return true;
			}
			else
			{
				return CurrentPage.Equals(TotalPages);
			}			
		}
	}
}
