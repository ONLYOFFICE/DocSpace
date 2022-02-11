import { makeAutoObservable } from "mobx";

class InfoPanelStore {
    isVisible = false;

    constructor() {
        makeAutoObservable(this);
    }

    toggleIsVisible = () => {
        this.isVisible = !this.isVisible;
    };

    setIsVisible = (bool) => {
        this.isVisible = bool;
    };

    get isVisible() {
        return this.isVisible;
    }
}

export default InfoPanelStore;
