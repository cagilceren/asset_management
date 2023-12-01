using NACTAM.ViewModels;

namespace NACTAM.ViewModels {

	/// <summary>
	/// AssetsOverviewModel holds List of AssetViewModels
	/// Every Currency has its own AssetViewModel
	/// Authornames: Philipp Eckel
	/// </summary>
	public class AssetsOverviewModel {

		public List<AssetsViewModel> Assets { get; set; }

		public AssetsOverviewModel(List<AssetsViewModel> assets) {
			Assets = assets;
		}
	}
}