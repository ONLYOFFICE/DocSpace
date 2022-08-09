import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import SelectFileDialog from "../SelectFileDialog";
import StyledComponent from "./StyledSelectFileInput";
import SimpleFileInput from "../../SimpleFileInput";
import store from "SRC_DIR/store";

class SelectFileInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      fileName: "",
      folderId: "",
    };
  }

  componentDidMount() {
    this.props.setFirstLoad(false);
  }

  onSetFileNameAndLocation = (fileName, id) => {
    this.setState({
      fileName: fileName,
      folderId: id,
    });
  };

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
      ...rest
    } = this.props;

    const { fileName, folderId } = this.state;

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

SelectFileInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFileInputBody.defaultProps = {
  hasError: false,
  placeholder: "",
};

const SelectFileInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFileInputBody));

class SelectFileInput extends React.Component {
  render() {
    return (
      <MobxProvider {...store}>
        <SelectFileInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFileInput;
