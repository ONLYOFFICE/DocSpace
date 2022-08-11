import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolder, getFolderPath } from "@appserver/common/api/files";
import toastr from "@appserver/components/toast/toastr";
import SelectFolderDialog from "../SelectFolderDialog";
import SimpleFileInput from "../../SimpleFileInput";
import { withTranslation } from "react-i18next";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
class SelectFolderInput extends React.PureComponent {
  constructor(props) {
    super(props);
    const { id, foldersType, withoutBasicSelection } = this.props;

    const isNeedLoader =
      !!id || !withoutBasicSelection || foldersType === "common";

    this.state = {
      isLoading: isNeedLoader,
      baseFolderPath: "",
      newFolderPath: "",
      resultingFolderTree: [],
      baseId: "",
    };
    this._isMount = false;
  }

  setBaseInfo = async () => {
    const {
      treeFolders,
      foldersType,
      id,
      onSelectFolder,
      foldersList,
      withoutBasicSelection,
    } = this.props;

    let resultingFolderTree, resultingId;

    try {
      [
        resultingFolderTree,
        resultingId,
      ] = await SelectionPanel.getBasicFolderInfo(
        treeFolders,
        foldersType,
        id,
        this.onSetBaseFolderPath,
        onSelectFolder,
        foldersList,

        withoutBasicSelection
      );
    } catch (e) {
      toastr.error(e);
      this.setState({
        isLoading: false,
      });
      return;
    }

    this.setState({
      resultingFolderTree,
      baseId: resultingId,
      ...(withoutBasicSelection && { isLoading: false }),
    });
  };
  componentDidMount() {
    this._isMount = true;

    const { setFirstLoad, foldersType } = this.props;

    setFirstLoad(false);
    foldersType && this.setBaseInfo();
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset, id, foldersType } = this.props;
    const { newFolderPath, baseFolderPath } = this.state;

    if (!isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      newFolderPath &&
        this.setState({
          baseFolderPath: newFolderPath,
          baseId: id,
          newId: null,
        });
    }

    if (!isReset && isReset !== prevProps.isReset) {
      this.setState({
        newFolderPath: baseFolderPath,
        baseId: id,
        newId: null,
      });
    }

    if (foldersType !== prevProps.foldersType) {
      this.setBaseInfo();
    }
  }

  componentWillUnmount() {
    this._isMount = false;
  }
  setFolderPath = async (folderId) => {
    const foldersArray = await getFolderPath(folderId);

    const convertFolderPath = (foldersArray) => {
      let path = "";
      if (foldersArray.length > 1) {
        for (let item of foldersArray) {
          if (!path) {
            path = path + `${item.title}`;
          } else path = path + " " + "/" + " " + `${item.title}`;
        }
      } else {
        for (let item of foldersArray) {
          path = `${item.title}`;
        }
      }
      return path;
    };

    const convertFoldersArray = convertFolderPath(foldersArray);

    return convertFoldersArray;
  };
  onSetNewFolderPath = async (folderId) => {
    let timerId = setTimeout(() => {
      this.setState({ isLoading: true });
    }, 500);

    try {
      const convertFoldersArray = await this.setFolderPath(folderId);
      clearTimeout(timerId);
      timerId = null;

      this._isMount &&
        this.setState({
          newFolderPath: convertFoldersArray,
          isLoading: false,
          newId: folderId,
        });
    } catch (e) {
      toastr.error(e);
      clearTimeout(timerId);
      timerId = null;

      this.setState({
        isLoading: false,
      });
    }
  };

  onSetBaseFolderPath = async (folderId) => {
    try {
      const convertFoldersArray = await this.setFolderPath(folderId);

      this._isMount &&
        this.setState({
          baseFolderPath: convertFoldersArray,
          isLoading: false,
        });
    } catch (e) {
      toastr.error(e);
      this.setState({
        isLoading: false,
      });
    }
  };

  onSelectFolder = (folderId) => {
    const { onSelectFolder: onSelect } = this.props;

    this.onSetFolderInfo(folderId);

    onSelect && onSelect(folderId);
  };

  onSetFolderInfo = (folderId) => {
    const { setExpandedPanelKeys, setParentId } = this.props;
    getFolder(folderId)
      .then((data) => {
        const pathParts = data.pathParts.map((item) => item.toString());
        pathParts?.pop();
        setExpandedPanelKeys(pathParts);
        setParentId(data.current.parentId);
      })
      .catch((e) => toastr.error(e));
  };

  render() {
    const {
      isLoading,
      baseFolderPath,
      newFolderPath,
      baseId,
      resultingFolderTree,
      newId,
    } = this.state;
    const {
      onClickInput,
      isError,
      t,
      placeholder,
      maxInputWidth,
      isDisabled,
      isPanelVisible,
      id,
      theme,
      isFolderTreeLoading = false,
      onSelectFolder,
      ...rest
    } = this.props;

    const passedId = newId ? newId : baseId;

    return (
      <StyledComponent maxWidth={maxInputWidth}>
        <SimpleFileInput
          theme={theme}
          className="select-folder_file-input"
          textField={newFolderPath || baseFolderPath}
          isError={isError}
          onClickInput={onClickInput}
          placeholder={placeholder}
          isDisabled={isFolderTreeLoading || isDisabled || isLoading}
        />

        {!isFolderTreeLoading && isPanelVisible && (
          <SelectFolderDialog
            {...rest}
            withInput={true}
            folderTree={resultingFolderTree}
            id={passedId}
            isPanelVisible={isPanelVisible}
            onSetBaseFolderPath={this.onSetBaseFolderPath}
            onSetNewFolderPath={this.onSetNewFolderPath}
            onSelectFolder={this.onSelectFolder}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFolderInput.propTypes = {
  onClickInput: PropTypes.func,
  hasError: PropTypes.bool,
  isDisabled: PropTypes.bool,
  placeholder: PropTypes.string,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
    "rooms",
    "",
  ]),
};

SelectFolderInput.defaultProps = {
  hasError: false,
  isDisabled: false,
  placeholder: "",
  foldersType: "",
};

export default inject(
  ({
    filesStore,
    treeFoldersStore,
    selectFolderDialogStore,
    selectedFolderStore,
  }) => {
    const { setFirstLoad } = filesStore;
    const { treeFolders, setExpandedPanelKeys } = treeFoldersStore;
    const { setFolderId } = selectFolderDialogStore;
    const { setParentId } = selectedFolderStore;
    return {
      setFirstLoad,
      treeFolders,
      setFolderId,
      setExpandedPanelKeys,
      setParentId,
    };
  }
)(observer(withTranslation("Translations")(SelectFolderInput)));
