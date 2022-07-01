import React, {useState, useEffect} from 'react';

import { Header } from './header/Header';
import { Article } from './article/Article';
import { Section } from './section/Section';
import {Dark, Base} from './theme/index';
import { ThemeProvider } from "styled-components";

export const Layout = ({theme, name, origin, language}) => {

  const [visible, setVisible] = useState(true);

  const themes = {
    light: Base,
    dark: Dark
  }

  const onResize = () => {
      if (window.innerWidth < 1025) {
          setVisible(false);
      } else {
          setVisible(true);
      }
  }

  useEffect(() => {
      window.addEventListener('resize', onResize)

      return () => window.removeEventListener('resize', onResize);
  }, [])

  return (
    <ThemeProvider theme={themes[theme || 'light']}>
      <Header />
      <div className='container' style={{display: "flex"}}>
        {visible && window.innerWidth > 1025 && <Article />}
        <Section name={name} origin={origin} lang={language} />
      </div>
    </ThemeProvider>
  );
};
