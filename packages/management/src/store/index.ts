import { createContext, useContext } from "react";

import authStore from "@docspace/common/store/AuthStore";

import BrandingStore from "./BrandingStore";

export class RootStore {
  authStore = authStore;
  brandingStore = new BrandingStore();
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
