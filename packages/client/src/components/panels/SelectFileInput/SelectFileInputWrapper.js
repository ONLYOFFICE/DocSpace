import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import stores from "../../../store/index.Files";
import store from "client/store";
import SelectFileInput from "./index";
import i18n from "./i18n";
const { auth: authStore } = store;

const SelectFileModalWrapper = (props) => <SelectFileInput {...props} />;

class SelectFileInputWrapper extends React.Component {
  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFileModalWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileInputWrapper;
