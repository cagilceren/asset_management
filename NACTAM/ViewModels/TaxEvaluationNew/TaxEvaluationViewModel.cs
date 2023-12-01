using NACTAM.Controllers;
using NACTAM.Models;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;

namespace NACTAM.ViewModels.TaxEvaluationNew {
	/// <summary>
	/// Viewmodel for displaying Tax evaluations
	///
	/// this is basically a complete rewrite of TaxEvaluationViewModel
	///
	/// author: Tuan Bui
	/// </summary>
	public class TaxEvaluationViewModelNew {
		/// <summary>
		/// summary of all the total values within the year
		/// </summary>
		public EvaluationPerCurrency Total { get; set; }
		/// <summary>
		/// summary of all the taxable total values within the year
		/// </summary>
		public Dictionary<string, EvaluationPerCurrency> TotalByCurrency { get; set; }

		/// <summary>
		/// current year
		/// </summary>
		public int Year = DateTime.Now.Year;

		/// <summary>
		/// all transactions
		/// </summary>
		public List<TransactionsViewModel> Transactions;

		/// <summary>
		/// gives back a list of years where a sell transaction was done
		/// </summary>
		///
		public IEnumerable<int> PossibleYears {
			get =>
			Transactions.Select(x => x.Date.Year).Distinct().OrderByDescending(x => x);
		}

		/// <summary>
		/// user this belongs to
		/// </summary>
		public User User;
	}

	public class EvaluationPerCurrency {
		/// <summary>
		/// total summary
		/// </summary>
		public SingleSummaryViewModel Total;
		/// <summary>
		/// taxed summary
		/// </summary>
		public SingleSummaryViewModel TotalTaxed;
		/// <summary>
		/// unsold transactions
		/// </summary>
		public List<AfterSaleBuyTransactionViewModel> Unsold;
		/// <summary>
		/// sold transactions
		/// </summary>
		public List<AfterSaleSellTransactionViewModel> Sold;

		/// <summary>
		/// is true, when there is at least on transaction involved
		/// </summary>
		public bool Exists { get => (Unsold.Count + Sold.Count) > 0; }

		/// <summary>
		/// gets the logo
		/// </summary>
		public string? Logo { get => Total.Logo; }
	}

	/// <summary>
	/// contains aggregate information
	///
	/// author: Tuan Bui
	/// </summary>
	public class SingleSummaryViewModel {
		/// <summary>
		/// the sum of all profitable gains
		/// </summary>
		public decimal Profits { get => ProfitsFromOther + ProfitsFromSelling; }
		/// <summary>
		/// the sum of all unprofitable gains
		/// </summary>
		public decimal Losses { get; set; }
		/// <summary>
		/// sum of all sales
		/// </summary>
		public decimal Income { get; set; }
		/// <summary>
		/// sum of all buy transactions
		/// </summary>
		public decimal Expenditures { get; set; }
		/// <summary>
		/// total fees
		/// </summary>
		public decimal Fees { get; set; }
		/// <summary>
		/// total profit from selling
		/// </summary>
		public decimal ProfitsFromSelling { get; set; }
		/// <summary>
		/// total sales from mining
		/// </summary>
		public decimal ProfitsFromMining { get; set; }
		/// <summary>
		/// total sales from staking
		/// </summary>
		public decimal ProfitsFromStaking { get; set; }
		/// <summary>
		/// total sales from anything other than Selling
		/// </summary>
		public decimal ProfitsFromOther { get => ProfitsFromMining + ProfitsFromStaking; }
		/// <summary>
		/// tax rule to be followed
		/// </summary>
		private readonly ITaxRule _taxRule;
		/// <summary>
		/// remaining possible tax free profits sales
		///
		/// applying RELU to avoid negative values
		/// </summary>
		public decimal RemainingTaxFreeSales { get => Math.Max(0M, _taxRule.CalculateSaleProfitAllowanceLimit(ProfitsFromSelling + Losses)); }
		/// <summary>
		/// remaining possible tax free profits form other sources
		/// </summary>
		public decimal RemainingTaxFreeOther { get => Math.Max(0M, _taxRule.CalculateOthersProfitAllowanceLimit(ProfitsFromOther)); }
		/// <summary>
		/// The logo to be used
		/// </summary>
		public string? Logo { get; set; }


		/// <summary>
		/// Empty constructor for binding the viewmodel
		/// </summary>
		public SingleSummaryViewModel() { }

		/// <summary>
		/// usable constructor
		/// </summary>
		/// <param name="taxRule">Tax rule to be used</param>
		public SingleSummaryViewModel(ITaxRule taxRule) {
			_taxRule = taxRule;
		}
	}
}
