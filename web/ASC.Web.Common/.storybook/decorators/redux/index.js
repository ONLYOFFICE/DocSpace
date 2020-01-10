import { Provider } from "react-redux";
import store from "./store";

const withProvider = story => <Provider store={store}>{story()}</Provider>;

export default withProvider;
