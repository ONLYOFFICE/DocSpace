import { Provider } from "react-redux";
import store from "studio/store";
import Frame from "studio/frame";

import "./custom.scss";

const FilesPage = () => (
  <Provider store={store}>
    <Frame page="files" />
  </Provider>
);

ReactDOM.render(<FilesPage />, document.getElementById("app"));
