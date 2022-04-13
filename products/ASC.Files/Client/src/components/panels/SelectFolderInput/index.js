import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolderPath } from "@appserver/common/api/files";
import toastr from "@appserver/components/toast/toastr";
import SelectFolderDialog from "../SelectFolderDialog";
import SimpleFileInput from "../../SimpleFileInput";
import { withTranslation } from "react-i18next";
class SelectFolderInput extends React.PureComponent {
  constructor(props) {
    super(props);
    const { id, foldersType } = this.props;

    const isNeedLoader =
      !!id || foldersType !== "third-party" || foldersType === "common";

    this.state = {
      isLoading: isNeedLoader,
      baseFolderPath: "",
      newFolderPath: "",
      resultingFolderTree: [],
      baseId: "",
    };
  }
  async componentDidMount() {
    const { setFirstLoad } = this.props;

    setFirstLoad(false);
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset } = this.props;
    const { newFolderPath, baseFolderPath } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      newFolderPath &&
        this.setState({
          baseFolderPath: newFolderPath,
        });
    }

    if (isReset && isReset !== prevProps.isReset) {
      this.setState({
        newFolderPath: baseFolderPath,
      });
    }
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

      this.setState({
        newFolderPath: convertFoldersArray,
        isLoading: false,
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

  onSetLoadingInput = (isLoading) => {
    this.setState({
      isLoading,
    });
  };
  render() {
    const { isLoading, baseFolderPath, newFolderPath, baseId } = this.state;
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
      ...rest
    } = this.props;

    const passedId = baseId !== id && id ? id : baseId;

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

        {!isFolderTreeLoading && (
          <SelectFolderDialog
            {...rest}
            id={passedId}
            isPanelVisible={isPanelVisible}
            onSetBaseFolderPath={this.onSetBaseFolderPath}
            onSetNewFolderPath={this.onSetNewFolderPath}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFolderInput.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  isDisabled: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFolderInput.defaultProps = {
  hasError: false,
  isDisabled: false,
  placeholder: "",
};

export default inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(withTranslation("Translations")(SelectFolderInput)));
