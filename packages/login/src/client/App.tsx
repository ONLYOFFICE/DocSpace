import React from "react";
import Login from "./components/Login";
import { Switch, Route } from "react-router-dom";
import InvalidRoute from "./components/Invalid";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = (props) => {
  return (
    <Switch>
      <Route path="/login" exact>
        <Login {...props} />
      </Route>
      <Route path="/login/error=:error">
        <InvalidRoute />
      </Route>
    </Switch>
  );
};

export default App;
