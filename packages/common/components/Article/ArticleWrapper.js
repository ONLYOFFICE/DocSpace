import React, { useEffect } from "react";
import { Provider as MobxProvider } from "mobx-react";
import store from "client/store";
import Article from "./";

const { auth: authStore } = store;

const ArticleWrapper = (props) => {
  useEffect(() => {
    authStore.init();
  }, []);

  return (
    <MobxProvider {...store}>
      <Article {...props} />
    </MobxProvider>
  );
};

export default ArticleWrapper;
