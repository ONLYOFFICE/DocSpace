import { createContext } from "react";

const LayoutContext = createContext({});

export const LayoutContextProvider = LayoutContext.Provider;
export const LayoutContextConsumer = LayoutContext.Consumer;
