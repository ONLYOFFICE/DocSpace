import { makeAutoObservable, runInAction } from "mobx";
import OformsFilter from "@docspace/common/api/oforms/filter";
import { submitToGallery } from "@docspace/common/api/oforms";

class OformsStore {
  authStore;

  oformFiles = null;
  oformsFilter = OformsFilter.getDefault();
  gallerySelected = null;
  oformsIsLoading = false;

  submitToGalleryTileIsVisible = !localStorage.getItem(
    "submitToGalleryTileIsHidden"
  );

  constructor(authStore) {
    makeAutoObservable(this);

    this.authStore = authStore;
  }

  getOforms = async (filter = OformsFilter.getDefault()) => {
    const oformData = await this.authStore.getOforms(filter);
    const oformsFilter = oformData?.data?.meta?.pagination;
    const newOformsFilter = this.oformsFilter.clone();

    if (oformsFilter) {
      newOformsFilter.page = oformsFilter.page;
      newOformsFilter.total = oformsFilter.total;
    }

    runInAction(() => {
      this.setOformsFilter(newOformsFilter);
      this.setOformFiles(oformData?.data?.data ?? []);
    });
  };

  submitToFormGallery = async (file, formName, language, signal = null) => {
    const res = await submitToGallery(
      this.authStore.settingsStore.formGallery.uploadUrl,
      file,
      formName,
      language,
      signal
    );

    return res;
  };

  setOformFiles = (oformFiles) => {
    this.oformFiles = oformFiles;
  };

  setOformsFilter = (oformsFilter) => {
    this.oformsFilter = oformsFilter;
  };

  setGallerySelected = (gallerySelected) => {
    this.gallerySelected = gallerySelected;
    this.authStore.infoPanelStore.setSelection(gallerySelected);
  };

  setOformsIsLoading = (oformsIsLoading) => {
    this.oformsIsLoading = oformsIsLoading;
  };

  loadMoreForms = async () => {
    if (!this.hasMoreForms || this.oformsIsLoading) return;

    // console.log("loadMoreForms");

    this.setOformsIsLoading(true);

    const newOformsFilter = this.oformsFilter.clone();

    newOformsFilter.page += 1;
    this.setOformsFilter(newOformsFilter);

    const oformData = await this.authStore.getOforms(newOformsFilter);
    const newForms = oformData?.data?.data ?? [];

    runInAction(() => {
      this.setOformFiles([...this.oformFiles, ...newForms]);
      this.setOformsIsLoading(false);
    });
  };

  get hasGalleryFiles() {
    return this.oformFiles && !!this.oformFiles.length;
  }

  //   get oformFilesLength() {
  //     return this.oformFiles.length;
  //   }

  get oformsFilterTotal() {
    return this.oformsFilter.total;
  }

  get hasMoreForms() {
    return this.oformFiles.length < this.oformsFilterTotal;
  }

  hideSubmitToGalleryTile = () => {
    localStorage.setItem("submitToGalleryTileIsHidden", true);
    this.submitToGalleryTileIsVisible = false;
  };
}

export default OformsStore;
