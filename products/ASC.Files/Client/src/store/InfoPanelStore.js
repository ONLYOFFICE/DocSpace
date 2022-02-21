import { makeAutoObservable } from "mobx";

class InfoPanelStore {
    isVisible = false;

    constructor() {
        makeAutoObservable(this);
    }

    toggleIsVisible = () => {
        this.isVisible = !this.isVisible;
    };

    setVisible = () => {
        this.isVisible = true;
    };

    setIsVisible = (bool) => {
        this.isVisible = bool;
    };

    get isVisible() {
        return this.isVisible;
    }
}

export default InfoPanelStore;
