import { makeAutoObservable } from "mobx";

class ConfirmStore {
  isLoading = false;
  isLoaded = false;

  constructor() {
    makeAutoObservable(this);
  }

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default ConfirmStore;
