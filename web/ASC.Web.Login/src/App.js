import React from "react";
import { Provider } from "react-redux";
import store from "studio/store";
import Shell from "studio/shell";

const App = () => {
  return (
    <Provider store={store}>
      <Shell />
    </Provider>
  );
};

export default App;
