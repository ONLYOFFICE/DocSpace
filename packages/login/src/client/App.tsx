import React from "react";
import Login from "./components/Login";
import { Switch, Route } from "react-router-dom";
import InvalidRoute from "./components/Invalid";
import CodeLogin from "./components/CodeLogin";
import initLoginStore from "../store";
import { Provider as MobxProvider } from "mobx-react";
import SimpleNav from "../client/components/sub-components/SimpleNav";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = (props) => {
  const loginStore = initLoginStore(props.currentColorScheme);
  return (
    <MobxProvider {...loginStore}>
      <SimpleNav />
      <Switch>
        <Route path="/login/error">
          <InvalidRoute />
        </Route>
        <Route path="/login/code">
          <CodeLogin />
        </Route>
        <Route path="/login">
          <Login {...props} />
        </Route>
      </Switch>
    </MobxProvider>
  );
};

export default App;
