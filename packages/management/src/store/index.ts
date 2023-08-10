import { createContext, useContext } from "react";

import SpacesStore from "./SpacesStore";

import store from "client/store";
const { auth: authStore } = store;

export class RootStore {
  authStore = authStore;
  spacesStore = new SpacesStore(this.authStore);
}

export const RootStoreContext = createContext<RootStore | null>(null);

export const useStore = () => {
  const context = useContext(RootStoreContext);
  if (context === null) {
    throw new Error(
      "You have forgotten to wrap your root component with RootStoreProvider"
    );
  }
  return context;
};
