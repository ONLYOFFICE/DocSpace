import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import SelectFileDialog from "../SelectFileDialog";
import StyledComponent from "./StyledSelectFileInput";
import SimpleFileInput from "../../SimpleFileInput";

class SelectFileInput extends React.PureComponent {
  constructor(props) {
    super(props);

    const { setExpandedPanelKeys, setFolderId, setFile } = props;

    setExpandedPanelKeys(null);
    setFolderId(null);
    setFile(null);
  }

  componentDidMount() {
    this.props.setFirstLoad(false);
  }

  render() {
    const {
      onClickInput,
      hasError,
      t,
      placeholder,
      maxInputWidth,
      maxFolderInputWidth,
      isPanelVisible,
      isDisabled,
      isError,
      fileName,
      folderId,
      ...rest
    } = this.props;

    return (
      <StyledComponent maxInputWidth={maxInputWidth}>
        <SimpleFileInput
          className="select-file_file-input"
          textField={fileName}
          isDisabled={isDisabled}
          isError={isError}
          onClickInput={onClickInput}
        />

        {isPanelVisible && (
          <SelectFileDialog
            {...rest}
            id={folderId}
            isPanelVisible={isPanelVisible}
            onSetFileNameAndLocation={this.onSetFileNameAndLocation}
            maxInputWidth={maxFolderInputWidth}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFileInput.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFileInput.defaultProps = {
  hasError: false,
  placeholder: "",
};

export default inject(
  ({ filesStore, treeFoldersStore, selectFileDialogStore }) => {
    const { setFirstLoad } = filesStore;
    const { folderId, setFolderId, setFile, fileInfo } = selectFileDialogStore;
    const fileName = fileInfo?.title;
    const { setExpandedPanelKeys } = treeFoldersStore;
    return {
      setFirstLoad,
      setFolderId,
      setFile,
      fileInfo,
      folderId,
      fileName,
      setExpandedPanelKeys,
    };
  }
)(observer(SelectFileInput));
