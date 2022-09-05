import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import store from "client/store";
import SelectFolderDialog from "./index";
import i18n from "./i18n";
import { getFolder } from "@docspace/common/api/files";
const { auth: authStore } = store;

class SelectFolderDialogBody extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      pathParts: [],
      parentId: null,
    };
  }

  componentDidMount() {
    const { setExpandedPanelKeys, setParentId, folderId } = this.props;

    if (folderId) {
      folderId &&
        getFolder(folderId)
          .then((data) => {
            const pathParts = data.pathParts.map((item) => item.toString());
            pathParts?.pop();

            setExpandedPanelKeys(pathParts);
            setParentId(data.current.parentId);

            this.setState({
              pathParts: pathParts,
              parentId: data.current.parentId,
            });
          })
          .catch((e) => toastr.error(e));
    }
  }
  componentDidUpdate(prevProps) {
    const { isPanelVisible, setParentId, setExpandedPanelKeys } = this.props;
    const { pathParts, parentId } = this.state;
    if (isPanelVisible && isPanelVisible !== prevProps.isPanelVisible) {
      setExpandedPanelKeys(pathParts);
      setParentId(parentId);
    }
  }

  render() {
    const { folderId, isPanelVisible } = this.props;
    const { pathParts, parentId } = this.state;
    return (
      isPanelVisible && (
        <SelectFolderDialog {...this.props} selectedId={folderId} />
      )
    );
  }
}
const SelectFolderModalWrapper = inject(
  ({ treeFoldersStore, selectedFolderStore }) => {
    const { setExpandedPanelKeys } = treeFoldersStore;
    const { setParentId } = selectedFolderStore;
    return {
      setExpandedPanelKeys,
      setParentId,
    };
  }
)(observer(SelectFolderDialogBody));

class SelectFolderModal extends React.Component {
  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <SelectFolderModalWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFolderModal;
