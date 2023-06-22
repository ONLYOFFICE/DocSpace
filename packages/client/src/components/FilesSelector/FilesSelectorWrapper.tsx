import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
// @ts-ignore
import store from "client/store";
import FilesSelector from "./";
import i18n from "./i18n";
// @ts-ignore
import { getFolder } from "@docspace/common/api/files";
import { FilesSelectorProps } from "./FilesSelector.types";
const { auth: authStore } = store;

const FilesSelectorWrapper = (props: FilesSelectorProps) => {
  React.useEffect(() => {
    authStore.init(true);
  }, []);

  console.log(props);

  return (
    <MobxProvider {...store}>
      <I18nextProvider i18n={i18n}>
        <FilesSelector {...props} />
      </I18nextProvider>
    </MobxProvider>
  );
};

export default FilesSelectorWrapper;
