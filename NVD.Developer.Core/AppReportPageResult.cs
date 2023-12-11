using System.ComponentModel.DataAnnotations;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Core
{
    public class AppReportPageResult
	{
		public int Start { get; set; } = 0;
		public int PageSize { get; set; }
		public int ItemCount { get; set; } = 0;

		public IEnumerable<ApplicationReport> Reports { get; set; }

		public AppReportPageResult()
		{
			Reports = new List<ApplicationReport>();
		}
		public AppReportPageResult(int pageSize, int start)
		{
			Reports = new ApplicationReport[0];
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
