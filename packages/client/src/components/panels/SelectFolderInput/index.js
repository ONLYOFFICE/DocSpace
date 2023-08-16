import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolder, getFolderPath } from "@docspace/common/api/files";
import toastr from "@docspace/components/toast/toastr";
import SelectFolderDialog from "../SelectFolderDialog";
import SimpleFileInput from "../../SimpleFileInput";
import { withTranslation } from "react-i18next";
import { FolderType } from "@docspace/common/constants";
class SelectFolderInput extends React.PureComponent {
  constructor(props) {
    super(props);
    const { id, withoutBasicSelection, setIsLoading } = this.props;

    const isNeedLoadPath = !!id || !withoutBasicSelection;
    setIsLoading(isNeedLoadPath);

    this._isMount = false;
  }

  componentDidMount() {
    this._isMount = true;

    const { setFirstLoad } = this.props;

    setFirstLoad(false);
  }

  componentWillUnmount() {
    this._isMount = false;
    this.props.toDefault();
  }
  setFolderPath = async (folderId) => {
    const foldersArray = await getFolderPath(folderId);

    if (foldersArray[0].rootFolderType === FolderType.SHARE) {
      return "";
    }
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
    const { setIsLoading, setNewFolderPath, setIsPathError } = this.props;
    let timerId = setTimeout(() => {
      setIsLoading(true);
    }, 500);

    try {
      const convertFoldersArray = await this.setFolderPath(folderId);

      setNewFolderPath(convertFoldersArray);

      clearTimeout(timerId);
      timerId = null;
      setIsPathError(false);
      setIsLoading(false);
    } catch (e) {
      toastr.error(e);
      clearTimeout(timerId);
      setIsLoading(false);
      timerId = null;
      setIsPathError(true);
    }
  };
  onSetBaseFolderPath = async (folderId) => {
    const {
      setIsLoading,
      setBaseFolderPath,
      setIsPathError,
      setResultingFolderId,
      resultingFolderTree,
    } = this.props;

    try {
      const convertFoldersArray = await this.setFolderPath(folderId);
      setBaseFolderPath(convertFoldersArray);
      setIsLoading(false);
      if (!convertFoldersArray) {
        setIsPathError(true);

        this.onSetFolderInfo(resultingFolderTree[0].id);
        setResultingFolderId(resultingFolderTree[0].id);
      }
    } catch (e) {
      toastr.error(e);
      setIsLoading(false);
      setIsPathError(true);
      this.onSetFolderInfo(resultingFolderTree[0].id);
      setResultingFolderId(resultingFolderTree[0].id);
    }
  };
  onSelectFolder = (folderId) => {
    const { onSelectFolder: onSelect } = this.props;
    if (!this._isMount) return;
    this.onSetFolderInfo(folderId);
    onSelect && onSelect(folderId);
  };
  onSetFolderInfo = (folderId) => {
    const { setExpandedPanelKeys, setParentId, clearLocalStorage } = this.props;

    getFolder(folderId)
      .then((data) => {
        const pathParts = data.pathParts.map((item) => item.toString());
        pathParts?.pop();

        setExpandedPanelKeys(pathParts);
        setParentId(data.current.parentId);
      })
      .catch((e) => {
        toastr.error(e);
        clearLocalStorage();
      });
  };

  render() {
    const {
      onClickInput,
      isError,
      t,
      placeholder,
      maxInputWidth,
      isDisabled,
      isPanelVisible,

      theme,
      isFolderTreeLoading = false,
      onSelectFolder,
      isLoading,
      newFolderPath,
      baseFolderPath,
      isPathError,
      isWaitingUpdate,
      ...rest
    } = this.props;

    const isReady = !isFolderTreeLoading && !isWaitingUpdate;

    return (
      <StyledComponent maxWidth={maxInputWidth}>
        <SimpleFileInput
          theme={theme}
          className="select-folder_file-input"
          textField={newFolderPath || baseFolderPath}
          isError={isError || isPathError}
          onClickInput={onClickInput}
          placeholder={placeholder}
          isDisabled={isFolderTreeLoading || isDisabled || isLoading}
        />
        {isReady && (
          <SelectFolderDialog
            {...rest}
            selectFolderInputExist
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
  filteredType: PropTypes.oneOf([
    "exceptSortedByTags",
    "exceptPrivacyTrashArchiveFolders",
    "roomsOnly",
    "userFolderOnly",
    "",
  ]),
};

SelectFolderInput.defaultProps = {
  hasError: false,
  isDisabled: false,
  isWaitingUpdate: false,
  placeholder: "",
  filteredType: "",
};

export default inject(
  ({
    filesStore,
    treeFoldersStore,
    selectFolderDialogStore,
    selectedFolderStore,
    backup,
  }) => {
    const { clearLocalStorage } = backup;
    const { setFirstLoad } = filesStore;
    const { setExpandedPanelKeys } = treeFoldersStore;
    const {
      isLoading,
      setIsLoading,
      baseFolderPath,
      setBaseFolderPath,
      newFolderPath,
      setNewFolderPath,
      toDefault,
      setIsPathError,
      isPathError,
      resultingFolderTree,
      setResultingFolderId,
    } = selectFolderDialogStore;
    const { setParentId } = selectedFolderStore;

    return {
      clearLocalStorage,
      setFirstLoad,
      setExpandedPanelKeys,
      setParentId,
      isLoading,
      setIsLoading,
      setBaseFolderPath,
      baseFolderPath,
      newFolderPath,
      setNewFolderPath,
      toDefault,
      setIsPathError,
      isPathError,
      resultingFolderTree,
      setResultingFolderId,
    };
  }
)(observer(withTranslation("Translations")(SelectFolderInput)));
