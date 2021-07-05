import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import i18n from "./i18n";
import stores from "../../../store/index";
import FileInputWithFolderPath from "./fileInputWithFolderPath";
import SelectFolderDialog from "../SelectFolderDialog/index";
import StyledComponent from "./styledSelectFolderInput";

let path = "";

class SelectFolder extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
      baseFolderPath: "",
      fullFolderPath: "",
      fullFolderPathDefault: "",
    };
    this._isMounted = false;
  }
  componentDidMount() {
    this._isMounted = true;
    const { folderPath } = this.props;

    if (folderPath.length !== 0) {
      this._isMounted &&
        this.setState({
          fullFolderPath: folderPath,
          fullFolderPathDefault: folderPath,
        });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }
  componentDidUpdate(prevProps) {
    const { isSetDefaultFolderPath, folderPath } = this.props;

    if (
      isSetDefaultFolderPath &&
      isSetDefaultFolderPath !== prevProps.isSetDefaultFolderPath
    ) {
      this.setState({
        fullFolderPath: this.state.fullFolderPathDefault,
      });
    }
    if (folderPath !== prevProps.folderPath) {
      this.setState({
        fullFolderPath: folderPath,
        fullFolderPathDefault: folderPath,
      });
    }
  }

  onSetFullPath = (pathName) => {
    this.setState({
      fullFolderPath: pathName,
    });
  };

  onSetBaseFolderPath = (pathName) => {
    this.setState({
      baseFolderPath: pathName,
    });
  };
  render() {
    const {
      name,
      onClickInput,
      isPanelVisible,
      withoutProvider,
      onClose,
      isError,
      isSavingProcess,
      isDisabled,
      onSelectFolder,
      onSetLoadingData,
      foldersType,
      folderPath,
      isNeedArrowIcon,
      isSetFolderImmediately,
    } = this.props;
    const { isLoading, baseFolderPath, fullFolderPath } = this.state;
    const zIndex = 310;

    return (
      <StyledComponent>
        <FileInputWithFolderPath
          name={name}
          className="input-with-folder-path"
          baseFolderPath={baseFolderPath}
          folderPath={fullFolderPath}
          isDisabled={isLoading || isSavingProcess || isDisabled}
          isError={isError}
          onClickInput={onClickInput}
        />

        <SelectFolderDialog
          zIndex={zIndex}
          isPanelVisible={isPanelVisible}
          onClose={onClose}
          folderPath={folderPath}
          onSelectFolder={onSelectFolder}
          onSetLoadingData={onSetLoadingData}
          foldersType={foldersType}
          withoutProvider={withoutProvider}
          onSetFullPath={this.onSetFullPath}
          onSetBaseFolderPath={this.onSetBaseFolderPath}
          onSetLoadingData={onSetLoadingData}
          isNeedArrowIcon={isNeedArrowIcon}
          isSetFolderImmediately={isSetFolderImmediately}
        />
      </StyledComponent>
    );
  }
}

SelectFolder.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
  onSelectFolder: PropTypes.func.isRequired,
  onSetLoadingData: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  name: PropTypes.string,
  withoutProvider: PropTypes.bool,
  isError: PropTypes.bool,
  isSavingProcess: PropTypes.bool,
  foldersType: PropTypes.oneOf(["common", "third-party"]),
};

SelectFolder.defaultProps = {
  withoutProvider: false,
  isDisabled: false,
  isError: false,
  folderPath: "",
};
const SelectFolderWrapper = withTranslation(["SelectFolder", "Common"])(
  SelectFolder
);

class SelectFolderModal extends React.Component {
  static setFullFolderPath = (foldersArray) => {
    path = "";
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
  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFolderWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFolderModal;
