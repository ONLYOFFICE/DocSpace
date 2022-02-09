import { makeAutoObservable } from "mobx";

class RoomInfoStore {
    isVisible = false;

    constructor() {
        makeAutoObservable(this);
    }

    toggleIsVisible = () => {
        isVisible = !this.isVisible;
    };
}

export default RoomInfoStore;
