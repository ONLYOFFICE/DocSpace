import { createContext } from 'react'

const RefContext = createContext({})
const IsVisibleContext = createContext();
export const RefContextProvider = RefContext.Provider
export const IsVisibleContextProvider = IsVisibleContext.Provider
export const RefContextConsumer = RefContext.Consumer
export const IsVisibleContextConsumer = IsVisibleContext.Consumer