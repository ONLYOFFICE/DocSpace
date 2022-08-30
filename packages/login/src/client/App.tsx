import React from "react";
import Login from "./components/Login";
import { Switch, Route } from "react-router-dom";
import InvalidRoute from "./components/Invalid";
import CodeLogin from "./components/CodeLogin";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = (props) => {
  return (
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
  );
};

export default App;
