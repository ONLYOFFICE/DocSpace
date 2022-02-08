import { makeAutoObservable } from "mobx";

class NotificationsStore {
    isVisible = false;

    constructor() {
        makeAutoObservable(this);
    }

    toggleIsVisible = () => {
        isVisible = !this.isVisible;
    };
}

export default NotificationsStore;
