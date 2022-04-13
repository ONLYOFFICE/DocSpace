import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import { getFolder } from "@appserver/common/api/files";
import SelectFolderDialogAsideView from "./AsideView";
import utils from "@appserver/components/utils";
import toastr from "studio/toastr";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import { FilterType } from "@appserver/common/constants";

const { desktop } = utils.device;

class SelectFolderDialog extends React.Component {
  constructor(props) {
    super(props);
    const { id, displayType, filter } = this.props;
    this.newFilter = filter.clone();
    this.newFilter.filterType = FilterType.FilesOnly;

    this.state = {
      isLoadingData: false,
      isInitialLoader: false,
      folderId: id ? id : "",
      displayType: displayType || this.getDisplayType(),
      canCreate: true,
      isAvailable: true,
      filesList: [],

      isNextPageLoading: false,
      page: 0,
      hasNextPage: true,
      files: [],
      expandedKeys: null,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }

  async componentDidMount() {
    const {
      treeFolders,
      foldersType,
      id,
      onSetBaseFolderPath,
      onSelectFolder,
      foldersList,
      displayType,
      isNeedArrowIcon = false,
      folderTree,
    } = this.props;

    !displayType && window.addEventListener("resize", this.throttledResize);

    this.expandedKeys = this.props.expandedKeys?.map((item) => item.toString());

    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    let resultingFolderTree, resultingId;

    if (!isNeedArrowIcon) {
      try {
        [
          resultingFolderTree,
          resultingId,
        ] = await SelectionPanel.getBasicFolderInfo(
          treeFolders,
          foldersType,
          id,
          onSetBaseFolderPath,
          onSelectFolder,
          foldersList,
          true
        );

        clearTimeout(timerId);
        timerId = null;
      } catch (e) {
        toastr.error(e);

        clearTimeout(timerId);
        timerId = null;
        this.setState({ isInitialLoader: false });

        return;
      }
    }

    const tree = isNeedArrowIcon ? folderTree : resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder(null);
      return;
    }
    const resId = isNeedArrowIcon ? id : resultingId;

    onSelectFolder && onSelectFolder(resId);
    isNeedArrowIcon && onSetBaseFolderPath(resId);

    this.setState({
      resultingFolderTree: tree,
      isInitialLoader: false,
      expandedKeys: this.expandedKeys ? this.expandedKeys : null,
      folderId: resId,
    });
  }

  componentDidUpdate(prevProps) {
    const { isReset } = this.props;

    if (isReset && isReset !== prevProps.isReset) {
      this.onResetInfo();
    }
  }

  componentWillUnmount() {
    const { setExpandedPanelKeys } = this.props;
    setExpandedPanelKeys(null);

    clearTimeout(this.timerId);
    this.timerId = null;

    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }
  }
  getDisplayType = () => {
    const displayType =
      window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";

    return displayType;
  };

  setDisplayType = () => {
    const displayType = this.getDisplayType();

    this.setState({ displayType: displayType });
  };

  onSelect = async (folder, treeNode) => {
    const { onSetFolderInfo } = this.props;
    const { folderId } = this.state;

    if (+folderId === +folder[0]) return;

    this.setState({
      folderId: folder[0],
      files: [],
      hasNextPage: true,
      page: 0,
    });

    onSetFolderInfo && onSetFolderInfo(folder, treeNode);
  };

  onButtonClick = (e) => {
    const {
      onClose,
      onSave,
      onSetNewFolderPath,
      onSelectFolder,
      withoutImmediatelyClose,
    } = this.props;
    const { folderId } = this.state;

    onSave && onSave(e, folderId);
    onSetNewFolderPath && onSetNewFolderPath(folderId);
    onSelectFolder && onSelectFolder(folderId);

    !withoutImmediatelyClose && onClose && onClose();
  };

  onResetInfo = async () => {
    const { id } = this.props;

    const pathParts = await SelectionPanel.getFolderPath(id);

    this.setState({
      folderId: id,
      expandedKeys: pathParts,
    });
  };

  _loadNextPage = () => {
    const { files, page, folderId, expandedKeys } = this.state;

    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, async () => {
      try {
        const data = await getFolder(folderId, this.newFilter);
        const convertedPathParts =
          page === 0
            ? data.pathParts.map((item) => item.toString())
            : expandedKeys;

        const finalData = [...data.files];
        const newFilesList = [...files].concat(finalData);
        const hasNextPage = newFilesList.length < data.total - 1;
        this._isLoadNextPage = false;
        this.setState((state) => ({
          isDataLoading: false,
          hasNextPage: hasNextPage,
          isNextPageLoading: false,
          page: state.page + 1,
          files: newFilesList,
          ...(page === 0 && {
            folderTitle: data.current.title,
            expandedKeys: convertedPathParts,
          }),
        }));
      } catch (e) {
        toastr.error(e);
        this.setState({
          isDataLoading: false,
        });
      }
    });
  };
  render() {
    const {
      t,
      theme,
      isPanelVisible,
      zIndex,
      onClose,
      withoutProvider,
      isNeedArrowIcon,
      header,
      dialogName,
      footer,
      buttonName,
      isDisableTree,
    } = this.props;
    const {
      folderId,
      displayType,
      canCreate,
      isLoadingData,
      isAvailable,
      resultingFolderTree,
      filesList,

      hasNextPage,
      isNextPageLoading,
      files,
      page,
      folderTitle,
      expandedKeys,
    } = this.state;

    const primaryButtonName = buttonName
      ? buttonName
      : t("Common:SaveHereButton");
    const name = dialogName ? dialogName : t("Common:SaveButton");

    return displayType === "aside" ? (
      <SelectFolderDialogAsideView
        theme={theme}
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        isNeedArrowIcon={isNeedArrowIcon}
        certainFolders={true}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onSelectFolder={this.onSelect}
        onButtonClick={this.onButtonClick}
        header={header}
        dialogName={isNeedArrowIcon ? t("Translations:FolderSelection") : name}
        footer={footer}
        canCreate={canCreate}
        isLoadingData={isLoadingData}
        primaryButtonName={
          isNeedArrowIcon ? t("Common:Select") : primaryButtonName
        }
        isAvailable={isAvailable}
        isDisableTree={isDisableTree}
      />
    ) : (
      <SelectionPanel
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        withoutProvider={withoutProvider}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onButtonClick={this.onButtonClick}
        header={header}
        dialogName={name}
        footer={footer}
        canCreate={canCreate}
        isLoadingData={isLoadingData}
        primaryButtonName={primaryButtonName}
        isAvailable={isAvailable}
        onSelectFolder={this.onSelect}
        filesList={filesList}
        isNextPageLoading={isNextPageLoading}
        page={page}
        hasNextPage={hasNextPage}
        files={files}
        loadNextPage={this._loadNextPage}
        folderTitle={folderTitle}
        expandedKeys={expandedKeys}
        isDisableTree={isDisableTree}
        folderSelection
      />
    );
  }
}

SelectFolderDialog.propTypes = {
  onSelectFolder: PropTypes.func,
  onClose: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
  ]),
  displayType: PropTypes.oneOf(["aside", "modal"]),
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
  withoutImmediatelyClose: PropTypes.bool,
  isDisableTree: PropTypes.bool,
};
SelectFolderDialog.defaultProps = {
  id: "",
  withoutProvider: false,
  withoutImmediatelyClose: false,
  isDisableTree: false,
};

export default inject(
  ({
    treeFoldersStore,
    selectedFolderStore,
    selectedFilesStore,
    filesStore,
    auth,
  }) => {
    const {
      treeFolders,
      expandedPanelKeys,
      setExpandedPanelKeys,
    } = treeFoldersStore;

    const { filter } = filesStore;
    const { id } = selectedFolderStore;
    const { setFolderId } = selectedFilesStore;

    const { settingsStore } = auth;
    const { theme } = settingsStore;
    return {
      theme: theme,
      storeFolderId: id,
      setExpandedPanelKeys,
      setFolderId,
      treeFolders,
      filter,
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
    };
  }
)(
  observer(
    withTranslation(["SelectFolder", "Common", "Translations"])(
      SelectFolderDialog
    )
  )
);
