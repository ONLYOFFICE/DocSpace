import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import SelectFileDialog from "./index";
import store from "SRC_DIR/store";
import i18n from "./i18n";
const { auth: authStore } = store;

class SelectFileDialogBody extends React.Component {
  componentDidMount() {
    const { settings, setFilesSettings } = this.props;
    settings && setFilesSettings(settings); // Remove after initialization settings in Editor
  }

  render() {
    return <SelectFileDialog {...this.props} />;
  }
}
const SelectFileWrapper = inject(({ settingsStore }) => {
  const { setFilesSettings } = settingsStore;

  return {
    setFilesSettings,
  };
})(observer(SelectFileDialogBody));

class SelectFileDialogWrapper extends React.Component {
  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <SelectFileWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileDialogWrapper;
