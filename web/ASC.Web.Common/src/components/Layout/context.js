import { createContext } from 'react'

const ThemeContext = createContext({})
export const ThemeContextProvider = ThemeContext.Provider
export const ThemeContextConsumer = ThemeContext.Consumer