import { Provider } from "react-redux";
import store from "studio/store";
import Frame from "studio/frame";

import "./custom.scss";

const PeoplePage = () => (
  <Provider store={store}>
    <Frame page="people" />
  </Provider>
);

ReactDOM.render(<PeoplePage />, document.getElementById("app"));