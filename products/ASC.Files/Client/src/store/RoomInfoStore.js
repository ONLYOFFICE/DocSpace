import { makeAutoObservable } from "mobx";

class RoomInfoStore {
    isVisible = false;

    constructor() {
        makeAutoObservable(this);
    }

    toggleIsVisible = () => {
        this.isVisible = !this.isVisible;
    };

    get isVisible() {
        return this.isVisible;
    }
}

export default RoomInfoStore;
